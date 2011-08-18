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
using SC2ParserApe;
using System.Xml;

namespace TheApeWriter
{
    class ApeWriter
    {
        static void Main(string[] args)
        {            
            Console.WriteLine("\nWelcome to SC2ParserApe's XML generator\n"
                             + "You may use the following arguments: \n"
                             + "\t-replayPath:PATH_OF_YOUR_REPLAY\t(alias: -rp:)\n"
                             + "\t-outputPath:OUTPUT_PATH_FOR_THE_XML\t(alias: -op:)\n");
            GetPaths(args);

            if (!System.IO.Directory.Exists(OutputPath))
            {
                System.IO.Directory.CreateDirectory(OutputPath);
            }
            if (!String.IsNullOrEmpty(OutputPath) && System.IO.Directory.Exists(OutputPath))
            {
                ParserApe parserApe = new ParserApe();
                foreach (String replayFilePath in ReplayFilePaths)
                {
                    if (!String.IsNullOrEmpty(replayFilePath) && System.IO.File.Exists(replayFilePath))
                    {
                        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                        timer.Start();
                        ParsedData dataParsed = parserApe.ParseReplay(replayFilePath);
                        timer.Stop();
                        Console.WriteLine("Replay parsed in: " + timer.ElapsedMilliseconds + " ms");

                        timer.Restart();
                        if (SaveParsedDataAsXML(dataParsed, System.IO.Path.GetFileNameWithoutExtension(replayFilePath)))
                        {
                            Console.WriteLine("Data successfully written in XML: " + System.IO.Path.GetFileName(replayFilePath));
                        }
                        else
                        {
                            Console.WriteLine("Failed to write data in XML: " + System.IO.Path.GetFileName(replayFilePath));
                        }
                        timer.Stop();
                        Console.WriteLine("Replay serialized in: " + timer.ElapsedMilliseconds + " ms");
                        timer.Reset();
                    }
                    else
                    {
                        Console.WriteLine("Invalid replay path: " + replayFilePath);
                    }
                }

                Console.WriteLine("Press enter to quit the console\n");
            }
            else
            {
                Console.WriteLine("You must enter a valid output path\n");
                return;
            }
        }

        static private void GetPaths(string[] args)
        {
            foreach (string arg in args)
            {
                if (arg.StartsWith("-replayPath:"))
                {
                    ReplayFilePaths.Add(arg.Remove(0, 12));
                }

                if (arg.StartsWith("-rp:"))
                {
                    ReplayFilePaths.Add(arg.Remove(0, 4));
                }

                if(arg.StartsWith("-outputPath:"))
                {
                    OutputPath = arg.Remove(0, 12);
                }

                if (arg.StartsWith("-op:"))
                {
                    OutputPath = arg.Remove(0, 4);
                }
            }
        }

        /// <summary>
        /// This is the interesting function here, serializes a ParsedData object in a XML file
        /// </summary>
        /// <param name="DataParsed">The data to write</param>
        /// <returns>True if it succeeded, false otherwise</returns>
        private static Boolean SaveParsedDataAsXML(ParsedData DataParsed, String ReplayName)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                
                XmlNode xmlnode = doc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
                doc.AppendChild(xmlnode);

                #region Serializing variable relative to everyone in the game.
                XmlElement generalVariables = doc.CreateElement("Game_Variables");
                
                generalVariables.AppendChild(SerializeElement(doc, "ReplayPath", DataParsed.ReplayPath));
                generalVariables.AppendChild(SerializeElement(doc, "Build", DataParsed.Build.ToString()));
                generalVariables.AppendChild(SerializeElement(doc, "GameTime", DataParsed.GameTime.ToString()));
                generalVariables.AppendChild(SerializeElement(doc, "GameCTime", DataParsed.GameCTime.ToString()));
                generalVariables.AppendChild(SerializeElement(doc, "GameFileTime", DataParsed.GameFileTime.ToString()));
                generalVariables.AppendChild(SerializeElement(doc, "GameLength", DataParsed.GameLength.ToString()));
                generalVariables.AppendChild(SerializeElement(doc, "GamePublic", DataParsed.GamePublic.ToString()));
                generalVariables.AppendChild(SerializeElement(doc, "GameSpeed", DataParsed.GameSpeed));
                generalVariables.AppendChild(SerializeElement(doc, "MapName", DataParsed.MapName));
                generalVariables.AppendChild(SerializeElement(doc, "Realm", DataParsed.Realm));
                generalVariables.AppendChild(SerializeElement(doc, "RealTeamSize", DataParsed.RealTeamSize));
                generalVariables.AppendChild(SerializeElement(doc, "RecorderID", DataParsed.RecorderID.ToString()));
                generalVariables.AppendChild(SerializeElement(doc, "TeamSize", DataParsed.TeamSize));
                generalVariables.AppendChild(SerializeElement(doc, "Version", DataParsed.Version));
                generalVariables.AppendChild(SerializeElement(doc, "WinnerKnown", DataParsed.WinnerKnown.ToString()));

