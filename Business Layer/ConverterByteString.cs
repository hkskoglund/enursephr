using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace eNursePHR.BusinessLayer
{
    public static class ConvertHelper
    {
        public static string convertToString(byte[] bArray)
        {
            if (bArray == null)
                return null;

            if (bArray.Length == 0)
                return null;

            StringBuilder sb = new StringBuilder();
            foreach (byte b in bArray)
                sb.Append((char)b);

            return sb.ToString();
        }

        public static byte[] convertToByteArray(string ntext)
        {
            if (ntext == null)
                return null;

            MemoryStream ms = new MemoryStream();

            foreach (char c in ntext.ToCharArray())
                ms.WriteByte((byte)c);

            return ms.ToArray();

        }


    }
}
