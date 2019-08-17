using System;
using mappingtester.ActionUtil;
using mappingtester.AxisModifiers;

namespace mappingtester.AxisActions
{
    public class AxisActions : AxisActionTrans
    {
        public enum AxisSide : uint
        {
            Center,
            Neg,
            Pos,
        }

        private Tester.AxisAlias id;

        private int min;
        private int max;
        private int mid;

        private double axisNorm;

        private ActionButton negButton = new ActionButton();
        private ActionButton posButton = new ActionButton();
        private ActionButton activeBtn;

        private AxisSide previousSide;
        private AxisSide currentSide;

        private AxisModTypes.Mods usedMods;
        private AxisDeadZone deadMod;

        public bool activeEvent = false;
        private bool runInter = false;

        public AxisActions(Tester.AxisAlias id, int min, int max, int mid = 0)
        {
            this.id = id;
            SetAxisRange(min, max, mid);

            deadMod = new AxisDeadZone(0.2, 1.0, 0.2);
            runInter = deadMod.ShouldInterpolate();
            usedMods = AxisModTypes.Mods.DeadZone;
        }

        public override void Prepare(Tester _, int value)
        {
            if (runInter)
            {
                Modifiers(value, out int _, out double axisNorm);
                activeEvent = axisNorm != 0.0 ? true : false;
            }
            else
            {
                double axisDir = value - mid;
                bool negative = axisDir < 0;
                double maxDir = (axisDir >= 0 ? max : min) - mid;
                axisNorm = axisDir / (negative ? -maxDir : maxDir);
            }

            if (axisNorm != 0.0)
            {
                currentSide = axisNorm > 0.0 ? AxisSide.Pos : AxisSide.Neg;
                activeEvent = true;
            }
        }

        public override void Event(Tester mapper)
        {
            if (currentSide != previousSide && activeBtn != null)
            {
                activeBtn.Event(mapper, false);
                activeBtn = null;
            }

            if (currentSide != AxisSide.Center)
            {
                activeBtn = currentSide == AxisSide.Neg ? negButton : posButton;
                activeBtn.Event(mapper, true);
            }

            previousSide = currentSide;
        }

        public override void Release(Tester mapper)
        {
            if (currentSide != AxisSide.Center)
            {
                activeBtn.Event(mapper, false);
            }

            currentSide = AxisSide.Center;
            activeBtn = null;
            activeEvent = false;
        }

        public void Modifiers(int value, out int axisOutValue, out double axisNorm)
        {
            int axisDir = value - mid;
            bool negative = axisDir < mid;
            int maxDir = (axisDir >= mid ? max : min) - mid;

            bool inSafeZone;
            if ((usedMods & AxisModTypes.Mods.DeadZone) == AxisModTypes.Mods.DeadZone)
            {
                deadMod.CalcOutValues(axisDir, maxDir, out axisNorm);
                inSafeZone = deadMod.inSafeZone;
            }
            else
            {
                axisNorm = axisDir / (negative ? -maxDir : maxDir);
                inSafeZone = axisNorm != 0.0;
            }

            if (inSafeZone)
            {

            }

            axisOutValue = (int)(Math.Abs(axisNorm) * maxDir + mid);
        }

        private void SetAxisRange(int min, int max, int mid)
        {
            this.min = min;
            this.max = max;
            if (mid == 0 && min == 0)
            {
                this.mid = (max + min + 1) / 2;
            }
            else
            {
                this.mid = mid;
            }
        }
    }
}
