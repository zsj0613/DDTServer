using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
namespace Bussiness
{
	public class CheckCode
	{
		private enum RandomStringMode
		{
			LowerLetter,
			UpperLetter,
			Letter,
			Digital,
			Mix
		}
		public static ThreadSafeRandom random = new ThreadSafeRandom();
		private static Color[] c = new Color[]
		{
			Color.Black,
			Color.Red,
			Color.DarkBlue,
			Color.Green,
			Color.Orange,
			Color.Brown,
			Color.DarkCyan,
			Color.Purple
		};
		private static string[] font = new string[]
		{
			"Verdana",
			"Microsoft Sans Serif",
			"Comic Sans MS",
			"Arial",
			"宋体"
		};
		private static char[] digitals = new char[]
		{
			'2',
			'3',
			'4',
			'6',
			'7',
			'8',
			'9'
		};
		private static char[] lowerLetters = new char[]
		{
			'a',
			'b',
			'c',
			'd',
			'e',
			'f',
			'h',
			'k',
			'm',
			'n',
			'p',
			'q',
			'r',
			't',
			'u',
			'w',
			'x',
			'y',
			'z'
		};
		private static char[] upperLetters = new char[]
		{
			'A',
			'B',
			'C',
			'D',
			'E',
			'F',
			'G',
			'H',
			'K',
			'M',
			'N',
			'P',
			'Q',
			'R',
			'T',
			'U',
			'W',
			'X',
			'Y',
			'Z'
		};
		private static char[] letters = new char[]
		{
			'a',
			'b',
			'c',
			'd',
			'e',
			'f',
			'g',
			'h',
			'i',
			'j',
			'k',
			'm',
			'n',
			'p',
			'q',
			'r',
			't',
			'u',
			'w',
			'x',
			'y',
			'z',
			'A',
			'B',
			'C',
			'D',
			'E',
			'F',
			'G',
			'H',
			'I',
			'J',
			'K',
			'M',
			'N',
			'P',
			'Q',
			'R',
			'T',
			'U',
			'W',
			'X',
			'Y',
			'Z'
		};
		private static char[] mix = new char[]
		{
			'2',
			'3',
			'4',
			'6',
			'7',
			'8',
			'9',
			'a',
			'b',
			'c',
			'd',
			'e',
			'f',
			'h',
			'k',
			'm',
			'n',
			'p',
			'q',
			'r',
			't',
			'u',
			'w',
			'x',
			'y',
			'z',
			'A',
			'B',
			'C',
			'D',
			'E',
			'F',
			'G',
			'H',
			'K',
			'M',
			'N',
			'P',
			'Q',
			'R',
			'T',
			'U',
			'W',
			'X',
			'Y',
			'Z'
		};
		public static byte[] CreateImage(string randomcode)
		{
			int randAngle = 30;
			int mapwidth = randomcode.Length * 30;
			Bitmap map = new Bitmap(mapwidth, 36);
			Graphics graph = Graphics.FromImage(map);
			byte[] result;
			try
			{
				graph.Clear(Color.White);
				int cindex = CheckCode.random.Next(7);
				Brush b = new SolidBrush(CheckCode.c[cindex]);
				for (int i = 0; i < 1; i++)
				{
					int x = CheckCode.random.Next(map.Width / 2);
					int x2 = CheckCode.random.Next(map.Width * 3 / 4, map.Width);
					int y = CheckCode.random.Next(map.Height);
					int y2 = CheckCode.random.Next(map.Height);
					graph.DrawBezier(new Pen(CheckCode.c[cindex], 2f), (float)x, (float)y, (float)((x + x2) / 4), 0f, (float)((x + x2) * 3 / 4), (float)map.Height, (float)x2, (float)y2);
				}
				char[] chars = randomcode.ToCharArray();
				StringFormat format = new StringFormat(StringFormatFlags.NoClip);
				format.Alignment = StringAlignment.Center;
				format.LineAlignment = StringAlignment.Center;
				for (int i = 0; i < chars.Length; i++)
				{
					int findex = CheckCode.random.Next(5);
					Font f = new Font(CheckCode.font[findex], 22f, FontStyle.Bold);
					Point dot = new Point(16, 16);
					float angle = (float)CheckCode.random.Next(-randAngle, randAngle);
					graph.TranslateTransform((float)dot.X, (float)dot.Y);
					graph.RotateTransform(angle);
					graph.DrawString(chars[i].ToString(), f, b, 1f, 1f, format);
					graph.RotateTransform(-angle);
					graph.TranslateTransform(2f, (float)(-(float)dot.Y));
				}
				MemoryStream ms = new MemoryStream();
				map.Save(ms, ImageFormat.Gif);
				result = ms.ToArray();
			}
			finally
			{
				graph.Dispose();
				map.Dispose();
			}
			return result;
		}
		private static string GenerateRandomString(int length, CheckCode.RandomStringMode mode)
		{
			string rndStr = string.Empty;
			string result;
			if (length == 0)
			{
				result = rndStr;
			}
			else
			{
				int[] array = new int[2];
				switch (mode)
				{
				case CheckCode.RandomStringMode.LowerLetter:
					for (int i = 0; i < length; i++)
					{
						rndStr += CheckCode.lowerLetters[CheckCode.random.Next(0, CheckCode.lowerLetters.Length)];
					}
					break;
				case CheckCode.RandomStringMode.UpperLetter:
					for (int i = 0; i < length; i++)
					{
						rndStr += CheckCode.upperLetters[CheckCode.random.Next(0, CheckCode.upperLetters.Length)];
					}
					break;
				case CheckCode.RandomStringMode.Letter:
					for (int i = 0; i < length; i++)
					{
						rndStr += CheckCode.letters[CheckCode.random.Next(0, CheckCode.letters.Length)];
					}
					break;
				case CheckCode.RandomStringMode.Digital:
					for (int i = 0; i < length; i++)
					{
						rndStr += CheckCode.digitals[CheckCode.random.Next(0, CheckCode.digitals.Length)];
					}
					break;
				default:
					for (int i = 0; i < length; i++)
					{
						rndStr += CheckCode.mix[CheckCode.random.Next(0, CheckCode.mix.Length)];
					}
					break;
				}
				result = rndStr;
			}
			return result;
		}
		public static string GenerateCheckCode()
		{
			return CheckCode.GenerateRandomString(4, CheckCode.RandomStringMode.Mix);
		}
	}
}
