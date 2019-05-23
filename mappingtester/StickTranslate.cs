using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mappingtester
{
    public class StickTranslate
    {
        private Tester.StickAlias id;

        private int axisMax;
        private int axisMin;
        private int axisMid;

        private double deadZone;
        private int _deadZoneP;
        private int _deadZoneM;

        private double maxZone;
        private int _maxZoneP;
        private int _maxZoneM;

        private double antiDeadZone;
        private int _antiDeadZoneP;
        private int _antiDeadZoneM;

        private bool circleDead;
        private bool runInter = false;

        private int previousXVal;
        private int previousYVal;

        private double xNorm = 0.0, yNorm = 0.0;
        public bool activeEvent = false;

        public StickTranslate(Tester.StickAlias id, int min, int max, int mid=0)
        {
            this.id = id;
            SetAxisRange(min, max);
            if (mid == 0 && min == 0)
                axisMid = (max + min + 1) / 2;
            else
                axisMid = mid;

            deadZone = 0.1;
            maxZone = 1.0;
            antiDeadZone = 0.25;
            circleDead = true;
            CalculateZonePoints();
            runInter = ShouldInterpolate();
        }

        public void Prepare(Tester mapper, int axisXVal, int axisYVal)
        {
            xNorm = 0.0; yNorm = 0.0;

            if (runInter)
            {
                int axisXTemp = 0, axisYTemp = 0;
                RunModifiers(axisXVal, axisYVal, out axisXTemp, out axisYTemp,
                    out xNorm, out yNorm);
                axisXVal = axisXTemp; axisYVal = axisYTemp;
            }
            else
            {
                int axisXDir = axisXVal - axisMid, axisYDir = axisYVal - axisMid;
                bool xNegative = axisXDir < 0;
                bool yNegative = axisYDir < 0;
                int maxDirX = (axisXVal >= axisMid ? axisMax : axisMin) - axisMid;
                int maxDirY = (axisYVal >= axisMid ? axisMax : axisMin) - axisMid;

                xNorm = (axisXDir / (double)maxDirX) * (xNegative ? -1.0 : 1.0);
                yNorm = (axisYDir / (double)maxDirY) * (yNegative ? -1.0 : 1.0);
            }

            activeEvent = true;
        }

        public void Event(Tester mapper)
        {
            mapper.SetStickEvent(id, xNorm, yNorm);
            activeEvent = false;
        }

        private void RunModifiers(int axisXVal, int axisYVal,
            out int axisXOut, out int axisYOut, out double xNorm, out double yNorm)
        {
            bool inSafeZone = false;
            int axisXDir = axisXVal - axisMid, axisYDir = axisYVal - axisMid;
            bool xNegative = axisXDir < 0;
            bool yNegative = axisYDir < 0;

            double angle = Math.Atan2(-(axisYDir), axisXDir);
            double angCos = Math.Abs(Math.Cos(angle)),
                angSin = Math.Abs(Math.Sin(angle));

            int maxDirX = (!xNegative ? axisMax : axisMin) - axisMid;
            int maxDirY = (!yNegative ? axisMax : axisMin) - axisMid;
            //int currentDeadX = (int)((!xNegative ? _deadZoneP : _deadZoneM) * angCos);
            //int currentDeadY = (int)((!yNegative ? _deadZoneP : _deadZoneM) * angSin);
            int currentDeadX = (int)(deadZone * maxDirX * angCos);
            int currentDeadY = (int)(deadZone * maxDirY * angSin);

            double stickDeadzoneSquared = (currentDeadX * currentDeadX) + (currentDeadY * currentDeadY);
            double stickSquared = Math.Pow(axisXDir, 2) + Math.Pow(axisYDir, 2);
            inSafeZone = stickSquared > stickDeadzoneSquared;
            if (inSafeZone)
            {
                double antiDeadX = antiDeadZone * angCos;
                double antiDeadY = antiDeadZone * angSin;
                
                //int maxZoneDirX = !xNegative ? _maxZoneP : _maxZoneM;
                //int maxZoneDirY = !yNegative ? _maxZoneP : _maxZoneM;
                int maxZoneDirX = (int)(maxZone * maxDirX);
                int maxZoneDirY = (int)(maxZone * maxDirY);

                int valueX = (axisXDir < 0 && axisXDir < maxZoneDirX) ? maxZoneDirX : (axisXDir > 0 && axisXDir > maxZoneDirX) ? maxZoneDirX : axisXDir;
                xNorm = (1.0 - antiDeadX) * ((valueX - currentDeadX) / (double)(maxZoneDirX - currentDeadX)) + antiDeadX;
                axisXOut = (int)(xNorm * maxDirX + axisMid);
                if (xNegative) xNorm *= -1.0;
                //Console.WriteLine("Val ({0}) Anti ({1}) Norm ({2})", valueX, antiDeadX, xNorm);

                int valueY = (axisYDir < 0 && axisYDir < maxZoneDirY) ? maxZoneDirY : (axisYDir > 0 && axisYDir > maxZoneDirY) ? maxZoneDirY : axisYDir;
                yNorm = (1.0 - antiDeadY) * ((valueY - currentDeadY) / (double)(maxZoneDirY - currentDeadY)) + antiDeadY;
                axisYOut = (int)(yNorm * maxDirY + axisMid);
                if (yNegative) yNorm *= -1.0;
            }
            else
            {
                xNorm = 0.0;
                yNorm = 0.0;
                axisXOut = axisMid;
                axisYOut = axisMid;
            }
        }

        public void Release(Tester mapper)
        {
            activeEvent = false;
            xNorm = 0.0; yNorm = 0.0;
            mapper.SetStickEvent(id, xNorm, yNorm);
        }

        private void SetAxisRange(int min, int max)
        {
            axisMin = min;
            axisMax = max;
        }

        private void CalculateZonePoints()
        {
            _deadZoneP = (int)(deadZone * (axisMax - axisMid));
            _deadZoneM = (int)(deadZone * (axisMin - axisMid));
            _maxZoneP = (int)(maxZone * (axisMax - axisMid));
            _maxZoneM = (int)(maxZone * (axisMin - axisMid));
            _antiDeadZoneP = (int)(antiDeadZone * (axisMax - axisMid));
            _antiDeadZoneM = (int)(antiDeadZone * (axisMin - axisMid));
        }

        private bool ShouldInterpolate()
        {
            bool result = deadZone != 0.0 || maxZone != 1.0 || antiDeadZone != 0.0;
            return result;
        }
    }
}
