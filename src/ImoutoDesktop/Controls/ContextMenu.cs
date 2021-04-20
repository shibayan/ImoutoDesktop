using System.Windows;
using System.Windows.Media;

namespace ImoutoDesktop.Controls
{
    public class ContextMenu : System.Windows.Controls.ContextMenu
    {
        static ContextMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ContextMenu), new FrameworkPropertyMetadata(typeof(ContextMenu)));
        }

        public ImageSource BackgroundImage
        {
            get { return (ImageSource)GetValue(BackgroundImageProperty); }
            set { SetValue(BackgroundImageProperty, value); }
        }

        public static readonly DependencyProperty BackgroundImageProperty =
            DependencyProperty.Register("BackgroundImage", typeof(ImageSource), typeof(ContextMenu));

        public ImageSource SidebarImage
        {
            get { return (ImageSource)GetValue(SidebarImageProperty); }
            set { SetValue(SidebarImageProperty, value); }
        }

        public static readonly DependencyProperty SidebarImageProperty =
            DependencyProperty.Register("SidebarImage", typeof(ImageSource), typeof(ContextMenu));
    }
}
