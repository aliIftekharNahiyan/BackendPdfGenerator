using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;

namespace BackendPdfGenerator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var document = new Document(PageSize.A4, 36, 36, 36, 36);
            //document.SetPageSize(PageSize.A4);
            var reportStream = new MemoryStream();

            try
            {
                PdfWriter writer = PdfWriter.GetInstance(document, reportStream);
                //writer.PageEvent = new PDFWriterEvents(true, true);
                writer.CloseStream = false;

                // Openning the Document
                document.Open();

                document.Add(PDFManager.GetTextLineElement("GESP MONTHLY REPORT (February 2021) FOR\nHEALTH SCIENCES AUTHORITY,\n11 OUTRAM ROAD,\nSINGAPORE 169078", 16f, ltrPadding : new[] { PDFManager.Padding, 100f, PDFManager.Padding, PDFManager.Padding }, alignment : PDFAlignment.Center, isBold : true));
                //document.Add(PDFManager.GetTextLineElement("", 14f, alignment= PDFAlignment.Center, isBold= true));

                document.NewPage();

                List<ResponseObjectModel> jsonData = null;
                string outputFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"data.json");
                if (File.Exists(outputFile))
                {
                    using (StreamReader r = new StreamReader(outputFile))
                    {
                        string json = r.ReadToEnd();
                        jsonData = JsonConvert.DeserializeObject<List<ResponseObjectModel>>(json);
                    }
                }

                await GenerateCoolingLoadChart(jsonData, document);

                document.Close();

                byte[] bytes = reportStream.ToArray();
                FileStream fs = new FileStream($@"E:\TestPDF\{DateTime.Now.ToString("yyMMddHHmmss")}.pdf", FileMode.OpenOrCreate);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();

                //return reportStream.ToArray();
            }
            catch (Exception ex)
            {

            }
        }

        public static async Task GenerateCoolingLoadChart(List<ResponseObjectModel> jsonData, Document document)
        {
            var service = new HighchartService();

            List<dynamic> systemCoolingloaddataSeries = new List<dynamic>();
            foreach (var value in jsonData)
            {
                if (value.DataPoint == "system coolingload" || value.DataPoint == "total coolingload")
                {
                    foreach (var value2 in value.data)
                    {
                        object[][] seriesData = new object[value2.Value.Count][];
                        for (int i = 0; i < value2.Value.Count; i++)
                        {
                            seriesData[i] = new object[] { value2.Timestamp[i], value2.Value[i] };
                        }
                        systemCoolingloaddataSeries.Add(new
                        {
                            date = value2.Date.ToUniversalTime(),
                            name = value2.Date.ToUniversalTime().ToString("dd/MM/yy") + " (" + ((DayOfWeek)value2.Key).ToString() + ") ",
                            data = seriesData,
                            type = "line",
                            color = GetWeekDaySeriesColor(value2.Key.ToString()),
                            yAxis = 0
                        });
                    }
                }
            }


            var options = new
            {
                chart = new
                {
                    type = "line",
                    zoomType = "x",
                    animation = new
                    {
                        duration = 1000
                    },
                    style = new
                    {
                        fontFamily = "Poppins",
                        fontWeight = "400",
                        fontSize = "13px",
                    },
                },
                credits = new
                {
                    enabled = false
                },
                boost = new
                {
                    enabled = false
                },
                lang = new
                {
                    noData = "No data to display in this date range"
                },
                noData = new
                {
                    style = new
                    {
                        fontWeight = "normal",
                        fontSize = "18px",
                        color = "#707070"
                    }
                },
                title = new
                {
                    text = "HSA Chiller Plant <br> System Cooling Load Profile",
                    x = -20, //center
                    style = new
                    {
                        fontSize = "13px",
                        fontWeight = "bold",
                    }
                },
                subtitle = new
                {
                    text = "Average= 0 RT",
                    align = "left",
                    y = -1,
                },
                xAxis = new
                {
                    type = "datetime",
                    dateTimeLabelFormats = new { day = "%d/%m/%y %H=%M" },
                    labels = new
                    {
                        enabled = true
                    },
                    title = new
                    {
                        text = "<b>Time(hh=mm)</b>"
                    }
                },
                yAxis = new[]{
                    new
                    {
                        min = 0,
                        max = 1000,
                        tickInterval = 200,
                        title = new
                        {
                            text = "<b>Cooling Load (RT)</b>",
                        }
                    }
                },
                legend = new
                {
                    layout = "horizontal",
                    align = "center",
                    verticalAlign = "bottom",
                    //borderWidth = 2,
                    enabled = true,
                    itemStyle = new
                    {
                        fontWeight = "normal"
                    },
                },
                plotOptions = new
                {
                    series = new
                    {
                        shadow = false,
                        connectNulls = false
                    }
                },
                series = systemCoolingloaddataSeries
            };

            var chartImage = await service.GetChartImageAsBytesAsync(options, 1200);

            File.WriteAllBytes($"__imageFromBytes_customSettings.jpg", chartImage);

            document.Add(PDFManager.GetImageContent(chartImage, PDFAlignment.Center, leftToRightPadding : new float[] { 4, 4, 4, 30 }));
        }

        private static string[] ColorArray = new[] { "#004d80", "#0075c2", "#1aaf5d", "#f2c500", "#8cbb2c", "#8e0000", "#0d948c" };

        private static string GetWeekDayName(int day)
        {
            switch (day)
            {
                case 1:
                    return "Sun";
                case 2:
                    return "Mon";
                case 3:
                    return "Tue";
                case 4:
                    return "Wed";
                case 5:
                    return "Thu";
                case 6:
                    return "Fri";
                case 7:
                    return "Sat";

            }
            return "";
        }

        private static string GetWeekDaySeriesColor(string day)
        {
            switch (day)
            {
                case "1":
                    return ColorArray[0];
                case "2":
                    return ColorArray[1];
                case "3":
                    return ColorArray[2];
                case "4":
                    return ColorArray[3];
                case "5":
                    return ColorArray[4];
                case "6":
                    return ColorArray[5];
                case "7":
                    return ColorArray[6];
                case "Sun":
                    return ColorArray[0];
                case "Mon":
                    return ColorArray[1];
                case "Tue":
                    return ColorArray[2];   //blue
                case "Wed":
                    return ColorArray[3];   //dark green
                case "Thu":
                    return ColorArray[4];   //dark pink
                case "Fri":
                    return ColorArray[5];   //dark gray
                case "Sat":
                    return ColorArray[6];
                case "Sun2":
                    return "#0099ff";
                case "Mon2":
                    return "#66c2ff";
                case "Tue2":
                    return "#21de76";
                case "Wed2":
                    return "#ffe366";
                case "Thu2":
                    return "#c2e283";
                case "Fri2":
                    return "#ff3333";
                case "Sat2":
                    return "#14ebdc";
                default:
                    return "#000000";
            }

        }
    }
}
