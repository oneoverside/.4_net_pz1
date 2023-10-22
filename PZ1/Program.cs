using System.Numerics;
using MathNet.Numerics;

Console.WriteLine(FindRoots(new double[]{-1, 1, 1}).ToMathString());
Console.WriteLine(X.FindRoots(new double[]{-1, 1, 1}).ToMathString());

Kata.Solution("abc", "bc");

List<Complex> FindRoots(double[] coefficients)
{
    var polynomial = new Polynomial(coefficients.ToArray().Reverse()); 
    var roots = polynomial.Roots();

    return new List<Complex>(roots);
}

public class Kata
{
    public static bool Solution(string str, string ending) => str.EndsWith(ending);
}

public static class ListExtensions
{
    public static string ToMathString(this List<Complex> roots) =>
    $"Roots: {
        string.Join(", ", roots.Where(x => Math.Abs(x.Imaginary) < 1e-3)
            .OrderByDescending(x => Math.Abs(x.Imaginary))
            .Select(x => $"{Math.Round(x.Real, 3)}"))
    }";
}