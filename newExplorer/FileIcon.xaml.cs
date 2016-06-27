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
        TextBlock tb = new TextBlock();
        FileInfo file;

        bool click;
        string name;

        public FileIcon(string str)
        {
            InitializeComponent();
            click = false;

            file = new FileInfo(str);

            Rectangle rect = new Rectangle();
            ImageBrush ibrush = new ImageBrush();
            ibrush.ImageSource = getIcon(str);
            rect.Fill = ibrush;
            rect.Width = 70;
            rect.Height = 70;
            rect.VerticalAlignment = VerticalAlignment.Top;
            rect.HorizontalAlignment = HorizontalAlignment.Center;

            //TextBlock tb = new TextBlock();
            tb.VerticalAlignment = VerticalAlignment.Bottom;
            tb.HorizontalAlignment = HorizontalAlignment.Center;

            FileInfo fi = new FileInfo(str);
            str = fi.Name;
            name = str;

            if (str.Length > 7)
            {
                str = str.Substring(0, 7) + "...";
            }

            tb.Text = str;
            tb.Foreground = new SolidColorBrush(Colors.Black);
            tb.HorizontalAlignment = HorizontalAlignment.Center;

            tb.HorizontalAlignment = HorizontalAlignment.Center;

            sp.VerticalAlignment = VerticalAlignment.Stretch;
            sp.HorizontalAlignment = HorizontalAlignment.Stretch;
            sp.Children.Add(rect);
            sp.Children.Add(tb);
            sp.Margin = new Thickness(2);
        }

        // 해당 파일의 아이콘을 얻어온다
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

        private void sp_MouseEnter(object sender, MouseEventArgs e)
        {
            sp.Background = Brushes.LightSkyBlue;
        }

        private void sp_MouseLeave(object sender, MouseEventArgs e)
        {
            if (click) return;
            sp.Background = Brushes.White;
        }

        private void sp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 더블클릭 메소드
            // 파일 실행을 시킨다
            if(e.ClickCount.Equals(2))
            {
                Process ps = new Process();
                ps.StartInfo.FileName = file.Name;
                ps.StartInfo.WorkingDirectory = file.Directory.ToString();
                ps.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;

                ps.Start();
            }

            // 파일이름이 너무 길경우 ... 처리
            string cutStr = "";
            if (tb.Text.Length > 7)
            {
                cutStr = tb.Text.Substring(0, 7) + "...";
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
                tb.Text = name;
                sp.Background = Brushes.LightSkyBlue;
                click = true;
            }
        }
    }
}
