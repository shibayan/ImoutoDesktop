using System.Windows.Input;

namespace ImoutoDesktop.Input
{
    public static class Commands
    {
        public static RoutedCommand Version { get; } = new RoutedCommand(nameof(Version), typeof(Commands));

        public static RoutedCommand Character { get; } = new RoutedCommand(nameof(Character), typeof(Commands));

        public static RoutedCommand Balloon { get; } = new RoutedCommand(nameof(Balloon), typeof(Commands));

        public static RoutedCommand Option { get; } = new RoutedCommand(nameof(Option), typeof(Commands));
    }
}
