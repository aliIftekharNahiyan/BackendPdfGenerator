using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendPdfGenerator
{
    public class DateRange
    {
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
    }

    public class ChartResponse
    {
        public string Type { get; set; }
        public string MethodName { get; set; }
        public List<ResponseObjectModel> Data { get; set; }
    }

    public class ChartResponseVS
    {
        public string Type { get; set; }
        public string MethodName { get; set; }
        public List<ResponseObjectDataModel> Data { get; set; }
    }

    public class ResponseTableModel
    {
        public string DataPoint { get; set; }
        public double? Value { get; set; }
    }

    public class TableResponse
    {
        public string Type { get; set; }
        public string MethodName { get; set; }
        public List<ResponseTableModel> Data { get; set; }
    }

    public class DynamicResponse
    {
        public string Type { get; set; }
        public string MethodName { get; set; }
        public object Data { get; set; }
    }

    public class ExcelResponse
    {
        public DateRange DateRange { get; set; }
        public List<ChartResponse> Charts { get; set; }
        public List<ChartResponseVS> ChartsVS { get; set; }
        public List<TableResponse> Tables { get; set; }
        public List<DynamicResponse> DynamicResponse { get; set; }
    }
    public class ResponseObjectModel
    {
        public string DataPoint { get; set; }
        public double? Avg { get; set; }
        public List<ResponseObjectDataModel> data { get; set; }
    }

    public class ResponseObjectDataModel
    {
        public string Datapoint { get; set; }
        public int Key { get; set; }
        public DateTime Date { get; set; }
        public List<double> Value { get; set; }
        public List<DateTime> RealTimestamp { get; set; }
        public List<double> Timestamp { get; set; }
    }
}
