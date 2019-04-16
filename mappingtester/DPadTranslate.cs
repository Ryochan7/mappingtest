using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mappingtester
{
    class DPadTranslate
    {
        public enum DpadDirections : uint
        {
            Centered,
            Up = 1,
            Right = 2,
            UpRight = 3,
            Down = 4,
            Left = 8,
            UpLeft = 9,
            DownLeft = 12,
        }

        private DpadDirections previous = DpadDirections.Centered;

        public DPadTranslate()
        {

        }

        public void Event(Tester mapper, DpadDirections value)
        {
            if (value != previous)
            {
                previous = value;
                mapper.SetDPadEvent(value);
            }
        }
    }
}
