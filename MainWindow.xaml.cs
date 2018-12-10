using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

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
            if (4 < this.cRootDir.Text.Length && System.IO.Directory.Exists(this.cRootDir.Text)) {
                this.cStartCheck.IsEnabled = true;
            } else {
                this.cStartCheck.IsEnabled = false;
            }
        }

        private void Check_Click(object sender, RoutedEventArgs e) {
            this.cResultList.DataContext = null;
            var dialog = new GitCheckDialog(this.cRootDir.Text, this.GitCheckResult);
            dialog.Owner = this;
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

        private void cResultList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            // var model = (CheckResultModel)((ListView)sender).DataContext;
            var model = ((FrameworkElement)e.OriginalSource).DataContext as CheckResultModel;
            if (0 < model?.Type?.Length) {
                var detail = new ResultDetail(model.Dir, model.BranchName, model.ConsoleResult);
                detail.Owner = this;
                detail.ShowDialog();
            }
        }

        public void GitCheckResult(bool result, ObservableCollection<CheckResultModel> model) {
            if (0 == model.Count) {
                var noData = new ObservableCollection<CheckResultModel>();
                noData.Add(new CheckResultModel() {DisplayDir = "No data found"});
                this.cResultList.DataContext = noData;
            } else {
                this.cResultList.DataContext = model;
            }
        }

        #endregion

    }
}
