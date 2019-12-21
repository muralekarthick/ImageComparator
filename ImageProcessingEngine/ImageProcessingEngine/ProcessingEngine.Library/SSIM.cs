using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingEngine.Library
{
    internal sealed class Grid
    {
        private double[,] _data;
        private int _width;
        private int _height;

        public Grid(int width, int height)
        {
            _data = new double[width, height];
            _width = width;
            _height = height;
        }

        public double this[int i, int j]
        {
            get { return _data[i, j]; }
            set { _data[i, j] = value; }
        }

        public double Total
        {
            get
            {
                double s = 0;
                foreach (var d in _data)
                {
                    s += d;
                }
                return s;
            }
        }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public static Grid operator +(Grid a, Grid b)
        {
            return Op((i, j) => a[i, j] + b[i, j], new Grid(a._width, a._height));
        }

        public static Grid operator -(Grid a, Grid b)
        {
            return Op((i, j) => a[i, j] - b[i, j], new Grid(a._width, a._height));
        }

        public static Grid operator *(Grid a, Grid b)
        {
            return Op((i, j) => a[i, j] * b[i, j], new Grid(a._width, a._height));
        }

        public static Grid operator /(Grid a, Grid b)
        {
            return Op((i, j) => a[i, j] / b[i, j], new Grid(a._width, a._height));
        }

        public static Grid Op(Func<int, int, double> f, Grid g)
        {
            int width = g._width;
            int height = g._height;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    g[i, j] = f(i, j);
                }
            }
            return g;
        }

        public static Grid CreateGaussianGrid(int size, double sigma)
        {
            var filter = new Grid(size, size);
            double s2 = sigma * sigma, c = (size - 1) / 2.0, dx, dy;

            Grid.Op(
                (i, j) =>
                {
                    dx = i - c;
                    dy = j - c;
                    return Math.Exp(-(dx * dx + dy * dy) / (2 * s2));
                },
                filter
            );

            var scale = 1.0 / filter.Total;
            Grid.Op((i, j) => filter[i, j] * scale, filter);
            return filter;
        }

        public static Grid SubSample(Grid img, int skip)
        {
            int width = img.Width;
            int height = img.Height;
            double scale = 1.0 / (skip * skip);
            var ans = new Grid(width / skip, height / skip);
            for (int i = 0; i < width - skip; i += skip)
            {
                for (int j = 0; j < height - skip; j += skip)
                {
                    double sum = 0;
                    for (int x = i; x < i + skip; ++x)
                    {
                        for (int y = j; y < j + skip; ++y)
                        {
                            sum += img[x, y];
                        }
                    }
                    ans[i / skip, j / skip] = sum * scale;
                }
            }
            return ans;
        }

        public static Grid Filter(Grid firstGrid, Grid secondGrid)
        {
            int ax = firstGrid.Width;
            int ay = firstGrid.Height;
            int bx = secondGrid.Width;
            int by = secondGrid.Height;
            int bcx = (bx + 1) / 2, bcy = (by + 1) / 2;
            var thirdGrid = new Grid(ax - bx + 1, ay - by + 1);
            for (int i = bx - bcx + 1; i < ax - bx; ++i)
            {
                for (int j = by - bcy + 1; j < ay - by; ++j)
                {
                    double sum = 0;
                    for (int x = bcx - bx + 1 + i; x < 1 + i + bcx; ++x)
                    {
                        for (int y = bcy - by + 1 + j; y < 1 + j + bcy; ++y)
                        {
                            sum += firstGrid[x, y] * secondGrid[bx - bcx - 1 - i + x, by - bcy - 1 - j + y];
                        }
                    }
                    thirdGrid[i - bcx, j - bcy] = sum;
                }
            }
            return thirdGrid;
        }

        public static Grid Linear(double s, Grid a, double c)
        {
            return Grid.Op((i, j) => s * a[i, j] + c, new Grid(a.Width, a.Height));
        }
    }
    public static class SSIM
    {
        private static readonly double K1 = 0.01;
        private static readonly double K2 = 0.03;
        private static readonly double L = 255;
        private static readonly Grid window = Grid.CreateGaussianGrid(11, 1.5);

        /// <summary>
        /// Computes the SSIM of two different images
        /// </summary>
        /// <param name="first">The first image</param>
        /// <param name="second">The second image</param>
        /// <returns>
        /// A floating point number between 0.0 and 1.0 signifying the SSIM between
        /// two images
        /// </returns>
        public static double Compute(Image first, Image second)
        {
            if (first == null || second == null)
            {
                throw new ArgumentNullException();
            }

            using (var firstBitmap = new Bitmap(first))
            {
                using (var secondBitmap = new Bitmap(second))
                {
                    return 1 - ComputeSSIM(
                        ConvertToGrayscale(firstBitmap),
                        ConvertToGrayscale(secondBitmap)
                    );
                }
            }
        }

        /// <summary>
        /// Calculate the SSIM between two IImageFrames
        /// </summary>
        /// <param name="first">The first image</param>
        /// <param name="second">The second image</param>
        /// <returns>The SSIM of two IImageFrames</returns>
        //public static double Compute(IImageFrame first, IImageFrame second)
        //{
        //    if (first == null || second == null)
        //    {
        //        throw new ArgumentNullException();
        //    }

        //    return ComputeSSIM(
        //        ConvertToGrayscale(first),
        //        ConvertToGrayscale(second)
        //    );
        //}

        //private static Grid ConvertToGrayscale(IImageFrame bitmap)
        //{
        //    return Grid.Op(
        //        (i, j) => ConvertColorToGrayscaleDouble(bitmap.GetPixel(i, j)),
        //        new Grid(bitmap.Width, bitmap.Height)
        //    );
        //}

        private static Grid ConvertToGrayscale(Bitmap bitmap)
        {
            return Grid.Op(
                (i, j) => ConvertColorToGrayscaleDouble(bitmap.GetPixel(i, j)),
                new Grid(bitmap.Width, bitmap.Height)
            );
        }

        private static double ConvertColorToGrayscaleDouble(Color color)
        {
            return 0.3 * color.R + 0.59 * color.G + 0.11 * color.B;
        }

        private static double ComputeSSIM(Grid img1, Grid img2)
        {
            // uses notation from paper
            // automatic downsampling
            int f = (int)Math.Max(1, Math.Round(Math.Min(img1.Width, img1.Height) / 256.0));
            if (f > 1)
            {
                // downsampling by f
                // use a simple low-pass filter and subsample by f
                img1 = Grid.SubSample(img1, f);
                img2 = Grid.SubSample(img2, f);
            }

            // normalize window - todo - do in window set {}
            double scale = 1.0 / window.Total;
            Grid.Op((i, j) => window[i, j] * scale, window);

            // image statistics
            var mu1 = Grid.Filter(img1, window);
            var mu2 = Grid.Filter(img2, window);

            var mu1mu2 = mu1 * mu2;
            var mu1SQ = mu1 * mu1;
            var mu2SQ = mu2 * mu2;

            var sigma12 = Grid.Filter(img1 * img2, window) - mu1mu2;
            var sigma1SQ = Grid.Filter(img1 * img1, window) - mu1SQ;
            var sigma2SQ = Grid.Filter(img2 * img2, window) - mu2SQ;

            // constants from the paper
            double C1 = K1 * L; C1 *= C1;
            double C2 = K2 * L; C2 *= C2;

            Grid ssimMap = null;
            if ((C1 > 0) && (C2 > 0))
            {
                ssimMap = Grid.Op((i, j) =>
                    (2 * mu1mu2[i, j] + C1) * (2 * sigma12[i, j] + C2) /
                    (mu1SQ[i, j] + mu2SQ[i, j] + C1) / (sigma1SQ[i, j] + sigma2SQ[i, j] + C2),
                    new Grid(mu1mu2.Width, mu1mu2.Height));
            }
            else
            {
                var num1 = Grid.Linear(2, mu1mu2, C1);
                var num2 = Grid.Linear(2, sigma12, C2);
                var den1 = Grid.Linear(1, mu1SQ + mu2SQ, C1);
                var den2 = Grid.Linear(1, sigma1SQ + sigma2SQ, C2);

                var den = den1 * den2; // total denominator
                ssimMap = new Grid(mu1.Width, mu1.Height);
                for (int i = 0; i < ssimMap.Width; ++i)
                    for (int j = 0; j < ssimMap.Height; ++j)
                    {
                        ssimMap[i, j] = 1;
                        if (den[i, j] > 0)
                        {
                            ssimMap[i, j] = num1[i, j] * num2[i, j] / (den1[i, j] * den2[i, j]);
                        }
                        else if ((den1[i, j] != 0) && (den2[i, j] == 0))
                        {
                            ssimMap[i, j] = num1[i, j] / den1[i, j];
                        }
                    }
            }

            //return ssimMap.Total % (ssimMap.Width * ssimMap.Height);

            // average all values
            return ssimMap.Total / (ssimMap.Width * ssimMap.Height);
        } // ComputeSSIM
    }

}
