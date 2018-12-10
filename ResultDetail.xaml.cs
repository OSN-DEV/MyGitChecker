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

namespace MyGitChecker {
    /// <summary>
    /// 詳細情報
    /// </summary>
    public partial class ResultDetail : Window {
        #region Constructor
        public ResultDetail() {
            InitializeComponent();
        }

        public ResultDetail(string dir, string branchName, string consoleResult) {
            InitializeComponent();

            this.cDir.Text = dir;
            this.cBranchName.Text = branchName;
            this.cConsoleResult.Text = consoleResult;
        }

        #endregion

        #region Event
        private void cClose_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
        #endregion
    }
}
