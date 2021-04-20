using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ImoutoDesktop.Input
{
    public static class Commands
    {
        private static readonly RoutedCommand _version = new RoutedCommand("Version", typeof(Commands));

        public static RoutedCommand Version
        {
            get { return Commands._version; }
        }

        private static readonly RoutedCommand _character = new RoutedCommand("Character", typeof(Commands));

        public static RoutedCommand Character
        {
            get { return Commands._character; }
        }

        private static readonly RoutedCommand _balloon = new RoutedCommand("Balloon", typeof(Commands));

        public static RoutedCommand Balloon
        {
            get { return Commands._balloon; }
        }

        private static readonly RoutedCommand _option = new RoutedCommand("Option", typeof(Commands));

        public static RoutedCommand Option
        {
            get { return Commands._option; }
        }
    }
}
