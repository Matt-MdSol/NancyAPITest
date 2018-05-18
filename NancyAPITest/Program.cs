using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Conventions;
using Nancy.Hosting.Self;

namespace NancyAPITest
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new HostConfiguration()
            {
                UrlReservations = new UrlReservations() { CreateAutomatically = true }
            };


            using (var host = new NancyHost(configuration, new Uri("http://localhost:1234")))
            {
                host.Start();
                string s = Console.ReadLine();
            }
        }
    }


    public class CustomBoostrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureConventions(NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);

            conventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("assets", @"C:\Users\mtelles\source\repos\NancyAPITest\NancyAPITest\Content")
            );
        }
    }
}
