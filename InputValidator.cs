namespace PortgateLib
{
	public static class InputValidator
	{
		public static int? GetClampedIntInput(string input, bool minRestricted, int minValue, bool maxRestricted, int maxValue)
		{
			if (!int.TryParse(input, out int newValue))
			{
				newValue = 0;
			}
			if (minRestricted && newValue < minValue)
			{
				return minValue;
			}
			if (maxRestricted && newValue > maxValue)
			{
				return maxValue;
			}


			return newValue;
		}

		public static float? GetClampedFloatInput(string input, bool minRestricted, float minValue, bool maxRestricted, float maxValue)
		{
			if (!float.TryParse(input, out float newValue))
			{
				newValue = 0;
			}
			if (minRestricted && newValue < minValue)
			{
				return minValue;
			}
			if (maxRestricted && newValue > maxValue)
			{
				return maxValue;
			}

			return newValue;
		}
	}
}