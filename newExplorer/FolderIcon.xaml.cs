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

        bool click;
        string name;
        string path;

        public FolderIcon(MainWindow main, string path, string str)
        {
            InitializeComponent();

            this.main = main;
            directory = new DirectoryInfo(str);
            this.path = path;

            click = false;
            name = str;

            Rectangle rect = new Rectangle();

            rect.Fill = new ImageBrush(new BitmapImage(new Uri(@"folder.png", UriKind.RelativeOrAbsolute)));
            rect.Width = 70;
            rect.Height = 70;
            rect.VerticalAlignment = VerticalAlignment.Top;
            rect.HorizontalAlignment = HorizontalAlignment.Center;

            tb.MaxWidth = 90;
            tb.TextWrapping = TextWrapping.Wrap;
            tb.VerticalAlignment = VerticalAlignment.Bottom;
            tb.HorizontalAlignment = HorizontalAlignment.Center;
            

            if (str.Length > 7)
            {
                str = str.Substring(0, 7) + "...";
            }

            tb.Text = str;
            tb.Foreground = new SolidColorBrush(Colors.Black);
            tb.HorizontalAlignment = HorizontalAlignment.Center;

            sp.VerticalAlignment = VerticalAlignment.Stretch;
            sp.HorizontalAlignment = HorizontalAlignment.Stretch;
            sp.Children.Add(rect);
            sp.Children.Add(tb);
            sp.Margin = new Thickness(2);
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
            if (e.ClickCount.Equals(2))
            {
                var folderPath = path + "\\" + directory.Name;
                if(folderPath.Contains("\\\\"))
                {
                    folderPath = folderPath.Replace("\\\\", "\\");
                }
                main.Drawing(folderPath);
            }

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
