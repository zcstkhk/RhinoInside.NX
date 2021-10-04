using System;

namespace RhinoInside.NX.Extensions
{
	/// <summary>Provides functions to do unit conversions of various sorts</summary>
	/// <remarks>
	/// Note that these are all multiplicative factors -- to do the conversion, you multiply by
	/// the numbers given below.
	/// </remarks>
	internal static class UnitConversion
	{
		/// <summary>Multiply by this number to convert Part Units to Millimeters (1 or 25.4)</summary>
		internal static double PartUnitsToMillimeters
		{
			get
			{
				return Globals.MillimetersPerUnit;
			}
		}

		/// <summary>Multiply by this number to convert Millimeters to Part Units (1 or 0.04)</summary>
		internal static double MillimetersToPartUnits
		{
			get
			{
				return 1.0 / UnitConversion.PartUnitsToMillimeters;
			}
		}

		/// <summary>Multiply by this number to convert Part Units to Inches (1 or 0.04) </summary>
		internal static double PartUnitsToInches
		{
			get
			{
				return Globals.InchesPerUnit;
			}
		}

		/// <summary>Multiply by this number to convert Inches to Part Units (either 1 or 25.4)</summary>
		internal static double InchesToPartUnits
		{
			get
			{
				return 1.0 / UnitConversion.PartUnitsToInches;
			}
		}

		/// <summary>Multiply by this number to convert Part Units to Meters, to go to Parasolid (0.001 or 0.0254)</summary>
		internal static double PartUnitsToMeters
		{
			get
			{
				return 0.001 * UnitConversion.PartUnitsToMillimeters;
			}
		}

		/// <summary>Multiply by this number to convert Meters to Part Units, when coming from Parasolid (1000 or 40)</summary>
		internal static double MetersToPartUnits
		{
			get
			{
				return 1000.0 * UnitConversion.MillimetersToPartUnits;
			}
		}

		/// <summary>Multiply by this number to convert Part Units to Points (for font sizes)</summary>
		internal static double PartUnitsToPoints
		{
			get
			{
				return UnitConversion.PartUnitsToInches * 72.0;
			}
		}

		/// <summary>Multiply by this number to convert Points to Part Units (for font sizes)</summary>
		internal static double PointsToPartUnits
		{
			get
			{
				return 0.013888888888888888 * UnitConversion.InchesToPartUnits;
			}
		}
	}
}
