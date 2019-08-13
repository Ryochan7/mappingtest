using System;

namespace mappingtester.StickModifiers
{
    public class StickDeadZone
    {
        private double deadZone;
        private double maxZone = 1.0;
        private double antiDeadZone;
        private bool circleDead = true;
        public bool inSafeZone;

        public StickDeadZone(double deadZone, double maxZone, double antiDeadZone)
        {
            this.deadZone = deadZone;
            this.maxZone = maxZone;
            this.antiDeadZone = antiDeadZone;
        }

        public void CalcOutValues(int axisXDir, int axisYDir,
            int maxDirX, int maxDirY,
            out double xNorm, out double yNorm)
        {
            xNorm = 0.0;
            yNorm = 0.0;

            bool xNegative = axisXDir < 0;
            bool yNegative = axisYDir < 0;
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

        public bool ShouldInterpolate()
        {
            bool result = deadZone != 0.0 || maxZone != 1.0 || antiDeadZone != 0.0;
            return result;
        }

        public void Release()
        {
            inSafeZone = false;
        }
    }
}
