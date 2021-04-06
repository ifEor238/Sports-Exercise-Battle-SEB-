using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_SEB_Eory
{
    public class History
    {
        public User UserOne { get; set; }
        public int Count { get; set; }
        public int Duration { get; set; }

        public History()
        {

        }

        public History(int count, int duration)
        {
            this.Count = count;
            this.Duration = duration;
        }

        public void addCount(int count)
        {
            this.Count = this.Count + count;
        }

        public void addDuration(int dur)
        {
            if ((this.Duration + dur) > 120)
            {
                throw new InvalidOperationException("Duration is over 2 minutes.");
            }
            else
            {
                this.Duration = this.Duration + dur;
            }
        }
    }
}
