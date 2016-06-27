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
        /// <summary>
        /// storePath : (뒤로가기, 앞으로가기) 경로를 저장하기 위한 string 배열
        /// pathIndex : 현재의 경로위치 index
        /// backStatue : 뒤로가기를 누른 상태를 나타내줌
        /// btnCall : 뒤로가기, 앞으로가기가 눌렸음을 나타내줌
        /// </summary>
        public List<string> storePath;
        public int pathIndex = -1;
        bool backStatue = false;
        bool btnCall = false;

        public MainWindow()
        {
            InitializeComponent();

            storePath = new List<string>();
        }

        // treeView 가 처음 로드되었을 때
        private void tv_path_Loaded(object sender, RoutedEventArgs e)
        {
            // 드라이브 목록을 가져옴
            foreach (string str in Directory.GetLogicalDrives())
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = str;
                item.Tag = str;
                // 확장되었을 때 이벤트 설정
                item.Expanded += new RoutedEventHandler(item_Expanded);
                item.Selected += new RoutedEventHandler(sub_Selected);

                // TreeVIew 에 아이템 추가
                tv_path.Items.Add(item);
                GetSubDirectories(item);
            }

            // 기본 경로를 C:\\ 로 설정
            txtBox_path.Text = "C:\\";
            storePath.Add(txtBox_path.Text);
            pathIndex++;
        }

        // 하위 디렉토리를 얻는 함수
        private void GetSubDirectories(TreeViewItem itemParent)
        {
            // TreeViewItem에 대한 예외처리
            if (itemParent.Equals(null)) return;
            if (!itemParent.Items.Count.Equals(0)) return;

            // 부모 경로 담기
            string strPath = itemParent.Tag as string;
            // 액세스 거부에 대한 폴더 직접 예외처리
            if (strPath.Contains("Documents and Settings")) return;
            else if (strPath.Contains("System Volume Information")) return;
            else if (strPath.Contains("Recovery")) return;

            // 드라이브에 대해 준비되지 않았다면 함수종료 (예외처리)
            DriveInfo driveReadyCheck = new DriveInfo(strPath);
            if (!driveReadyCheck.IsReady) return;

            DirectoryInfo dInfoParent = new DirectoryInfo(strPath);

            // 디렉토리를 하나씩 뽑아서, TreeViewItem 으로 추가함
            foreach (DirectoryInfo dInfo in dInfoParent.GetDirectories())
            {
                // 이름과 경로 정보를 추가하고
                TreeViewItem item = new TreeViewItem();
                item.Header = dInfo.Name;
                item.Tag = dInfo.FullName;
                // 확장이벤트와 선택이벤트 추가
                item.Expanded += new RoutedEventHandler(item_Expanded);
                item.Selected += new RoutedEventHandler(sub_Selected);
                // 부모 TreeViewItem 에 자식 item으로 추가
                itemParent.Items.Add(item);
            }
        }

        // TreeViewItem이 확장되었을 경우 이벤트
        private void item_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem itemParent = sender as TreeViewItem;

            if (itemParent.Equals(null)) return;
            if (itemParent.Items.Count.Equals(0)) return;

            foreach (TreeViewItem item in itemParent.Items)
            {
                GetSubDirectories(item);
            }
        } // Method

        // 자식 TreeViewItem이 선택되었을 경우 이벤트
        private void sub_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            btnCall = false;

            e.Handled = true;
            Drawing(item.Tag.ToString());
        }

        // WrapPanel에 아이콘을 추가해주는 메소드
        // path : 전체경로를 받음
        public void Drawing(string path)
        {
            // WrapPanel 초기화
            wp_item.Children.Clear();

            DirectoryInfo di = new DirectoryInfo(path);
            string[] name;

            // 디렉터리가 존재하지 않다면 예외 메세지를 띄워주고 함수종료
            if (!di.Exists)
            {
                txtBox_path.Text = "존재하지 않는 경로입니다";
                return;
            }

            // 해당 경로에 대한 하위디렉토리 정보를 모두 가져옴
            var directories = di.GetDirectories();
            int directoryNum = directories.Length;

            DirectoryInfo[] dArray = new DirectoryInfo[directoryNum];
            name = new string[directoryNum];


            // 디렉토리 갯수만큼 반복
            for (int i = 0; i < directoryNum; i++)
            {
                // 해당 디렉토리가 존재하지 않다면 리턴
                if (!di.Exists) return;

                if (chkBox_Hidden.IsChecked.Equals(true))
                {
                    if (directories[i].Attributes.ToString().Contains("Hidden")) continue;
                }

                // 폴더에 대한 정보를 새로 선언
                FolderIcon folder = new FolderIcon(this, path, directories[i].Name);
                folder.Tag = path + directories[i].Name + "\\";

                wp_item.Children.Add(folder);
            }

            // 파일에 대한 정보를 가져옴
            var files = di.GetFiles();
            int fileNum = files.Length;

            // 파일의 갯수만큼
            for (int i = 0; i < fileNum; i++)
            {
                FileInfo[] fArray = new FileInfo[fileNum];
                name = new string[fileNum];    

                // 해당 경로에 아이콘을 추가 후
                FileIcon fIcon = new FileIcon(path + "\\" + files[i].Name);
                fIcon.Tag = path + files[i].Name;

                // WrapPanel 에 자식으로 추가
                wp_item.Children.Add(fIcon);
            }

            // 해당 경로를 업데이트 해줌
            txtBox_path.Text = path;

            if(btnCall)
            {
                btnCall = false;
                return;
            }
            setCurrentPath();
        }

        // 주소창에서 Enter 기호로 경로를 이동시키는 메소드
        private void txtBox_path_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                setCurrentPath();
                Drawing(txtBox_path.Text);
            }
        }

        // 뒤로가기 버튼을 눌렀을 때
        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            btnCall = true;
            backStatue = true;
            if (pathIndex.Equals(0)) return;
            pathIndex--;
            txtBox_path.Text = storePath[pathIndex];
            Drawing(txtBox_path.Text);
        }

        // 앞으로가기 버튼을 눌렀을 때
        private void btn_foward_Click(object sender, RoutedEventArgs e)
        {
            btnCall = true;
            // 현재 경로와 같다면 함수종료
            if (storePath.Count.Equals(pathIndex + 1)) return;
            pathIndex++;
            // 절대경로 업데이트
            txtBox_path.Text = storePath[pathIndex];
            Drawing(txtBox_path.Text);
        }

        // 현재의 경로를 설정하는 메소드
        public void setCurrentPath()
        {
            // 현재 경로와 같다면 함수종료
            if (txtBox_path.Text.Equals(storePath[pathIndex])) return;
            // 뒤로가기 상태라면
            if (backStatue)
            {
                // 저장되어 있는 경로를 지워줌
                backStatue = false;
                var range = storePath.Count - (pathIndex + 1);
                storePath.RemoveRange(pathIndex + 1, range);
            }

            // 해당 인덱스를 조정
            pathIndex++;
            // 절대경로 업데이트
            storePath.Add(txtBox_path.Text);
        }
    }
}