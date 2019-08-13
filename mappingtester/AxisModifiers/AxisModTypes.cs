using System;

namespace mappingtester.AxisModifiers
{
    public class AxisModTypes
    {
        [Flags]
        public enum Mods : uint
        {
            None,
            Sensitivity = 1,
            DeadZone = 1 << 1,
            OutCurve = 1 << 2,
        }
    }
}