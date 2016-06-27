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
using System.IO;
using System.Windows.Media.Animation;
using System.Diagnostics;

namespace newExplorer
{
    /// <summary>
    /// FileIcon.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FileIcon : UserControl
    {
        /// <summary>
        /// tb : 파일 이름을 저장
        /// file : 파일 이름과, 디렉토리를 따로 뽑아내기 위해 선언
        /// click : 파일이 클릭되었는지 체크하기 위한 bool 값
        /// cutStr : 파일 이름이 너무 길 경우 자른 값
        /// </summary>
        TextBlock tb = new TextBlock();
        FileInfo file;

        bool click;
        string cutStr;

        public FileIcon(string str)
        {
            InitializeComponent();
            click = false;

            file = new FileInfo(str);

            // 아이콘 이미지를 설정하기 위한 Rectangle Class 사용
            Rectangle rect = new Rectangle();

            // 아이콘 이미지를 긁어옴
            ImageBrush ibrush = new ImageBrush();
            ibrush.ImageSource = getIcon(str);
            // rect 에 추가
            rect.Fill = ibrush;

            // 이미지 크기와 위치 조정
            rect.Width = 70;
            rect.Height = 70;
            rect.VerticalAlignment = VerticalAlignment.Top;
            rect.HorizontalAlignment = HorizontalAlignment.Center;

            // 파일이름(Text Block)의 크기와 위치 조정
            tb.Text = file.Name;
            cutStr = file.Name;
            tb.VerticalAlignment = VerticalAlignment.Bottom;
            tb.HorizontalAlignment = HorizontalAlignment.Center;

            // 파일이름(Text Block)이 너무 길 경우 조절
            if (file.Name.Length > 7)
            {
                cutStr = file.Name.Substring(0, 7) + "...";
                tb.Text = cutStr;
            }

            // 파일이름(TextBlock) 위치 및 색 조정
            tb.Foreground = new SolidColorBrush(Colors.Black);
            tb.HorizontalAlignment = HorizontalAlignment.Center;

            // StackPanel 조정 및 파일(Rectangle)과 파일이름(TextBlock) 추가
            sp.VerticalAlignment = VerticalAlignment.Stretch;
            sp.HorizontalAlignment = HorizontalAlignment.Stretch;
            sp.Children.Add(rect);
            sp.Children.Add(tb);
            // 여백설정
            sp.Margin = new Thickness(2);
        }

        // 해당 파일의 아이콘을 얻어오는 메소드
        public System.Windows.Media.ImageSource getIcon(string filename)
        {
            System.Windows.Media.ImageSource icon;

            using (System.Drawing.Icon sysicon = System.Drawing.Icon.ExtractAssociatedIcon(filename))
            {
                icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                sysicon.Handle,
                System.Windows.Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            return icon;
        }

        // 파일(StackPanel)에 마우스가 올려졌을 경우
        private void sp_MouseEnter(object sender, MouseEventArgs e)
        {
            sp.Background = Brushes.LightSkyBlue;
        }

        // 파일(StackPanel)에서 마우스가 떨어졌을 경우
        private void sp_MouseLeave(object sender, MouseEventArgs e)
        {
            if (click) return;
            sp.Background = Brushes.White;
        }

        // 파일(StackPanel)에 마우스 클릭 이벤트가 발생하였을 경우
        private void sp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 더블클릭 메소드
            // 파일 실행을 시킨다
            if(e.ClickCount.Equals(2))
            {
                // 프로세스의 이름과 경로를 받고
                Process ps = new Process();
                ps.StartInfo.FileName = file.Name;
                ps.StartInfo.WorkingDirectory = file.Directory.ToString();
                ps.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;

                // 프로세스 실행
                ps.Start();
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
                tb.Text = file.Name;
                sp.Background = Brushes.LightSkyBlue;
                click = true;
            }
        }
    }
}
