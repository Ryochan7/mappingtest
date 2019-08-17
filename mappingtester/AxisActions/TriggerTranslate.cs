using mappingtester.AxisModifiers;

namespace mappingtester.AxisActions
{
    public class TriggerTranslate : AxisActionTrans
    {
        public enum ActivateStyle : uint
        {
            Normal,
            HipFire,
            HipFireAggresive
        }

        private Tester.AxisAlias id;
        public Tester.AxisAlias Id => id;

        private int min;
        private int max;

        private TriggerButton softPullBtn;
        private TriggerButton fullPullBtn;
        //private ActivateStyle activateMode;
        private TriggerButton activeButton;
        private TriggerButton previousButton;
        private bool releaseButton;
        //private StopWatch activateTimeDelay;

        private int softPullPoint;
        public int SoftFullPoint { get => softPullPoint; set => softPullPoint = value; }

        private bool useAnalog;
        private AxisModTypes.Mods usedMods;
        private AxisDeadZone deadZone;

        private int currentValue;
        private double axisNorm;
        public bool activeEvent = false;

        private bool runInter = false;

        public TriggerTranslate(Tester.AxisAlias id, int min, int max)
        {
            this.id = id;
            this.min = min;
            this.max = max;
            softPullPoint = (int)(0.25 * max);

            softPullBtn = new TriggerButton(this);
            fullPullBtn = new TriggerButton(this);
            activeButton = null;
            //activateMode = ActivateStyle.Normal;
            useAnalog = true;

            deadZone = new AxisDeadZone(0.2, 1.0, 0.0);
            runInter = deadZone.ShouldInterpolate();
            usedMods = AxisModTypes.Mods.DeadZone;
        }

        public override void Prepare(Tester mapper, int value)
        {
            if (useAnalog)
            {
                if (runInter)
                {
                    RunModifiers(value, out axisNorm);
                }
                else
                {
                    currentValue = value;
                }
            }
            else
            {
                if (value >= softPullPoint)
                {
                    if (value == max && activeButton != fullPullBtn)
                    {
                        releaseButton = true;
                        previousButton = activeButton;
                        activeButton = fullPullBtn;
                    }
                    else if (activeButton != softPullBtn)
                    {
                        releaseButton = true;
                        previousButton = activeButton;
                        activeButton = softPullBtn;
                    }

                    activeButton.Event(mapper, value);
                }
                else if (activeButton != null)
                {
                    releaseButton = true;
                    previousButton = activeButton;
                    activeButton = null;
                }
            }

            activeEvent = true;
        }

        public override void Event(Tester mapper)
        {
            if (useAnalog)
            {
                mapper.SetAxisEvent(id, axisNorm);
                if (axisNorm == 0.0) activeEvent = false;
            }
            else
            {
                if (releaseButton)
                {
                    previousButton.Event(mapper, currentValue);
                    previousButton = null;
                }

                if (activeButton != null)
                {
                    activeButton.Event(mapper, currentValue);
                    activeEvent = false;
                }
            }
        }

        public override void Release(Tester mapper)
        {
            if (useAnalog)
            {
                mapper.SetAxisEvent(id, 0.0);
            }
            else
            {
                previousButton = null;
                activeButton = null;
            }

            axisNorm = 0.0;
            activeEvent = false;
        }

        /*private void SetAxisRange(int min, int max)
        {
            this.min = min;
            this.max = max;
        }
        */

        public void RunModifiers(int value, out double axisNorm)
        {
            double tempNorm;
            if ((usedMods & AxisModTypes.Mods.DeadZone) ==
                AxisModTypes.Mods.DeadZone)
                deadZone.CalcOutValues(value, max, out tempNorm);
            else
                tempNorm = value / (double)max;

            axisNorm = tempNorm;
        }
    }
}

