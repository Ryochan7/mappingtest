using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mappingtester
{
    public class DPadTranslate
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
        private DpadDirections current = DpadDirections.Centered;
        public bool IsActive => current != DpadDirections.Centered;
        public bool activeEvent;

        public DPadTranslate()
        {
        }

        public void Prepare(Tester mapper, DpadDirections value)
        {
            if (value != current)
            {
                previous = current;
                current = value;
                activeEvent = true;
            }
        }

        public void Event(Tester mapper)
        {
            mapper.SetDPadEvent(current);
            if (current == DpadDirections.Centered) activeEvent = false;
        }

        public void Release(Tester mapper)
        {
            if (current != DpadDirections.Centered)
            {
                mapper.SetDPadEvent(DpadDirections.Centered);
            }
        }
    }
}
