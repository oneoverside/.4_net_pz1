using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using PZ1.Services.Resolvers;

namespace PZ1.Services.Benchmark;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80, launchCount: 1, warmupCount: 1, iterationCount: 3)]
public class PolynomialBenchmark
{
    private readonly double[] _coefficients = {-1, 1, 1};

    [Benchmark] 
    public void MsPolynomialResolverBenchmark()
    {
        MsPolynomialResolver.FindRoots(this._coefficients);
    }

    [Benchmark]
    public void MyPolynomialResolverBenchmark()
    {
        MyPolynomialResolver.FindRoots(this._coefficients);
    }

    [Benchmark]
    public void MyPolynomialResolverParallelBenchmark()
    { 
        MyPolynomialResolverParallelFor.FindRoots(this._coefficients, 100);
    }
    
    [Benchmark]
    public void MyPolynomialResolverParallelThreadBenchmark()
    { 
        MyPolynomResolverParallelThread.FindRoots(this._coefficients, 100);
    }
}
