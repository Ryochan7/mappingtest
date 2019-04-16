using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mappingtester
{
    class ButtonTranslate
    {
        private Tester.ButtonAlias id;
        private uint value;

        private bool state;

        public ButtonTranslate(Tester.ButtonAlias id, uint value)
        {
            this.id = id;
            this.value = value;
        }

        public void Event(Tester mapper, bool status)
        {
            if (status != state)
            {
                state = status;
                mapper.SetButtonEvent(id, status, value);
            }
        }
    }
}
