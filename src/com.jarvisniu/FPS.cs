/**
 * FPS - Measure FPS of your animation.
 * Jarvis Niu(牛俊为) - http://jarvisniu.com/
 * MIT Licence
 * 
 * ## Usage
 *     1. Create a instance of ButtonAnimator.
 *         private FPS fps = new FPS();
 *     2. Add it into you animation loop.
 *         fps.update();
 *     3. It will output the fps in the standard output one time a second.
 *         "FPS: 60"
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.jarvisniu
{
    class FPS
    {
        // Update time in the current second
        int count = 0;

        // The timestamp of current tick(update)
        int thisStartTick = -1;

        // The timestamp when last FPS showed
        int lastStartTick = -1;

        // Constructor
        public FPS()
        {
            lastStartTick = Environment.TickCount;
        }

        // Record ticks(updates) of the animation loop
        public void update()
        {
            count++;
            thisStartTick = Environment.TickCount;
            if (thisStartTick - lastStartTick > 999)
            {
                Console.WriteLine("FPS:" + count);
                lastStartTick = thisStartTick;
                count = 0;
            }
        }

        // EOC
    }
}
