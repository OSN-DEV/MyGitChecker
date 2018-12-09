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

namespace MyGitChecker {
    /// <summary>
    /// 処理中ダイアログ(not use)
    /// </summary>
    public partial class ProgressDialog : Window {
        // 参考
        // 重い処理の実行中にプログレスバー{IsIndeterminate=True}付きウィンドウを表示する
        // https://qiita.com/dhq_boiler/items/930ff2dc960fd950d7a3

        #region Declaration
        BackgroundWorker _backgroundWorker = new BackgroundWorker();
        Action _action;
        public bool IsComplete { set; get; }
        public bool IsClosed { set; get; }
        public bool IsCanceld { set; get; }

        #endregion


        #region Constructor
        public ProgressDialog() {
            InitializeComponent();
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="message">display message</param>
        /// <param name="action">background task</param>
        public ProgressDialog(string message, Action action) {
            InitializeComponent();

            this._action = action;
            this.DataContext = message;
            this.IsComplete = false;
            this.IsClosed = false;
            this.IsCanceld = false;

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
            this._action?.Invoke();
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

        private void cCancel_Click(object sender, RoutedEventArgs e) {
            cCancel.IsEnabled = false;
        }
        #endregion

    }
}
