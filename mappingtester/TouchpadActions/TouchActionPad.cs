namespace mappingtester.TouchpadActions
{
    public class TouchActionPad
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

        public bool activeEvent = false;

        public void Prepare(Tester mapper, ref TouchReport report)
        {

        }

        public void Event(Tester mapper)
        {

        }

        public void Release(Tester mapper)
        {

        }
    }
}
