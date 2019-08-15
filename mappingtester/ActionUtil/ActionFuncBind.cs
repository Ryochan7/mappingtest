using static mappingtester.Tester;

namespace mappingtester.ActionUtil
{
    public class ActionFuncBind
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

        public OutMode outMode;
        //public uint outId;
        public OutputIds outputId;

        public ActionFuncBind()
        {
        }

        public ActionFuncBind(OutMode mode, uint code)
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

        public override string ToString()
        {
            return string.Empty;
        }
    }
}
