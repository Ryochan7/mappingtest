namespace mappingtester.ButtonActions
{
    public class ButtonTranslate
    {
        private Tester.ButtonAlias id;
        private uint value;

        private bool active;
        public bool IsActive => active;

        private bool previousState;
        public bool activeEvent = false;

        public ButtonTranslate(Tester.ButtonAlias id, uint value)
        {
            this.id = id;
            this.value = value;
        }

        public void Prepare(Tester _, bool status)
        {
            if (status != active)
            {
                previousState = active;
                active = status;
                activeEvent = true;
            }
        }

        public void Event(Tester mapper)
        {
            mapper.SetButtonEvent(id, active, value);
            if (!active) activeEvent = false;
        }

        public void Release(Tester mapper)
        {
            active = false;
            activeEvent = false;
            mapper.SetButtonEvent(id, false, value);
        }
    }
}
