using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureAIDemo.Helpers
{
    public class ImageHelper
    {
        public static byte[] DrawBoxes(IList<Rectangle> boxes, IList<string> Desc, byte[] ImageData,string Remark)
        {
            try
            {
                var bmp = SKBitmap.Decode(ImageData);
                var g = new SKCanvas(bmp);
                
                var paint = new SKPaint();
                paint.Color = SKColors.Red;
                paint.Style = SKPaintStyle.Stroke;
                var paintText = new SKPaint();
                paintText.TextSize = 20;
                paintText.Color = SKColors.Black;
                var counter = 0;
               
                    foreach (var box in boxes)
                    {
                        var rect = new SKRect(box.Left, box.Top, box.Left+box.Width, box.Top+box.Height);
                        g.DrawRect(rect,paint);
                        counter++;
                        if (counter < Desc.Count)
                        {
                            g.DrawText(Desc[counter], box.Left, box.Top, paintText);
                        }
                    }
                    if(!string.IsNullOrEmpty(Remark))
                    {
                        g.DrawText(Remark, 5, 5, paintText);
                    }
                g.Dispose();
                if (bmp != null)
                    return bmp.Encode(SKEncodedImageFormat.Jpeg,100).ToArray();
                

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            return default;
        }
        
        public static byte[] DrawPolygon(IList<System.Drawing.PointF[]> polygons, IList<string> Desc, byte[] ImageData,string Remark)
        {
            try
            {
                var bmp = SKBitmap.Decode(ImageData);
                var g = new SKCanvas(bmp);

                var paint = new SKPaint();
                paint.Color = SKColors.LightGreen;
                paint.Style = SKPaintStyle.Stroke;
                var paintText = new SKPaint();
                paintText.TextSize = 20;
                paintText.Color = SKColors.Black;
                var counter = 0;
                
               
                    foreach (var poly in polygons)
                    {
                        if (poly.Length > 0)
                        {
                            var points = poly.Select(x => new SKPoint(x.X,x.Y)).ToArray();
                            g.DrawPoints(SKPointMode.Polygon,points,paint);
                            counter++;
                            if (counter < Desc.Count)
                            {
                                g.DrawText(Desc[counter], poly[0].X, poly[0].Y,paintText);
                            }
                        }
                    }
                    if(!string.IsNullOrEmpty(Remark))
                    {
                        g.DrawText(Remark, 5, 5, paintText);
                    }

                g.Dispose();
                if (bmp != null)
                    return bmp.Encode(SKEncodedImageFormat.Jpeg, 100).ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            return default;
        }

       

        public static string ConvertToBase64(byte[] data)
        {
            string base64String = Convert.ToBase64String(data, 0, data.Length);
            var img = "data:image/png;base64," + base64String;
            return img;
        }
    }
}
