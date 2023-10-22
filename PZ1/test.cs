

public class X
{
    public static string FindRoots(double[] coefficients)
    {
        var degree = coefficients.Length;

        // Check if the equation is linear
        if (degree == 1)
        {
            return Find1DegreeRoots(coefficients);
        }

        // Check if the equation is quadratic
        if (degree == 2)
        {
            return Find2DegreeRoots(coefficients);
        }
        
        // Solve higher degree equations using Newton-Raphson method
        else
        {
            var roots = new List<double>();

            // Initial guess for roots
            var initialGuess = GetInitialGuess(coefficients);

            // Iterate for each root
            for (var i = 0; i < degree; i++)
            {
                var guess = initialGuess[i];
                var tolerance = 0.00001; // Adjust the tolerance as per your requirement
                var root = FindRootUsingNewtonRaphson(coefficients, guess, tolerance);

                if (!double.IsNaN(root))
                {
                    roots.Add(root);
                }
            }

            Console.WriteLine($"Roots: {string.Join(", " ,roots)};");
        }
    }

    #region 1 Degree Solver

    private static string Find1DegreeRoots(IReadOnlyList<double> coefficients)
    {
        var root = -coefficients[0] / coefficients[1];
        return $"One distinct real root: {root}";
    }

    #endregion // 1 Degree Solver

    #region 2 Degree Solver

    private static string Find2DegreeRoots(IReadOnlyList<double> coefficients)
    {
        var c = coefficients[0];
        var b = coefficients[1];
        var a = coefficients[2];
        double discriminant = b * b - 4 * a * c;

        if (discriminant > 0)
        {
            double root1 = (-b + Math.Sqrt(discriminant)) / (2 * a);
            double root2 = (-b - Math.Sqrt(discriminant)) / (2 * a);
            return $"Two distinct real roots: {root1} and {root2}";
        }
        else if (discriminant == 0)
        {
            double root = -b / (2 * a);
            return $"One real root: {root}";
        }
        else
        {
            double realPart = -b / (2 * a);
            double imaginaryPart = Math.Sqrt(-discriminant) / (2 * a);
            return $"Two complex roots: {realPart} + {imaginaryPart}i and {realPart} - {imaginaryPart}i";
        }
    }

    #endregion // 2 Degree Solver

    #region N Degree Solver

    // Helper method to get initial guess for roots
    private static double[] GetInitialGuess(double[] coefficients)
    {
        var degree = coefficients.Length - 1;
        var initialGuess = new double[degree];

        // Set initial guess to zero
        for (var i = 0; i < degree; i++)
        {
            initialGuess[i] = 0;
        }

        return initialGuess;
    }

    // Helper method to find root using Newton-Raphson method
    private static double FindRootUsingNewtonRaphson(double[] coefficients, double guess, double tolerance)
    {
        var derivativeTolerance = 0.00001; // Adjust the derivative tolerance as per your requirement

        var fx = EvaluateEquation(coefficients, guess);
        var dfx = EvaluateDerivative(coefficients, guess);
        var maxIterations = 100; // Adjust the maximum number of iterations as per your requirement

        var counter = 0;
        while (Math.Abs(fx) > tolerance && Math.Abs(dfx) > derivativeTolerance && counter < maxIterations)
        {
            guess = guess - fx / dfx;
            fx = EvaluateEquation(coefficients, guess);
            dfx = EvaluateDerivative(coefficients, guess);

            counter++;
        }

        // Check if root was found within the specified tolerance
        if (Math.Abs(fx) <= tolerance)
        {
            return guess;
        }
        else
        {
            return double.NaN; // Return NaN if root was not found
        }
    }

    // Helper method to evaluate the equation for a given value of x
    private static double EvaluateEquation(double[] coefficients, double x)
    {
        var degree = coefficients.Length - 1;
        double result = 0;

        for (var i = 0; i <= degree; i++)
        {
            result += coefficients[i] * Math.Pow(x, degree - i);
        }

        return result;
    }

    // Helper method to evaluate the derivative of the equation for a given value of x
    private static double EvaluateDerivative(double[] coefficients, double x)
    {
        var degree = coefficients.Length - 1;
        double result = 0;

        for (var i = 0; i < degree; i++)
        {
            result += (degree - i) * coefficients[i] * Math.Pow(x, degree - i - 1);
        }

        return result;
    }

    #endregion // N Degree Solver
}