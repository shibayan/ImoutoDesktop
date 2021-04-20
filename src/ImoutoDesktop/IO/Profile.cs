using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ImoutoDesktop.IO
{
    [Serializable]
    public class Profile
    {
        public Guid LastBalloon { get; set; }

        public Point BalloonOffset { get; set; }

        public int Age { get; set; }

        public int TsundereLevel { get; set; }
    }
}
