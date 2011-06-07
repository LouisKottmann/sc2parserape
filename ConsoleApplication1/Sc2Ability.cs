using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SC2ParserApe
{
    public class Sc2Ability
    {
        public Sc2Ability()
        { }

        public Int32 AbilityCode = -1;
        public String Description = String.Empty;
        public String Name = String.Empty;
        public Int32 Type = 0;
        public String TypeString = String.Empty;
        public Int32 SubType = 0;
        public String SubTypeString = String.Empty;
        public Int32 Mineral = -1;
        public Int32 Gas = -1;
        public Int32 Supply = -1;
    }
}
