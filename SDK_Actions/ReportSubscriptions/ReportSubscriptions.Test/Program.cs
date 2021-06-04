using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReportSubscriptions.Logic;
using ReportSubscriptions.Model;
namespace ReportSubscriptions.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Configuration));
            string configContent;
            using (System.IO.StreamReader file = new System.IO.StreamReader($@"{System.IO.Path.GetTempPath()}\report.subscription\configuration"))
            {
                configContent = file.ReadToEnd();
            }
            Configuration config = null;
            using (var contentReader = new System.IO.StringReader(configContent))
            {
                config = serializer.Deserialize(contentReader) as Configuration;                
            }
            config.Page = 1;
            config.Size = 1000;

            var clientProvider = new AutenticatedClientProvider(config);
            var client = clientProvider.GetClientAsync().Result;
            ApiManager apiManager = new ApiManager(client, config);
            var reportData = apiManager.GetReportDataAsync().Result;
            string tableHtml = (new TableGenerator(reportData, config)).Generate();
        }
    }
}
