using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_SEB_Eory
{
    public class PushUpEntry
    {
        public string Count { get; set; }
        public string Duration { get; set; }

        public PushUpEntry(string count)
        {
            this.Count = count;
        }

        public PushUpEntry()
        {
        }
    }
}
