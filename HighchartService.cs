using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendPdfGenerator
{
    public class HighchartService
    {
        private readonly string _highchartsServer = "http://export.highcharts.com/";

        public async Task<byte[]> GetChartImageAsBytesAsync(object options, int? width = 800)
        {
            try
            {
                var settings = new HighchartsSetting
                {
                    ExportImageType = "jpg",
                    ScaleFactor = 2,
                    ImageWidth = width.Value,
                    ServerAddress = _highchartsServer
                };

                var client = new HighchartsClient(settings);

                return await client.GetChartImageFromOptionsAsync(JsonConvert.SerializeObject(options));
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

        public async Task<string> GetChartImageLinkAsync(object options, int? width = null)
        {
            try
            {
                var settings = new HighchartsSetting
                {
                    ExportImageType = "jpg",
                    ScaleFactor = 2,
                    ImageWidth = width.Value,
                    ServerAddress = _highchartsServer
                };

                var client = new HighchartsClient(settings);

                return await client.GetChartImageLinkFromOptionsAsync(JsonConvert.SerializeObject(options));
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }
    }
}
