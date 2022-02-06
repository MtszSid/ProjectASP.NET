using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Web;

namespace Projekt
{
    public class MainHub : Hub
    {
        static ConcurrentDictionary<string, GroupInfo> _dictionary = new ConcurrentDictionary<string, GroupInfo>();
        static Regex messageRegex = new Regex(@"[0-2],[0-2]");
        public override Task OnConnected()
        { 
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            var clientId = this.Context.ConnectionId;

            var groups = from e in _dictionary
                         where (e.Value.CrossesClientId == clientId
                         || e.Value.NoughtsClientId == clientId)
                         select e.Value.GroupId;

            foreach(var e in groups)
            {
                GiveUp(e);
            }

            return base.OnDisconnected(stopCalled);
        }
        public void MakeMove(string GroupName, string message)
        {
            if (!messageRegex.IsMatch(message))
            {
                return;
            }
            var messageSplit = message.Split(',');

            var clientId = Context.ConnectionId;

            var x = int.Parse(messageSplit[0]);
            var y = int.Parse(messageSplit[1]);

            GroupInfo group;
            _dictionary.TryGetValue(GroupName, out group);

            if (group == null)
            {
                return;
            }
            else if (group.Ended)
            { }
            if (!group.Started)
            {
                return;
            }

            string theOther;
            string symbol;

            if (clientId == group.CrossesClientId)
            {
                theOther = group.NoughtsClientId;
                symbol = "X";
            }
            else if (clientId == group.NoughtsClientId)
            {
                theOther = group.CrossesClientId;
                symbol = "O";
            }
            else
            {
                return;
            }

            if(group.Turn == clientId)
            {
                if(group.Board[x,y] == "")
                {
                    group.Board[x, y] = symbol;
                    group.Turn = theOther;
                }

                if (group.isWin())
                {
                    group.Ended = true;
                    group.Turn = "";
                }
                
            }   
        }
        public bool JoinGroup(string name, string RoomName)
        {
            var clientId = this.Context.ConnectionId;
            if (_dictionary.ContainsKey(RoomName) && !_dictionary[RoomName].Started)
            {
                var group = MainHub._dictionary[RoomName];
                lock (group)
                {
                    if (group.Started == false)
                    {
                        this.Groups.Add(clientId, RoomName);
                        group.NoughtsClientId = clientId;
                        group.NoughtsName = name;
                        group.Started = true;
                        return true;
                    }
                }
            }
            else if (!_dictionary.ContainsKey(RoomName))
            {
                var group = new GroupInfo(RoomName, clientId, name);
                this.Groups.Add(clientId, RoomName);
                _dictionary[RoomName] = group;
                return true;
            }
            return false;
        }
        public GroupInfo GetGameState(string groupId)
        {
            GroupInfo element;
            if(_dictionary.TryGetValue(groupId, out element))
            {
                return element;
            }
            return null;
        }

        public List<GroupElement> GetGroups()
        {
            List<GroupElement> list = (from elem in _dictionary
                                       where elem.Value.Started == false
                                       select new GroupElement(elem.Value.GroupId, elem.Value.CrossesName)).ToList();
            return list;
        }

        public List<FullGroupElement> GetStartedGroups()
        {
            List<FullGroupElement> list = (from elem in _dictionary
                                       where elem.Value.Started == true
                                       select new FullGroupElement(
                                           elem.Value.GroupId, 
                                           elem.Value.CrossesName,
                                           elem.Value.NoughtsName)).ToList();
            return list;
        }

        public void GiveUp(string GroupName)
        { 
            var clientId = this.Context.ConnectionId;
            if(_dictionary.ContainsKey(GroupName))
            {
                var group = _dictionary[GroupName];
                if (clientId == group.CrossesClientId)
                {
                    group.CrossesClientId = null;
                }
                if(clientId == group.NoughtsClientId)
                {
                    group.NoughtsClientId = null;
                }
                this.Groups.Remove(clientId, GroupName);
                group.Ended = true;
                if(group.NoughtsClientId == null && group.CrossesClientId == null)
                {
                    try 
                    {
                        GroupInfo groupInfo;
                        _dictionary.TryRemove(GroupName, out groupInfo); 
                    }
                    catch { }

                }
            }
        }
    }

    public class GroupInfo
    {
        public string Turn { get; set; }
        public string GroupId { get; set; }
        public string CrossesClientId { get; set; }
        public string NoughtsClientId { get; set; }
        public string CrossesName { get; set; }
        public string NoughtsName { get; set; }
        public bool   Started { get; set; }
        public bool   Ended { get; set; }
        public string Winner { get; set; }
        public string[,] Board { get; set; }

        public GroupInfo(string ID, string CLientId, string name)
        {
            this.Turn = CLientId;
            this.GroupId = ID;
            this.CrossesClientId = CLientId;
            this.CrossesName = name;
            this.NoughtsClientId = null;
            this.NoughtsName = null;
            this.Started = false;
            this.Ended = false;
            this.Winner = null;
            this.Board = new string[3, 3];

            for (int i = 0; i < 3; i++)
            {
                for ( int j = 0; j < 3; j++)
                {
                    this.Board[i, j] = "";
                }
            }
        }

        public bool isWin()
        {
            if(Winner != null)
            {
                return true;
            }
            for (int i = 0; i < 3; i++)
            {
                if (Board[i, 0] != "" && Board[i, 0] == Board[i, 1] && Board[i, 0] == Board[i, 2])
                {
                    if (Board[i, 0] == "X")
                    {
                        Winner = CrossesName;
                        return true;
                    }
                    if (Board[i, 0] == "O")
                    {
                        Winner = NoughtsName;
                        return true;
                    }
                }
            }
            for (int i = 0; i < 3; i++)
            {
                if (Board[0, i] != "" && Board[0, i] == Board[1, i] && Board[0, i] == Board[2, i])
                {
                    if (Board[0, i] == "X")
                    {
                        Winner = CrossesName;
                        return true;
                    }
                    if (Board[0, i] == "O")
                    {
                        Winner = NoughtsName;
                        return true;
                    }
                }
            }
            if (Board[0, 0] != "" && Board[0, 0] == Board[1, 1] && Board[0, 0] == Board[2, 2])
            {
                if (Board[0, 0] == "X")
                {
                    Winner = CrossesName;
                    return true;
                }
                if (Board[0, 0] == "O")
                {
                    Winner = NoughtsName;
                    return true;
                }
            }
            if (Board[0, 2] != "" && Board[0, 2] == Board[1, 1] && Board[0, 2] == Board[2, 0])
            {
                if (Board[0, 2] == "X")
                {
                    Winner = CrossesName;
                    return true;
                }
                if (Board[0, 2] == "O")
                {
                    Winner = NoughtsName;
                    return true;
                }
            }
            return false;
        }
    }
    public class GroupElement
    {
        public string GroupId { get; set; }
        public string playerName { get; set; }

        public GroupElement(string Id, string Name)
        {
            this.GroupId = Id;
            this.playerName = Name;
        }
    }

    public class FullGroupElement
    {
        public string GroupId { get; set; }
        public string playerName1 { get; set; }
        public string playerName2 { get; set; }

        public FullGroupElement(string Id, string Name1, string Name2)
        {
            this.GroupId = Id;
            this.playerName1 = Name1;
            this.playerName2 = Name2;
        }
    }
}