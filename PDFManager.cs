using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace BackendPdfGenerator
{
    public enum PDFAlignment
    {
        General = Element.ALIGN_BASELINE, Left = Element.ALIGN_LEFT, Center = Element.ALIGN_CENTER, Right = Element.ALIGN_RIGHT, Justified = Element.ALIGN_JUSTIFIED, Justified_All = Element.ALIGN_JUSTIFIED_ALL
    }

    public class ChartModel
    {
        public string X { get; set; }
        public object Y { get; set; }
    }

    public static class PDFManager
    {
        public static string FontName { get; set; } = "Calibri";
        public static float FontSize { get; set; } = 10f;
        public static string FontColor { get; set; } = "#000000";
        public static string BgColor { get; set; } = "#ffffff";
        public static float Padding { get; set; } = 4f;


        public static PdfPTable GetTextLineElement(string txt, float? fontSize, string fontColor = null, string bgColor = null, string fontName = null, float tableWidth = 100, bool isBold = false, bool isUnderlined = false, PDFAlignment alignment = PDFAlignment.Left, bool borderless = true, float[] ltrBorder = null, string borderColor = null, float[] ltrPadding = null)
        {
            try
            {
                var pdfTable = new PdfPTable(1) { WidthPercentage = tableWidth };
                var font = FontFactory.GetFont(fontName ?? FontName, fontSize ?? FontSize, Color(fontColor ?? FontColor));
                if (isBold) font.SetStyle(iTextSharp.text.Font.BOLD);
                if (isUnderlined) font.SetStyle(iTextSharp.text.Font.UNDERLINE);
                if (isUnderlined && isBold) font.SetStyle(iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE);
                PdfPCell cell = new PdfPCell(new Phrase(txt ?? " ", font));

                cell.UseAscender = true;
                cell.HorizontalAlignment = (int)alignment;
                cell.Padding = Padding;
                cell.BackgroundColor = Color(bgColor ?? BgColor);

                if (ltrPadding != null)
                {
                    cell.PaddingLeft = ltrPadding[0];
                    cell.PaddingTop = ltrPadding[1];
                    cell.PaddingRight = ltrPadding[2];
                    cell.PaddingBottom = ltrPadding[3];
                }

                if (borderless) cell.Border = 0;

                if (ltrBorder != null)
                {
                    cell.BorderWidthLeft = ltrBorder[0];
                    cell.BorderWidthTop = ltrBorder[1];
                    cell.BorderWidthRight = ltrBorder[2];
                    cell.BorderWidthBottom = ltrBorder[3];
                }

                if (!string.IsNullOrEmpty(borderColor)) cell.BorderColor = Color(borderColor);

                cell.SetLeading(0, 2);
                pdfTable.AddCell(cell);
                return pdfTable;
            }
            catch (Exception ex)
            {
                throw new Exception("Error on GetTextLineElement: " + ex.Message);
            }

        }


        public static PdfPTable GetImageContent(byte[] imageBytes, PDFAlignment alignment = PDFAlignment.Left, bool borderLess = true, bool sideBorderLess = true, float[] leftToRightPadding = null, float tableWidth = 100)
        {
            try
            {
                var pdfTable = new PdfPTable(1) { WidthPercentage = tableWidth };
                var img = iTextSharp.text.Image.GetInstance(ByteArrayToBitmap(imageBytes), BaseColor.WHITE);
                PdfPCell cell = new PdfPCell();
                cell.Image = img;

                cell.HorizontalAlignment = (int)alignment;
                if (leftToRightPadding != null)
                {
                    cell.PaddingLeft = leftToRightPadding[0];
                    cell.PaddingTop = leftToRightPadding[1];
                    cell.PaddingRight = leftToRightPadding[2];
                    cell.PaddingBottom = leftToRightPadding[3];
                }
                else
                    cell.Padding = 4;

                if (borderLess)
                {
                    cell.Border = 0;
                }
                if (sideBorderLess)
                {
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthRight = 0;
                }
                pdfTable.AddCell(cell);
                return pdfTable;
            }
            catch (Exception ex)
            {
                throw new Exception("Error on GetTableImageCell: " + ex.Message);
            }

        }

        public static Bitmap GraphImage(List<ChartModel> items, string title, int width, int height, int interval, string xTitle = null, string yTitle = null)
        {
            var chart = new Chart
            {
                Width = width,
                Height = height,
                AntiAliasing = AntiAliasingStyles.All,
                TextAntiAliasingQuality = TextAntiAliasingQuality.High,
                Palette = ChartColorPalette.None,
                PaletteCustomColors = new System.Drawing.Color[] { System.Drawing.Color.CornflowerBlue }
            };

            chart.Titles.Add(title);
            chart.Titles[0].Font = new System.Drawing.Font(FontName, 9f, FontStyle.Bold);

            chart.ChartAreas.Add("");
            chart.ChartAreas[0].AxisX.Title = xTitle;
            chart.ChartAreas[0].AxisY.Title = yTitle;
            chart.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font(FontName, 8f);
            chart.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font(FontName, 8f);
            chart.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font(FontName, 10f);
            chart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            chart.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.Black;
            chart.ChartAreas[0].AxisY.Interval = interval;
            chart.ChartAreas[0].BackColor = System.Drawing.Color.White;
            chart.Series.Add("");
            chart.Series[0].ChartType = SeriesChartType.Column;
            chart.Series[0].CustomProperties = "PointWidth=0.5";


            foreach (var item in items)
            {
                chart.Series[0].Points.AddXY(item.X, item.Y);
            }

            var chartimage = new MemoryStream();
            chart.SaveImage(chartimage, ChartImageFormat.Png);
            return new Bitmap(chartimage);
        }

        public static BaseColor Color(string hex)
        {
            return new BaseColor(ColorTranslator.FromHtml(hex));
        }

        private static Bitmap ByteArrayToBitmap(byte[] byteBuffer)
        {
            Bitmap bmpReturn = null;
            MemoryStream memoryStream = null;

            try
            {
                memoryStream = new MemoryStream(byteBuffer);


                memoryStream.Position = 0;


                bmpReturn = (Bitmap)Bitmap.FromStream(memoryStream);


                memoryStream.Close();

                return bmpReturn;
            }
            finally
            {
                memoryStream?.Dispose();
            }
            
        }
    }
}
