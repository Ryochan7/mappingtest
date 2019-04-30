
namespace mappingtester
{
    class ButtonGenTranslate
    {
        private VirtButtonBinding binding;
        private bool state;

        public ButtonGenTranslate(VirtButtonBinding binding)
        {
            this.binding = binding;
            state = false;
        }

        public void Event(Tester mapper, bool status)
        {
            if (status != state)
            {
                state = status;
                VirtButtonBinding.BindingType bindingType = binding.BindType;
                if (bindingType == VirtButtonBinding.BindingType.Keyboard)
                {
                    mapper.SetKeyEvent((ushort)binding.BindCode, status);
                }
                else if (bindingType == VirtButtonBinding.BindingType.MouseButton)
                {
                    mapper.SetMouseButton(status ? binding.BindCode : binding.BindCodeUp,
                        status);
                }
                else if (bindingType == VirtButtonBinding.BindingType.JoyButton)
                {
                    mapper.SetButtonEvent(binding.JoyBId, status, binding.JoyBtnValue);
                }
                else if (bindingType == VirtButtonBinding.BindingType.JoyAxis)
                {
                    mapper.SetAxisEvent(binding.JoyAId,
                        status ? binding.JoyAxisValue : 0.0);
                }
                else if (bindingType == VirtButtonBinding.BindingType.JoyTrigger)
                {
                    mapper.SetAxisEvent(binding.JoyAId,
                        status ? binding.JoyAxisValue : 0.0);
                }
            }
        }
    }
}
