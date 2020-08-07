using System.Security.Cryptography;
using System.Text;

namespace CinemaTicketing.Helpers
{
	public static class Utils
	{
		public static string GetMD5WithString(this string str)
		{
			StringBuilder md5Str = new StringBuilder();
			byte[] data = Encoding.GetEncoding("utf-8").GetBytes(str);
			System.Security.Cryptography.MD5 md5 = new MD5CryptoServiceProvider();
			byte[] bytes = md5.ComputeHash(data);
			for (int i = 0; i < bytes.Length; i++)
			{
				md5Str.Append(bytes[i].ToString("x2"));
			}
			return md5Str.ToString();
		}
	}
}
