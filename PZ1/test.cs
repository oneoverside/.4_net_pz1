using System.Numerics;

public class X
{
    public static List<Complex> FindRoots(double[] coefficients)
    {
        int degree = coefficients.Length - 1;

        List<Complex> roots = new List<Complex>();

        Complex initial = new Complex(0.4, 0.9); 
        for (int i = 0; i < degree; i++)
        {
            roots.Add(Complex.Pow(initial, i));
        }

        // Итерации Durand-Kerner
        const int MaxIterations = 1000;
        for (int iteration = 0; iteration < MaxIterations; iteration++)
        {
            List<Complex> newRoots = new List<Complex>();
            for (int i = 0; i < degree; i++)
            {
                Complex value = EvaluatePolynomial(coefficients, roots[i]);
                Complex newValue = roots[i] - value / ComputeProduct(roots, i);
                newRoots.Add(newValue);
            }

            bool converged = true;
            for (int i = 0; i < degree; i++)
            {
                if (Complex.Abs(roots[i] - newRoots[i]) > 1e-10)
                {
                    converged = false;
                    break;
                }
            }

            roots = newRoots;

            if (converged)
            {
                break;
            }
        }
        
        return roots.Select(x =>
        {
            var real = Math.Round(1 / x.Real, 3);
            var complex = Math.Round(x.Imaginary, 3);
            return new Complex(real, complex);
        }).ToList();
    }

    private static Complex ComputeProduct(List<Complex> roots, int skipIndex)
    {
        Complex result = 1;
        for (int i = 0; i < roots.Count; i++)
        {
            if (i == skipIndex)
            {
                continue;
            }

            result *= (roots[skipIndex] - roots[i]);
        }
        return result;
    }

    private static Complex EvaluatePolynomial(double[] coefficients, Complex x)
    {
        Complex result = 0;
        for (int i = 0; i < coefficients.Length; i++)
        {
            result += coefficients[i] * Complex.Pow(x, i);
        }
        return result;
    }
}