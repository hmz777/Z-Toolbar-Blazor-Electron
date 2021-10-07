using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorElectronToolbar.Server.Helpers
{
    public static class IconTools
    {
        private const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;

        public static byte[] GetIcon(string path, bool isDirectory, byte[] iconBytes = null)
        {
            if (iconBytes?.Length > 0)
                return iconBytes;

            IntPtr hIcon = GetJumboIcon(GetIconIndex(path, isDirectory));
            byte[] IconBytes;

            using (Icon ico = (Icon)System.Drawing.Icon.FromHandle(hIcon).Clone())
            {
                using (var stream = new MemoryStream())
                {
                    ico.ToBitmap().Save(stream, ImageFormat.Png);
                    IconBytes = stream.ToArray();
                }
            }

            _ = NativeMethods.DestroyIcon(hIcon);

            return IconBytes;
        }

        internal static int GetIconIndex(string pszFile, bool isDirectory)
        {
            uint attributes = FILE_ATTRIBUTE_NORMAL;
            if (isDirectory)
                attributes |= FILE_ATTRIBUTE_DIRECTORY;

            SHFILEINFO sfi = new SHFILEINFO();
            NativeMethods.SHGetFileInfo(pszFile
                , attributes //0
                , ref sfi
                , (uint)System.Runtime.InteropServices.Marshal.SizeOf(sfi)
                , (uint)(SHGFI.SysIconIndex | SHGFI.LargeIcon | SHGFI.UseFileAttributes));
            return sfi.iIcon;
        }

        // 256*256
        internal static IntPtr GetJumboIcon(int iImage)
        {
            IImageList spiml = null;
            Guid guil = new Guid(NativeMethods.IID_IImageList); //or IID_IImageList2

            _ = NativeMethods.SHGetImageList(NativeMethods.SHIL_EXTRALARGE, ref guil, ref spiml);
            IntPtr hIcon = IntPtr.Zero;
            _ = spiml.GetIcon(iImage, NativeMethods.ILD_TRANSPARENT | NativeMethods.ILD_IMAGE, ref hIcon); //
            return hIcon;
        }

        internal static byte[] GetByteArrayFromImage(string imagePath)
        {
            using (var image = Image.FromFile(imagePath))
            {
                var converter = new ImageConverter();
                return (byte[])converter.ConvertTo(image, typeof(byte[]));
            }
        }
    }
}
