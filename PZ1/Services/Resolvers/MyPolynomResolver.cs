using System.Numerics;
using PZ1.Extensions;

namespace PZ1.Services.Resolvers;

/// <summary>
/// Provides functionality to resolve polynomial equations to find their roots.
/// </summary>
public abstract class MyPolynomialResolver
{
    #region Main Resolver Method

    /// <summary>
    /// Attempts to find the roots of the polynomial described by the given coefficients.
    /// </summary>
    /// <param name="coefficients">The coefficients of the polynomial, where coefficients[i] is x^i.</param>
    /// <returns>A collection of complex numbers representing the roots of the polynomial.</returns>
    public static string FindRoots(IReadOnlyList<double> coefficients)
    {
        int degree = coefficients.Count - 1;

        List<Complex> roots = InitializeRoots(degree);

        const int MaxIterations = 10;
        for (int iteration = 0; iteration < MaxIterations; iteration++)
        {
            List<Complex> newRoots = ComputeNextRoots(coefficients, roots, degree);

            if (HasConverged(roots, newRoots))
            {
                newRoots = newRoots.Select(x => new Complex(x.Real * -1, x.Imaginary)).ToList();
                return newRoots.ToMathString();
            }

            roots = newRoots;
        }

        return $"Method did not converge after {MaxIterations} iterations.";
    }

    #endregion // Main Resolver Method

    #region Private Supported Methods

    /// <summary>
    /// Initializes a list of complex roots based on the given degree using polar coordinates.
    /// </summary>
    /// <param name="degree">The degree of the polynomial.</param>
    /// <returns>A list of complex numbers.</returns>
    private static List<Complex> InitializeRoots(int degree)
    {
        List<Complex> roots = new List<Complex>();
        double angleStep = 2.0 * Math.PI / degree;
        for (int i = 0; i < degree; i++)
        {
            roots.Add(Complex.FromPolarCoordinates(1, i * angleStep));
        }
        return roots;
    }

    /// <summary>
    /// Computes the next set of polynomial roots.
    /// </summary>
    /// <param name="coefficients">The coefficients of the polynomial.</param>
    /// <param name="roots">The current set of roots.</param>
    /// <param name="degree">The degree of the polynomial.</param>
    /// <returns>A new list of complex numbers representing the roots.</returns>
    private static List<Complex> ComputeNextRoots(IReadOnlyList<double> coefficients, IReadOnlyList<Complex> roots, int degree)
    {
        List<Complex> newRoots = new List<Complex>();
        for (int i = 0; i < degree; i++)
        {
            Complex value = EvaluatePolynomial(coefficients, roots[i]);
            Complex newValue = roots[i] - value / ComputeProduct(roots, i);
            newRoots.Add(newValue);
        }
        return newRoots;
    }
    
    /// <summary>
    /// Checks if the iteration has converged to a solution.
    /// </summary>
    /// <param name="roots">The previous set of roots.</param>
    /// <param name="newRoots">The new set of roots.</param>
    /// <returns><c>true</c> if converged, <c>false</c> otherwise.</returns>
    private static bool HasConverged(IReadOnlyList<Complex> roots, IReadOnlyList<Complex> newRoots)
    {
        for (int i = 0; i < roots.Count; i++)
        {
            if (Complex.Abs(roots[i] - newRoots[i]) > 1e-10)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Computes the product used in the iteration formula, skipping a given index.
    /// </summary>
    /// <param name="roots">The current set of roots.</param>
    /// <param name="skipIndex">The index to be skipped in the product computation.</param>
    /// <returns>The computed complex product.</returns>
    private static Complex ComputeProduct(IReadOnlyList<Complex> roots, int skipIndex)
    {
        Complex result = 1;
        for (int i = 0; i < roots.Count; i++)
        {
            if (i == skipIndex)
            {
                continue;
            }

            result *= roots[skipIndex] - roots[i];
        }
        return result;
    }

    /// <summary>
    /// Evaluates the polynomial at the given point.
    /// </summary>
    /// <param name="coefficients">The coefficients of the polynomial.</param>
    /// <param name="x">The complex point at which the polynomial is to be evaluated.</param>
    /// <returns>The value of the polynomial at the given point.</returns>
    private static Complex EvaluatePolynomial(IReadOnlyList<double> coefficients, Complex x)
    {
        Complex result = 0;
        for (int i = 0; i < coefficients.Count; i++)
        {
            result += coefficients[i] * Complex.Pow(x, i);
        }
        return result;
    }

    #endregion // Private Supported Methods
}