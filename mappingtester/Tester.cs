using System;
using System.Collections.Generic;
using System.Threading;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
//using Nefarius.ViGEm.Client.Targets.DualShock4;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace mappingtester
{
    public class Tester
    {
        public enum StickAlias : uint
        {
            LeftStick,
            RightStick
        }

        public enum AxisAlias: uint
        {
            LeftTrigger,
            RightTrigger,
            LeftX,
            LeftY,
            RightX,
            RightY
        }

        public enum ButtonAlias : uint
        {
            A,
            B,
            X,
            Y,
            Start,
            Back,
            Guide,
            LeftShoulder,
            RightShoulder,
            LeftThumb,
            RightThumb,
            Up,
            Down,
            Left,
            Right
        }

        public enum DPadAlias : uint
        {
            Dpad1
        }

        private Thread vbusThr;
        private Thread contThr;
        //private DualShock4Controller dsc = null;
        private Xbox360Controller xbux = null;
        private ViGEmClient vigemTestClient = null;
        //private DualShock4Report reportshock = new DualShock4Report();
        public Xbox360Report reportx = new Xbox360Report();
        private ushort tempbuttons;
        private Dictionary<ushort, uint> keysCount = new Dictionary<ushort, uint>();

        private StickTranslate testLeftStick = new StickTranslate(StickAlias.LeftStick, 0, 255);
        private StickTranslate testRightStick = new StickTranslate(StickAlias.RightStick, 0, 255);

        private TriggerTranslate testLT = new TriggerTranslate(AxisAlias.LeftTrigger, 0, 255);
        private TriggerTranslate testRT = new TriggerTranslate(AxisAlias.RightTrigger, 0, 255);

        //private ButtonTranslate testABtn = new ButtonTranslate(ButtonAlias.A, (uint)Xbox360Buttons.A);
        //private VirtButtonBinding bitchLasagna = 
        /*private ButtonGenTranslate testABtn = new ButtonGenTranslate(new VirtButtonBinding(VirtButtonBinding.BindingType.Keyboard)
        {
            BindCode = (uint)0x41
        });
        */

        /*private ButtonGenTranslate testABtn = new ButtonGenTranslate(new VirtButtonBinding(VirtButtonBinding.BindingType.MouseButton)
        {
            BindCode = InputMethods.MOUSEEVENTF_LEFTDOWN,
            BindCodeUp = InputMethods.MOUSEEVENTF_LEFTUP,
        });
        */

        /*private ButtonGenTranslate testABtn = new ButtonGenTranslate(new VirtButtonBinding(VirtButtonBinding.BindingType.JoyButton)
        {
            JoyBId = ButtonAlias.B,
            JoyBtnValue = (uint)Xbox360Buttons.B,
        });
        */

        private ButtonGenTranslate testABtn = new ButtonGenTranslate(new VirtButtonBinding(VirtButtonBinding.BindingType.JoyAxis)
        {
            JoyAId = AxisAlias.LeftX,
            JoyAxisValue = 1.0,
        });

        private ButtonTranslate testBBtn = new ButtonTranslate(ButtonAlias.B, (uint)Xbox360Buttons.B);
        private ButtonTranslate testXBtn = new ButtonTranslate(ButtonAlias.X, (uint)Xbox360Buttons.X);
        private ButtonTranslate testYBtn = new ButtonTranslate(ButtonAlias.Y, (uint)Xbox360Buttons.Y);

        private ButtonTranslate testLBBtn = new ButtonTranslate(ButtonAlias.LeftShoulder, (uint)Xbox360Buttons.LeftShoulder);
        private ButtonTranslate testRBBtn = new ButtonTranslate(ButtonAlias.RightShoulder, (uint)Xbox360Buttons.RightShoulder);

        private ButtonTranslate testBackBtn = new ButtonTranslate(ButtonAlias.Back, (uint)Xbox360Buttons.Back);
        private ButtonTranslate testStartBtn = new ButtonTranslate(ButtonAlias.Start, (uint)Xbox360Buttons.Start);

        private ButtonTranslate testLThumbBtn = new ButtonTranslate(ButtonAlias.LeftThumb, (uint)Xbox360Buttons.LeftThumb);
        private ButtonTranslate testRThumbBtn = new ButtonTranslate(ButtonAlias.RightThumb, (uint)Xbox360Buttons.RightThumb);

        private DPadTranslate testDpad = new DPadTranslate();

        private double mouseX = 0;
        private double mouseY = 0;
        private double mouseXRemainder = 0.0;
        private double mouseYRemainder = 0.0;

        public void Start()
        {
            DS4Windows.DS4Devices.isExclusiveMode = false;
            DS4Windows.DS4Devices.findControllers();

            // Change thread affinity of bus object to not be tied
            // to GUI thread
            vbusThr = new Thread(() =>
            {
                vigemTestClient = new ViGEmClient();
            });

            vbusThr.Priority = ThreadPriority.Normal;
            vbusThr.IsBackground = true;
            vbusThr.Start();
            vbusThr.Join(); // Wait for bus object start

            contThr = new Thread(() =>
            {
                xbux = new Xbox360Controller(vigemTestClient);
                xbux.Connect();
                //dsc = new DualShock4Controller(vigemTestClient);
                //dsc.Connect();
            });
            contThr.Priority = ThreadPriority.Normal;
            contThr.IsBackground = true;
            contThr.Start();
            contThr.Join();

            IEnumerable<DS4Windows.DS4Device> devices =
                DS4Windows.DS4Devices.getDS4Controllers();
            int ind = 0;

            foreach (DS4Windows.DS4Device currentDev in devices)
            {
                currentDev.LightBarColor = new DS4Windows.DS4Color(0, 0, 255);
                currentDev.Report += ReadReport;
                // Start input data thread
                currentDev.StartUpdate();
                ind++;
            }
        }

        public void ReadReport(DS4Windows.DS4Device sender, EventArgs e)
        {
            DS4Windows.DS4Device dev = sender;
            DS4Windows.DS4State current = dev.getCurrentStateRef();
            DS4Windows.DS4State previous = dev.getPreviousStateRef();
            Mapping(current, previous);
            xbux.SendReport(reportx);
        }

        public void Mapping(DS4Windows.DS4State current,
            DS4Windows.DS4State previous)
        {
            if (current.LX != previous.LX || current.LY != previous.LY)
            {
                testLeftStick.Event(this, current.LX, current.LY);
            }

            if (current.RX != previous.RX || current.RY != previous.RY)
            {
                testRightStick.Event(this, current.RX, current.RY);
            }

            if (current.L2 != previous.L2)
            {
                testLT.Event(this, current.L2);
            }

            if (current.R2 != previous.R2)
            {
                testRT.Event(this, current.R2);
            }

            if (current.Cross != previous.Cross)
            {
                testABtn.Event(this, current.Cross);
            }

            if (current.Circle != previous.Circle)
            {
                testBBtn.Event(this, current.Circle);
            }

            if (current.Square != previous.Square)
            {
                testXBtn.Event(this, current.Square);
            }

            if (current.Triangle != previous.Triangle)
            {
                testYBtn.Event(this, current.Triangle);
            }

            if (current.L1 != previous.L1)
            {
                testLBBtn.Event(this, current.L1);
            }

            if (current.R1 != previous.R1)
            {
                testRBBtn.Event(this, current.R1);
            }

            if (current.Share != previous.Share)
            {
                testBackBtn.Event(this, current.Share);
            }

            if (current.Options != previous.Options)
            {
                testStartBtn.Event(this, current.Options);
            }

            if (current.L3 != previous.L3)
            {
                testLThumbBtn.Event(this, current.L3);
            }

            if (current.R3 != previous.R3)
            {
                testRThumbBtn.Event(this, current.R3);
            }

            unchecked
            {
                DPadTranslate.DpadDirections currentDpad =
                DPadTranslate.DpadDirections.Centered;

                if (current.DpadUp)
                    currentDpad |= DPadTranslate.DpadDirections.Up;
                if (current.DpadRight)
                    currentDpad |= DPadTranslate.DpadDirections.Right;
                if (current.DpadDown)
                    currentDpad |= DPadTranslate.DpadDirections.Down;
                if (current.DpadLeft)
                    currentDpad |= DPadTranslate.DpadDirections.Left;

                testDpad.Event(this, currentDpad);
            }

            reportx.Buttons = tempbuttons;
            GenerateMouseEvent();
        }

        public void SetStickEvent(StickAlias id, double xNorm, double yNorm)
        {
            if (id == StickAlias.LeftStick)
            {
                reportx.LeftThumbX = (short)(xNorm * (xNorm < 0.0 ? 32768 : 32767));
                reportx.LeftThumbY = (short)(-yNorm * (yNorm < 0.0 ? 32767 : 32768));
            }
            else
            {
                reportx.RightThumbX = (short)(xNorm * (xNorm < 0.0 ? 32768 : 32767));
                reportx.RightThumbY = (short)(-yNorm * (yNorm < 0.0 ? 32767 : 32768));
            }
        }

        public void SetButtonEvent(ButtonAlias id, bool status, uint btnvalue)
        {
            unchecked
            {
                if (status)
                    tempbuttons |= (ushort)btnvalue;
                else
                    tempbuttons &= (ushort)~btnvalue;
            }
        }

        public void SetAxisEvent(AxisAlias id, double axisNorm)
        {
            switch(id)
            {
                case AxisAlias.LeftTrigger:
                    reportx.LeftTrigger = (byte)(axisNorm * 255);
                    break;
                case AxisAlias.RightTrigger:
                    reportx.RightTrigger = (byte)(axisNorm * 255);
                    break;
                case AxisAlias.LeftX:
                    reportx.LeftThumbX = (short)(axisNorm * (axisNorm < 0.0 ? 32768 : 32767));
                    break;
                case AxisAlias.LeftY:
                    reportx.LeftThumbY = (short)(axisNorm * (axisNorm < 0.0 ? 32767 : 32768));
                    break;
                case AxisAlias.RightX:
                    reportx.RightThumbX = (short)(axisNorm * (axisNorm < 0.0 ? 32768 : 32767));
                    break;
                case AxisAlias.RightY:
                    reportx.RightThumbY = (short)(axisNorm * (axisNorm < 0.0 ? 32767 : 32768));
                    break;
                default:
                    break;
            }
        }

        public void SetDPadEvent(DPadTranslate.DpadDirections value)
        {
            unchecked
            {
                const ushort dpadReset = (ushort)~15;
                tempbuttons &= dpadReset;
                if ((value & DPadTranslate.DpadDirections.Up) == DPadTranslate.DpadDirections.Up)
                {
                    tempbuttons |= (ushort)Xbox360Buttons.Up;
                }

                if ((value & DPadTranslate.DpadDirections.Right) == DPadTranslate.DpadDirections.Right)
                {
                    tempbuttons |= (ushort)Xbox360Buttons.Right;
                }

                if ((value & DPadTranslate.DpadDirections.Down) == DPadTranslate.DpadDirections.Down)
                {
                    tempbuttons |= (ushort)Xbox360Buttons.Down;
                }

                if ((value & DPadTranslate.DpadDirections.Right) == DPadTranslate.DpadDirections.Right)
                {
                    tempbuttons |= (ushort)Xbox360Buttons.Right;
                }
            }
        }

        public void SetKeyEvent(ushort value, bool pressed)
        {
            keysCount.TryGetValue(value, out uint count);
            if (count == 0 && pressed)
            {
                keysCount[value] = count + 1;
                InputMethods.performKeyPress(value);
            }
            else if (count == 1 && !pressed)
            {
                InputMethods.performKeyRelease(value);
                keysCount.Remove(value);
            }
            else if (!pressed)
            {
                keysCount[value] = count - 1;
            }
        }

        public void SetMouseButton(uint value, bool pressed)
        {
            InputMethods.MouseEvent(value);
        }

        public void SetMouseCusorMovement(double x, double y)
        {
            mouseX = x;
            mouseY = y;
            //InputMethods.MoveCursorBy(x, y);
        }

        public void SetAbsMousePosition(double xNorm, double yNorm)
        {

        }

        private static double remainderCutoff(double dividend, double divisor)
        {
            return dividend - (divisor * (int)(dividend / divisor));
        }

        public void GenerateMouseEvent()
        {
            if (mouseX != 0.0 && mouseY != 0.0)
            {
                if ((mouseX > 0.0 && mouseXRemainder > 0.0) || (mouseX < 0.0 && mouseXRemainder < 0.0))
                {
                    mouseX += mouseXRemainder;
                }

                double mouseXTemp = mouseX - (remainderCutoff(mouseX * 1000.0, 1.0) / 1000.0);
                int mouseXInt = (int)(mouseXTemp);
                mouseXRemainder = mouseXTemp - mouseXInt;

                if ((mouseY > 0.0 && mouseYRemainder > 0.0) || (mouseY < 0.0 && mouseYRemainder < 0.0))
                {
                    mouseY += mouseYRemainder;
                }

                double mouseYTemp = mouseX - (remainderCutoff(mouseY * 1000.0, 1.0) / 1000.0);
                int mouseYInt = (int)(mouseYTemp);
                mouseYRemainder = mouseYTemp - mouseYInt;
                InputMethods.MoveCursorBy(mouseXInt, mouseYInt);
            }
            else
            {
                mouseXRemainder = mouseYRemainder = 0.0;
            }
        }

        public void Stop()
        {
            DS4Windows.DS4Devices.stopControllers();
            xbux.Disconnect();
            vigemTestClient.Dispose();
            vigemTestClient = null;
        }
    }
}
