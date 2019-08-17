namespace mappingtester.AxisActions
{
    public abstract class AxisActionTrans
    {
        public abstract void Prepare(Tester mapper, int value);

        public abstract void Event(Tester mapper);

        public abstract void Release(Tester mapper);
    }
}
