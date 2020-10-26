using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Telerik.Windows.Controls.Map.VEWPFImageryService;

namespace PinnaFit.WPF
{
    public static class ImageUtil
    {
        public static BitmapImage ToImage(byte[] toImage)
        {
            var image = new BitmapImage();//null;// 
            if (toImage != null)
            {
                //image = new BitmapImage();
                using (var ms = new MemoryStream(toImage))
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                }
            }
            return image;
        }
        public static byte[] ToBytes(BitmapImage image)
        {
            if (image == null || image.UriSource == null)
                return null;

            var imageArray = new byte[0];
            try
            {
                imageArray = File.ReadAllBytes(image.UriSource.AbsolutePath);
            }
            catch
            {
                MessageBox.Show("Problem getting photo.....", "Photo problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //MessageBox.Show("image directory may contain spaces, change the location of the image and try agin.....",
                //    "Invalid directory location", 
                //    MessageBoxButtons.OK, 
                //    MessageBoxIcon.Error);
                return null;
            }
            return imageArray;
        }


        public static Image GetImage(byte[] toImage)
        {
            var byteBlobData = (toImage);
            if (byteBlobData == null) return null;
            var stmBlobData = new MemoryStream(byteBlobData);
            return Image.FromStream(stmBlobData);

        }

    }
}
