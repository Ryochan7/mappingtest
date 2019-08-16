using mappingtester.ActionUtil;
using mappingtester.AxisModifiers;
using mappingtester.DPadActions;
using mappingtester.StickModifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mappingtester.StickActions
{
    public class StickActionPad
    {
        public enum DPadMode : uint
        {
            Standard,
            EightWay,
            FourWayCardinal,
            FourWayDiagonal,
        }

        private Tester.StickAlias id;

        private int axisMax;
        private int axisMin;
        private int axisMid;

        private StickModTypes.Mods usedMods;
        private StickDeadZone deadMod;
        private AxisOutCurves.Curve outCurve =
            AxisOutCurves.Curve.Linear;

        private DpadDirections previous = DpadDirections.Centered;
        private DpadDirections activeDir = DpadDirections.Centered;
        public bool IsActive => activeDir != DpadDirections.Centered;
        private DPadMode currentMode = DPadMode.Standard;
        public DPadMode DpadMode
        {
            get => currentMode;
            set => currentMode = value;
        }

        private ActionButton[] actBtns = new ActionButton[13]
        {
            null,
            new ActionButton(),
            new ActionButton(),
            new ActionButton(),
            new ActionButton(),
            null,
            new ActionButton(),
            null,
            new ActionButton(),
            new ActionButton(),
            null,
            null,
            new ActionButton(),
        };
        private ActionButton activeBtn;
        private ActionButton previousBtn;

        //private int diagonalRange;

        private double xNorm = 0.0, yNorm = 0.0;
        private double prevXNorm = 0.0, prevYNorm = 0.0;
        public bool activeEvent;
        private bool runInter = false;

        public StickActionPad(Tester.StickAlias id, int min, int max, int mid = 0)
        {
            this.id = id;
            SetAxisRange(min, max, mid);
            deadMod = new StickDeadZone(0.05, 1.0, 0.0);
            runInter = deadMod.ShouldInterpolate();
            usedMods = StickModTypes.Mods.DeadZone | StickModTypes.Mods.OutCurve;
        }

        public void Prepare(Tester mapper, int axisXVal, int axisYVal)
        {
            xNorm = 0.0; yNorm = 0.0;

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

            if (xNorm != prevXNorm || yNorm != prevYNorm)
            {
                previousBtn = activeBtn;
                DetermineActiveDir();
                if (previous != activeDir)
                {
                    activeBtn = actBtns[(int)activeDir];
                }
            }
            
            activeEvent = true;
        }

        public void Event(Tester mapper)
        {
            if (previousBtn != null)
            {
                previousBtn.Event(mapper, false);
                previousBtn = null;
            }

            activeBtn.Event(mapper, activeEvent);
            prevXNorm = xNorm; prevYNorm = yNorm;
            previous = activeDir;
            activeEvent = false;
        }

        public void Release(Tester mapper)
        {
            if (activeDir != DpadDirections.Centered)
            {
                activeDir = DpadDirections.Centered;
                activeEvent = false;
                activeBtn.Event(mapper, activeEvent);
            }
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

        private void SetAxisRange(int min, int max, int mid)
        {
            axisMin = min;
            axisMax = max;
            if (mid == 0 && min == 0)
                axisMid = (max + min + 1) / 2;
            else
                axisMid = mid;
        }

        private void DetermineActiveDir()
        {
            if (xNorm == 0.0 && yNorm == 0.0)
            {
                activeDir = DpadDirections.Centered;
            }
            else
            {
                const double upLeftEnd = 360 - 22.5;
                const double upRightBegin = 22.5;
                const double rightBegin = 90 - 22.5;
                const double downRightBegin = 90 + 22.5;
                const double downBegin = 180 - 22.5;
                const double downLeftBegin = 180 + 22.5;
                const double leftBegin = 270 - 22.5;
                const double upLeftBegin = 270 + 22.5;

                double angleRad = Math.Atan2(-yNorm, xNorm);
                double angle = (angleRad >= 0 ? angleRad : (2 * Math.PI + angleRad)) * 180 / Math.PI;
                if (angle == 0.0)
                {
                    activeDir = DpadDirections.Centered;
                }
                else if (angle > upLeftEnd || angle < upRightBegin)
                {
                    activeDir = DpadDirections.Up;
                }
                else if (angle >= upRightBegin && angle < rightBegin)
                {
                    activeDir = DpadDirections.UpRight;
                }
                else if (angle >= rightBegin && angle < downRightBegin)
                {
                    activeDir = DpadDirections.Right;
                }
                else if (angle >= downRightBegin && angle < downBegin)
                {
                    activeDir = DpadDirections.DownRight;
                }
                else if (angle >= downBegin && angle < downLeftBegin)
                {
                    activeDir = DpadDirections.Down;
                }
                else if (angle >= downLeftBegin && angle < leftBegin)
                {
                    activeDir = DpadDirections.DownLeft;
                }
                else if (angle >= leftBegin && angle < upLeftBegin)
                {
                    activeDir = DpadDirections.Left;
                }
                else if (angle >= upLeftBegin && angle <= upLeftEnd)
                {
                    activeDir = DpadDirections.UpLeft;
                }
            }
        }

        public ActionButton GetActionButton(DpadDirections dir)
        {
            return actBtns[(int)dir];
        }
    }
}
