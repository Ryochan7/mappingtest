using mappingtester.AxisModifiers;
using System;

namespace mappingtester.AxisActions
{
    public class AxisTranslate : AxisActionTrans
    {
        private Tester.AxisAlias id;

        private int min;
        private int max;
        private int mid;

        private AxisModTypes.Mods usedMods;
        public AxisModTypes.Mods UsedMods
        {
            get => usedMods;
            set
            {
                usedMods = value;
                runInter = usedMods != AxisModTypes.Mods.None;
            }
        }
        private AxisDeadZone deadMod;
        public AxisDeadZone DeadMod => deadMod;

        private double axisNorm;

        public bool activeEvent = false;
        private bool runInter = false;

        public AxisTranslate(Tester.AxisAlias id, int min, int max, int mid = 0)
        {
            this.id = id;
            SetAxisRange(min, max, mid);

            deadMod = new AxisDeadZone(0.2, 1.0, 0.2);
            runInter = usedMods != AxisModTypes.Mods.None;
            usedMods = AxisModTypes.Mods.DeadZone;
        }

        public override void Prepare(Tester mapper, int value)
        {
            if (runInter)
            {
                Modifiers(value, out int tempval, out double axisNorm);
            }
            else
            {
                double axisDir = value - mid;
                bool negative = axisDir < 0;
                double maxDir = (axisDir >= 0 ? max : min) - mid;
                axisNorm = axisDir / (negative ? -maxDir : maxDir);
            }
        }

        public override void Event(Tester mapper)
        {
            mapper.SetAxisEvent(id, axisNorm);
            if (axisNorm == 0.0) activeEvent = false;
        }

        public override void Release(Tester mapper)
        {
            mapper.SetAxisEvent(id, 0.0);
            axisNorm = 0.0;
            activeEvent = false;
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
    }
}
