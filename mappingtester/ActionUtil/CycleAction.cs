using System.Collections.Generic;

namespace mappingtester.ActionUtil
{
    public class CycleAction
    {
        private List<NormalPressFunc> actList = new List<NormalPressFunc>();
        //public List<NormalPressFunc> ActList => actList;

        private int currentIndex = 0;
        private bool firstRun;
        private bool ignoreFirst;
        public bool IgnoreFirst { get => ignoreFirst; set => ignoreFirst = value; }
        private NormalPressFunc currentFunc;

        public CycleAction()
        {
        }

        public void Execute(Tester mapper, bool forward)
        {
            if (forward)
            {
                currentIndex = (firstRun && !ignoreFirst ? currentIndex : ++currentIndex)
                    % actList.Count;
            }
            else
            {
                currentIndex = --currentIndex % actList.Count;
                
            }

            currentFunc = actList[currentIndex];
            currentFunc.Event(mapper, true);
            firstRun = false;
        }

        public void Release(Tester mapper)
        {
            currentFunc?.Release(mapper);
        }
    }
}
