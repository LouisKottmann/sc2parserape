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
using PHP.Core;

namespace SC2ParserApe
{
    /// <summary>
    /// Wrapper class: retrieves and casts variables to use in C#.
    /// Tip: binary data (e.g. that are read form file) are returned as byte[], since PHP is not fully Unicode
    /// </summary>
    public class ParserApe
    {
        /// <summary>
        /// Only fills in the abilities types and subtypes + the difficulties.
        /// </summary>
        public ParserApe()
        {
            //String stuff = "sdfgv436413";
            //String getIT = String.Empty;
            //ICSharpCode.SharpZipLib.BZip2.BZip2.Decompress(new System.IO.MemoryStream(stuff), , true);
            //System.IO.Compression.GZipStream
            AbilityType[0] = "undefined type";
            AbilityType[1] = "normal units";
            AbilityType[2] = "workers, Drone/SCV/Probe, MULE calldown";
            AbilityType[3] = "any kind of buildings";
            AbilityType[4] = "building addons(terran)";
            AbilityType[5] = "any kind of upgrade";
            AbilityType[6] = "a unit or building ability that is not an upgrade";
            AbilityType[7] = "transform a building to another building";
            AbilityType[8] = "anything that doesn't fit into earlier categories";

            AbilitySubtype[0] = "undefined subtype";
            AbilitySubtype[1] = "opposite of cancel";
            AbilitySubtype[2] = "cancel";

            Difficulties[0] = "Very easy";
            Difficulties[1] = "Easy";
            Difficulties[2] = "Medium";
            Difficulties[3] = "Hard";
            Difficulties[4] = "Very Hard";
            Difficulties[5] = "Insane";

            sc2maps = SC2ReplayUtils.getMapArray(ScriptContext.CurrentContext) as PhpArray;
        }

