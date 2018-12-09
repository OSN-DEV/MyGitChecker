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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace MyGitChecker {
    /// <summary>
    /// Main
    /// </summary>
    public partial class MainWindow : Window {
        // 参考
        // http://pro.art55.jp/?eid=1177249
        // https://www.sejuku.net/blog/56841

        #region Constructor
        public MainWindow() {
            InitializeComponent();

            this.cRootDir.Text = Properties.Settings.Default["RootDir"].ToString();
        }
        #endregion

        #region Event
        private void cRootDir_TextChanged(object sender, TextChangedEventArgs e) {
            this.cStartCheck.IsEnabled = (0 < this.cRootDir.Text.Length);
        }

        private void Check_Click(object sender, RoutedEventArgs e) {
            var dialog = new GitCheckDialog(this.cRootDir.Text, this.GitCheckResult);
            dialog.ShowDialog();

            Properties.Settings.Default["RootDir"] = this.cRootDir.Text;
            Properties.Settings.Default.Save();
        }

        public void GitCheckResult(bool result, ObservableCollection<CheckResultModel> model) {
        }
        #endregion


    }
}