                #endregion

                #region Serializing the chatlog
                XmlElement chatMessages = doc.CreateElement("ChatMessages");
                foreach (Message mess in DataParsed.ChatMessages)
                {
                    XmlElement singleMess = doc.CreateElement("SingleMessage");

                    singleMess.AppendChild(SerializeElement(doc, "MessageID", mess.MessageID.ToString()));
                    singleMess.AppendChild(SerializeElement(doc, "MessageName", mess.MessageName));
                    singleMess.AppendChild(SerializeElement(doc, "MessageTime", mess.MessageTime.ToString()));
                    singleMess.AppendChild(SerializeElement(doc, "MessageContent", mess.MessageContent));
                    singleMess.AppendChild(SerializeElement(doc, "MessageTarget", mess.MessageTarget.ToString()));

                    chatMessages.AppendChild(singleMess);
                }
                generalVariables.AppendChild(chatMessages);
                #endregion

                #region Serializing events.
                XmlElement events = doc.CreateElement("Events");
                foreach (Sc2Event sc2Event in DataParsed.Events)
                {
                    XmlElement singleSc2Event = doc.CreateElement("SingleEvent");

                    singleSc2Event.AppendChild(SerializeElement(doc, "PlayerID", sc2Event.PlayerID.ToString()));
                    singleSc2Event.AppendChild(SerializeElement(doc, "EventTime", sc2Event.EventTime.ToString()));
                    singleSc2Event.AppendChild(SerializeElement(doc, "EventAbilityID", sc2Event.EventAbilityID.ToString()));
                    singleSc2Event.AppendChild(SerializeSingleAbility(doc, sc2Event.EventAbility));

                    events.AppendChild(singleSc2Event);
                }
                generalVariables.AppendChild(events);
                #endregion

                #region Serializing players infos.
                XmlElement players = doc.CreateElement("Players");
                foreach (PlayerInfo playerInfo in DataParsed.PlayersInfo)
                {
                    XmlElement singlePlayer = doc.CreateElement("SinglePlayer");

                    singlePlayer.AppendChild(SerializeElement(doc, "Name", playerInfo.Name));
                    singlePlayer.AppendChild(SerializeElement(doc, "UID", playerInfo.UID.ToString()));
                    singlePlayer.AppendChild(SerializeElement(doc, "UIDIndex", playerInfo.UIDIndex.ToString()));
                    singlePlayer.AppendChild(SerializeElement(doc, "Color", playerInfo.Color.ToString()));
                    singlePlayer.AppendChild(SerializeElement(doc, "ApmTotal", playerInfo.ApmTotal.ToString()));
                    singlePlayer.AppendChild(SerializeElement(doc, "PType", playerInfo.PType));
                    singlePlayer.AppendChild(SerializeElement(doc, "Handicap", playerInfo.Handicap.ToString()));
                    singlePlayer.AppendChild(SerializeElement(doc, "Team", playerInfo.Team.ToString()));
                    singlePlayer.AppendChild(SerializeElement(doc, "LRace", playerInfo.LRace));
                    singlePlayer.AppendChild(SerializeElement(doc, "Race", playerInfo.Race));
                    singlePlayer.AppendChild(SerializeElement(doc, "ID", playerInfo.ID.ToString()));
                    singlePlayer.AppendChild(SerializeElement(doc, "isComputer", playerInfo.isComputer.ToString()));
                    singlePlayer.AppendChild(SerializeElement(doc, "isObserver", playerInfo.isObserver.ToString()));
                    singlePlayer.AppendChild(SerializeElement(doc, "Difficulty", playerInfo.Difficulty));
                    singlePlayer.AppendChild(SerializeElement(doc, "sColor", playerInfo.sColor));
                    singlePlayer.AppendChild(SerializeElement(doc, "sRace", playerInfo.sRace));
                    singlePlayer.AppendChild(SerializeElement(doc, "ColorIndex", playerInfo.ColorIndex.ToString()));
                    singlePlayer.AppendChild(SerializeElement(doc, "Won", playerInfo.Won.ToString()));

                    XmlElement playerApm = doc.CreateElement("Apm");
                    foreach (KeyValuePair<Int32, Int32> apm in playerInfo.Apm)
                    {
                        playerApm.AppendChild(SerializeElement(doc, "SingleApmValue", (apm.Key + ";" + apm.Value)));
                    }
                    singlePlayer.AppendChild(playerApm);

                    XmlElement playerNumEvents = doc.CreateElement("NumEvents");
                    foreach (KeyValuePair<Sc2Ability, Int32> numEvent in playerInfo.NumEvents)
                    {
                        XmlElement singleNumEvent = doc.CreateElement("NumEvent");

                        singleNumEvent.AppendChild(SerializeElement(doc, "TimesOccured", numEvent.Value.ToString()));
                        singleNumEvent.AppendChild(SerializeSingleAbility(doc, numEvent.Key));

                        playerNumEvents.AppendChild(singleNumEvent);
                    }
                    singlePlayer.AppendChild(playerNumEvents);

                    XmlElement playerFirstEvents = doc.CreateElement("FirstEvents");
                    foreach (KeyValuePair<Sc2Ability, Int32> firstEvent in playerInfo.FirstEvents)
                    {
                        XmlElement singleFirstEvent = doc.CreateElement("FirstEvent");

                        singleFirstEvent.AppendChild(SerializeElement(doc, "Time", firstEvent.Value.ToString()));
                        singleFirstEvent.AppendChild(SerializeSingleAbility(doc, firstEvent.Key));

                        playerFirstEvents.AppendChild(singleFirstEvent);
                    }
                    singlePlayer.AppendChild(playerFirstEvents);

                    players.AppendChild(singlePlayer);
                }
                generalVariables.AppendChild(players);
                #endregion

