namespace Cinem_NBomber
{
    using NBomber.Contracts.Stats;
    using NBomber.CSharp;
    using System.Net.Http;

    namespace ConsoleApp2
    {
        public class DataEndpointTest
        {
            public static void RunTest()
            {
                string url = "https://localhost:7219/BuyTicket/ChooseSeat/15";
                var httpClient = new HttpClient();

                var scenario = Scenario.Create("TestEndpoint", async context =>
                {
                    var response = await httpClient.GetAsync(url);
                    return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
                })
                .WithoutWarmUp()
                .WithLoadSimulations(
                    Simulation.Inject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(2))
                );

                NBomberRunner
                    .RegisterScenarios(scenario)
                    .WithReportFileName("nbomber_test_report")
                    .WithReportFolder("reports")
                    .WithReportFormats(ReportFormat.Html)
                    .Run();
            }
        }

        internal class Program
        {
            static void Main(string[] args)
            {
                DataEndpointTest.RunTest();
            }
        }
    }

}
