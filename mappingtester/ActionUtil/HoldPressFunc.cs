using System.Diagnostics;

namespace mappingtester.ActionUtil
{
    public class HoldPressFunc : ActionFunc
    {
        private int durationMs;
        public int DurationMs { get => durationMs; set => durationMs = value; }

        private Stopwatch elapsed = new Stopwatch();
        private bool waited;

        public override void Event(Tester mapper, bool active)
        {
            if (this.active != active)
            {
                this.active = active;
                waited = false;

                if (active)
                {
                    elapsed.Restart();
                }
                else
                {
                    elapsed.Reset();
                }
            }

            if (active && !waited)
            {
                if (elapsed.ElapsedMilliseconds >= durationMs)
                {
                    waited = true;
                    // Execute system event
                    SendEvent(mapper);
                }
            }
        }

        public override void Release(Tester mapper)
        {
            if (active && waited)
            {
                active = false;
                waited = false;
                ReleaseEvent(mapper);
            }
        }
    }
}
