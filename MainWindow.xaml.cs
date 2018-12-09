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
        }
        #endregion

        #region Event
        private void RowDefinition_Loaded(object sender, RoutedEventArgs e) {
            this.cRootDir.Text = Properties.Settings.Default.RootDir;
        }


        private void cRootDir_TextChanged(object sender, TextChangedEventArgs e) {
            if (0 < this.cRootDir.Text.Length && System.IO.Directory.Exists(this.cRootDir.Text)) {
                this.cStartCheck.IsEnabled = true;
            } else {
                this.cStartCheck.IsEnabled = false;
            }
        }

        private void Check_Click(object sender, RoutedEventArgs e) {
            var dialog = new GitCheckDialog(this.cRootDir.Text, this.GitCheckResult);
            dialog.ShowDialog();

            Properties.Settings.Default.RootDir = this.cRootDir.Text;
            Properties.Settings.Default.Save();
        }

        private void Window_DragEnter(object sender, DragEventArgs e) {
            DragDropEffects effectts = DragDropEffects.None;

            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                foreach (var file in files) {
                    if (System.IO.Directory.Exists(file)) {
                        effectts = DragDropEffects.Copy;
                        break;
                    }
                }
            }

            e.Effects = effectts;
        }

        private void Window_Drop(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                foreach (var file in files) {
                    if (System.IO.Directory.Exists(file)) {
                        this.cRootDir.Text = file;
                        break;
                    }
                }
            }
        }

        public void GitCheckResult(bool result, ObservableCollection<CheckResultModel> model) {
        }

        #endregion

    }
}
