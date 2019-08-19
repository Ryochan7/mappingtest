using mappingtester.ActionUtil;

namespace mappingtester.TouchpadActions
{
    public class TouchMouse
    {
        public enum TouchReportType : uint
        {
            None,
            Absolute,
            Relative,
        }

        public struct TouchReport
        {
            public TouchReportType reportType;
            public int valueX;
            public int valueY;
            public int deltaX;
            public int deltaY;
            public double elapsed;
            public double pressure;
            public bool clicked;
        }

        public enum ButtonMode : uint
        {
            None,
            One,
            Two,
            Three,
        }

        private ButtonMode currentBtnMode;
        public ButtonMode CurrentBtnMode
        {
            get => currentBtnMode;
            set => currentBtnMode = value;
        }
        private ActionButton[] touchButtons = new ActionButton[4];

        public bool activeEvent = false;

        public void Prepare(Tester mapper, ref TouchReport report)
        {
            activeEvent = true;
        }

        public void Event(Tester mapper)
        {
            activeEvent = false;
        }

        public void Release(Tester mapper)
        {
            activeEvent = false;
        }

        private void ReleaseTouchButtons(Tester mapper)
        {
            for (int i = 0; i < touchButtons.Length; i++)
            {
                touchButtons[i]?.Release(mapper);
            }
        }
    }
}
