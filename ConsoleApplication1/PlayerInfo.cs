using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SC2ParserApe
{
    public class PlayerInfo
    {
        public PlayerInfo()
        { }

        public String Name = String.Empty;
        public Int32 UID = -1;
        public Int32 UIDIndex = -1;
        public String Color = String.Empty;
        public Int32 ApmTotal = -1;
        public Dictionary<Int32, Int32> Apm = new Dictionary<Int32, Int32>();
        public String PType = String.Empty;
        public Int32 Handicap = -1;
        public Int32 Team = -1;
        public String LRace = String.Empty;
        public String Race = String.Empty;
        public Int32 ID = -1;
        public Boolean isComputer = false;
        public Boolean isObserver = false;
        public String Difficulty = String.Empty;
        public String sColor = String.Empty;
        public String sRace = String.Empty;
        public Int32 ColorIndex = -1;
        public Boolean Won = false;
        public Dictionary<Sc2Ability, Int32> NumEvents = new Dictionary<Sc2Ability, Int32>(); //The key contains info on the ability, the value contains the number of times it appears.
        public Dictionary<Sc2Ability, Int32> FirstEvents = new Dictionary<Sc2Ability, Int32>(); //The key contains info on the ability, the value contains the time it appeared first.
    }
}
