using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Font = System.Drawing.Font;
using Image = System.Drawing.Image;
using Point = System.Drawing.Point;
using PointF = System.Drawing.PointF;

namespace AzureAIDemo.Helpers
{
    public class ImageHelper2
    {
        public static byte[] DrawBoxes(IList<Rectangle> boxes, IList<string> Desc, byte[] ImageData, string Remark)
        {
            try
            {
                Bitmap bmp;
                using (var ms = new MemoryStream(ImageData))
                {
                    bmp = new Bitmap(ms);
                }
                var counter = 0;
                using (var g = Graphics.FromImage(bmp))
                {
                    foreach (var box in boxes)
                    {
                        var rect = new Rectangle(box.Left, box.Top, box.Width, box.Height);
                        g.DrawRectangle(Pens.Red, rect);
                        counter++;
                        if (counter < Desc.Count)
                        {
                            g.DrawString(Desc[counter], new Font("Arial", 12), Brushes.Red, new Point(box.Left, box.Top));
                        }
                    }
                    if (!string.IsNullOrEmpty(Remark))
                    {
                        g.DrawString(Remark, new Font("Arial", 12), Brushes.Black, new Point(5, 5));
                    }
                }
                if (bmp != null)
                    return ImageToByte(bmp);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            return default;
        }

        public static byte[] DrawPolygon(IList<PointF[]> polygons, IList<string> Desc, byte[] ImageData, string Remark)
        {
            try
            {
                Bitmap bmp;
                using (var ms = new MemoryStream(ImageData))
                {
                    bmp = new Bitmap(ms);
                }
                var counter = 0;
                using (var g = Graphics.FromImage(bmp))
                {
                    foreach (var poly in polygons)
                    {
                        if (poly.Length > 0)
                        {
                            g.DrawPolygon(Pens.LightGreen, poly);
                            counter++;
                            if (counter < Desc.Count)
                            {
                                g.DrawString(Desc[counter], new Font("Arial", 12), Brushes.Green, new Point((int)poly[0].X, (int)poly[0].Y));
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(Remark))
                    {
                        g.DrawString(Remark, new Font("Arial", 12), Brushes.Black, new Point(5, 5));
                    }
                }
                if (bmp != null)
                    return ImageToByte(bmp);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            return default;
        }

        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        public static string ConvertToBase64(byte[] data)
        {
            string base64String = Convert.ToBase64String(data, 0, data.Length);
            var img = "data:image/png;base64," + base64String;
            return img;
        }
    }
}
