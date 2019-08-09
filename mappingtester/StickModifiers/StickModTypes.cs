using System;

namespace mappingtester.StickModifiers
{
    public class StickModTypes
    {
        [Flags]
        public enum Mods : uint
        {
            None,
            Sensitivity,
            DeadZone,
            OutCurve,
            Rotation,
            SquareStick,
            Smoothing,
        }
    }
}
