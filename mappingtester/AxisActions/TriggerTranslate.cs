using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mappingtester
{
    public class TriggerTranslate
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
        private int previousValue;

        private TriggerButton softPullBtn;
        private TriggerButton fullPullBtn;
        private ActivateStyle activateMode;
        private TriggerButton activeButton;
        private TriggerButton previousButton;
        private bool releaseButton;
        //private StopWatch activateTimeDelay;

        private int softPullPoint;
        public int SoftFullPoint { get => softPullPoint; set => softPullPoint = value; }

        private bool useAnalog;
        private double analogDeadZone;
        private int _analogDeadZone;
        private double analogMaxZone;
        private int _analogMaxZone;

        private int currentValue;
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
            activateMode = ActivateStyle.Normal;

            useAnalog = true;
            analogDeadZone = 0.2;
            analogMaxZone = 1.0;

            CalculateZonePoints();
            runInter = ShouldInterpolate();
        }

        public void Prepare(Tester mapper, int value)
        {
            if (useAnalog)
            {
                if (runInter)
                {
                    Modifiers(value, out int temp, out double axisNorm);
                    value = currentValue = temp;
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

        public void Event(Tester mapper)
        {
            if (useAnalog)
            {
                double axisNorm = currentValue / (double)max;
                mapper.SetAxisEvent(id, axisNorm);
            }
            else
            {
                if (releaseButton)
                {
                    previousButton.Event(mapper, currentValue);
                    previousButton = null;
                }

                if (activeButton != null)
                    activeButton.Event(mapper, currentValue);
            }
        }

        private void SetAxisRange(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        private void CalculateZonePoints()
        {
            _analogDeadZone = (int)(analogDeadZone * max);
            _analogMaxZone = (int)(analogMaxZone * max);
        }

        public void Modifiers(int value, out int axisValue, out double axisNorm)
        {
            axisValue = value;
            axisNorm = axisValue / (double)_analogMaxZone;
        }

        public void Release()
        {
            previousButton = null;
            activeButton = null;
            activeEvent = false;
        }

        private bool ShouldInterpolate()
        {
            bool result = analogDeadZone != 0.0 || analogMaxZone != 1.0;
            return result;
        }
    }
}

