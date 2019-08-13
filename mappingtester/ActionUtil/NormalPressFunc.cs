namespace mappingtester.ActionUtil
{
    public class NormalPressFunc : ActionFunc
    {
        public NormalPressFunc() : base()
        {
        }

        public NormalPressFunc(OutMode mode, uint code) : base(mode, code)
        {
        }

        public override void Event(Tester mapper, bool active)
        {
            if (this.active != active)
            {
                this.active = active;
                SendEvent(mapper);
            }
        }

        public override void Release(Tester mapper)
        {
            active = false;
            ReleaseEvent(mapper);
        }
    }
}