        public ParsedData ParseReplay(String Path)
        {
            DataParsed = new ParsedData();

            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            var mpqfile = new MPQFile(Path);            

            if (mpqfile != null)
            {
                var replay = mpqfile.parseReplay() as SC2Replay;

                if (replay != null)
                {
                    timer.Stop();
                    Console.WriteLine("Parsed replay in: " + timer.ElapsedMilliseconds + " ms");
                    timer.Restart();

                    Debug.Assert(replay != null, "Unexpected mpqfile.parseReplay() result.");

                    //Retrieving general values.
                    DataParsed.ReplayPath = Path;
                    DataParsed.Build = System.Convert.ToInt32(replay.getBuild());
                    DataParsed.GameCTime = System.Convert.ToInt32(replay.getCtime());
                    DataParsed.GameFileTime = System.Convert.ToInt64(replay.getFiletime());
                    DataParsed.GameLength = System.Convert.ToInt32(replay.getGameLength());
                    DataParsed.GamePublic = System.Convert.ToBoolean(replay.isGamePublic());
                    DataParsed.GameSpeed = replay.getGameSpeedText() as String;
                    DataParsed.MapName = UTF8.GetString(replay.getMapName() as byte[]);
                    DataParsed.Realm = UTF8.GetString(replay.getRealm() as byte[]);
                    DataParsed.RealTeamSize = replay.getRealTeamSize() as String;
                    DataParsed.RecorderID = System.Convert.ToInt32(replay.getRecorder());
                    DataParsed.TeamSize = UTF8.GetString(replay.getTeamSize() as byte[]);
                    DataParsed.Version = mpqfile.getVersionString() as String;                    
                    DataParsed.WinnerKnown = System.Convert.ToBoolean(replay.isWinnerKnown());

                    //Attempt to translate the name of the map.
                    if (DataParsed.Realm != "US")
                    {
                        String translation = String.Empty;
                        if (findMap(out translation))
                        {
                            DataParsed.MapName = DataParsed.MapName + " (" + translation + ")";
                        }
                    }

                    //Converting CTime to DateTime.
                    DateTime CTimeBaseDate = new DateTime(1970, 1, 1);
                    DataParsed.GameTime = CTimeBaseDate.AddSeconds(DataParsed.GameCTime);

                    //Retrieveing ChatLog.
                    var chatLog = replay.getMessages(ScriptContext.CurrentContext) as PhpArray;
                    if (chatLog != null)
                    {
                        foreach (var messageLogged in chatLog)
                        {
                            PhpArray messageInfo = (messageLogged.Value as PhpArray);
                            Message newMessage = new Message();
                            newMessage.MessageID = System.Convert.ToInt32(messageInfo["id"]);
                            newMessage.MessageName = ((messageInfo["name"] as PhpBytes) != null ? 
                                UTF8.GetString((messageInfo["name"] as PhpBytes).Data as byte[]) : "NoName");
                            newMessage.MessageTarget = System.Convert.ToInt32(messageInfo["target"]);
                            newMessage.MessageTime = System.Convert.ToInt32(messageInfo["time"]);
                            newMessage.MessageContent = ((messageInfo["message"] as PhpBytes) != null ?
                                UTF8.GetString((messageInfo["message"] as PhpBytes).Data as byte[]) : String.Empty);
                            DataParsed.ChatMessages.Add(newMessage);
                        }
                    }

                    //Retrieving values from the player array.
                    var players = replay.getPlayers(ScriptContext.CurrentContext) as PhpArray;
                    foreach (var player in players)
                    {
                        PhpArray info = (player.Value as PhpArray);
                        PlayerInfo newPlayer = new PlayerInfo();

                        //Define first who is the "player".
                        newPlayer.isComputer = System.Convert.ToBoolean(info["isComp"]);
                        newPlayer.isObserver = System.Convert.ToBoolean(info["isObs"]);

                        //These get created whichever kind of "player' it is.
                        newPlayer.Name = UTF8.GetString((info["name"] as PhpBytes).Data as byte[]);
                        newPlayer.Team = System.Convert.ToInt32(info["team"]);
                        newPlayer.ID = System.Convert.ToInt32(info["id"]);
                        newPlayer.sColor = info["sColor"] as String;

                        if (!newPlayer.isObserver)
                        {
                            //These are only for real players.
                            Int32 DifficultyID = System.Convert.ToInt32(info["difficulty"]);
                            newPlayer.Difficulty = Difficulties[DifficultyID];
                            newPlayer.Color = info["color"] as String;
                            newPlayer.PType = info["ptype"] as String;
                            newPlayer.Handicap = System.Convert.ToInt32(UTF8.GetString((info["handicap"] as PhpBytes).Data as byte[]));
                            newPlayer.LRace = UTF8.GetString((info["lrace"] as PhpBytes).Data as byte[]);
                            newPlayer.Race = UTF8.GetString(((info["race"] as PhpBytes) != null ? (info["race"] as PhpBytes) : new PhpBytes("")).Data as byte[]);
                            newPlayer.sRace = UTF8.GetString((info["srace"] as PhpBytes).Data as byte[]);
                            newPlayer.ColorIndex = System.Convert.ToInt32(info["colorIndex"]);
                            newPlayer.Won = System.Convert.ToBoolean(System.Convert.ToInt32(info["won"]));

                            if (!newPlayer.isComputer)
                            {
                                newPlayer.UID = System.Convert.ToInt32(info["uid"]);
                                newPlayer.UIDIndex = System.Convert.ToInt32(info["uidIndex"]);
                                newPlayer.ApmTotal = System.Convert.ToInt32(info["apmtotal"]);
                                foreach (var apmVal in (info["apm"] as PhpArray))
                                {
                                    newPlayer.Apm.Add(System.Convert.ToInt32(apmVal.Key.Integer), System.Convert.ToInt32(apmVal.Value));
                                }

                                //Parsing NumEvents, which is an array containing how many times the used abilities have been used.
                                var numEvents = info["numevents"] as PhpArray;
                                if (numEvents != null)
                                {
                                    foreach (var numEvent in numEvents)
                                    {
                                        Sc2Ability newEventAbility = new Sc2Ability();
                                        if (GetSc2Ability(numEvent.Key.Integer, out newEventAbility))
                                        {
                                            newPlayer.NumEvents.Add(newEventAbility, System.Convert.ToInt32(numEvent.Value));
                                        }
                                    }
                                }

                                //Parsing firstevents, which is an array containing when units were first seen.
                                var firstEvents = info["firstevents"] as PhpArray;
                                if (firstEvents != null)
                                {
                                    foreach (var firstEvent in firstEvents)
                                    {
                                        Sc2Ability newEventAbility = new Sc2Ability();
                                        if (GetSc2Ability(firstEvent.Key.Integer, out newEventAbility))
                                        {
                                            newPlayer.FirstEvents.Add(newEventAbility, System.Convert.ToInt32(firstEvent.Value));
                                        }
                                    }
                                }
                            }
                        }
                        DataParsed.PlayersInfo.Add(newPlayer);
                    }

                    //Finally, retrieving values from sc2Replay->events.
                    var gameEvents = replay.getEvents(ScriptContext.CurrentContext) as PhpArray;
                    if (gameEvents != null)
                    {
                        foreach (var gameEvent in gameEvents)
                        {
                            Sc2Event newSc2Event = new Sc2Event();
                            PhpArray singleGameEventArray = gameEvent.Value as PhpArray;

                            newSc2Event.PlayerID = System.Convert.ToInt32(singleGameEventArray["p"]);
                            newSc2Event.EventTime = (System.Convert.ToDouble(singleGameEventArray["t"]) / 16);
                            newSc2Event.EventAbilityID = System.Convert.ToInt32(singleGameEventArray["a"]);
                            if (GetSc2Ability(newSc2Event.EventAbilityID, out newSc2Event.EventAbility))
                            {
                                DataParsed.Events.Add(newSc2Event);
                            }
                        }
                    }

                    timer.Stop();
                    Console.WriteLine("Values casted in: " + timer.ElapsedMilliseconds + " ms");
                    timer.Reset();

                    return DataParsed;
                }
                Console.WriteLine("Failed to parse the replay (replay empty)\n");
                return null;
            }
            Console.WriteLine("Failed to parse the replay (mpqfile empty)\n");
            return null;
        }

