using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using mappingtester.DPadActions;
using mappingtester.AxisActions;

namespace mappingtester
{
    public class ActionSet
    {
        private const int DEFAULT_LAYERS = 4;

        private string displayName;
        private List<ActionLayer> actionLayers;
        public ActionLayer currentActionLayer;
        private int currentLayerNum = 0;

        public ButtonTranslate testABtn;
        public ButtonTranslate testBBtn;
        public StickTranslate testLeftStick;
        public StickMouse testRightStick;
        public TriggerTranslate testLT;
        public TriggerTranslate testRT;
        public ButtonTranslate testLBBtn;
        public ButtonTranslate testRBBtn;
        public ButtonTranslate testBackBtn;
        public ButtonTranslate testStartBtn;
        public ButtonTranslate testXBtn;
        public ButtonTranslate testYBtn;
        public ButtonTranslate testLThumbBtn;
        public ButtonTranslate testRThumbBtn;
        public DPadTranslate testDpad;

        public string DisplayName { get => displayName; set => displayName = value; }

        public ActionSet()
        {
            actionLayers = new List<ActionLayer>(DEFAULT_LAYERS);
            actionLayers.Add(new ActionLayer());
            currentActionLayer = actionLayers[0];
            currentLayerNum = 0;
            displayName = "Default";

            testABtn = new ButtonTranslate(Tester.ButtonAlias.A, (uint)Xbox360Buttons.A);
            testBBtn = new ButtonTranslate(Tester.ButtonAlias.B, (uint)Xbox360Buttons.B);
            testLeftStick = new StickTranslate(Tester.StickAlias.LeftStick, 0, 255);
            testRightStick = new StickMouse(0, 255);
            testRightStick.SetSpeed(100, 100);
            testLT = new TriggerTranslate(Tester.AxisAlias.LeftTrigger, 0, 255);
            testRT = new TriggerTranslate(Tester.AxisAlias.RightTrigger, 0, 255);
            testLBBtn = new ButtonTranslate(Tester.ButtonAlias.LeftShoulder, (uint)Xbox360Buttons.LeftShoulder);
            testRBBtn = new ButtonTranslate(Tester.ButtonAlias.RightShoulder, (uint)Xbox360Buttons.RightShoulder);
            testBackBtn = new ButtonTranslate(Tester.ButtonAlias.Back, (uint)Xbox360Buttons.Back);
            testStartBtn = new ButtonTranslate(Tester.ButtonAlias.Start, (uint)Xbox360Buttons.Start);
            testXBtn = new ButtonTranslate(Tester.ButtonAlias.X, (uint)Xbox360Buttons.X);
            testYBtn = new ButtonTranslate(Tester.ButtonAlias.Y, (uint)Xbox360Buttons.Y);
            testLThumbBtn = new ButtonTranslate(Tester.ButtonAlias.LeftThumb, (uint)Xbox360Buttons.LeftThumb);
            testRThumbBtn = new ButtonTranslate(Tester.ButtonAlias.RightThumb, (uint)Xbox360Buttons.RightThumb);
            testDpad = new DPadTranslate();
        }

        public void Release(Tester mapper)
        {
            if (testABtn.IsActive) testABtn.Release(mapper);
        }
    }
}
