//#define CONFIG_CAMERA
#define WIN_HELLO
//#define LOGI
using OpenCvSharp;

namespace CVPingpong
{
    internal class Program
    {
        static int x, y;
        static int dx, dy;


        static void DrawCircle(Mat img, Rect rect)
        {
            int nx = rect.X + rect.Width / 2;
            int ny = rect.Y + rect.Height / 2;
            int r = rect.Right - nx;
            Cv2.ArrowedLine(img, new Point(x, y), new Point(nx, ny), new Scalar(0, 255, 0), 1);
            Cv2.Circle(img, new Point(nx, ny), 2, new Scalar(0, 255, 0), 2);
            Cv2.Circle(img, new Point(nx, ny), r, new Scalar(0, 255, 0), 1);
            dx = nx - x; x = nx;
            dy = ny - y; y = ny;
        }
        static void Main(string[] args)
        {
#if CONFIG_CAMERA
            int lowh = 4, lows = 150, lowv = 162;
            int highh = 40, highs = 235, highv = 240;
            Cv2.NamedWindow("Settings", WindowFlags.Normal);
            Cv2.CreateTrackbar("Low_H", "Settings", ref lowh,255);
            Cv2.CreateTrackbar("Low_S", "Settings", ref lows, 255);
            Cv2.CreateTrackbar("Low_V", "Settings", ref lowv, 255);
            Cv2.CreateTrackbar("High_H", "Settings", ref highh, 255);
            Cv2.CreateTrackbar("High_S", "Settings", ref highs, 255);
            Cv2.CreateTrackbar("High_V", "Settings", ref highv, 255);
#else
            #region Servo
            SerialPorts.SetSerialPort("COM4");
            var leftTop = new Servo(2, type: ServoType.Inverse);
            var leftBottom = new Servo(3, type: ServoType.Normal);
            var rightTop = new Servo(1, type: ServoType.Normal);
            var rightBottom = new Servo(0, type: ServoType.Inverse);
            Controller controller = new Controller(leftTop, leftBottom, rightTop, rightBottom);
            #endregion
#if LOGI
            var lower = new Scalar(4, 120, 120);
            var upper = new Scalar(40, 235, 255);
#endif
#if WIN_HELLO
            var lower = new Scalar(7, 157, 134);
            var upper = new Scalar(40, 235, 240);
#endif
#endif
            var video = new VideoCapture(0);
            var rawFps = video.Get(VideoCaptureProperties.Fps);
            var fps = 30;
            int count = 0;
            int limit = (int)(rawFps / fps);
            Point center = new Point(video.FrameWidth / 2, video.FrameHeight / 2);
            (x, y) = (center.X, center.Y);
#if !CONFIG_CAMERA
            controller.center = center;
#endif
            if (!video.IsOpened())
            {
                Console.WriteLine("摄像头打开失败");
                return;
            }
            var img = new Mat();
            var hsv = new Mat();
            var mask = new Mat();
            var output = new Mat();
            var hierarchy = new Mat();
            while (true)
            {
                video.Read(img);
                if (img.Empty())
                {
                    break;
                }
                count++;
                if (count < limit)
                {
                    continue;
                }
                else
                {
                    count = 0;
                }
                Cv2.CvtColor(img, hsv, ColorConversionCodes.BGR2HSV);
#if CONFIG_CAMERA
                var lower = new Scalar(lowh, lows, lowv);
                var upper = new Scalar(highh, highs, highv);
#endif
                Cv2.InRange(hsv, lower, upper, mask);
                Cv2.BitwiseAnd(img, img, output, mask);
                Cv2.FindContours(mask, out var contours, hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxNone);

                Cv2.CvtColor(mask, mask, ColorConversionCodes.GRAY2BGR);

                Cv2.Circle(img, center, 2, new Scalar(255, 0, 0), 2);

                dx = 0; dy = 0;

                var contour = contours.MaxBy(e => Cv2.ContourArea(e));
                if (contour != null)
                {
                    var area = Cv2.ContourArea(contour);
                    if (area > 300) {
                        var rect = Cv2.BoundingRect(contour);
                        DrawCircle(img, rect);
                    }
                }
#if !CONFIG_CAMERA
                controller.Move(x, y, dx, dy);
#endif
                var window = new Window("Camera", img);

                if (Cv2.WaitKey(30) == 27 || Cv2.WaitKey(30) == 32)
                {
                    break;
                }
            }
        }
    }
}