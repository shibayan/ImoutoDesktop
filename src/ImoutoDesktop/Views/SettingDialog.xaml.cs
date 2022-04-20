using System.Windows;

namespace ImoutoDesktop.Views;

/// <summary>
/// SettingDialog.xaml の相互作用ロジック
/// </summary>
public partial class SettingDialog
{
    public SettingDialog()
    {
        InitializeComponent();
    }

    public int Age
    {
        get => (int)GetValue(AgeProperty);
        set => SetValue(AgeProperty, value);
    }

    public static readonly DependencyProperty AgeProperty =
        DependencyProperty.Register(nameof(Age), typeof(int), typeof(SettingDialog), new UIPropertyMetadata(10));

    public int TsundereLevel
    {
        get => (int)GetValue(TsundereLevelProperty);
        set => SetValue(TsundereLevelProperty, value);
    }

    public static readonly DependencyProperty TsundereLevelProperty =
        DependencyProperty.Register("TsundereLevel", typeof(int), typeof(SettingDialog), new UIPropertyMetadata(4));

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
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
        BindingGroup.BeginEdit();
    }
}
