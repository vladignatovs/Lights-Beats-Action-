using System;
using System.Globalization;

public static class SafeFloat {
    /// <summary>
    /// Tries to parse a float from a string, replacing commas with dots and using invariant culture.
    /// </summary>
    /// <param name="value">The string to parse.</param>
    /// <param name="result">The parsed float value, or 0 if parsing fails.</param>
    /// <returns>True if parsing was successful, false otherwise.</returns>
    public static bool FloatTryParse(this string value, out float result) {
        return float.TryParse(
            value.Replace(',', '.'), 
            NumberStyles.Float, 
            CultureInfo.InvariantCulture, 
            out result
        );
    }

    /// <summary>
    /// Parses a float from a string, throwing a FormatException if parsing fails.
    /// </summary>
    /// <param name="value">The string to parse.</param>
    /// <returns>The parsed float value, or the default value if parsing fails.</returns>
    /// <exception cref="FormatException" />
    public static float FloatParse(this string value) {
        if (FloatTryParse(value, out float result)) return result;
        throw new FormatException($"Cannot parse '{value}' as float.");
    }
}