using System.Collections.Generic;
using OpenCvSharp;

public static class MobDetector
{
    private const int MinMobSize = 500;
    private const int ScanRadius = 300;

    public static OpenCvSharp.Point[] Detect(Mat frame, ref Mat prevGray)
    {
        if (frame.Empty()) return Array.Empty<OpenCvSharp.Point>();

        Mat gray = new Mat();
        Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);
        Cv2.GaussianBlur(gray, gray, new Size(11, 11), 0);

        if (prevGray.Empty())
        {
            gray.CopyTo(prevGray);
            return Array.Empty<OpenCvSharp.Point>();
        }

        Mat delta = new Mat();
        Cv2.Absdiff(prevGray, gray, delta);
        Mat thresh = new Mat();
        Cv2.Threshold(delta, thresh, 25, 255, ThresholdTypes.Binary);
        Cv2.Dilate(thresh, thresh, null);

        Point[][] contours = Cv2.FindContoursAsArray(thresh, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
        var center = new OpenCvSharp.Point(frame.Width / 2, frame.Height / 2);
        var mobs = new List<OpenCvSharp.Point>();

        foreach (var cnt in contours)
        {
            double area = Cv2.ContourArea(cnt);
            if (area < MinMobSize) continue;

            var rect = Cv2.BoundingRect(cnt);
            int dist = (int)Math.Sqrt(Math.Pow(rect.X + rect.Width / 2 - center.X, 2) +
                                     Math.Pow(rect.Y + rect.Height / 2 - center.Y, 2));
            if (dist <= ScanRadius)
                mobs.Add(new OpenCvSharp.Point(rect.X, rect.Y));
        }

        gray.CopyTo(prevGray);
        return mobs.ToArray();
    }
}