        /// <summary>
        /// Used to get and cast an Sc2Ability.
        /// </summary>
        /// <param name="eventPhpArray"></param>
        /// <returns>Sc2Ability object containing all the info</returns>
        private Boolean GetSc2Ability(Int32 abilityID, out Sc2Ability newEventAbility)
        {
            newEventAbility = new Sc2Ability();
            PhpArray ability = SC2ReplayUtils.getAbilityArray(ScriptContext.CurrentContext, abilityID, DataParsed.Build) as PhpArray;

            if (ability != null)
            {
                newEventAbility.AbilityCode = abilityID;
                newEventAbility.Description = ability["desc"] as String;
                newEventAbility.Name = ability["name"] as String;
                newEventAbility.Type = System.Convert.ToInt32(ability["type"]);
                newEventAbility.TypeString = AbilityType[newEventAbility.Type];
                newEventAbility.SubType = System.Convert.ToInt32(ability["subtype"]);
                newEventAbility.SubTypeString = AbilitySubtype[newEventAbility.SubType];
                newEventAbility.Mineral = System.Convert.ToInt32(ability["min"]);
                newEventAbility.Gas = System.Convert.ToInt32(ability["gas"]);
                newEventAbility.Supply = System.Convert.ToInt32(ability["sup"]);

                return true;
            }

            return false;            
        }

        private Boolean findMap(out String translatedName) 
        {
            foreach (KeyValuePair<IntStringKey, object> map in sc2maps)
            {
                PhpArray singleMapArray = map.Value as PhpArray;
                if (singleMapArray["enUS"] as String == "Xel'Naga Caverns")
                { }
                foreach (var mapTranslation in singleMapArray)
                {
                    if ((mapTranslation.Value as String) == DataParsed.MapName)
                    {
                        translatedName = singleMapArray["enUS"] as String;
                        return true;
                    }
                }
            }

            translatedName = "";
            return false; 
        }

        ParsedData DataParsed = new ParsedData();
        System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

        //Initialized in constructor.
        String[] AbilityType = new String[9];
        String[] AbilitySubtype = new String[3];
        String[] Difficulties = new String[6];
        PhpArray sc2maps = null;        
    }
}
