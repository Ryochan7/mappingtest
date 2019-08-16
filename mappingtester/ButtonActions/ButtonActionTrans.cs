namespace mappingtester.ButtonActions
{
    public abstract class ButtonActionTrans
    {
        protected bool active;
        protected bool previousState;
        public bool IsActive => active;
        public bool activeEvent = false;

        public abstract void Prepare(Tester mapper, bool active);
        public abstract void Event(Tester mapper);
        public abstract void Release(Tester mapper);
    }
}
