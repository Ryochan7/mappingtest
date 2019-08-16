namespace mappingtester.StickActions
{
    public abstract class StickActionTrans
    {
        public bool activeEvent = false;

        public abstract void Prepare(Tester mapper, int axisXVal, int axisYVal);
        public abstract void Event(Tester mapper);
        public abstract void Release(Tester mapper);
    }
}
