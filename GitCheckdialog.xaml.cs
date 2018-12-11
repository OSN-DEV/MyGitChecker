using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace MyGitChecker {
    /// <summary>
    /// Gitのチェック
    /// </summary>
    public partial class GitCheckDialog : Window {

        #region Declaration
        public delegate void GitCheckResult(bool isSuccess, ObservableCollection<CheckResultModel> resultList);

        BackgroundWorker _backgroundWorker = new BackgroundWorker();

        private GitCheckResult _callback = null;
        private string _rootDir = "";
        private bool _isCanceled = false;
        private bool _isClosed = false;
        private ObservableCollection<CheckResultModel> _resultList = new ObservableCollection<CheckResultModel>();
        #endregion

        #region Constructor
        public GitCheckDialog() {
            InitializeComponent();
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="rootDir">display message</param>
        public GitCheckDialog(string rootDir, GitCheckResult callback) {
            InitializeComponent();

            this._callback = callback;
            this._rootDir = rootDir;

            this._backgroundWorker.DoWork += this.Background_WorkerDoWork;
            this._backgroundWorker.RunWorkerCompleted += this.BackgroundWorker_RunWorkerCompleted;
            this._backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(this.BackgroundWorker_ProgressChanged);
            this._backgroundWorker.WorkerReportsProgress = true;
            this._backgroundWorker.RunWorkerAsync();
        }
        #endregion

        #region Event
        private void cCancel_Click(object sender, RoutedEventArgs e) {
            this.cCancel.IsEnabled = false;
            this._isCanceled = true;
        }

        protected override void OnClosing(CancelEventArgs e) {
            if (!this._isClosed) {
                e.Cancel = true;
            }
        }

        private void Background_WorkerDoWork(object sender, DoWorkEventArgs e) {
            try {
                var dirs = new DirectoryInfo(this._rootDir).GetDirectories(".git", SearchOption.AllDirectories);
                this._backgroundWorker.ReportProgress(-1, dirs.Length);
                string result;

                foreach (var dir in dirs) {
                    if (this._isCanceled) {
                        return;
                    }

                    this._backgroundWorker.ReportProgress(0, dir.FullName);

                    var changeDir = string.Format("cd /d {0}", '\"' + dir.FullName.Replace(@"\.git", "") + '\"');

                    result = this.RunCommand(changeDir, "git fetch");
                    if (0 < result?.Length) {
                        _resultList.Add(new CheckResultModel() {
                            Type = "F",
                            BranchName = "",
                            DisplayDir = Directory.GetParent(dir.FullName).Name,
                            Dir = dir.FullName,
                            ConsoleResult = "> git fetch\n\n" + result
                        });
                    }


                    result = this.RunCommand(changeDir, "git branch");
                    foreach (var branchBase in result.Split('\n')) {
                        if (0 == branchBase.Length) {
                            continue;
                        }
                        if (!branchBase.StartsWith("* ")) {
                            continue;
                        }
                        var branch = branchBase.Replace("* ", "");
                        result = this.RunCommand(changeDir, "git status");
                        if (-1 == result.IndexOf("nothing to commit, working tree clean")) {
                            _resultList.Add(new CheckResultModel() {
                                Type = "C",
                                BranchName = branch,
                                DisplayDir = Directory.GetParent(dir.FullName).Name,
                                Dir = dir.FullName,
                                ConsoleResult = "> git status\n\n" + result
                            });
                        }
                        result = this.RunCommand(changeDir, string.Format("git log origin/{0}..{0}", branch));
                        if (0 < result.Length) {
                            _resultList.Add(new CheckResultModel() {
                                Type = "P",
                                BranchName = branch,
                                DisplayDir = Directory.GetParent(dir.FullName).Name,
                                Dir = dir.FullName,
                                ConsoleResult = string.Format("> git log origin/{0}..{0}\n\n", branch) + result
                            });
                        }
                        result = this.RunCommand(changeDir, string.Format("git log {0}..origin/{0}", branch));
                        if (0 < result.Length) {
                            _resultList.Add(new CheckResultModel() {
                                Type = "M",
                                BranchName = branch,
                                DisplayDir = Directory.GetParent(dir.FullName).Name,
                                Dir = dir.FullName,
                                ConsoleResult = string.Format("> git log {0}..origin/{0}\n\n", branch) + result
                            });
                        }
                    }

                    // wait
                    System.Threading.Thread.Sleep(100);
                }
            } catch(Exception ex) {
                Debug.Print(ex.Message);
            }
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            bool result = false;
            this._isClosed = true;
            if (e.Cancelled) {
            } else if (null != e.Error) {
            } else {
                result = true;
            }
            this.Close();
            if (null != _callback) {
                this._callback(result, this._resultList);
            }
        }
        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            if (-1 == e.ProgressPercentage) {
                this.cProgress.IsIndeterminate = false;
                this.cProgress.Maximum = (int)e.UserState;
            } else {
                this.cProgress.Value++;
                this.cStatus.Text = e.UserState.ToString();
            }
        }
        
        #endregion

        #region Private Method
        private string RunCommand(params string[] command) {
            Process p = new Process();
            p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardInput = false;
            p.StartInfo.CreateNoWindow = true;
            // p.StartInfo.StandardErrorEncoding = System.Text.Encoding.UTF8;
            p.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
            //            p.StartInfo.Arguments = @"/c dir c:\ /w";
            p.StartInfo.Arguments = "/c " + string.Join(" && ", command) ;
            p.Start();
            string results = p.StandardOutput.ReadToEnd();
            if (null == results || 0 == results.Length) {
                results = p.StandardError.ReadToEnd();
            }

            //プロセス終了まで待機する
            //WaitForExitはReadToEndの後である必要がある
            //(親プロセス、子プロセスでブロック防止のため)
            p.WaitForExit();
            p.Close();
            return results;
        }

        #endregion

    }
}
