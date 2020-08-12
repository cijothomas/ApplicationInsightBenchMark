using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
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
                this.basicConfig.TelemetryChannel = new EmptyChannel();
                this.basicClient = new TelemetryClient(this.basicConfig);
                this.configWithMetricExtractor = TelemetryConfiguration.CreateDefault();
                this.configWithMetricExtractor.TelemetryChannel = new EmptyChannel();
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
                client.TrackRequest("MyReq", DateTimeOffset.Now, TimeSpan.FromMilliseconds(34),
                    resCodes[rand.Next(0, 5)], (rand.Next(1, 10) < 4) ? true : false);
            }
        }

        internal class MyRoleInstanceInitialier : ITelemetryInitializer
        {
            public void Initialize(ITelemetry telemetry)
            {
                telemetry.Context.Cloud.RoleInstance = "MyRoleInstance";
            }
        }

        internal class MyRoleNameInitialier : ITelemetryInitializer
        {
            string[] roles = new string[] { "RoleNameA", "RoleNameB" };
            Random rand = new Random();

            public void Initialize(ITelemetry telemetry)
            {
                telemetry.Context.Cloud.RoleName = roles[rand.Next(0, 2)];
            }
        }

        internal class EmptyChannel : ITelemetryChannel
        {
            public bool? DeveloperMode { get; set; }
            public string EndpointAddress { get; set; }

            public void Dispose()
            {
            }

            public void Flush()
            {
            }

            public void Send(ITelemetry item)
            {
            }
        }
    }
}

