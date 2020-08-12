using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation.Tracing;
using System;

namespace ApplicationInsightsBenchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var summary = BenchmarkRunner.Run<BasicBenchmark>();
        }

        [MemoryDiagnoser]
        public class BasicBenchmark
        {
            private readonly TelemetryConfiguration basicConfig;
            private readonly TelemetryClient basicClient;
            private readonly TelemetryConfiguration configWithMetricExtractor;
            private readonly TelemetryClient clientWithMetricExtractor;
            string[] types = new string[] { "type1", "type2", "type3", "type4", "type5" };
            string[] resCodes = new string[] { "200", "500", "503", "401", "429" };
            string[] targets = new string[] { "targetA", "targetB", "targetC", "targetD", "targetE", "targetF", "targetG", "targetH", "targetI" };
            Random rand = new Random();


            public BasicBenchmark()
            {
                this.basicConfig = TelemetryConfiguration.CreateDefault();
                this.basicClient = new TelemetryClient(this.basicConfig);
                this.configWithMetricExtractor = TelemetryConfiguration.CreateDefault();
                this.configWithMetricExtractor.DefaultTelemetrySink.TelemetryProcessorChainBuilder.Use((next) => new AutocollectedMetricsExtractor(next));
                this.configWithMetricExtractor.DefaultTelemetrySink.TelemetryProcessorChainBuilder.Build();
                this.clientWithMetricExtractor = new TelemetryClient(configWithMetricExtractor);
            }

            [Benchmark]
            public void Basic()
            {
                TrackRequest(this.basicClient);
            }

            [Benchmark]
            public void WithRequestExtractor()
            {
                TrackRequest(this.clientWithMetricExtractor);
            }

            private void TrackRequest(TelemetryClient client)
            {
                for (int i = 0; i < 10000; i++)
                {
                    client.TrackRequest("MyReq", DateTimeOffset.Now, TimeSpan.FromMilliseconds(34),
                        resCodes[rand.Next(0, 5)], (rand.Next(1, 10) < 4) ? true : false);
                }
            }
        }
    }
}

