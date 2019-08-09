using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mappingtester.AxisModifiers
{
    public class AxisOutCurves
    {
        public enum Curve : uint
        {
            Linear,
            Quadratic,
            Cubic,
            EnhancedPrecision,
            EaseoutQuad,
            EaseoutCubic,
            Bezier,
        }

        public static double CalcOutValue(Curve type, double axisValue)
        {
            double outputValue = 0.0;
            switch(type)
            {
                case Curve.Linear:
                    outputValue = axisValue;
                    break;

                case Curve.Quadratic:
                    outputValue = (axisValue >= 0.0 ? 1.0 : -1.0) *
                        axisValue * axisValue;
                    break;

                case Curve.Cubic:
                    outputValue = axisValue * axisValue * axisValue;
                    break;

                case Curve.EnhancedPrecision:
                    double absVal = Math.Abs(axisValue);
                    double temp = outputValue;

                    if (absVal <= 0.4)
                    {
                        temp = 0.55 * absVal;
                    }
                    else if (absVal <= 0.75)
                    {
                        temp = absVal - 0.18;
                    }
                    else if (absVal > 0.75)
                    {
                        temp = (absVal * 1.72) - 0.72;
                    }

                    outputValue = (axisValue >= 0.0 ? 1.0 : -1.0) * temp;
                    break;

                case Curve.EaseoutQuad:
                    outputValue = axisValue;
                    break;

                case Curve.EaseoutCubic:
                    outputValue = axisValue;
                    break;

                case Curve.Bezier:
                    outputValue = axisValue;
                    break;

                default:
                    outputValue = axisValue;
                    break;
            }

            return outputValue;
        }
    }
}
