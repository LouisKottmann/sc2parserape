/*This file is part of Sc2ParserApe
    Sc2ParserApe is an adaptation of phpsc2replay, making the very same data available in C# (parses *.sc2replay files).   
 
    Copyright (C) 2011  Louis Kottmann louis.kottmann@gmail.com

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

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

        public String ReplayPath = String.Empty;
        public Int32 Build = -1;
        //TODO: Events
        public DateTime GameTime = new DateTime(); //Relative to the user's GMT.
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
        public String Version = String.Empty;
        public Boolean WinnerKnown = false;
        public List<PlayerInfo> PlayersInfo = new List<PlayerInfo>();
        public List<Sc2Event> Events = new List<Sc2Event>();
    }
}
