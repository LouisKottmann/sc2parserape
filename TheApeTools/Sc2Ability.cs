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
