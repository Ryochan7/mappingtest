using mappingtester.ActionUtil;

namespace mappingtester.DPadActions
{
    public class DpadActionPad
    {
        public enum DPadMode : uint
        {
            Standard,
            EightWay,
            FourWayCardinal,
            FourWayDiagonal,
        }

        private DpadDirections previous = DpadDirections.Centered;
        private DpadDirections current = DpadDirections.Centered;
        private DpadDirections activeDir = DpadDirections.Centered;
        public bool IsActive => current != DpadDirections.Centered;
        private DPadMode currentMode = DPadMode.Standard;
        public DPadMode DpadMode
        {
            get => currentMode;
            set => currentMode = value;
        }

        private ActionButton[] actBtns = new ActionButton[13]
        {
            null,
            new ActionButton(),
            new ActionButton(),
            new ActionButton(),
            new ActionButton(),
            null,
            new ActionButton(),
            null,
            new ActionButton(),
            new ActionButton(),
            null,
            null,
            new ActionButton(),
        };
        private ActionButton activeBtn;

        public bool activeEvent;

        public DpadActionPad()
        {
        }

        public void Prepare(Tester mapper, DpadDirections value)
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
                activeBtn = actBtns[(int)activeDir];
            }
        }

        public void Event(Tester mapper)
        {
            activeBtn.Event(mapper, activeEvent);
        }

        public void Release(Tester mapper)
        {
            if (activeDir != DpadDirections.Centered)
            {
                current = DpadDirections.Centered;
                activeDir = current;
                activeEvent = false;
                activeBtn.Event(mapper, activeEvent);
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

        public ActionButton GetActionButton(DpadDirections dir)
        {
            return actBtns[(int)dir];
        }
    }
}
