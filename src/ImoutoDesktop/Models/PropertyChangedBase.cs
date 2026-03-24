using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ImoutoDesktop.Models;

public class PropertyChangedBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
