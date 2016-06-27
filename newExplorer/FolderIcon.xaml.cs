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
using System.Windows.Media.Animation;
using System.IO;

namespace newExplorer
{
    /// <summary>
    /// FolderIcon.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FolderIcon : UserControl
    {
        MainWindow main;
        TextBlock tb = new TextBlock();

        DirectoryInfo directory;

        /// <summary>
        /// click : 클릭이 되었는지 체크하기 위한 값
        /// name : 인자로 넘어오는 파일명을 따로 저장하기 위한 값
        /// path : 경로를 저장하기 위한 값
        /// cutStr : 이름을 ... 로 축약시킨 값
        /// </summary>
        bool click;
        string name;
        string path;

        string cutStr;

        public FolderIcon(MainWindow main, string path, string str)
        {
            InitializeComponent();

            this.main = main;
            directory = new DirectoryInfo(str);

            click = false;
            this.path = path;

            // 폴더 이미지를 부여하기 위해 Rectangle 클래스 사용
            Rectangle rect = new Rectangle();

            // 폴더이미지를 따로 설정
            rect.Fill = new ImageBrush(new BitmapImage(new Uri(@"folder.png", UriKind.RelativeOrAbsolute)));
            rect.Width = 70;
            rect.Height = 70;
            rect.VerticalAlignment = VerticalAlignment.Top;
            rect.HorizontalAlignment = HorizontalAlignment.Center;

            // 파일명을 나타내주기 위해 TextBlock 사용
            tb.MaxWidth = 90;
            tb.Text = directory.Name;
            cutStr = directory.Name;

            // 파일명이 너무 길어질 경우 자동으로 줄바꿈
            tb.TextWrapping = TextWrapping.Wrap;
            tb.VerticalAlignment = VerticalAlignment.Bottom;
            tb.HorizontalAlignment = HorizontalAlignment.Center;
            
            // 파일명이 너무 길경우 ... 으로 표시
            if (directory.Name.Length > 7)
            {
                cutStr = directory.Name.Substring(0, 7) + "...";
                tb.Text = cutStr;
            }

            // 파일명의 색과 위치 조정
            tb.Foreground = new SolidColorBrush(Colors.Black);
            tb.HorizontalAlignment = HorizontalAlignment.Center;

            // 자동으로 채워질 수 있게끔 정렬
            sp.VerticalAlignment = VerticalAlignment.Stretch;
            sp.HorizontalAlignment = HorizontalAlignment.Stretch;

            // Stack Panel 에 아이콘을 추가 (Rectangle, textBlock)
            sp.Children.Add(rect);
            sp.Children.Add(tb);
            // 여백설정
            sp.Margin = new Thickness(2);
        }

        // 마우스가 아이콘(Stack Panel)에 진입하였을 경우
        // 색깔을 바꿔주는 Event 발생
        private void sp_MouseEnter(object sender, MouseEventArgs e)
        {
            sp.Background = Brushes.LightSkyBlue;
        }

        // 마우스가 아이콘(Stack Panel)에서 아웃됬을 경우
        private void sp_MouseLeave(object sender, MouseEventArgs e)
        {
            // 이미 클릭상태라면 Return
            if (click) return;
            sp.Background = Brushes.White;
        }

        // 마우스가 아이콘(Stack Panel)을 클릭하였을 경우
        private void sp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 더블클릭 이벤트
            if (e.ClickCount.Equals(2))
            {
                // 폴더의 경로를 수정
                var folderPath = path + "\\" + directory.Name;
                if(folderPath.Contains("\\\\"))
                {
                    folderPath = folderPath.Replace("\\\\", "\\");
                }
                main.Drawing(folderPath);
            }

            // 클릭이 되어있는 상태
            if (click)
            {
                tb.Text = cutStr;
                sp.Background = Brushes.White;
                click = false;
            }
            // 클릭이 안되어있는 상태
            else
            {
                tb.Text = directory.Name;
                sp.Background = Brushes.LightSkyBlue;
                click = true;
            }
        }
    }
}
