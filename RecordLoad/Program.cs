using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Net.Http;
using System.Net.Http.Headers;

namespace RecordLoad
{
    class Program
    {
        static void Main(string[] args)
        {
            //Record 100000 requests for two servers simultaneously. 
            Task t = RunAsync("AD01");
            Task t1 = RunAsync("AD02");
            Console.Read();
        }

        static async Task RunAsync(string serverName)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost/ServerTrack/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("api/ServerStatus");
                Random r = new Random();

                for (int i = 0; i < 100000; i++)
                {
                    //Create random number for RAM Load
                    int ramInt = r.Next(0, 100);
                    var ramDouble = Convert.ToDouble(ramInt);

                    //Create random number for CPT Load
                    int cpuInt = r.Next(0, 100);
                    var cpuDouble = Convert.ToDouble(cpuInt);

                    var status = new ServerStatus() { ServerName = serverName, RAMLoad = ramDouble, CPULoad = cpuDouble };
                    response = await client.PostAsJsonAsync("api/ServerStatus", status);
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("{0}-{1}", serverName, i);
                    }
                }
            }
        }       
    }
}
;