using System;
using System.Collections.Generic;
using System.Threading;
using mappingtester.DPadActions;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
//using Nefarius.ViGEm.Client.Targets.DualShock4;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace mappingtester
{
    public class IntermediateState
    {
        public bool[] buttons = new bool[(int)Tester.ButtonAlias.Right];
        public static DS4Windows.DS4State bacon;
        public ref bool A => ref bacon.Cross;
    }

    public class Tester
    {
        public enum StickAlias : uint
        {
            None,
            LeftStick,
            RightStick
        }

        public enum AxisAlias: uint
        {
            None,
            LeftTrigger,
            RightTrigger,
            LeftX,
            LeftY,
            RightX,
            RightY
        }

        public enum ButtonAlias : uint
        {
            None,
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
            None,
            Dpad1,
        }

        [Flags]
        public enum TestButtons : uint
        {
            None = 0x0,
            A = 1 << 0,
            B = 1 << 2,
            X = 1 << 3,
            Y = 1 << 4,
            LB = 1 << 5,
            RB = 1 << 6,
            Back = 1 << 7,
            Start = 1 << 8,
            LSClick = 1 << 9,
            RSClick = 1 << 10,
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
        private bool keyActive = false;
        private DS4Windows.DS4State currentstate;

        //private StickTranslate testLeftStick = new StickTranslate(StickAlias.LeftStick, 0, 255);
        //private StickTranslate testRightStick = new StickTranslate(StickAlias.RightStick, 0, 255);
        //private StickMouse testRightStick = new StickMouse(0, 255);

        //private TriggerTranslate testLT = new TriggerTranslate(AxisAlias.LeftTrigger, 0, 255);
        //private TriggerTranslate testRT = new TriggerTranslate(AxisAlias.RightTrigger, 0, 255);

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

        /*private ButtonGenTranslate testABtn = new ButtonGenTranslate(new VirtButtonBinding(VirtButtonBinding.BindingType.JoyAxis)
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
        */

        private ActionSet testSet = new ActionSet();

        private double mouseX = 0;
        private double mouseY = 0;
        private double mouseXRemainder = 0.0;
        private double mouseYRemainder = 0.0;

        private double mouseWheelX = 0.0;
        private double mouseWheelY = 0.0;
        private double mouseWheelXRemainder = 0.0;
        private double mouseWheelYRemainder = 0.0;

        public double timeElapsed;
        //private IntermediateState arbys = new IntermediateState();
        private TestButtons testham = TestButtons.None;
        private Profile testProf;

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

            testProf = new Profile("");
            testProf.Load();

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
            currentstate = current;
            DS4Windows.DS4State previous = dev.getPreviousStateRef();
            Mapping(current, previous);
            xbux.SendReport(reportx);
            if (keyActive)
            {
                SendKeyboardEvents();
                keyActive = false;
            }

            if (mouseX != 0.0 || mouseY != 0.0)
                GenerateMouseMoveEvent();

            if (mouseWheelX != 0.0 || mouseWheelY != 0.0)
                GenerateWheelEvent();
        }

        public void Mapping(DS4Windows.DS4State current,
            DS4Windows.DS4State previous)
        {
            timeElapsed = current.elapsedTime;
            ActionSet ass = testProf.currentActionSet;

            if (current.LX != previous.LX || current.LY != previous.LY)
            {
                ass.testLeftStick.Prepare(this, current.LX, current.LY);
            }

            if (ass.testLeftStick.activeEvent)
                ass.testLeftStick.Event(this);

            if (current.RX != previous.RX || current.RY != previous.RY)
            {
                ass.testRightStick.Prepare(this, current.RX, current.RY);
            }
            if (ass.testRightStick.activeEvent)
                ass.testRightStick.Event(this);

            if (current.Cross != previous.Cross)
            {
                ass.testABtn.Prepare(this, current.Cross);
            }
            if (ass.testABtn.activeEvent)
                ass.testABtn.Event(this);

            if (current.Circle != previous.Circle)
            {
                ass.testBBtn.Prepare(this, current.Circle);
            }
            if (ass.testBBtn.activeEvent)
                ass.testBBtn.Event(this);

            if (current.Square != previous.Square)
            {
                ass.testXBtn.Prepare(this, current.Square);
            }
            if (ass.testXBtn.activeEvent)
                ass.testXBtn.Event(this);

            if (current.Triangle != previous.Triangle)
            {
                ass.testYBtn.Prepare(this, current.Triangle);
            }
            if (ass.testYBtn.activeEvent)
                ass.testYBtn.Event(this);

            if (current.Share != previous.Share)
            {
                ass.testBackBtn.Prepare(this, current.Share);
            }
            if (ass.testBackBtn.activeEvent)
                ass.testBackBtn.Event(this);

            if (current.Options != previous.Options)
            {
                ass.testStartBtn.Prepare(this, current.Options);
            }
            if (ass.testStartBtn.activeEvent)
                ass.testStartBtn.Event(this);

            if (current.L2 != previous.L2)
            {
                ass.testLT.Prepare(this, current.L2);
            }
            if (ass.testLT.activeEvent)
                ass.testLT.Event(this);

            if (current.R2 != previous.R2)
            {
                ass.testRT.Prepare(this, current.R2);
            }
            if (ass.testRT.activeEvent)
                ass.testRT.Event(this);

            if (current.L1 != previous.L1)
            {
                ass.testLBBtn.Prepare(this, current.L1);
            }
            if (ass.testLBBtn.activeEvent)
                ass.testLBBtn.Event(this);

            if (current.R1 != previous.R1)
            {
                ass.testRBBtn.Prepare(this, current.R1);
            }
            if (ass.testRBBtn.activeEvent)
                ass.testRBBtn.Event(this);

            if (current.L3 != previous.L3)
            {
                ass.testLThumbBtn.Prepare(this, current.L3);
            }
            if (ass.testLThumbBtn.activeEvent)
                ass.testLThumbBtn.Event(this);

            if (current.R3 != previous.R3)
            {
                ass.testRThumbBtn.Prepare(this, current.R3);
            }
            if (ass.testRThumbBtn.activeEvent)
                ass.testRThumbBtn.Event(this);

            unchecked
            {
                DpadDirections currentDpad =
                DpadDirections.Centered;

                if (current.DpadUp)
                    currentDpad |= DpadDirections.Up;
                if (current.DpadRight)
                    currentDpad |= DpadDirections.Right;
                if (current.DpadDown)
                    currentDpad |= DpadDirections.Down;
                if (current.DpadLeft)
                    currentDpad |= DpadDirections.Left;

                ass.testDpad.Prepare(this, currentDpad);
                if (ass.testDpad.activeEvent)
                    ass.testDpad.Event(this);
            }

            /*if (current.L2 != previous.L2)
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
            */

            reportx.Buttons = tempbuttons;
        }

        public double GetElapsed()
        {
            return currentstate.elapsedTime;
        }

        public void SendKeyboardEvents()
        {
            foreach (KeyValuePair<ushort, uint> pair in keysCount)
            {

                if (pair.Value > 0)
                {
                    InputMethods.performKeyPress(pair.Key);
                }
                else
                {
                    InputMethods.performKeyRelease(pair.Key);
                }
            }

            keysCount.Clear();
        }

        private void PopIS(DS4Windows.DS4State current)
        {
            //arbys.buttons[(int)ButtonAlias.A] = current.Cross;
            if (current.Cross) testham |= TestButtons.A;
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

        public void SetButtonEvent(ButtonAlias _, bool status, uint btnvalue)
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

        public void SetDPadEvent(DpadDirections value)
        {
            unchecked
            {
                const ushort dpadReset = (ushort)~15;
                tempbuttons &= dpadReset;
                if ((value & DpadDirections.Up) == DpadDirections.Up)
                {
                    tempbuttons |= (ushort)Xbox360Buttons.Up;
                }

                if ((value & DpadDirections.Right) == DpadDirections.Right)
                {
                    tempbuttons |= (ushort)Xbox360Buttons.Right;
                }

                if ((value & DpadDirections.Down) == DpadDirections.Down)
                {
                    tempbuttons |= (ushort)Xbox360Buttons.Down;
                }

                if ((value & DpadDirections.Right) == DpadDirections.Right)
                {
                    tempbuttons |= (ushort)Xbox360Buttons.Right;
                }
            }
        }

        public void SetKeyEvent(ushort value, bool pressed)
        {
            keyActive = true;
            keysCount.TryGetValue(value, out uint count);
            if (count == 0 && pressed)
            {
                keysCount[value] = count + 1;
                //InputMethods.performKeyPress(value);
            }
            else if (count == 1 && !pressed)
            {
                //InputMethods.performKeyRelease(value);
                keysCount[value] = 0;
            }
            else if (!pressed)
            {
                keysCount[value] = count - 1;
            }
        }

        public void SetMouseButton(uint value, bool pressed)
        {
            InputMethods.MouseEvent(value, pressed ? 0 : 1);
        }

        public void SetMouseCusorMovement(double x, double y)
        {
            mouseX += x >= 0 ? Math.Max(x, mouseX) : Math.Min(x, mouseX);
            mouseY += y >= 0 ? Math.Max(y, mouseY) : Math.Min(y, mouseY);
        }

        public void SetMouseWheel(double vertical, double horizontal)
        {
            mouseWheelY += vertical;
            mouseWheelX += horizontal;
        }

        public void SetAbsMousePosition(double xNorm, double yNorm)
        {

        }

        private static double remainderCutoff(double dividend, double divisor)
        {
            return dividend - (divisor * (int)(dividend / divisor));
        }

        public void GenerateMouseMoveEvent()
        {
            if (mouseX != 0.0 || mouseY != 0.0)
            {
                if ((mouseX > 0.0 && mouseXRemainder > 0.0) || (mouseX < 0.0 && mouseXRemainder < 0.0))
                {
                    mouseX += mouseXRemainder;
                }
                else
                {
                    mouseXRemainder = 0.0;
                }

                double mouseXTemp = mouseX - (remainderCutoff(mouseX * 1000.0, 1.0) / 1000.0);
                int mouseXInt = (int)(mouseXTemp);
                mouseXRemainder = mouseXTemp - mouseXInt;

                if ((mouseY > 0.0 && mouseYRemainder > 0.0) || (mouseY < 0.0 && mouseYRemainder < 0.0))
                {
                    mouseY += mouseYRemainder;
                }
                else
                {
                    mouseYRemainder = 0.0;
                }

                double mouseYTemp = mouseY - (remainderCutoff(mouseY * 1000.0, 1.0) / 1000.0);
                int mouseYInt = (int)(mouseYTemp);
                mouseYRemainder = mouseYTemp - mouseYInt;
                InputMethods.MoveCursorBy(mouseXInt, mouseYInt);
            }
            else
            {
                mouseXRemainder = mouseYRemainder = 0.0;
            }

            mouseX = mouseY = 0.0;
        }

        private void GenerateWheelEvent()
        {
            if (mouseWheelX != 0.0 || mouseWheelY != 0.0)
            {
                if ((mouseWheelX > 0.0 && mouseWheelXRemainder > 0.0) || (mouseWheelX < 0.0 && mouseWheelXRemainder < 0.0))
                {
                    mouseWheelX += mouseWheelXRemainder;
                }

                if ((mouseWheelY > 0.0 && mouseWheelYRemainder > 0.0) || (mouseWheelY < 0.0 && mouseWheelYRemainder < 0.0))
                {
                    mouseWheelY += mouseWheelYRemainder;
                }

                mouseWheelXRemainder = mouseWheelX % (mouseWheelX > 0.0 ? 120.0 : -120.0);
                mouseWheelYRemainder = mouseWheelY % (mouseWheelY > 0.0 ? 120.0 : -120.0);
                if (mouseWheelY >= 120.0 || mouseWheelY <= -120.0)
                    InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_WHEEL, (int)mouseWheelY);

                if (mouseWheelX >= 120.0 || mouseWheelX <= -120.0)
                    InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_HWHEEL, (int)mouseWheelX);
            }
            else
            {
                mouseWheelXRemainder = mouseWheelYRemainder = 0.0;
            }

            mouseWheelX = mouseWheelY = 0.0;
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
