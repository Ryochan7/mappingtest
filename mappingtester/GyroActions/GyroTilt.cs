using mappingtester.ActionUtil;

namespace mappingtester.GyroActions
{
    public class GyroTilt
    {
        public struct AccelAxis
        {
            public int min;
            public int max;
            public int mid;
            public int current;
        }

        private AccelAxis accelXInfo;
        private AccelAxis accelYInfo;
        private AccelAxis accelZInfo;

        private ActionButton tiltUpBtn = new ActionButton();
        private ActionButton tiltDownBtn = new ActionButton();
        private ActionButton tiltYActiveBtn;
        private ActionButton tiltYPrevBtn;

        private ActionButton tiltLeftBtn = new ActionButton();
        private ActionButton tiltRightBtn = new ActionButton();
        private ActionButton tiltXActiveBtn;
        private ActionButton tiltXPrevBtn;

        private double timeElapsed;

        public GyroTilt()
        {

        }

        public GyroTilt(AccelAxis accelX, AccelAxis accelY, AccelAxis accelZ)
        {
            accelXInfo = accelX;
            accelYInfo = accelY;
            accelZInfo = accelZ;
        }

        public void Prepare(Tester _, int _1, int _2, int _3,
            int accelX, int accelY, int accelZ, double timeElapsed)
        {
            accelXInfo.current = accelX;
            accelYInfo.current = accelY;
            accelZInfo.current = accelZ;
            this.timeElapsed = timeElapsed;
        }

        public void Event(Tester mapper)
        {
            if (tiltXPrevBtn != null && tiltXPrevBtn != tiltXActiveBtn)
            {
                tiltXPrevBtn.Event(mapper, false);
                tiltXPrevBtn = null;
            }

            if (tiltYPrevBtn != null && tiltYPrevBtn != tiltYActiveBtn)
            {
                tiltYPrevBtn.Event(mapper, false);
                tiltYPrevBtn = null;
            }

            if (tiltXActiveBtn != null)
            {
                tiltXActiveBtn.Event(mapper, true);
            }

            if (tiltYActiveBtn != null)
            {
                tiltYActiveBtn.Event(mapper, true);
            }
        }

        public void Release(Tester mapper)
        {
            if (tiltXActiveBtn != null)
            {
                tiltXActiveBtn.Event(mapper, false);
            }

            if (tiltYActiveBtn != null)
            {
                tiltYActiveBtn.Event(mapper, false);
            }
        }
    }
}
