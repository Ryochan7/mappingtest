
namespace mappingtester.ActionUtil
{
    public class FuncConv
    {
        public enum FuncType : uint
        {
            None,
            NormalPress,
            HoldPress,
        }

        public FuncType type;
        public NormalPressFunc pressFunc;
        public HoldPressFunc holdPressFunc;

        public FuncConv()
        {
        }

        public FuncConv(ActionFunc action)
        {
            if (action is NormalPressFunc)
            {
                pressFunc = action as NormalPressFunc;
                type = FuncType.NormalPress;
            }
            else if (action is HoldPressFunc)
            {
                holdPressFunc = action as HoldPressFunc;
                type = FuncType.HoldPress;
            }
        }
    }
}
