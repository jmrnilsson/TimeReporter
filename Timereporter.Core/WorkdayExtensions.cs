using System;
using System.Text;

namespace Timereporter.Core
{
	public static class WorkdayExtensions
	{
		public static long ToFnv1aHashInt64(this string text)
		{
			long Fnv1a(byte[] bytes_)
			{
				const uint offset = 0x811C9DC5;
				const uint prime = 0x01000193;
				uint hash = offset;

				for (var i = 0; i < bytes_.Length; i++)
				{
					unchecked
					{
						hash ^= bytes_[i];
						hash *= prime;
					}
				}

				return BitConverter.ToInt64(bytes_, 0);
				// Convert.ToBase64String(BitConverter.GetBytes(hash));
			}

			byte[] bytes = Encoding.UTF8.GetBytes(text);
			return Fnv1a(bytes);
		}

		public static int ToFnv1aHashInt32(this string text)
		{
			int Fnv1a(byte[] bytes_)
			{
				const uint offset = 0x811C9DC5;
				const uint prime = 0x01000193;
				uint hash = offset;

				for (var i = 0; i < bytes_.Length; i++)
				{
					unchecked
					{
						hash ^= bytes_[i];
						hash *= prime;
					}
				}

				return BitConverter.ToInt32(bytes_, 0);
				// Convert.ToBase64String(BitConverter.GetBytes(hash));
			}

			byte[] bytes = Encoding.UTF8.GetBytes(text);
			return Fnv1a(bytes);
		}
	}
}
