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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;

namespace newExplorer
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        // Directory 에 대한 정보 (Browser)
        struct DirectoryAttributes
        {
            /// <summary>
            /// 디렉토리의 생성 날짜
            /// </summary>
            public DateTime CreateTime;

            /// <summary>
            /// 디렉토리의 하위 파일 갯수
            /// </summary>
            public int SubFileCount;

            /// <summary>
            /// 디렉토리의 하위 디렉토리 갯수
            /// </summary>
            public int SubDirectoryCount;
        }

        public List<string> storePath;
        public int pathCount = -1;
        bool backStatue = false;

        public MainWindow()
        {
            InitializeComponent();

            storePath = new List<string>();
        }

        private void tv_path_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (string str in Directory.GetLogicalDrives())
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = str;
                item.Tag = str;
                item.Expanded += new RoutedEventHandler(item_Expanded);
                //item.Selected += new RoutedEventHandler(sub_Selected);

                tv_path.Items.Add(item);
                GetSubDirectories(item);
            } // foreach

            txtBox_path.Text = "C:\\";
            storePath.Add(txtBox_path.Text);
            pathCount++;
        }

        private void GetSubDirectories(TreeViewItem itemParent)
        {
            if (itemParent.Equals(null)) return;
            if (!itemParent.Items.Count.Equals(0)) return;

            string strPath = itemParent.Tag as string;
            // 액세스 거부에 대한 폴더 직접 예외처리
            if (strPath.Contains("Documents and Settings")) return;
            else if (strPath.Contains("System Volume Information")) return;
            else if (strPath.Contains("Recovery")) return;

            DirectoryInfo dInfoParent = new DirectoryInfo(strPath);

            // 드라이브에 대해 준비되지 않았다면 함수종료 (예외처리)
            DriveInfo driveReadyCheck = new DriveInfo(strPath);
            if (!driveReadyCheck.IsReady) return;

            foreach (DirectoryInfo dInfo in dInfoParent.GetDirectories())
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = dInfo.Name;
                item.Tag = dInfo.FullName;
                item.Expanded += new RoutedEventHandler(item_Expanded);
                item.Selected += new RoutedEventHandler(sub_Selected);
                itemParent.Items.Add(item);
            }
        }

        public void item_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem itemParent = sender as TreeViewItem;

            if (itemParent.Equals(null)) return;
            if (itemParent.Items.Count.Equals(0)) return;

            foreach (TreeViewItem item in itemParent.Items)
            {
                GetSubDirectories(item);
            }
        } // Method

        void sub_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            item.IsSelected = false;

            e.Handled = true;
            Drawing(item.Tag.ToString());
        }

        public void Drawing(string path)
        {
            wp_item.Children.Clear();

            /* GetDirectoryInfo Start */
            DirectoryAttributes da = new DirectoryAttributes();

            DirectoryInfo di = new DirectoryInfo(path);

            if (!di.Exists) return;

            da.CreateTime = di.CreationTime;
            da.SubDirectoryCount = di.GetDirectories().Length;
            da.SubFileCount = di.GetFiles().Length;
            /* GetDirectoryInfo End */

            int n = da.SubDirectoryCount;

            for (int i = 0; i < n; i++)
            {
                if (!di.Exists) return;

                int l = di.GetDirectories().Length;
                DirectoryInfo[] dArray = new DirectoryInfo[l];
                string[] name = new string[l];

                dArray = di.GetDirectories();
                for (int j = 0; j < l; j++)
                {
                    name[j] = dArray[j].Name;
                }

                FolderIcon folder = new FolderIcon(this, path, name[i]);
                folder.Tag = path + name[i] + "\\";

                wp_item.Children.Add(folder);
            }

            int fn = da.SubFileCount;

            for (int i = 0; i < fn; i++)
            {
                int l = di.GetFiles().Length;
                string[] name = new string[l];

                FileInfo[] fArray = new FileInfo[l];
                fArray = di.GetFiles();

                for (int j = 0; j < l; j++)
                {
                    name[j] = fArray[j].Name;
                }               

                FileIcon fIcon = new FileIcon(path + "\\" + name[i]);
                fIcon.Tag = path + name[i];

                wp_item.Children.Add(fIcon);
            }
            txtBox_path.Text = path;

            setCurrentPath();
        }

        private void txtBox_path_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                setCurrentPath();

                Drawing(txtBox_path.Text);
            }
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            backStatue = true;
            if (pathCount.Equals(0)) return;
            pathCount--;
            txtBox_path.Text = storePath[pathCount];
            Drawing(txtBox_path.Text);
        }

        private void btn_foward_Click(object sender, RoutedEventArgs e)
        {
            if (storePath.Count.Equals(pathCount + 1)) return;
            pathCount++;
            txtBox_path.Text = storePath[pathCount];
            if (storePath.Count.Equals(pathCount + 1)) backStatue = false;
            Drawing(txtBox_path.Text);
        }

        public void setCurrentPath()
        {
            if (txtBox_path.Text.Equals(storePath[pathCount])) return;
            if (backStatue)
            {
                backStatue = false;
                var range = storePath.Count - (pathCount + 1);
                storePath.RemoveRange(pathCount + 1, range);
                pathCount--;
            }

            pathCount++;
            storePath.Add(txtBox_path.Text);
        }
    }
}