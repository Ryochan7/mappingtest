using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static mappingtester.ActionUtil.ActionFunc;
using static mappingtester.Tester;

namespace mappingtester.ActionUtil
{
    public class ActionButton
    {
        /*public enum OutMode : uint
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
        */

        private bool active;
        public bool IsActive => active;

        private List<ActionFunc> pressFuncs = new List<ActionFunc>();
        private int currentPressFuncIdx;

        private List<ActionFunc> releaseFuncs = new List<ActionFunc>();
        private int currentReleaseFuncIdx;

        private List<ActionFunc> activeFuncs = new List<ActionFunc>();

        public ActionButton()
        {
            NormalPressFunc func = new NormalPressFunc(OutMode.KB, 40);
            pressFuncs.Add(func);
        }

        public void Event(Tester mapper, bool active)
        {
            if (active != this.active)
            {
                this.active = active;

                if (active)
                {
                    foreach(ActionFunc func in pressFuncs)
                    {
                        func.Event(mapper, active);
                        activeFuncs.Add(func);
                    }
                }
                else
                {
                    for (int i = activeFuncs.Count; i >= 0; i--)
                    {
                        ActionFunc current = activeFuncs[i];
                        current.Event(mapper, false);
                    }

                    activeFuncs.Clear();
                }
            }
        }

        public void Release(Tester mapper)
        {
            if (active)
            {
                active = false;
                for (int i = activeFuncs.Count; i >= 0; i--)
                {
                    ActionFunc current = activeFuncs[i];
                    current.Event(mapper, false);
                }

                activeFuncs.Clear();
            }
        }

        public void AddActionFunc(ActionFunc func)
        {
            pressFuncs.Add(func);
        }

        public void RemoveActionFunc(int index)
        {
            pressFuncs.RemoveAt(index);
        }

        public void InsertActionFunc(int index, ActionFunc func)
        {
            pressFuncs.Insert(index, func);
        }
    }
}