                doc.AppendChild(generalVariables);

                //Saving and returning true, serialization successful.
                doc.Save(OutputPath + "\\" + ReplayName + ".xml");               

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Prepares an Sc2Ability for serialization.
        /// </summary>
        /// <param name="doc">The XmlDocument being created</param>
        /// <param name="abilityToSerialize">The Sc2Ability to serialize</param>
        /// <returns>An XmlElement to append</returns>
        private static XmlElement SerializeSingleAbility(XmlDocument doc, Sc2Ability abilityToSerialize)
        {
            XmlElement eventAbility = doc.CreateElement("EventAbility");

            eventAbility.AppendChild(SerializeElement(doc, "AbilityCode", abilityToSerialize.AbilityCode.ToString()));
            eventAbility.AppendChild(SerializeElement(doc, "Description", abilityToSerialize.Description));
            eventAbility.AppendChild(SerializeElement(doc, "Name", abilityToSerialize.Name));
            eventAbility.AppendChild(SerializeElement(doc, "Type", abilityToSerialize.Type.ToString()));
            eventAbility.AppendChild(SerializeElement(doc, "TypeString", abilityToSerialize.TypeString));
            eventAbility.AppendChild(SerializeElement(doc, "SubType", abilityToSerialize.SubType.ToString()));
            eventAbility.AppendChild(SerializeElement(doc, "SubTypeString", abilityToSerialize.SubTypeString));
            eventAbility.AppendChild(SerializeElement(doc, "Mineral", abilityToSerialize.Mineral.ToString()));
            eventAbility.AppendChild(SerializeElement(doc, "Gas", abilityToSerialize.Gas.ToString()));
            eventAbility.AppendChild(SerializeElement(doc, "Supply", abilityToSerialize.Supply.ToString()));                       

            return eventAbility;
        }

        /// <summary>
        /// Prepares an element for serialization, this is to prevent code duplication.
        /// </summary>
        /// <param name="doc">The target XmlDocument</param>
        /// <param name="nodeName">The name of the new node to create</param>
        /// <param name="nodeValue">The value for that node</param>
        /// <returns></returns>
        private static XmlElement SerializeElement(XmlDocument doc, String nodeName, String nodeValue)
        {
            XmlElement newElement = doc.CreateElement(nodeName);
            newElement.InnerXml = nodeValue;
            return newElement;
        }

        //List of replay paths to be parsed, casted and serialized.
        private static List<String> ReplayFilePaths = new List<string>();

        //One output path to dump them all.
        private static String OutputPath = System.IO.Directory.GetCurrentDirectory() + "\\Replays parsed";
    }
}
