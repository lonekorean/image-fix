using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace CodersBlock.ImageFix
{
    public static class ImageFixer
    {
        /// <summary>
        /// Scales and crops an image to a more usable size.
        /// </summary>
        public static byte[] ResizeImage(byte[] imageFile, int newWidth, int newHeight, long quality)
        {
            // turn byte array into stream and image
            var originalStream = new MemoryStream(imageFile);
            var originalImg = Image.FromStream(originalStream, false, true);

            // defaults values
            double scaledWidth = newWidth;
            double scaledHeight = newHeight;
            int offsetX = 0;
            int offsetY = 0;

            // make adjustments if aspect ratios differ
            var originalAspectRatio = (double)originalImg.Width / (double)originalImg.Height;
            var newAspectRatio = (double)newWidth / (double)newHeight;
            if (newAspectRatio > originalAspectRatio)
            {
                // wider ratio, adust vertically to scale
                var widthScale = (double)newWidth / (double)originalImg.Width;
                scaledHeight = Math.Ceiling(originalImg.Height * widthScale);
                offsetY = (int)Math.Floor((scaledHeight - newHeight) / 2);
            }
            else if (newAspectRatio < originalAspectRatio)
            {
                // taller ratio, adjust horizontally to scale
                var heightScale = (double)newHeight / (double)originalImg.Height;
                scaledWidth = Math.Ceiling(originalImg.Width * heightScale);
                offsetX = (int)Math.Floor((scaledWidth - newWidth) / 2);
            }

            // color depth and resolution settings
            var bitmap = new Bitmap(newWidth, newHeight);
            bitmap.SetResolution(72, 72);

            // anti-aliasing and quality
            var g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // crop and resize
            var newImg = Image.FromStream(originalStream);
            var destRect = new Rectangle(-offsetX, -offsetY, (int)scaledWidth, (int)scaledHeight);
            g.DrawImage(newImg, destRect, 0, 0, originalImg.Width, originalImg.Height, GraphicsUnit.Pixel);

            // save as jpg and convert back to a byte array
            var newStream = new MemoryStream();
            var encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);
            bitmap.Save(newStream, GetEncoder(ImageFormat.Jpeg), encoderParams);

            // dispose of resources and possible file locks
            originalImg.Dispose();
            newImg.Dispose();
            bitmap.Dispose();
            g.Dispose();

            // return the new byte array
            return newStream.ToArray();
        }

        /// <summary>
        /// Finds the appropriate ImageCodecInfo given an ImageFormat.
        /// </summary>
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders())
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
