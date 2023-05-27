using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVPingpong
{
    public class Controller
    {
        private Servo LeftTop, LeftBottom, RightTop, RightBottom;

        public Point center;

        public Controller(Servo leftTop, Servo leftBottom, Servo rightTop, Servo rightBottom)
        {
            this.LeftTop = leftTop;
            this.LeftBottom = leftBottom;
            this.RightTop = rightTop;
            this.RightBottom = rightBottom;
            SendPosition();
        }

        public void Move(int x, int y, int dx, int dy)
        {
            var xx = (center.X - x) * 0.1 - dx;
            var yy = (center.Y - y) * 0.1 - dy;
            //var xx = -dx;
            //var yy = -dy;
            if (Math.Abs(xx) > 300 || Math.Abs(yy) > 300)
            {
                return;
            }
            else
            {
                LeftTop.Reset(); LeftBottom.Reset(); RightTop.Reset(); RightBottom.Reset();

                xx /= 50;
                yy /= 50;

                for (int i = 0; i < Math.Abs(xx); i++) { 
                    if (xx > 0)
                    {
                        LeftTop.Up();
                        LeftBottom.Up();
                        RightTop.Down();
                        RightBottom.Down();
                    }
                    else
                    {
                        LeftTop.Down();
                        LeftBottom.Down();
                        RightTop.Up();
                        RightBottom.Up();
                    }
                }

                for (int i = 0; i < Math.Abs(yy); i++) 
                { 
                    if (yy > 0)
                    {
                        LeftTop.Down();
                        RightTop.Down();
                        LeftBottom.Up();
                        RightBottom.Up();
                    }
                    else
                    {
                        LeftTop.Up();
                        RightTop.Up();
                        LeftBottom.Down();
                        RightBottom.Down();
                    }
                }
                SendPosition();
            }
        }

        public void SendPosition()
        {
            var data = new byte[4];
            data[LeftTop.id] = (byte)LeftTop.position;
            data[LeftBottom.id] = (byte)(LeftBottom.position);
            data[RightTop.id] = (byte)(RightTop.position);
            data[RightBottom.id] = (byte)(RightBottom.position);
#if DEBUG
            Console.WriteLine(string.Join(",", data));
#endif
            SerialPorts.SendData(data);
        }
    }
    public enum ServoType
    {
        Normal = 0,
        Inverse = 1
    }
    public class Servo
    {
        public byte id;
        public int position;
        public readonly int horizontalPosition;
        public int step;
        public int maximumDegree;
        public ServoType type;

        public Servo(byte id, int horizontalPosition = 90, int step = 15, int maximumDegree = 180, ServoType type = ServoType.Normal)
        {
            this.id = id;
            this.position = this.horizontalPosition = horizontalPosition;
            this.step = step;
            this.maximumDegree = maximumDegree;
            this.type = type;
        }

        public void Reset()
        {
            position = horizontalPosition;
        }

        public void Down(int count = 1)
        {
            if (type == ServoType.Normal)
            {
                if (position + count * step <= maximumDegree)
                {
                    position += count * step;
                }
                else
                {
                    position = maximumDegree;
                }
            }
            else
            {
                if (position - count * step >= 0)
                {
                    position -= count * step;
                }
                else
                {
                    position = 0;
                }
            }
            //SerialPorts.SendData(new byte[] { id, (byte)position });
        }

        public void Up(int count = 1)
        {
            if (type == ServoType.Normal)
            {
                if (position - count * step >= 0)
                {
                    position -= count * step;
                }
                else
                {
                    position = 0;
                }
            }
            else
            {
                if (position + count * step <= maximumDegree)
                {
                    position += count * step;
                }
                else
                {
                    position = maximumDegree;
                }
            }
            //SerialPorts.SendData(new byte[] { id, (byte)position });
        }
    }
}
