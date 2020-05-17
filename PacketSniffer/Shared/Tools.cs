using System;
using System.Text;

namespace PacketSniffer.Shared
{
    public static class Tools
    {

        public static T[] GetSubArray<T>(T[] arr, int startIndex, int length)
        {
            if (arr == null || arr.Length < 1 || startIndex < 0 || startIndex >= arr.Length || length <= 0)
            {
                return new T[] { };
            }
            try
            {
                T[] subArray = new T[length];
                Array.Copy(arr, startIndex, subArray, 0, length);
                return subArray;
            }
            catch (Exception ex)
            {
                throw new Exception($"GetSubArray > {ex.Message}");
            }
        }

        public static string ConvertByteArrayToByteString(byte[] byteArr)
        {
            if (byteArr == null || byteArr.Length <= 0)
            {
                return "";
            }
            try
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < byteArr.Length; i++)
                {
                    sb.Append(Convert.ToString(byteArr[i], 2).PadLeft(8, '0'));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"ConvertByteArrayToByteString > {ex.Message}");
            }
        }

        public static string ConvertByteArrayToHex(byte[] byteArr)
        {
            if (byteArr == null || byteArr.Length <= 0)
            {
                return "";
            }
            try
            {
                StringBuilder sb = new StringBuilder(byteArr.Length * 2);
                foreach (byte b in byteArr)
                {
                    sb.AppendFormat("{0:x2}", b);
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"ConvertByteArrayToHex > {ex.Message}");
            }
        }

        public static string ConvertByteArrayToAscii(byte[] byteArr)
        {
            if (byteArr == null || byteArr.Length <= 0)
            {
                return "";
            }
            try
            {
                return Encoding.ASCII.GetString(byteArr);
            }
            catch (Exception ex)
            {
                throw new Exception($"ConvertByteArrayToAscii > {ex.Message}");
            }
        }

        public static string ConvertByteArrayToBase64(byte[] byteArr)
        {
            if (byteArr == null || byteArr.Length <= 0)
            {
                return "";
            }
            try
            {
                return Convert.ToBase64String(byteArr);
            }
            catch (Exception ex)
            {
                throw new Exception($"ConvertByteArrayToBase64 > {ex.Message}");
            }
        }
    }
}
