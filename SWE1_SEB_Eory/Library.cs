using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_SEB_Eory
{
    public class Library
    {
        public List<PushUpEntry> PushUpEntryRecord = new List<PushUpEntry>();
        public User user { get; set; }

        public Library(User user)
        { this.user = user; }

        public void addMMC(PushUpEntry pue)
        {
            PushUpEntryRecord.Add(pue);
        }

        public void printLib(PushUpEntry pue)
        {
            Console.WriteLine("Push-Up-Library of User: " + user.Name + ":");
            foreach (PushUpEntry pueSingle in PushUpEntryRecord)
            {
                Console.WriteLine(pueSingle.Count);
                Console.WriteLine(pueSingle.Duration);
            }
        }
    }
}
