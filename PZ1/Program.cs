using BenchmarkDotNet.Running;
using PZ1.Services.Benchmark;
using PZ1.Services.Resolvers;

Console.WriteLine(MsPolynomialResolver.FindRoots(new double[]{-1, 1, 1}));
Console.WriteLine(MyPolynomialResolver.FindRoots(new double[]{-1, 1, 1}));
Console.WriteLine(MyPolynomialResolverParallelFor.FindRoots(new double[]{-1, 1, 1}), 4);
Console.WriteLine(MyPolynomResolverParallelThread.FindRoots(new double[]{-1, 1, 1}), 4);

BenchmarkRunner.Run<PolynomialBenchmark>();