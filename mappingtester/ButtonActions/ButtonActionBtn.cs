using mappingtester.ActionUtil;

namespace mappingtester.ButtonActions
{
    public class ButtonActionBtn : ButtonActionTrans
    {
        private ActionButton actBtn;

        public ButtonActionBtn()
        {
            actBtn = new ActionButton();
            NormalPressFunc func = new NormalPressFunc();
            actBtn.AddActionFunc(func);
        }

        public override void Prepare(Tester _, bool active)
        {
            if (this.active != active)
            {
                previousState = this.active;
                this.active = active;
                activeEvent = true;
            }
        }

        public override void Event(Tester mapper)
        {
            actBtn.Event(mapper, active);
            if (!active) activeEvent = false;
        }

        public override void Release(Tester mapper)
        {
            active = false;
            activeEvent = false;
            actBtn.Release(mapper);
        }
    }
}
