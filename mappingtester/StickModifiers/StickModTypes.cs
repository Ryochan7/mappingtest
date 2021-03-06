﻿using System;

namespace mappingtester.StickModifiers
{
    public class StickModTypes
    {
        [Flags]
        public enum Mods : uint
        {
            None,
            Sensitivity = 1,
            DeadZone = 1 << 1,
            OutCurve = 1 << 2,
            Rotation = 1 << 3,
            SquareStick = 1 << 4,
            Smoothing = 1 << 5,
        }
    }
}
