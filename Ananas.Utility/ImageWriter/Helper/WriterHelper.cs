using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MimeDetective;
using MimeDetective.Extensions.Graphics;
namespace Ananas.Utility.ImageWriter.Helper {
    public class WriterHelper {
        public static FileType GetImageFormat (byte[] bytes) {
            FileType type = bytes.GetFileType ();
            return type;
        }
        public static bool CheckAcceptImageFormat (byte[] bytes) {
            return AcceptImageType ().Contains (GetImageFormat (bytes));
        }
        public static FileType[] AcceptImageType () {
            return new FileType[] { MimeTypes.PNG, MimeTypes.JPEG, MimeTypes.GIF, MimeTypes.BMP, MimeTypes.ICO };
        }
    }
}