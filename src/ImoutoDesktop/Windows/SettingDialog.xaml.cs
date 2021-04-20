using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ImoutoDesktop.Windows
{
    /// <summary>
    /// SettingDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingDialog : Window
    {
        public SettingDialog()
        {
            InitializeComponent();
        }

        public int Age
        {
            get { return (int)GetValue(AgeProperty); }
            set { SetValue(AgeProperty, value); }
        }

        public static readonly DependencyProperty AgeProperty =
            DependencyProperty.Register("Age", typeof(int), typeof(SettingDialog), new UIPropertyMetadata(10));

        public int TsundereLevel
        {
            get { return (int)GetValue(TsundereLevelProperty); }
            set { SetValue(TsundereLevelProperty, value); }
        }

        public static readonly DependencyProperty TsundereLevelProperty =
            DependencyProperty.Register("TsundereLevel", typeof(int), typeof(SettingDialog), new UIPropertyMetadata(4));

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Password = password.Password;
            BindingGroup.CommitEdit();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            BindingGroup.CancelEdit();
            DialogResult = false;
        }

        private void SettingDialog_Loaded(object sender, RoutedEventArgs e)
        {
            password.Password = Settings.Default.Password;
            BindingGroup.BeginEdit();
        }
    }
}
