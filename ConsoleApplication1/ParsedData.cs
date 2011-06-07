using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SC2ParserApe
{
    public class ParsedData
    {
        /// <summary>
        /// Empty Constructor
        /// </summary>
        public ParsedData() { }

        public Int32 Build = -1;
        //TODO: Events
        public DateTime GameTime = new DateTime(); //Relative to the user's clock.
        public Int32 GameCTime = 0;
        public Int64 GameFileTime = 0;
        public Int32 GameLength = 0;
        public Boolean GamePublic = false;
        public String GameSpeed = String.Empty;
        public String MapName = String.Empty;
        public List<Message> ChatMessages = new List<Message>();
        public String Realm = String.Empty;
        public String RealTeamSize = String.Empty;
        public Int32 RecorderID = -1;
        public String TeamSize = String.Empty;
        public Int32 Version = -1;
        public Boolean WinnerKnown = false;
        public List<PlayerInfo> PlayersInfo = new List<PlayerInfo>();
        public List<Sc2Event> Events = new List<Sc2Event>();
    }
}
