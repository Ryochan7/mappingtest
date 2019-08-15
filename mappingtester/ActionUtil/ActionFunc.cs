using System.Collections.Generic;
using static mappingtester.Tester;

namespace mappingtester.ActionUtil
{
    public abstract class ActionFunc
    {
        protected bool active;
        protected List<ActionFuncBind> bindings = new List<ActionFuncBind>();

        public ActionFunc()
        {
        }

        public abstract void Event(Tester mapper, bool active);
        public abstract void Release(Tester mapper);

        protected void SendOutputEvent(Tester mapper)
        {
            foreach (ActionFuncBind binding in bindings)
            {
                switch (binding.outMode)
                {
                    case ActionFuncBind.OutMode.None:
                        break;
                    case ActionFuncBind.OutMode.ContBtn:
                        mapper.SetButtonEvent(binding.outputId.buttonId, active,
                            binding.outputId.buttonValue);
                        break;
                    case ActionFuncBind.OutMode.ContAxis:
                        mapper.SetAxisEvent(binding.outputId.axisId, active ? 1.0 : 0.0);
                        break;
                    case ActionFuncBind.OutMode.KB:
                        mapper.SetKeyEvent(binding.outputId.keyCode, active);
                        break;
                    case ActionFuncBind.OutMode.MouseButton:
                        mapper.SetMouseButton(binding.outputId.mouseCode, active);
                        break;
                    //case ActionFuncBind.OutMode.Mouse:
                    //    mapper.SetMouseCusorMovement(0.0, 0.0);
                    //    break;
                    default:
                        break;
                }
            }
        }

        protected void ReleaseOuputEvent(Tester mapper)
        {
            foreach (ActionFuncBind binding in bindings)
            {
                switch (binding.outMode)
                {
                    case ActionFuncBind.OutMode.None:
                        break;
                    case ActionFuncBind.OutMode.ContBtn:
                        mapper.SetButtonEvent(binding.outputId.buttonId, active,
                            binding.outputId.buttonValue);
                        break;
                    case ActionFuncBind.OutMode.ContAxis:
                        mapper.SetAxisEvent(binding.outputId.axisId, active ? 1.0 : 0.0);
                        break;
                    case ActionFuncBind.OutMode.KB:
                        mapper.SetKeyEvent(binding.outputId.keyCode, active);
                        break;
                    case ActionFuncBind.OutMode.MouseButton:
                        mapper.SetMouseButton(binding.outputId.mouseCode, active);
                        break;
                    case ActionFuncBind.OutMode.Mouse:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
