using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SC2ParserApe
{
    public class Sc2Unit
    {
        public Sc2Unit()
        { }

        public String Name = String.Empty;
        public Int32 Type = 0;
        public String TypeString = String.Empty;
        public Int32 Upgrades_To = -1;
    }
}
