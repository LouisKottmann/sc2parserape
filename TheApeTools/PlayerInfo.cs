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
