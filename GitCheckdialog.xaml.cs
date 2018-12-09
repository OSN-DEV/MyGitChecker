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

namespace MyGitChecker {
    /// <summary>
    /// Gitのチェック
    /// </summary>
    public partial class GitCheckDialog : Window {

        #region Declaration
        BackgroundWorker _backgroundWorker = new BackgroundWorker();

        public bool IsComplete { set; get; }
        public bool IsClosed { set; get; }
        public bool IsCanceld { set; get; }

        public delegate void GitCheckResult(bool result, ObservableCollection<CheckResultModel> model);
        private GitCheckResult _callback = null;
        private string _rootDir = "";
        private bool _isCanceled = false;
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
            this._backgroundWorker.RunWorkerAsync();
        }
        #endregion

        #region Event
        protected override void OnClosing(CancelEventArgs e) {
            if (!this.IsClosed) {
                e.Cancel = true;
            }
        }

        private void Background_WorkerDoWork(object sender, DoWorkEventArgs e) {
           
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            this.IsClosed = true;
            if (e.Cancelled) {
                this.IsComplete = false;
            } else if (null != e.Error) {
                this.IsComplete = false;
            } else {
                this.IsComplete = true;
            }
            this.Close();
        }

        #endregion

        #region Private Method
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
