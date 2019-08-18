using System;

namespace mappingtester.GyroActions
{
    public class GyroMouse
    {
        public struct AccelAxis
        {
            public int min;
            public int max;
            public int mid;
            public int current;
        }

        private AccelAxis accelXInfo;
        private AccelAxis accelYInfo;
        private AccelAxis accelZInfo;

        private int pitch;
        private int yaw;
        private int roll;

        private double mouseX;
        private double mouseY;

        private int deadZone;
        private bool trigger;

        private double timeElapsed;

        // Abstract to struct for different input devices
        private const double GYRO_MOUSE_COEFFICIENT = 0.0095;
        public const int GYRO_MOUSE_DEADZONE = 10;
        private const double GYRO_MOUSE_OFFSET = 0.1463;
        private const double GYRO_SMOOTH_MOUSE_OFFSET = 0.14698;
        private const double TOUCHPAD_MOUSE_OFFSET = 0.015;

        private int gyroCursorDeadZone = GYRO_MOUSE_DEADZONE;
        public int GyroCursorDeadZone { get => gyroCursorDeadZone; set => gyroCursorDeadZone = value; }

        private bool activeEvent;

        public GyroMouse()
        {
        }

        public GyroMouse(AccelAxis accelX, AccelAxis accelY, AccelAxis accelZ)
        {
            accelXInfo = accelX;
            accelYInfo = accelY;
            accelZInfo = accelZ;
        }

        public void Prepare(Tester _, int pitch, int yaw, int roll,
            int accelX, int accelY, int accelZ, double timeElapsed)
        {
            this.pitch = pitch;
            this.yaw = yaw;
            this.roll = roll;

            accelXInfo.current = accelX;
            accelYInfo.current = accelY;
            accelZInfo.current = accelZ;
            this.timeElapsed = timeElapsed;

            int deltax = pitch;
            int deltay = yaw;

            double tempAngle = Math.Atan2(-deltay, deltax);
            double normX = Math.Abs(Math.Cos(tempAngle));
            double normY = Math.Abs(Math.Sin(tempAngle));
            int signX = Math.Sign(deltax);
            int signY = Math.Sign(deltay);
            double offset = GYRO_MOUSE_OFFSET;

            // Base default speed on 5 ms
            double timeBase = timeElapsed * 200.0;
            double mouseX = pitch != 0 ? pitch * timeBase + (normX * (offset * signX)) :
                0;
            double mouseY = yaw != 0 ? yaw * timeBase + (normY * (offset * signY)) :
                0;

            activeEvent = mouseX != 0.0 || mouseY != 0.0;
        }

        public void Event(Tester mapper)
        {
            mapper.SetMouseCusorMovement(mouseX, mouseY);
        }

        public void Release(Tester _)
        {
        }
    }
}
