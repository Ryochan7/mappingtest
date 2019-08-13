namespace mappingtester.AxisModifiers
{
    public class AxisDeadZone
    {
        private double deadZone;
        private double maxZone = 1.0;
        private double antiDeadZone;
        public bool inSafeZone;

        public AxisDeadZone(double deadZone, double maxZone, double antiDeadZone)
        {
            this.deadZone = deadZone;
            this.maxZone = maxZone;
            this.antiDeadZone = antiDeadZone;
        }

        public void CalcOutValues(int value, int maxDirValue, out double axisNorm)
        {
            int maxZoneDirVal = (int)(maxZone * maxDirValue);
            axisNorm = value / (double)maxZoneDirVal;
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
