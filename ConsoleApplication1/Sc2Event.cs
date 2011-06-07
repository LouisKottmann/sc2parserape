using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SC2ParserApe
{
    public class Sc2Event
    {
        public Sc2Event()
        { }

        public Int32 PlayerID = -1;
        public Double EventTime = -1;
        public Int32 EventAbilityID = -1;
        public Sc2Ability EventAbility = null;
    }
}
