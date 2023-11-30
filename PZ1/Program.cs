using PZ1.Services.Resolvers;

Console.WriteLine(MsPolynomialResolver.FindRoots(new double[]{-1, 1, 1}));
Console.WriteLine(MyPolynomialResolver.FindRoots(new double[]{-1, 1, 1}));
Console.WriteLine(MyPolynomialResolverParallel.FindRoots(new double[]{-1, 1, 1}), 4);