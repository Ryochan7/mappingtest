using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mappingtester
{
    class AxisTranslate
    {
        private Tester.AxisAlias id;

        private int min;
        private int max;
        private int mid;

        private double deadZone;
        private int _deadZoneP;
        private int _deadZoneM;

        private double maxZone;
        private int _maxZoneP;
        private int _maxZoneM;

        private double antiDeadZone;
        private int _antiDeadZoneP;
        private int _antiDeadZoneM;

        private bool runInter = false;

        public AxisTranslate(Tester.AxisAlias id, int min, int max, int mid=0)
        {
            this.id = id;
            SetAxisRange(min, max);
            maxZone = 1.0;
            if (mid == 0 && min == 0)
            {
                this.mid = (max + min + 1) / 2;
            }
            else
            {
                this.mid = mid;
            }

            deadZone = 0.2;
            antiDeadZone = 0.2;
            maxZone = 1.0;
            CalculateZonePoints();
            runInter = ShouldInterpolate();
        }

        public void Event(Tester mapper, int value)
        {
            if (runInter)
            {
                Modifiers(value, out int tempval, out double axisNorm);
                mapper.SetAxisEvent(id, axisNorm);
            }
            else
            {
                double axisNorm = value / (double)max;
                mapper.SetAxisEvent(id, axisNorm);
            }
        }

        private void SetAxisRange(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public void Modifiers(int value, out int axisOutValue, out double axisNorm)
        {
            bool inSafeZone = false;
            int axisDir = value - mid;
            bool negative = axisDir < 0;
            int currentDead = (!negative ? _deadZoneP : _deadZoneM);
            inSafeZone = axisDir > currentDead;
            if (inSafeZone)
            {
                int maxDir = (value >= mid ? max : min) - mid;
                double maxZoneDir = maxDir * maxZone;

                double temp = (axisDir - currentDead) / (double)(maxDir - currentDead);
                axisNorm = (1.0 - antiDeadZone) * temp + antiDeadZone;
                axisOutValue = (int)(axisNorm * maxDir + mid);
                if (negative) axisNorm *= -1.0;
            }
            else
            {
                axisNorm = 0.0;
                axisOutValue = mid;
            }
        }

        private void CalculateZonePoints()
        {
            _deadZoneP = (int)(deadZone * (max - mid));
            _deadZoneM = (int)(deadZone * (min - mid));
            _maxZoneP = (int)(maxZone * (max - mid));
            _maxZoneM = (int)(maxZone * (min - mid));
            _antiDeadZoneP = (int)(antiDeadZone * (max - mid));
            _antiDeadZoneM = (int)(antiDeadZone * (min - mid));
        }

        private bool ShouldInterpolate()
        {
            bool result = deadZone != 0.0 || maxZone != 1.0 || antiDeadZone != 0.0;
            return result;
        }
    }
}
