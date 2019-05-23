using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mappingtester
{
    public class ButtonTranslate
    {
        private Tester.ButtonAlias id;
        private uint value;

        private bool state;
        public bool IsActive => state;

        private bool previousState;
        public bool activeEvent = false;

        public ButtonTranslate(Tester.ButtonAlias id, uint value)
        {
            this.id = id;
            this.value = value;
        }

        public void Prepare(Tester mapper, bool status)
        {
            if (status != state)
            {
                previousState = state;
                state = status;
                activeEvent = true;
            }
        }

        public void Event(Tester mapper)
        {
            mapper.SetButtonEvent(id, state, value);
            if (!state) activeEvent = false;
        }

        public void Release(Tester mapper)
        {
            state = false;
            activeEvent = false;
            mapper.SetButtonEvent(id, false, value);
        }
    }
}
