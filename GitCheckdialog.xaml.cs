using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;

using System.Windows.Threading;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.IO;
using System.Diagnostics;

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

            this.cStatus.Text = "hogehoge";

        }
        #endregion

        #region Event
        protected override void OnClosing(CancelEventArgs e) {
            if (!this._isClosed) {
                e.Cancel = true;
            }
        }

        private void Background_WorkerDoWork(object sender, DoWorkEventArgs e) {
            var dirs = new DirectoryInfo(this._rootDir).GetDirectories(".git", SearchOption.AllDirectories);
            this._backgroundWorker.ReportProgress(-1, dirs.Length);
            string[] result;

            foreach (var dir in dirs) {
                this._backgroundWorker.ReportProgress(0, dir.FullName);

                var changeDir = string.Format("cd /d {0}", '\"' +  dir.FullName.Replace(@"\.git", "") + '\"');
                result = this.RunCommand(changeDir, "git branch");

                foreach(var branchbase in result) {
                    // string branch = branch.Replace("* ","");
                }


                System.Threading.Thread.Sleep(500);

                //UpdateDelegate update = new UpdateDelegate(UpdateLabel);
                //cStatus.Dispatcher.BeginInvoke(DispatcherPriority.Normal, update, 10);

            }
        }
        private void UpdateLabel(int i) {
            this.cStatus.Text = i.ToString();
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
        private delegate void UpdateDelegate(int i);

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            if (-1 == e.ProgressPercentage) {
                this.cProgress.IsIndeterminate = false;
                this.cProgress.Maximum = (int)e.UserState;
            } else {
                this.cProgress.Value++;
                this.cStatus.Text = e.UserState.ToString();
            }
            //Dispatcher.Invoke(() => {
            //    if (-1 == e.ProgressPercentage) {
            //        this.cProgress.IsIndeterminate = false;
            //        this.cProgress.Maximum = (int)e.UserState;
            //    } else {
            //        this.cProgress.Value++;
            //        this.cStatus.Text = e.UserState.ToString();
            //    }
            //}
            //);

        }
        
        #endregion

        #region Private Method
        private string[] RunCommand(params string[] command) {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = false;
            p.StartInfo.CreateNoWindow = true;
//            p.StartInfo.Arguments = @"/c dir c:\ /w";
            p.StartInfo.Arguments = "/c " + string.Join(" && ", command) ;
            p.Start();
            string results = p.StandardOutput.ReadToEnd();

            //プロセス終了まで待機する
            //WaitForExitはReadToEndの後である必要がある
            //(親プロセス、子プロセスでブロック防止のため)
            p.WaitForExit();
            p.Close();

            //出力された結果を表示
            return results.Split('\n');
        }

        private void DoEvents() {
            DispatcherFrame frame = new DispatcherFrame();
            var callback = new DispatcherOperationCallback(ExitFrames);
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, callback, frame);
            Dispatcher.PushFrame(frame);
        }
        private object ExitFrames(object obj) {
            ((DispatcherFrame)obj).Continue = false;
            return null;
        }


        #endregion
    }
}
