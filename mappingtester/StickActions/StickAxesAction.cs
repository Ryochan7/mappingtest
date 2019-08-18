using System;
using mappingtester.AxisActions;
using mappingtester.AxisModifiers;
using mappingtester.StickModifiers;

namespace mappingtester.StickActions
{
    public class StickAxesAction : StickActionTrans
    {
        private Tester.StickAlias id;

        private int axisMax;
        private int axisMin;
        private int axisMid;

        private AxisTranslate xAxisAct;
        private AxisTranslate yAxisAct;

        private StickModTypes.Mods usedMods;
        private StickDeadZone deadMod;
        private AxisOutCurves.Curve outCurve =
            AxisOutCurves.Curve.Linear;

        private bool runInter = false;

        private double xNorm = 0.0, yNorm = 0.0;

        public StickAxesAction(Tester.StickAlias id, int min, int max, int mid = 0)
        {
            this.id = id;
            SetAxisRange(min, max, mid);
            xAxisAct = new AxisTranslate(Tester.AxisAlias.None, min, max, mid);
            xAxisAct.UsedMods = AxisModTypes.Mods.None;
            yAxisAct = new AxisTranslate(Tester.AxisAlias.None, min, max, mid);
            yAxisAct.UsedMods = AxisModTypes.Mods.None;

            deadMod = new StickDeadZone(0.05, 1.0, 0.0);
            runInter = usedMods != StickModTypes.Mods.None;
            usedMods = StickModTypes.Mods.DeadZone;
        }

        public override void Prepare(Tester mapper, int axisXVal, int axisYVal)
        {
            xNorm = 0.0; yNorm = 0.0;

            if (runInter)
            {
                RunModifiers(axisXVal, axisYVal,
                    out int axisXTemp, out int axisYTemp,
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

            xAxisAct.Prepare(mapper, axisXVal);
            yAxisAct.Prepare(mapper, axisYVal);

            activeEvent = true;
        }

        public override void Event(Tester mapper)
        {
            if (xAxisAct.activeEvent) xAxisAct.Event(mapper);
            if (yAxisAct.activeEvent) yAxisAct.Event(mapper);
            activeEvent = false;
        }

        public override void Release(Tester mapper)
        {
            if (xAxisAct.activeEvent) xAxisAct.Release(mapper);
            if (yAxisAct.activeEvent) yAxisAct.Release(mapper);
        }

        private void SetAxisRange(int min, int max, int mid)
        {
            axisMin = min;
            axisMax = max;
            if (mid == 0 && min == 0)
                axisMid = (max + min + 1) / 2;
            else
                axisMid = mid;
        }

        private void RunModifiers(int axisXVal, int axisYVal,
            out int axisXOut, out int axisYOut, out double xNorm, out double yNorm)
        {
            int axisXDir = axisXVal - axisMid, axisYDir = axisYVal - axisMid;

            bool xNegative = axisXDir < 0;
            bool yNegative = axisYDir < 0;
            int maxDirX = (!xNegative ? axisMax : axisMin) - axisMid;
            int maxDirY = (!yNegative ? axisMax : axisMin) - axisMid;

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

            if (inSafeZone)
            {
                if ((usedMods & StickModTypes.Mods.OutCurve) == StickModTypes.Mods.OutCurve &&
                    outCurve != AxisOutCurves.Curve.Linear)
                {
                    xNorm = AxisOutCurves.CalcOutValue(outCurve, xNorm);
                    yNorm = AxisOutCurves.CalcOutValue(outCurve, yNorm);
                }
            }
            else
            {
                xNorm = 0.0;
                yNorm = 0.0;
            }

            axisXOut = (int)(Math.Abs(xNorm) * maxDirX + axisMid);
            axisYOut = (int)(Math.Abs(yNorm) * maxDirY + axisMid);
        }
    }
}
