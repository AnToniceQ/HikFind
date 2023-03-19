using System;
using System.Drawing;
using System.Threading.Tasks;

namespace HIKFind
{
    public static class ImageEditor
    {
        async public static Task<Bitmap> Crop(Bitmap image)
        {
            if (image == null || image.Width <= 0 || image.Height <= 0)
            {
                throw new ArgumentException("Invalid input image.");
            }

            Rectangle bounds = await FindObjectBounds(image);
            Bitmap croppedImage = new Bitmap(bounds.Width, bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(croppedImage);
            g.DrawImage(image, new Rectangle(0, 0, croppedImage.Width, croppedImage.Height), bounds, GraphicsUnit.Pixel);
            g.Dispose();
            return croppedImage;
        }

        public static Task<Bitmap> WhiteBackground(Bitmap image)
        {
            if (image == null || image.Width <= 0 || image.Height <= 0)
            {
                throw new ArgumentException("Invalid input image.");
            }

            Bitmap whiteBackgroundImage = new Bitmap(image.Width, image.Height, image.PixelFormat);
            using (Graphics g = Graphics.FromImage(whiteBackgroundImage))
            {
                g.Clear(Color.White);
                g.DrawImage(image, 0, 0);
            }

            return Task.FromResult(whiteBackgroundImage);
        }

        private static Task<Rectangle> FindObjectBounds(Bitmap image)
        {
            int centerX = image.Width / 2;
            int centerY = image.Height / 2;

            int minX = centerX;
            int minY = centerY;
            int maxX = centerX;
            int maxY = centerY;

            int xStep = Math.Max(1, centerX / 2);
            int yStep = Math.Max(1, centerY / 2);

            while (yStep > 0)
            {
                bool foundMinY = false;
                bool foundMaxY = false;

                for (int y = minY - yStep; y >= 0; y -= yStep)
                {
                    if (HasNonTransparentPixelsInRow(image, y))
                    {
                        minY = y;
                        foundMinY = true;
                        break;
                    }
                }

                for (int y = maxY + yStep; y < image.Height; y += yStep)
                {
                    if (HasNonTransparentPixelsInRow(image, y))
                    {
                        maxY = y;
                        foundMaxY = true;
                        break;
                    }
                }

                if (!foundMinY && !foundMaxY)
                {
                    yStep /= 2;
                }
            }

            while (xStep > 0)
            {
                bool foundMinX = false;
                bool foundMaxX = false;

                for (int x = minX - xStep; x >= 0; x -= xStep)
                {
                    if (HasNonTransparentPixelsInColumn(image, x))
                    {
                        minX = x;
                        foundMinX = true;
                        break;
                    }
                }

                for (int x = maxX + xStep; x < image.Width; x += xStep)
                {
                    if (HasNonTransparentPixelsInColumn(image, x))
                    {
                        maxX = x;
                        foundMaxX = true;
                        break;
                    }
                }

                if (!foundMinX && !foundMaxX)
                {
                    xStep /= 2;
                }
            }

            Rectangle bounds = new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
            return Task.FromResult(bounds);
        }

        static bool HasNonTransparentPixelsInRow(Bitmap bitmap, int row)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                Color pixelColor = bitmap.GetPixel(x, row);
                if (pixelColor.A != 0)
                {
                    return true;
                }
            }
            return false;
        }

        static bool HasNonTransparentPixelsInColumn(Bitmap bitmap, int col)
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                Color pixelColor = bitmap.GetPixel(col, y);
                if (pixelColor.A != 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}