namespace mappingtester.AxisModifiers
{
    public class AxisSensMod
    {
        private double sensMulti = 1.0;

        public const double MaxSensValue = 5.0;
        public const double MinSensValue = 0.0;

        public AxisSensMod()
        {
        }

        public AxisSensMod(double sensMulti)
        {
            if (MinSensValue <= sensMulti && sensMulti <= MaxSensValue)
                this.sensMulti = sensMulti;
        }

        public AxisSensMod(int sensMulti)
        {
            double tempSens = sensMulti / 100.0;
            if (MinSensValue <= tempSens && tempSens <= MaxSensValue)
                this.sensMulti = tempSens;
        }

        public double CalcOutValue(double currentAxisNorm)
        {
            currentAxisNorm = currentAxisNorm * sensMulti;
            double axisNorm = currentAxisNorm > 1.0 ? 1.0 : (currentAxisNorm < -1.0) ? -1.0 : currentAxisNorm;
            return axisNorm;
        }
    }
}
