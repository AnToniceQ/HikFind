using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace HIKFind.ImageEditing
{
    public static class ImageEditor
    {
        async public static Task<Bitmap> FastCrop(Bitmap image)
        {
            Random rnd = new Random();

            Color backgroundColor = image.GetPixel(0, 0);

            int w = image.Width;
            int h = image.Height;

            int topMostBorder;
            int leftMostBorder;
            int rightMostBorder;
            int bottomMostBorder;

            ImageDirectionEnum top = ImageDirectionEnum.Top;
            ImageDirectionEnum left = ImageDirectionEnum.Left;
            ImageDirectionEnum right = ImageDirectionEnum.Right;
            ImageDirectionEnum bottom = ImageDirectionEnum.Bottom;

            int startingPointW = w / 2;
            int startingPointH = h / 2;

            Func<ImageDirectionEnum, Bitmap, int> LastNaNBackgroundPixelInLine =
                (direction, bmp) =>
                {
                    if (direction == ImageDirectionEnum.Top)
                    {
                        for (int i = 0; i < startingPointH; i++)
                        {
                            if (bmp.GetPixel(startingPointW, i) != backgroundColor)
                            {
                                return i;
                            }
                        }
                        return h - 1;
                    }

                    if (direction == ImageDirectionEnum.Left)
                    {
                        for (int i = 0; i < startingPointW; i++)
                        {
                            if (bmp.GetPixel(i, startingPointH) != backgroundColor)
                            {
                                return i;
                            }
                        }
                        return 0;
                    }

                    if (direction == ImageDirectionEnum.Right)
                    {
                        for (int i = w - 1; i >= startingPointW; i--)
                        {
                            if (bmp.GetPixel(i, startingPointH) != backgroundColor)
                            {
                                return i;
                            }
                        }
                        return w - 1;
                    }

                    for (int i = h - 1; i >= startingPointH; i--)
                    {
                        if (bmp.GetPixel(startingPointW, i) != backgroundColor)
                        {
                            return i;
                        }
                    }
                    return 0;
                };
            Func<ImageDirectionEnum, int, Bitmap, int> LastNaNBackgroundPixelInDirection =
                (direction, position, bmp) =>
                {
                    int index;
                    int mainIndex = position;
                    int moverX;
                    int moverY;
                    if (direction == ImageDirectionEnum.Top || direction == ImageDirectionEnum.Bottom)
                    {
                        if (direction == ImageDirectionEnum.Top)
                        {
                            moverY = -1;
                        }
                        else
                        {
                            moverY = 1;
                        }
                        for (int i = 0; i < 2; i++)
                        {
                            index = position;
                            if (i == 0)
                            {
                                moverX = 1;
                            }
                            else
                            {
                                moverX = -1;
                            }
                            while (index < w - 1 && mainIndex < h - 1 && index > 0 && mainIndex > 0)
                            {
                                if (bmp.GetPixel(index + moverX, mainIndex) != backgroundColor)
                                {
                                    mainIndex += moverY;
                                }
                                else
                                {
                                    index += moverX;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (direction == ImageDirectionEnum.Left)
                        {
                            moverX = -1;
                        }
                        else
                        {
                            moverX = 1;
                        }
                        for (int i = 0; i < 2; i++)
                        {
                            index = position;
                            if (i == 0)
                            {
                                moverY = 1;
                            }
                            else
                            {
                                moverY = -1;
                            }

                            while (index < h - 1 && mainIndex < w - 1 && index > 0 && mainIndex > 0)
                            {
                                if (bmp.GetPixel(mainIndex, index + moverY) != backgroundColor)
                                {
                                    mainIndex += moverX;
                                }
                                else
                                {
                                    index += moverY;
                                }
                            }
                        }
                    }
                    return mainIndex;
                };

            Func<ImageDirectionEnum, Bitmap, int> GetBorderPoint =
                (direction, bmp) =>
                {
                    int mostPixelInLine = LastNaNBackgroundPixelInLine(direction, bmp);
                    int mostPixelInDirection = LastNaNBackgroundPixelInDirection(direction, mostPixelInLine, bmp);

                    return mostPixelInDirection;
                };


            for (int i = 0; i < 100; i++)
            {
                if (image.GetPixel(startingPointW, startingPointH) != backgroundColor)
                {
                    Bitmap topImage = new Bitmap(image);
                    Bitmap rightImage = new Bitmap(image);
                    Bitmap leftImage = new Bitmap(image);
                    Bitmap bottomImage = new Bitmap(image);

                    Task<int> taskTopMostBorder = Task.Run(() => GetBorderPoint(top, topImage));
                    Task<int> taskLeftMostBorder = Task.Run(() => GetBorderPoint(left, leftImage));
                    Task<int> taskRightMostBorder = Task.Run(() => GetBorderPoint(right, rightImage));
                    Task<int> taskBottomMostBorder = Task.Run(() => GetBorderPoint(bottom, bottomImage));

                    topMostBorder = await taskTopMostBorder;
                    leftMostBorder = await taskLeftMostBorder;
                    rightMostBorder = await taskRightMostBorder;
                    bottomMostBorder = await taskBottomMostBorder;

                    int croppedWidth = rightMostBorder - leftMostBorder;
                    int croppedHeight = bottomMostBorder - topMostBorder;

                    var target = new Bitmap(croppedWidth, croppedHeight);
                    using (Graphics g = Graphics.FromImage(target))
                    {
                        g.DrawImage(image,
                          new RectangleF(0, 0, croppedWidth, croppedHeight),
                          new RectangleF(leftMostBorder, topMostBorder, croppedWidth, croppedHeight),
                          GraphicsUnit.Pixel);
                    }
                    return target;
                }
                startingPointW = rnd.Next(w);
                startingPointH = rnd.Next(h);
            }
            return FullCrop(image).Result;
        }
        async public static Task<Bitmap> FullCrop(Bitmap image)
        {
            Color backgroundColor = image.GetPixel(0, 0);
            int w = image.Width;
            int h = image.Height;
            Func<int, bool> allRow = row =>
            {
                for (int i = 0; i < w; ++i)
                    if (image.GetPixel(i, row) != backgroundColor)
                        return false;
                return true;
            };
            Func<int, bool> allColumn = col =>
            {
                for (int i = 0; i < h; ++i)
                    if (image.GetPixel(col, i) != backgroundColor)
                        return false;
                return true;
            };
            int topmost = 0;
            for (int row = 0; row < h; ++row)
            {
                if (allRow(row))
                    topmost = row;
                else break;
            }
            int bottommost = 0;
            for (int row = h - 1; row >= 0; --row)
            {
                if (allRow(row))
                    bottommost = row;
                else break;
            }
            int leftmost = 0, rightmost = 0;
            for (int col = 0; col < w; ++col)
            {
                if (allColumn(col))
                    leftmost = col;
                else
                    break;
            }
            for (int col = w - 1; col >= 0; --col)
            {
                if (allColumn(col))
                    rightmost = col;
                else
                    break;
            }
            if (rightmost == 0) rightmost = w;
            if (bottommost == 0) bottommost = h;
            int croppedWidth = rightmost - leftmost;
            int croppedHeight = bottommost - topmost;
            if (croppedWidth == 0)
            {
                leftmost = 0;
                croppedWidth = w;
            }
            if (croppedHeight == 0)
            {
                topmost = 0;
                croppedHeight = h;
            }
            var target = new Bitmap(croppedWidth, croppedHeight);
            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(image,
                  new RectangleF(0, 0, croppedWidth, croppedHeight),
                  new RectangleF(leftmost, topmost, croppedWidth, croppedHeight),
                  GraphicsUnit.Pixel);
            }
            return target;
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
    }
}