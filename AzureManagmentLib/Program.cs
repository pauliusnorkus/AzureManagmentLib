using System;

namespace AzureManagmentLib
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var managment = new AzureManagment();
            var cloudServices = managment.GetCloudDeployments().Result;

            foreach (var cs in cloudServices)
            {
                foreach (var d in cs.Deployments)
                {
                    Console.WriteLine(string.Format("CloudService={0}; Slot={1}; DeploymentName={2}", 
                        cs.HostedService.ServiceName, d.Key, d.Value != null ? d.Value.Name : "NULL"));
                }
            }
        }
    }
}
