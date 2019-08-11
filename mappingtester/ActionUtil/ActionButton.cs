using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static mappingtester.Tester;

namespace mappingtester.ActionUtil
{
    public class ActionButton
    {
        public enum OutMode : uint
        {
            None,
            KB,
            Mouse,
            MouseButton,
            ContBtn,
            ContAxis,
        }

        public struct OutputIds
        {
            public ushort keyCode;
            public uint mouseCode;
            public ButtonAlias buttonId;
            public uint buttonValue;
            public AxisAlias axisId;
            public uint mouseSpeed;
        }

        private bool active;
        private OutMode outMode;
        //private uint outId;
        private OutputIds outputId;
        public bool IsActive => active;

        public ActionButton()
        {

        }

        public ActionButton(OutMode mode, uint code)
        {
            switch(mode)
            {
                case OutMode.ContBtn:
                    outputId.buttonValue = code;
                    break;
                case OutMode.ContAxis:
                    outputId.axisId = (AxisAlias)code;
                    break;
                case OutMode.None:
                    break;
                case OutMode.KB:
                    outputId.keyCode = (ushort)code;
                    break;
                case OutMode.MouseButton:
                    outputId.mouseCode = code;
                    break;
                case OutMode.Mouse:
                    outputId.mouseSpeed = code;
                    break;
                default:
                    break;
            }
        }

        public void Event(Tester mapper, bool active)
        {
            if (active != this.active)
            {
                this.active = active;

                switch(outMode)
                {
                    case OutMode.None:
                        break;
                    case OutMode.ContBtn:
                        mapper.SetButtonEvent(outputId.buttonId, active, outputId.buttonValue);
                        break;
                    case OutMode.ContAxis:
                        mapper.SetAxisEvent(outputId.axisId, active ? 1.0 : 0.0);
                        break;
                    case OutMode.KB:
                        mapper.SetKeyEvent(outputId.keyCode, active);
                        break;
                    case OutMode.MouseButton:
                        mapper.SetMouseButton(outputId.mouseCode, active);
                        break;
                    //case OutMode.Mouse:
                    //    mapper.SetMouseCusorMovement(0.0, 0.0);
                    //    break;
                    default:
                        break;
                }
            }
        }

        public void Release(Tester mapper)
        {
            if (active)
            {
                active = false;
                switch (outMode)
                {
                    case OutMode.None:
                        break;
                    case OutMode.ContBtn:
                        mapper.SetButtonEvent(outputId.buttonId, active, outputId.buttonValue);
                        break;
                    case OutMode.ContAxis:
                        mapper.SetAxisEvent(outputId.axisId, active ? 1.0 : 0.0);
                        break;
                    case OutMode.KB:
                        mapper.SetKeyEvent(outputId.keyCode, active);
                        break;
                    case OutMode.MouseButton:
                        mapper.SetMouseButton(outputId.mouseCode, active);
                        break;
                    case OutMode.Mouse:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
