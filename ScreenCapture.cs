using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using OpenCvSharp.Extensions;
using OpenCvSharp;

public static class ScreenCapture
{
    public static Mat Capture(Rectangle gameRect)
    {
        try
        {
            var captureArea = gameRect == Rectangle.Empty ? Screen.PrimaryScreen.Bounds : gameRect;
            using (var bitmap = new Bitmap(captureArea.Width, captureArea.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(captureArea.Location, Point.Empty, captureArea.Size);
                }
                return BitmapConverter.ToMat(bitmap);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка захвата экрана: {ex.Message}");
            return null!;
        }
    }
}
