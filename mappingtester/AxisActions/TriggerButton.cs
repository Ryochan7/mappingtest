using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mappingtester.AxisActions
{
    class TriggerButton
    {
        private TriggerTranslate parent;

        public TriggerButton(TriggerTranslate parent)
        {
            this.parent = parent;
        }

        public void Event(Tester mapper, int value)
        {
            mapper.SetAxisEvent(parent.Id, value);
        }

        public void Release()
        {

        }
    }
}
