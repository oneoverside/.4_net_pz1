using MathNet.Numerics;
using PZ1.Extensions;

namespace PZ1.Services.Resolvers;

public abstract class MsPolynomialResolver
{
    public static string FindRoots(IEnumerable<double> coefficients)
    {
        var polynomial = new Polynomial(coefficients.ToArray().Reverse()); 
        var roots = polynomial.Roots();
        return roots.ToMathString();
    }
}