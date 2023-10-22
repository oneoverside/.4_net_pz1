static void FindRoots(double a, double b, double c)
{
    double discriminant = b * b - 4 * a * c;

    if (discriminant > 0)
    {
        double root1 = (-b + Math.Sqrt(discriminant)) / (2 * a);
        double root2 = (-b - Math.Sqrt(discriminant)) / (2 * a);

        Console.WriteLine("Two distinct real roots: {0} and {1}", root1, root2);
    }
    else if (discriminant == 0)
    {
        double root = -b / (2 * a);

        Console.WriteLine("One real root: {0}", root);
    }
    else
    {
        double realPart = -b / (2 * a);
        double imaginaryPart = Math.Sqrt(-discriminant) / (2 * a);

        Console.WriteLine("Two complex roots: {0} + {1}i and {0} - {1}i", realPart, imaginaryPart);
    }
}

// X.FindRoots(2, -4, 5, 2);
Kata.Solution("abc", "bc");

public class Kata
{
    public static bool Solution(string str, string ending) => str.EndsWith(ending);
}