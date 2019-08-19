using mappingtester.AxisModifiers;
using mappingtester.StickModifiers;
using System;

namespace mappingtester.TouchpadActions
{
    public class TouchStickAction
    {
        public struct TouchAxis
        {
            public int min;
            public int max;
            public int mid;
            public int current;
        }

        public enum TouchReportType : uint
        {
            None,
            Absolute,
            Relative,
        }

        public struct TouchReport
        {
            public TouchReportType reportType;
            public int valueX;
            public int valueY;
            public int deltaX;
            public int deltaY;
            public double elapsed;
            public double pressure;
            public bool clicked;
        }

        private TouchAxis touchXAxis;
        private TouchAxis touchYAxis;

        private Tester.StickAlias id;
        private StickModTypes.Mods usedMods;
        private StickDeadZone deadMod;
        private AxisOutCurves.Curve outCurve =
            AxisOutCurves.Curve.Linear;
        private AxisSensMod xsensMod = new AxisSensMod();
        public AxisSensMod XSensMod => xsensMod;
        private AxisSensMod ysensMod = new AxisSensMod();
        public AxisSensMod YSensMod => ysensMod;
        private bool invertX;
        private bool invertY;

        private bool requireClick;
        private double xNorm, yNorm;
        private bool runInter = false;
        public bool activeEvent = false;

        public TouchStickAction(Tester.StickAlias id, TouchAxis xaxis, TouchAxis yaxis)
        {
            this.id = id;
            touchXAxis = xaxis;
            touchYAxis = yaxis;
            deadMod = new StickDeadZone(0.05, 1.0, 0.0);
            usedMods = StickModTypes.Mods.DeadZone;
            runInter = usedMods != StickModTypes.Mods.None;
        }

        public void Prepare(Tester _, ref TouchReport report)
        {
            if (report.reportType == TouchReportType.Absolute)
            {
                xNorm = yNorm = 0.0;

                if (!requireClick || report.clicked)
                {
                    int axisXDir = report.valueX - touchXAxis.mid,
                    axisYDir = report.valueY - touchYAxis.mid;
                    bool xNegative = axisXDir < 0;
                    bool yNegative = axisYDir < 0;
                    double maxDirX = (report.valueX >= touchXAxis.mid ? touchXAxis.max :
                        touchXAxis.min) - touchXAxis.mid;
                    double maxDirY = (report.valueY >= touchYAxis.mid ? touchYAxis.max :
                        touchYAxis.min) - touchYAxis.mid;

                    xNorm = axisXDir / maxDirX * (xNegative ? -1.0 : 1.0);
                    if (xNorm != 0.0 && invertX) xNorm *= -1.0;
                    yNorm = axisYDir / maxDirY * (yNegative ? -1.0 : 1.0);
                    if (yNorm != 0.0 && invertY) yNorm *= -1.0;
                }
            }
            else
            {
                xNorm = yNorm = 0.0;
            }

            activeEvent = true;
        }

        public void Event(Tester mapper)
        {
            mapper.SetStickEvent(id, xNorm, yNorm);
            activeEvent = false;
        }

        public void Release(Tester mapper)
        {
            mapper.SetStickEvent(id, 0.0, 0.0);
            activeEvent = false;
        }

        private void RunModifiers(int axisXVal, int axisYVal,
            out int axisXOut, out int axisYOut, out double xNorm, out double yNorm)
        {
            const StickModTypes.Mods nonDead = ~StickModTypes.Mods.DeadZone;

            int axisXDir = axisXVal - touchXAxis.mid, axisYDir = axisYVal - touchXAxis.mid;

            bool xNegative = axisXDir < 0;
            bool yNegative = axisYDir < 0;
            int maxDirX = (!xNegative ? touchXAxis.max : touchXAxis.min) - touchXAxis.mid;
            int maxDirY = (!yNegative ? touchYAxis.max : touchYAxis.mid) - touchYAxis.mid;

            bool inSafeZone;
            if ((usedMods & StickModTypes.Mods.DeadZone) == StickModTypes.Mods.DeadZone)
            {
                deadMod.CalcOutValues(axisXDir, axisYDir, maxDirX,
                    maxDirY, out xNorm, out yNorm);
                inSafeZone = deadMod.inSafeZone;
            }
            else
            {
                xNorm = axisXDir / (xNegative ? -maxDirX : maxDirX);
                yNorm = axisYDir / (yNegative ? -maxDirY : maxDirY);
                inSafeZone = xNorm != 0.0 || yNorm != 0.0;
            }

            if (inSafeZone && (usedMods & nonDead) != StickModTypes.Mods.None)
            {
                if ((usedMods & StickModTypes.Mods.Sensitivity) == StickModTypes.Mods.Sensitivity)
                {
                    xNorm = xsensMod.CalcOutValue(xNorm);
                    yNorm = ysensMod.CalcOutValue(yNorm);
                }

                if ((usedMods & StickModTypes.Mods.OutCurve) == StickModTypes.Mods.OutCurve &&
                    outCurve != AxisOutCurves.Curve.Linear)
                {
                    xNorm = AxisOutCurves.CalcOutValue(outCurve, xNorm);
                    yNorm = AxisOutCurves.CalcOutValue(outCurve, yNorm);
                }
            }

            if (xNorm != 0.0 && invertX)
            {
                xNorm *= -1.0;
                xNegative = !xNegative;
                maxDirX = (!xNegative ? touchXAxis.max : touchXAxis.min) - touchXAxis.mid;
            }

            if (yNorm != 0.0 && invertY)
            {
                yNorm *= -1.0;
                yNegative = !yNegative;
                maxDirY = (!yNegative ? touchYAxis.max : touchYAxis.min) - touchYAxis.mid;
            }

            axisXOut = (int)(Math.Abs(xNorm) * maxDirX + touchXAxis.mid);
            axisYOut = (int)(Math.Abs(yNorm) * maxDirY + touchYAxis.mid);
        }
    }
}
