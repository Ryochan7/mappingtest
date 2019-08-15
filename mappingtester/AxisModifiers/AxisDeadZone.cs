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

        public void CalcOutValues(int valueDir, int maxDirValue, out double axisNorm)
        {
            double maxZoneDirVal = maxZone * maxDirValue;
            bool negative = valueDir < 0;
            axisNorm = valueDir / (negative ? -maxZoneDirVal : maxZoneDirVal);
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
