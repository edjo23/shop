using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace PointOfSale.RT.Services
{
    public class ImageService
    {
        public const string ImageExtension = ".png";

        public ImageService()
        {
        }

        public IList<string> GetImageList()
        {
            var task = GetImageListAsync();
            task.Wait();

            return task.Result;
        }

        protected async Task<IList<string>> GetImageListAsync()
        {
            var files = await Windows.Storage.ApplicationData.Current.TemporaryFolder.GetFilesAsync();

            return new List<string>(files.Where(o => o.FileType == ImageExtension).Select(o => o.Name.Substring(0, o.Name.Length - ImageExtension.Length)));
        }

        public BitmapImage GetImage(string code)
        {            
            return new BitmapImage(new Uri(String.Format("ms-appdata:///temp/{0}{1}", code, ImageExtension)));
        }

        public async void SetImage(string code, byte[] data)
        {
            if (data.Length > 0)
            {
                var result = await Windows.Storage.ApplicationData.Current.TemporaryFolder.CreateFileAsync(code + ImageExtension, CreationCollisionOption.ReplaceExisting);
                await Windows.Storage.FileIO.WriteBytesAsync(result, data);
            }
        }
    }
}
