namespace mappingtester.DPadActions
{
    public class DPadTranslate
    {
        public enum DPadMode : uint
        {
            Standard,
            FourWayCardinal,
            FourWayDiagonal,
        }

        private DpadDirections previous = DpadDirections.Centered;
        private DpadDirections current = DpadDirections.Centered;
        private DpadDirections activeDir = DpadDirections.Centered;
        public bool IsActive => current != DpadDirections.Centered;
        private DPadMode currentMode = DPadMode.Standard;
        public bool activeEvent;

        public DPadTranslate()
        {
        }

        public void Prepare(Tester _, DpadDirections value)
        {
            activeEvent = false;

            if (value != current)
            {
                previous = current;
                current = value;
                if (currentMode != DPadMode.Standard)
                    DetermineActiveDir();
                else
                    activeDir = current;

                activeEvent = true;
            }
        }

        public void Event(Tester mapper)
        {
            mapper.SetDPadEvent(current);
        }

        public void Release(Tester mapper)
        {
            if (activeDir != DpadDirections.Centered)
            {
                mapper.SetDPadEvent(DpadDirections.Centered);
                current = DpadDirections.Centered;
                activeDir = current;
                activeEvent = false;
            }
        }

        private void DetermineActiveDir()
        {
            if (currentMode == DPadMode.Standard)
            {
                activeDir = current;
            }
            else if (currentMode == DPadMode.FourWayCardinal)
            {
                if (current == DpadDirections.Up || current == DpadDirections.UpRight)
                {
                    activeDir = DpadDirections.Up;
                }
                else if (current == DpadDirections.Right || current == DpadDirections.DownRight)
                {
                    activeDir = DpadDirections.Right;
                }
                else if (current == DpadDirections.Down || current == DpadDirections.DownLeft)
                {
                    activeDir = DpadDirections.Down;
                }
                else if (current == DpadDirections.Left || current == DpadDirections.UpLeft)
                {
                    activeDir = DpadDirections.Left;
                }
            }
            else if (currentMode == DPadMode.FourWayDiagonal)
            {
                if (current == DpadDirections.Up || current == DpadDirections.UpRight)
                {
                    activeDir = DpadDirections.UpRight;
                }
                else if (current == DpadDirections.Right || current == DpadDirections.DownRight)
                {
                    activeDir = DpadDirections.DownRight;
                }
                else if (current == DpadDirections.Down || current == DpadDirections.DownLeft)
                {
                    activeDir = DpadDirections.DownLeft;
                }
                else if (current == DpadDirections.Left || current == DpadDirections.UpLeft)
                {
                    activeDir = DpadDirections.UpLeft;
                }
            }
        }
    }
}
