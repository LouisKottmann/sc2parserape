using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SC2ParserApe
{
    public class Message
    {
        public Int32 MessageID = -1; //Player ID
        public String MessageName = String.Empty; //Player name
        public Int32 MessageTime = -1; //Seconds elapsed since game start
        public String MessageContent = String.Empty; //Chatlog
        public Int32 MessageTarget = -1; //0 for all?
    }
}
