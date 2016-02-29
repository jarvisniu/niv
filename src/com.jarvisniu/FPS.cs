using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.jarvisniu
{
    class FPS
    {
        int count = 0;
        int thisStartTick = -1;
        int lastStartTick = -1;

        public FPS()
        {
            lastStartTick = Environment.TickCount;
        }

        public void update()
        {
            count++;
            thisStartTick = Environment.TickCount;
            if (thisStartTick - lastStartTick > 999)
            {
                // Console.WriteLine("FPS:" + count);
                lastStartTick = thisStartTick;
                count = 0;
            }
        }
        
        // EOC
    }
}
