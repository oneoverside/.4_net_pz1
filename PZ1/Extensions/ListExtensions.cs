using System.Numerics;

namespace PZ1.Extensions;

public static class ListExtensions
{
    public static string ToMathString(this IEnumerable<Complex> roots) =>
        $"Roots: {
            string.Join(", ", roots.Where(x => Math.Abs(x.Imaginary) < 1e-3)
                .OrderBy(x => x.Real)
                .Select(x => $"{Math.Round(x.Real, 3)}"))
        }";
}