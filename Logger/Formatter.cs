using System.Text;

namespace PortgateLib.Logger
{
	public static class Formatter
	{
		public static string AlignLeftBetweenBrackets(string s, int size)
		{
			var startSpaces = 1;
			var remainingSpaces = size - s.Length;
			var endSpaces = remainingSpaces - 1;

			return $"[{Space(startSpaces)}{s}{Space(endSpaces)}]";
		}

		private static string Space(int size)
		{
			var stringBuilder = new StringBuilder();
			for (var i = 0; i < size; i++)
			{
				stringBuilder.Append(" ");
			}
			return stringBuilder.ToString();
		}
	}
}