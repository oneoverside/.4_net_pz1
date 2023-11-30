using System.Collections.Concurrent;
using System.Numerics;
using PZ1.Extensions;

namespace PZ1.Services.Resolvers;

/// <summary>
/// Provides functionality to resolve polynomial equations to find their roots.
/// </summary>
public abstract class MyPolynomResolverParallelThread
{
    #region Main Resolver Method

    /// <summary>
    /// Attempts to find the roots of the polynomial described by the given coefficients.
    /// </summary>
    /// <param name="coefficients">The coefficients of the polynomial, where coefficients[i] is x^i.</param>
    /// <param name="maxDegreeOfParallelism">Number of threads available for this problem.</param>
    /// <returns>A collection of complex numbers representing the roots of the polynomial.</returns>
    public static string FindRoots(IReadOnlyList<double> coefficients, int maxDegreeOfParallelism = 4)
    {
        var degree = coefficients.Count - 1;

        var roots = InitializeRoots(degree);

        const int maxIterations = 10;
        for (var iteration = 0; iteration < maxIterations; iteration++)
        {
            var newRoots = ComputeNextRootsParallel(coefficients, roots, degree, maxDegreeOfParallelism);

            if (HasConvergedParallel(roots, newRoots, maxDegreeOfParallelism))
            {
                newRoots = newRoots.Select(x => new Complex(x.Real * -1, x.Imaginary)).ToList();
                return newRoots.ToMathString();
            }

            roots = newRoots;
        }

        return $"Method did not converge after {maxIterations} iterations.";
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
        var roots = new List<Complex>();
        var angleStep = 2.0 * Math.PI / degree;
        for (var i = 0; i < degree; i++)
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
    /// <param name="maxDegreeOfParallelism">Number of threads available for this problem.</param>
    /// <returns>A new list of complex numbers representing the roots.</returns>
    private static List<Complex> ComputeNextRootsParallel(IReadOnlyList<double> coefficients, IReadOnlyList<Complex> roots, int degree, int maxDegreeOfParallelism)
    {
        var newRoots = new ConcurrentBag<Complex>();

        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism };

        Parallel.For(0, degree, parallelOptions, i =>
        {
            var value = EvaluatePolynomialParallel(coefficients, roots[i], maxDegreeOfParallelism);
            var newValue = roots[i] - value / ComputeProductParallel(roots, i, maxDegreeOfParallelism);
            newRoots.Add(newValue);
        });

        return newRoots.ToList();
    }
    
    /// <summary>
    /// Checks if the iteration has converged to a solution.
    /// </summary>
    /// <param name="roots">The previous set of roots.</param>
    /// <param name="newRoots">The new set of roots.</param>
    /// <param name="maxDegreeOfParallelism">Number of threads available for this problem.</param>
    /// <returns><c>true</c> if converged, <c>false</c> otherwise.</returns>
    private static bool HasConvergedParallel(IReadOnlyList<Complex> roots, IReadOnlyList<Complex> newRoots, int maxDegreeOfParallelism)
    {
        int totalRoots = roots.Count;
        int rootsPerThread = (int)Math.Ceiling(totalRoots / (double)maxDegreeOfParallelism);
        var threads = new Thread[maxDegreeOfParallelism];
        var converged = new bool[maxDegreeOfParallelism];
        for (int i = 0; i < converged.Length; i++) converged[i] = true;

        for (int threadIndex = 0; threadIndex < maxDegreeOfParallelism; threadIndex++)
        {
            int start = threadIndex * rootsPerThread;
            int end = Math.Min(start + rootsPerThread, totalRoots);
            int localIndex = threadIndex; 
            threads[threadIndex] = new Thread(() =>
            {
                for (int i = start; i < end; i++)
                {
                    if (Complex.Abs(roots[i] - newRoots[i]) > 1e-10)
                    {
                        converged[localIndex] = false;
                        break;
                    }
                }
            });

            threads[threadIndex].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        return converged.All(c => c);
    }

    /// <summary>
    /// Computes the product used in the iteration formula, skipping a given index.
    /// </summary>
    /// <param name="roots">The current set of roots.</param>
    /// <param name="skipIndex">The index to be skipped in the product computation.</param>
    /// <param name="maxDegreeOfParallelism">Number of threads available for this problem.</param>
    /// <returns>The computed complex product.</returns>
    private static Complex ComputeProductParallel(IReadOnlyList<Complex> roots, int skipIndex, int maxDegreeOfParallelism)
    {
        int totalRoots = roots.Count;
        int rootsPerThread = (int)Math.Ceiling(totalRoots / (double)maxDegreeOfParallelism);
        var results = new Complex[maxDegreeOfParallelism];
        var threads = new Thread[maxDegreeOfParallelism];

        for (int threadIndex = 0; threadIndex < maxDegreeOfParallelism; threadIndex++)
        {
            int start = threadIndex * rootsPerThread;
            int end = Math.Min(start + rootsPerThread, totalRoots);
            int localIndex = threadIndex; // Локальная переменная для безопасного использования в лямбда-выражении
            threads[threadIndex] = new Thread(() =>
            {
                Complex localProduct = new Complex(1, 0);
                for (int i = start; i < end; i++)
                {
                    if (i != skipIndex)
                    {
                        localProduct *= roots[skipIndex] - roots[i];
                    }
                }
                results[localIndex] = localProduct; // Использование локальной переменной
            });

            threads[threadIndex].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        var totalProduct = new Complex(1, 0);

        return results.Aggregate(totalProduct, (current, product) => current * product);
    }

    /// <summary>
    /// Evaluates the polynomial at the given point.
    /// </summary>
    /// <param name="coefficients">The coefficients of the polynomial.</param>
    /// <param name="x">The complex point at which the polynomial is to be evaluated.</param>
    /// <param name="maxDegreeOfParallelism">Number of threads available for this problem.</param>
    /// <returns>The value of the polynomial at the given point.</returns>
    private static Complex EvaluatePolynomialParallel(IReadOnlyList<double> coefficients, Complex x, int maxDegreeOfParallelism)
    {
        var totalCoefficients = coefficients.Count;
        var coefficientsPerThread = (int)Math.Ceiling(totalCoefficients / (double)maxDegreeOfParallelism);
        var results = new Complex[maxDegreeOfParallelism];
        var threads = new Thread[maxDegreeOfParallelism];

        for (var threadIndex = 0; threadIndex < maxDegreeOfParallelism; threadIndex++)
        {
            var start = threadIndex * coefficientsPerThread;
            var end = Math.Min(start + coefficientsPerThread, totalCoefficients);
            var localIndex = threadIndex; 
            threads[threadIndex] = new Thread(() =>
            {
                var localSum = new Complex(0, 0);
                for (var i = start; i < end; i++)
                {
                    localSum += coefficients[i] * Complex.Pow(x, i);
                }
                results[localIndex] = localSum;
            });

            threads[threadIndex].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        var totalSum = new Complex(0, 0);

        return results.Aggregate(totalSum, (current, result) => current + result);
    }

    #endregion // Private Supported Methods
}