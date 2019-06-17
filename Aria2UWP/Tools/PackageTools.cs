using System;
using System.IO.Compression;
using System.Threading.Tasks;
using Windows.Storage;

namespace Aria2UWP.Tools
{
    public static class PackageTools
    {
        /// <summary>
        /// 解压文件到local目录
        /// </summary>
        /// <param name="uri">Zip压缩包地址</param>
        /// <returns></returns>
        public static async Task UnZip(Uri uri)
        {
            StorageFile sf = await StorageFile.GetFileFromApplicationUriAsync(uri);
            sf = await sf.CopyAsync(ApplicationData.Current.LocalFolder, sf.Name, NameCollisionOption.ReplaceExisting);
            using (ZipArchive archive = ZipFile.Open(sf.Path, ZipArchiveMode.Update))
            {
                archive.ExtractToDirectory(ApplicationData.Current.LocalFolder.Path, true);
            }
        }


    }
}
