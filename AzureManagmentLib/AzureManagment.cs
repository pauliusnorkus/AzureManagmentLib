using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.Compute;
using Microsoft.WindowsAzure.Management.Compute.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace AzureManagmentLib
{
    public class AzureManagment
    {
        public const string CERT = "<ManagementCertificate Base64>";
        public const string SUBSCRIPTION_ID = "<Subscription Id>";

        private static CertificateCloudCredentials credentials;
        public static CertificateCloudCredentials Credentials
        {
            get
            {
                if (credentials == null)
                {
                    var cert = new X509Certificate2(Convert.FromBase64String(CERT));
                    credentials = new CertificateCloudCredentials(SUBSCRIPTION_ID, cert);
                }
                return credentials;
            }
        }


        public async Task<List<CloudService>> GetCloudDeployments()
        {
            List<CloudService> result = new List<CloudService>();
            using (var computeClient = new ComputeManagementClient(Credentials))
            {
                var cs = await computeClient.HostedServices.ListAsync();
                foreach (var c in cs)
                {
                    var service = new CloudService { HostedService = c, Deployments = new Dictionary<DeploymentSlot, DeploymentGetResponse>() };
                    foreach (var slot in Enum.GetValues(typeof(DeploymentSlot)).Cast<DeploymentSlot>())
                    {
                        var deployment = await GetDeploymnet(computeClient, c.ServiceName, slot);
                        service.Deployments.Add(slot, deployment);
                    }
                    result.Add(service);
                }
            }
            return result;
        }

        public static async Task<DeploymentGetResponse> GetDeploymnet(ComputeManagementClient computeClient, string serviceName, DeploymentSlot slot)
        {
            DeploymentGetResponse d = null;
            try
            {
                d = await computeClient.Deployments.GetBySlotAsync(serviceName, slot);
            }
            catch (Exception e) { }
            return d;
        }
    }

    public class CloudService
    {
        public HostedServiceListResponse.HostedService HostedService { get; set; }
        public Dictionary<DeploymentSlot, DeploymentGetResponse> Deployments { get; set; }
    }
}
