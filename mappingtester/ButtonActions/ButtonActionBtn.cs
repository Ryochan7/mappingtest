using mappingtester.ActionUtil;

namespace mappingtester.ButtonActions
{
    public class ButtonActionBtn
    {
        private Tester.ButtonAlias id;
        private bool active;
        public bool IsActive => active;

        private bool previousState;
        public bool activeEvent = false;

        private ActionButton actBtn;

        public ButtonActionBtn(Tester.ButtonAlias id)
        {
            this.id = id;
            actBtn = new ActionButton();
            NormalPressFunc func = new NormalPressFunc();
            actBtn.AddActionFunc(func);
        }

        public void Prepare(Tester _, bool active)
        {
            if (this.active != active)
            {
                previousState = this.active;
                this.active = active;
                activeEvent = true;
            }
        }

        public void Event(Tester mapper)
        {
            actBtn.Event(mapper, active);
            //mapper.SetButtonEvent(id, active, value);
            if (!active) activeEvent = false;
        }

        public void Release(Tester mapper)
        {
            active = false;
            activeEvent = false;
            actBtn.Release(mapper);
            //mapper.SetButtonEvent(id, false, value);
        }
    }
}
