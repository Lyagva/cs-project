using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customtimer
{
    internal class OurTimer
    {
        private long seconds;

        public OurTimer(long seconds = 0)
        {
            this.seconds = seconds;
        }

        public void setSeconds(long seconds)
        {
            this.seconds = seconds;
        }

        public long getAllSeconds()
        {
            return seconds;
        }

        public void incSeconds()
        {
            ++seconds;
        }

        public int getSeconds()
        {
            return (int)(seconds % 60);
        }

        public int getMinutes()
        {
            return (int)(seconds % 3600 / 60);
        }

        public long getHours()
        {
            return seconds / 3600;
        }
    }
}
