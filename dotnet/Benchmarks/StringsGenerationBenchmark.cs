using BenchmarkDotNet.Attributes;

namespace Benchmarks;

[MemoryDiagnoser]
public class StringsGenerationBenchmark
{
    private const string InfoMessageTemplate =
        "Уведомление об изменении синхронизированной сущности {1} с внешним идентификатором {2} отправлено.{0}" +
        "Дата отправки: {3}.{0}" +
        "Идентификатор уведомления: {4}.";

    private (string _1, string _2, string _3, string _4)[] _data;
    
    [GlobalSetup]
    public void Setup()
    {
        _data = new (string _1, string _2, string _3, string _4)[]
        {
            new() { _1 = "name_1", _2 = "id_1", _3 = "2000-01-01", _4 = "id_11" },
            new() { _1 = "name_2", _2 = "id_2", _3 = "2000-01-02", _4 = "id_22" },
            new() { _1 = "name_3", _2 = "id_3", _3 = "2000-01-03", _4 = "id_33" }
        };
    }
    
    [Benchmark(Baseline = true)]
    public void FromConst() {
        foreach (var data in _data)
        {
            var res = string.Format(InfoMessageTemplate, Environment.NewLine, data._1, data._2, data._3, data._4);
        }
    }

    [Benchmark]
    public void FromVerbatim()
    {
        foreach (var data in _data)
        {
            var res = @$"Уведомление об изменении синхронизированной сущности {data._1} с внешним идентификатором {data._2} отправлено.
Дата отправки: {data._3}.
Идентификатор уведомления: {data._4}.";
        }

    }
}