using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kofax.Base64ConnectorV1
{
    internal class FileToBase64Encoder
    {
        public static string EncodeFileToBase64(string filePath)
        {
            try
            {
                // Check if the file exists
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("File not found.", filePath);
                }

                // Read the file contents
                byte[] fileBytes = File.ReadAllBytes(filePath);

                // Convert the file contents to Base64
                string base64String = Convert.ToBase64String(fileBytes);

                return base64String;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might occur during file reading or encoding
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        public static string EncodeFileToBase64(byte[] fileSerialized)
        {
            try
            {
                // Convert the file contents to Base64
                string base64String = Convert.ToBase64String(fileSerialized);

                return base64String;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might occur during file reading or encoding
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        public static string GetMimeType(string fileExtension, byte[] fileBytes)
        {
            // Attempt to get MIME type based on the file extension
            if (!fileExtension.StartsWith(".")) { fileExtension = "." + fileExtension; }
            string mimeType = "application/octet-stream"; // Default unknown type
            if (!string.IsNullOrWhiteSpace(fileExtension))
            {
                try
                {
                    mimeType = System.Web.MimeMapping.GetMimeMapping(fileExtension);
                }
                catch
                {
                    // Handle exceptions if the MimeMapping fails or is unavailable
                }

                // If a valid MIME type was retrieved from the extension, return it
                if (!string.IsNullOrWhiteSpace(mimeType) && !mimeType.Equals("application/octet-stream", StringComparison.OrdinalIgnoreCase))
                {
                    return mimeType;
                }
            }

            // If extension-based lookup failed, use byte-signature-based lookup
            if (fileBytes == null || fileBytes.Length < 4)
                return mimeType;  // Default unknown type

            // Define file signatures in byte arrays
            byte[] jpg = new byte[] { 0xFF, 0xD8 };
            byte[] png = new byte[] { 0x89, 0x50, 0x4E, 0x47 };
            byte[] gif = new byte[] { 0x47, 0x49, 0x46 };
            byte[] tiffI = new byte[] { 0x49, 0x49, 0x2A, 0x00 };
            byte[] tiffM = new byte[] { 0x4D, 0x4D, 0x00, 0x2A };
            byte[] pdf = new byte[] { 0x25, 0x50, 0x44, 0x46 };

            // Compare file signature with defined signatures
            if (fileBytes.Take(jpg.Length).SequenceEqual(jpg))
                return "image/jpeg";

            if (fileBytes.Take(png.Length).SequenceEqual(png))
                return "image/png";

            if (fileBytes.Take(gif.Length).SequenceEqual(gif))
                return "image/gif";

            if (fileBytes.Take(tiffI.Length).SequenceEqual(tiffI) || fileBytes.Take(tiffM.Length).SequenceEqual(tiffM))
                return "image/tiff";

            if (fileBytes.Take(pdf.Length).SequenceEqual(pdf))
                return "application/pdf";

            return mimeType;  // Return default type if none of the byte signatures match
        }

    }
}
