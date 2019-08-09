using System;
using mappingtester.StickModifiers;
using mappingtester.AxisModifiers;

namespace mappingtester
{
    public class StickMouse
    {
        private const int MOUSESPEEDFACTOR = 20;
        private const double MOUSESTICKOFFSET = 0.0495;
        private const int MOUSESPEED = 50;

        private int axisMax;
        private int axisMin;
        private int axisMid;

        private StickDeadZone deadMod;

        private bool runInter = false;
        private AxisOutCurves.Curve outCurve =
            AxisOutCurves.Curve.EnhancedPrecision;

        //private int previousXVal;
        //private int previousYVal;

        private int mouseXSpeed = MOUSESPEED;
        private int mouseYSpeed = MOUSESPEED;

        private double xNorm = 0.0, yNorm = 0.0;
        public bool activeEvent = false;

        public StickMouse(int min, int max, int mid = 0)
        {
            SetAxisRange(min, max);
            if (mid == 0 && min == 0)
                axisMid = (max + min + 1) / 2;
            else
                axisMid = mid;

            deadMod = new StickDeadZone(0.05, 1.0, 0.0);
            runInter = deadMod.ShouldInterpolate();
            mouseXSpeed = mouseYSpeed = MOUSESPEED;
        }

        public void Prepare(Tester mapper, int axisXVal, int axisYVal)
        {
            xNorm = yNorm = 0.0;

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

            activeEvent = xNorm != 0.0 || yNorm != 0.0;
        }

        public void Event(Tester mapper)
        {
            if (xNorm != 0.0 || yNorm != 0.0)
            {
                double xUnit = Math.Abs(xNorm);
                double yUnit = Math.Abs(yNorm);
                double tempMouseOffsetX = xUnit * MOUSESTICKOFFSET;
                double tempMouseOffsetY = yUnit * MOUSESTICKOFFSET;
                double mouseX = ((mouseXSpeed * MOUSESPEEDFACTOR * mapper.timeElapsed) - tempMouseOffsetX) * xNorm + (tempMouseOffsetX * (xNorm > 0.0 ? 1.0 : -1.0)) * mouseXSpeed;
                double mouseY = ((mouseYSpeed * MOUSESPEEDFACTOR * mapper.timeElapsed) - tempMouseOffsetY) * yNorm + (tempMouseOffsetY * (yNorm > 0.0 ? 1.0 : -1.0)) * mouseYSpeed;
                //Console.WriteLine("X{0} {1} Y{2} {3} {4}", mouseX, xNorm, mouseY, yNorm, mapper.timeElapsed);
                mapper.SetMouseCusorMovement(mouseX, mouseY);
            }
        }

        private void RunModifiers(int axisXVal, int axisYVal,
            out int axisXOut, out int axisYOut, out double xNorm, out double yNorm)
        {
            axisXOut = axisYOut = axisMid;
            int axisXDir = axisXVal - axisMid, axisYDir = axisYVal - axisMid;

            bool xNegative = axisXDir < 0;
            bool yNegative = axisYDir < 0;
            int maxDirX = (!xNegative ? axisMax : axisMin) - axisMid;
            int maxDirY = (!yNegative ? axisMax : axisMin) - axisMid;
            //int currentDeadX = (int)((!xNegative ? _deadZoneP : _deadZoneM) * angCos);
            //int currentDeadY = (int)((!yNegative ? _deadZoneP : _deadZoneM) * angSin);

            deadMod.CalcOutValues(axisXDir, axisYDir, maxDirX,
                maxDirY, out xNorm, out yNorm);
            if (deadMod.inSafeZone)
            {
                if (outCurve != AxisOutCurves.Curve.Linear)
                {
                    xNorm = AxisOutCurves.CalcOutValue(outCurve, xNorm);
                    yNorm = AxisOutCurves.CalcOutValue(outCurve, yNorm);
                }

                axisXOut = (int)(xNorm * maxDirX + axisMid);
                axisYOut = (int)(yNorm * maxDirY + axisMid);
            }
            else
            {
                xNorm = 0.0;
                yNorm = 0.0;
            }
        }

        public void Release(Tester mapper)
        {
            activeEvent = false;
            xNorm = 0.0;
            yNorm = 0.0;
        }

        public void SetSpeed(int speedX, int speedY)
        {
            mouseXSpeed = speedX;
            mouseYSpeed = speedY;
        }

        private void SetAxisRange(int min, int max)
        {
            axisMin = min;
            axisMax = max;
        }
    }
}
