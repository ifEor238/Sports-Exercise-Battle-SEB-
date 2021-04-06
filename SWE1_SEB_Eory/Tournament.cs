using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SWE1_SEB_Eory
{
    public class Tournament
    {
        public List<User> allUsers { get; set; } = new List<User>();
        public List<User> winners { get; set; } = new List<User>();
        public User winner { get; set; }

        Dictionary<User, History> history = new Dictionary<User, History>();

        public Tournament()
        { }

        public void AddUser(User user, int count, int duration)
        {
            this.allUsers.Add(user);
            this.history.Add(user, new History(count, duration));
        }

        public void cleanTournament()
        {
            history.Clear();
        }

        public void handleTournament()
        {

            foreach (KeyValuePair<User, History> kvp in history)
            {
                Console.WriteLine("Name = {0}, Score = {1}", kvp.Key.Name, kvp.Value.Count);
            }
            var maxScore = 0;
            for (int i = 0; i < allUsers.Count; i++)
            {
                if (history.ElementAt(i).Value.Count > maxScore)
                {
                    //winners.Add(allUsers[i]);
                    winner = allUsers[i];
                    maxScore = history.ElementAt(i).Value.Count;
                }
            }
            if (winners.Count > 1)
            {
                Console.WriteLine("Draw!");
                foreach (var winner in winners)
                {
                    Console.WriteLine("Drawee Name: {0}", winner.Name);
                }
            }
            else
            {
                Console.WriteLine("Winner: " + winner.Name);
            }
        }

        public void updateHistory(User user, int count, int duration)
        {
            history[user].addCount(count);
            history[user].addDuration(duration);
        }
    }
}
