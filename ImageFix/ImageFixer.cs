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
        public static byte[] ResizeImage(byte[] imageFile, int scaleWidth, int maxHeight, bool allowStretching, long quality)
        {
            // turn byte array into stream and image
            MemoryStream originalStream = new MemoryStream(imageFile);
            Image originalImg = Image.FromStream(originalStream, false, true);

            if (!allowStretching)
            {
                // don't let scale width exceed original width
                scaleWidth = Math.Min(scaleWidth, originalImg.Width);
            }

            // determine the scaling factor required to resize image to new width
            double scaleFactor = (double)scaleWidth / (double)originalImg.Width;

            // determine if height would be too long, even after scaling
            int newHeight = 0;
            int originalCropHeight = 0;
            if (originalImg.Height * scaleFactor > maxHeight)
            {
                // too long (final image will essentially be scaled from a vertically cropped original image)
                newHeight = maxHeight;
                originalCropHeight = Convert.ToInt32(maxHeight / scaleFactor);
            }
            else
            {
                // height is acceptable, keep same scale as width
                newHeight = Convert.ToInt32(originalImg.Height * scaleFactor);
                originalCropHeight = originalImg.Height;
            }

            // color depth and resolution settings
            Bitmap bitmap = new Bitmap(scaleWidth, newHeight);
            bitmap.SetResolution(72, 72);

            // anti-aliasing and quality
            Graphics g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // crop and resize
            Image newImg = Image.FromStream(originalStream);
            g.DrawImage(newImg, new Rectangle(0, 0, scaleWidth, newHeight), 0, 0, originalImg.Width, originalCropHeight, GraphicsUnit.Pixel);

            // save as jpg and convert back to a byte array
            MemoryStream newStream = new MemoryStream();
            EncoderParameters encoderParams = new EncoderParameters(1);
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
