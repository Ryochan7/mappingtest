using System;

namespace mappingtester
{
    class StickMouse
    {
        private const int MOUSESPEEDFACTOR = 20;
        private const double MOUSESTICKOFFSET = 0.0495;

        private int axisMax;
        private int axisMin;
        private int axisMid;

        private double deadZone;
        private double maxZone;
        private double antiDeadZone;

        private bool circleDead;
        private bool runInter = false;

        private int previousXVal;
        private int previousYVal;

        private int mouseXSpeed;
        private int mouseYSpeed;

        public StickMouse(int min, int max, int mid = 0)
        {
            SetAxisRange(min, max);
            if (mid == 0 && min == 0)
                axisMid = (max + min + 1) / 2;
            else
                axisMid = mid;

            deadZone = 0.05;
            maxZone = 1.0;
            antiDeadZone = 0.0;
            circleDead = true;
            runInter = ShouldInterpolate();
            mouseXSpeed = mouseYSpeed = 50;
        }

        public void Event(Tester mapper, int axisXVal, int axisYVal)
        {
            double xNorm = 0.0, yNorm = 0.0;

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

            if (xNorm != 0.0 || yNorm != 0.0)
            {
                double xUnit = Math.Abs(xNorm);
                double yUnit = Math.Abs(yNorm);
                double tempMouseOffsetX = xUnit * MOUSESTICKOFFSET;
                double tempMouseOffsetY = yUnit * MOUSESTICKOFFSET;
                double mouseX = ((mouseXSpeed * MOUSESPEEDFACTOR * mapper.timeElapsed) - tempMouseOffsetX) * xNorm + (tempMouseOffsetX * (xNorm > 0.0 ? 1.0 : -1.0));
                double mouseY = ((mouseYSpeed * MOUSESPEEDFACTOR * mapper.timeElapsed) - tempMouseOffsetY) * yNorm + (tempMouseOffsetY * (yNorm > 0.0 ? 1.0 : -1.0));
                //Console.WriteLine("X{0} {1} Y{2} {3} {4}", mouseX, xNorm, mouseY, yNorm, mapper.timeElapsed);
                mapper.SetMouseCusorMovement(mouseX, mouseY);
            }
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

        private void SetAxisRange(int min, int max)
        {
            axisMin = min;
            axisMax = max;
        }

        private bool ShouldInterpolate()
        {
            bool result = deadZone != 0.0 || maxZone != 1.0 || antiDeadZone != 0.0;
            return result;
        }
    }
}
