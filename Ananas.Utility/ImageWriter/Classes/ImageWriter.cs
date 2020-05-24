using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Ananas.Utility.ImageWriter.Helper;
using Ananas.Utility.ImageWriter.Interface;
using Microsoft.AspNetCore.Http;

namespace Ananas.Utility.ImageWriter.Classes {
    public class ImageWriter : IImageWriter {
        public async Task<string> UploadImage (IFormFile file) {
            if (CheckIfImageFile (file)) {
                try {
                    return await WriteFile (file);
                } catch (Exception e) {
                    throw e;
                }
            } else {
                throw new Exception ("Invalid image file");
            }
        }
        public async Task<string> UploadImage (IFormFile file, string place) {
            if (CheckIfImageFile (file)) {
                try {
                    return await WriteFile (file, place);
                } catch (Exception e) {
                    throw e;
                }
            } else {
                throw new Exception ("Invalid image file");
            }
        }
        /// <summary>
        /// Method to check if file is image file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool CheckIfImageFile (IFormFile file) {
            byte[] fileBytes;
            using (var ms = new MemoryStream ()) {
                file.CopyTo (ms);
                fileBytes = ms.ToArray ();
            }

            return WriterHelper.CheckAcceptImageFormat (fileBytes);
        }

        /// <summary>
        /// Method to write file onto the disk
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<string> WriteFile (IFormFile file) {
            string fileName;
            try {
                var extension = "." + file.FileName.Split ('.') [file.FileName.Split ('.').Length - 1];
                fileName = Guid.NewGuid ().ToString () + extension; //Create a new Name 
                //for the file due to security reasons.
                var path = Path.Combine (Directory.GetCurrentDirectory (), "wwwroot/images", fileName);

                using (var bits = new FileStream (path, FileMode.Create)) {
                    await file.CopyToAsync (bits);
                }
            } catch (Exception e) {
                return e.Message;
            }

            return fileName;
        }
        /// <summary>
        /// Method to write file onto the disk
        /// </summary>
        /// <param name="file"></param>
        /// <param name="place"></param>
        /// <returns></returns>
        public async Task<string> WriteFile (IFormFile file, string place) {
            string fileName;
            try {
                var extension = "." + file.FileName.Split ('.') [file.FileName.Split ('.').Length - 1];
                fileName = Guid.NewGuid ().ToString () + extension; //Create a new Name 
                //for the file due to security reasons.
                var path = Path.Combine (Directory.GetCurrentDirectory (), $"wwwroot/images/{place}", fileName);

                using (var bits = new FileStream (path, FileMode.Create)) {
                    await file.CopyToAsync (bits);
                }
            } catch (Exception e) {
                return e.Message;
            }

            return fileName;
        }

        public async Task<int> RemoveImage (string fileName, string place) {
            int result = -1;
            string filePath = Path.Combine (Directory.GetCurrentDirectory (), $"wwwroot/images/{place}", fileName);
            if (File.Exists (filePath)) {
                File.Delete (filePath);
                result = 1;
            }
            return result;
        }
    }
}