using System.Windows.Input;

namespace ImoutoDesktop.Input;

public static class DefaultCommands
{
    public static RoutedCommand Version { get; } = new(nameof(Version), typeof(DefaultCommands));

    public static RoutedCommand Character { get; } = new(nameof(Character), typeof(DefaultCommands));

    public static RoutedCommand Balloon { get; } = new(nameof(Balloon), typeof(DefaultCommands));

    public static RoutedCommand Option { get; } = new(nameof(Option), typeof(DefaultCommands));
}
