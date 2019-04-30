using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mappingtester
{
    public class VirtButtonBinding
    {
        public enum BindingType : uint
        {
            None = 0, JoyButton, JoyAxis, JoyTrigger,
            Keyboard, Mouse, MouseButton,
        }

        private BindingType bindType;
        public BindingType BindType => bindType;

        uint bindCode;
        public uint BindCode { get => bindCode; set => bindCode = value; }

        uint bindCodeUp;
        public uint BindCodeUp { get => bindCodeUp; set => bindCodeUp = value; }

        Tester.ButtonAlias joyBId;
        public Tester.ButtonAlias JoyBId { get => joyBId; set => joyBId = value; }

        private uint joyBtnValue;
        public uint JoyBtnValue { get => joyBtnValue; set => joyBtnValue = value; }

        Tester.AxisAlias joyAId;
        public Tester.AxisAlias JoyAId { get => joyAId; set => joyAId = value; }

        private double joyAxisValue;
        public double JoyAxisValue { get => joyAxisValue; set => joyAxisValue = value; }

        /*private int joyAxisMax;
        public int JoyAxisMax { get => joyAxisMax; set => joyAxisMax = value; }

        private int joyAxisMin;
        public int JoyAxisMin { get => joyAxisMin; set => joyAxisMin = value; }        
        */

        public VirtButtonBinding(BindingType type)
        {
            bindType = type;
        }
    }
}
