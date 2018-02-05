using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Cryptography;

namespace OperationOrganise
{
    public class ComparingImages
    {
        public enum CompareResult
        {
            CompareOk,
            PixelMismatch,
            SizeMismatch
        };

        public static CompareResult Compare(Bitmap bmp1, Bitmap bmp2)
        {
            var cr = CompareResult.CompareOk;

            //Test to see if we have the same size of image
            if (bmp1.Size != bmp2.Size)            
                cr = CompareResult.SizeMismatch;
            else
            {
                //Convert each image to a byte array
                var ic = new ImageConverter();
                var btImage1 = new byte[1];
                btImage1 = (byte[])ic.ConvertTo(bmp1, btImage1.GetType());
                var btImage2 = new byte[1];
                btImage2 = (byte[])ic.ConvertTo(bmp2, btImage2.GetType());

                //Compute a hash for each image
                var shaM = new SHA256Managed();
                var hash1 = shaM.ComputeHash(btImage1);
                var hash2 = shaM.ComputeHash(btImage2);

                //Compare the hash values
                for (var i = 0; i < hash1.Length && i < hash2.Length && cr == CompareResult.CompareOk; i++)                
                    if (hash1[i] != hash2[i])
                        cr = CompareResult.PixelMismatch;                
            }
            return cr;
        }
    }
}