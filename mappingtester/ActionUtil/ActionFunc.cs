using static mappingtester.Tester;

namespace mappingtester.ActionUtil
{
    public abstract class ActionFunc
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

        protected bool active;
        protected OutMode outMode;
        //protected uint outId;
        protected OutputIds outputId;

        public ActionFunc()
        {
        }

        public ActionFunc(OutMode mode, uint code)
        {
            switch (mode)
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

        public abstract void Event(Tester mapper, bool active);
        public abstract void Release(Tester mapper);

        protected void SendEvent(Tester mapper)
        {
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
                //case OutMode.Mouse:
                //    mapper.SetMouseCusorMovement(0.0, 0.0);
                //    break;
                default:
                    break;
            }
        }

        protected void ReleaseEvent(Tester mapper)
        {
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
