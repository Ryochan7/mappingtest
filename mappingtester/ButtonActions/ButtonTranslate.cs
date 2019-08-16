namespace mappingtester.ButtonActions
{
    public class ButtonTranslate : ButtonActionTrans
    {
        private Tester.ButtonAlias id;
        private uint value;

        public ButtonTranslate(Tester.ButtonAlias id, uint value)
        {
            this.id = id;
            this.value = value;
        }

        public override void Prepare(Tester _, bool status)
        {
            if (status != active)
            {
                previousState = active;
                active = status;
                activeEvent = true;
            }
        }

        public override void Event(Tester mapper)
        {
            mapper.SetButtonEvent(id, active, value);
            if (!active) activeEvent = false;
        }

        public override void Release(Tester mapper)
        {
            active = false;
            activeEvent = false;
            mapper.SetButtonEvent(id, false, value);
        }
    }
}
