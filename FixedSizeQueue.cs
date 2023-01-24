using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Push-only string fixed size queue 
 * 
 */

namespace Sharpvin
{
    internal class FixedStringQueue
    {
        //fields
        private String[] queue;
        public int Length { get; private set; }

        //constructor
        private FixedStringQueue(int length) 
        {
            this.queue = new String[length];
            this.Length = length;
        }

        //factory method
        public static FixedStringQueue Create(int length)
        {
            if (length < 1)
            {
                throw new ArgumentOutOfRangeException("FixedStringQueue - Size must be greater than 0");
            }
            else
            {
                return new FixedStringQueue(length);
            }
        }


        public void Push(String item)
        {
            for (int i = 0; i < this.Length - 1; i++)
            {
                this.queue[i] = this.queue[i + 1];
            }

            this.queue[this.Length - 1] = item;
        }

        public bool isRepeating()
        {
            String first = this.queue[0];

            foreach (String item in this.queue)
            {
                if (first != item)
                    return false;
            }

            return true;
        }
    }
}
