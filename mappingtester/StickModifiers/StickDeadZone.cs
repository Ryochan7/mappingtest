using System;

namespace mappingtester.StickModifiers
{
    public class StickDeadZone
    {
        private double deadZone;
        private double maxZone = 1.0;
        private double antiDeadZone;
        private bool circleDead = true;

        private double deadZoneX;
        private double deadZoneY;

        private double maxZoneX = 1.0;
        private double maxZoneY = 1.0;

        private double antiDeadZoneX;
        private double antiDeadZoneY;

        public bool inSafeZone;

        public bool CircleDead { get => circleDead; set => circleDead = value; }

        public StickDeadZone(double deadZone, double maxZone, double antiDeadZone)
        {
            this.deadZone = deadZone;
            this.maxZone = maxZone;
            this.antiDeadZone = antiDeadZone;
        }

        public StickDeadZone(double deadZoneX, double deadZoneY,
            double maxZoneX, double maxZoneY,
            double antiDeadZoneX, double antiDeadZoneY)
        {
            this.deadZoneX = deadZoneX;
            this.deadZoneY = deadZoneY;
            this.maxZoneX = maxZoneX;
            this.maxZoneY = maxZoneY;
            this.antiDeadZoneX = antiDeadZoneX;
            this.antiDeadZoneY = antiDeadZoneY;
            circleDead = false;
        }

        public void CalcOutValues(int axisXDir, int axisYDir,
            int maxDirX, int maxDirY,
            out double xNorm, out double yNorm)
        {
            bool xNegative = axisXDir < 0;
            bool yNegative = axisYDir < 0;

            if (circleDead)
            {
                double angle = Math.Atan2(-(axisYDir), axisXDir);
                double angCos = Math.Abs(Math.Cos(angle)),
                    angSin = Math.Abs(Math.Sin(angle));

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
                    if (xNegative) xNorm *= -1.0;
                    //Console.WriteLine("Val ({0}) Anti ({1}) Norm ({2})", valueX, antiDeadX, xNorm);

                    int valueY = (axisYDir < 0 && axisYDir < maxZoneDirY) ? maxZoneDirY : (axisYDir > 0 && axisYDir > maxZoneDirY) ? maxZoneDirY : axisYDir;
                    yNorm = (1.0 - antiDeadY) * ((valueY - currentDeadY) / (double)(maxZoneDirY - currentDeadY)) + antiDeadY;
                    if (yNegative) yNorm *= -1.0;
                }
                else
                {
                    xNorm = 0.0;
                    yNorm = 0.0;
                }
            }
            else
            {
                int currentDeadX = (int)(deadZoneX * maxDirX);
                int currentDeadY = (int)(deadZoneY * maxDirY);

                inSafeZone = axisXDir >= 0 ? (axisXDir > currentDeadX) :
                    (axisXDir < currentDeadX);
                if (inSafeZone)
                {
                    double antiDeadX = antiDeadZoneX;
                    int maxZoneDirX = (int)(maxZoneX * maxDirX);
                    int valueX = (axisXDir < 0 && axisXDir < maxZoneDirX) ? maxZoneDirX : (axisXDir > 0 && axisXDir > maxZoneDirX) ? maxZoneDirX : axisXDir;
                    xNorm = (1.0 - antiDeadX) * ((valueX - currentDeadX) / (double)(maxZoneDirX - currentDeadX)) + antiDeadX;
                    if (xNegative) xNorm *= -1.0;
                }
                else
                {
                    xNorm = 0.0;
                }

                inSafeZone = axisYDir >= 0 ? (axisYDir > currentDeadY) : (axisYDir < currentDeadY);
                if (inSafeZone)
                {
                    double antiDeadY = antiDeadZoneY;
                    int maxZoneDirY = (int)(maxZoneY * maxDirY);
                    int valueY = (axisYDir < 0 && axisYDir < maxZoneDirY) ? maxZoneDirY : (axisYDir > 0 && axisYDir > maxZoneDirY) ? maxZoneDirY : axisYDir;
                    yNorm = (1.0 - antiDeadY) * ((valueY - currentDeadY) / (double)(maxZoneDirY - currentDeadY)) + antiDeadY;
                    if (yNegative) yNorm *= -1.0;
                }
                else
                {
                    yNorm = 0.0;
                }
            }
        }

        public bool ShouldInterpolate()
        {
            bool result;
            if (circleDead)
            {
                result = deadZone != 0.0 || maxZone != 1.0 || antiDeadZone != 0.0;
            }
            else
            {
                result = deadZoneX != 0.0 || deadZoneY != 0.0 ||
                    maxZoneX != 1.0 || maxZoneY != 1.0 ||
                    antiDeadZoneX != 0.0 || antiDeadZoneY != 0.0;
            }

            return result;
        }

        public void Release()
        {
            inSafeZone = false;
        }
    }
}
