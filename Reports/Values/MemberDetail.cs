using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.Reports.Values
{
    public class MemberDetail
    {
        protected string FontHeader = "Tahoma";
        protected int FontHeaderSize = 14;

        protected string FontNormal = "Tahoma";
        protected int FontNormalSize = 10;

        public MemberDetail()
        {
            DetailHeaders = new List<ProjectHeader>();
        }

        public IList<ProjectHeader> DetailHeaders { get; set; }

        public int TotalMembers(string projectCode)
        {
            var members = (from dh in DetailHeaders
                           where dh.ProjectCode == projectCode
                               select dh.Members).FirstOrDefault();
            return members;
        }
        public string Title { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public int EmployeeID { get; set; }
        public string Position { get; set; }

        public string FullName { get; set; }
        public string Department { get; set; }

        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }

        public static DateTime FirstDayOfWeek(DateTime date)
        {
            var candidateDate = date;
            while (candidateDate.DayOfWeek != DayOfWeek.Sunday)
            {
                candidateDate = candidateDate.AddDays(-1);
            }
            return candidateDate;
        }
        public static DateTime LastDayOfWeek(DateTime date)
        {
            var candidateDate = date;
            while (candidateDate.DayOfWeek != DayOfWeek.Saturday)
            {
                candidateDate = candidateDate.AddDays(1);
            }
            return candidateDate;
        }

        public IList<TimesheetDetail> Details 
        {
            get
            {
                var listOfDetail = new List<TimesheetDetail>();
                foreach (var header in DetailHeaders)
                {
                    listOfDetail.AddRange(header.Details);
                }
                return listOfDetail.OrderBy(d => d.Date).ToList();
            }
        }

        protected void setHeader(ExcelWorksheet ws, ETypeOfHeader typeOfHeader = ETypeOfHeader.Person, int offset = 0)
        {
            var a1 = ws.Cells["A1"];
            a1.Value = Title;
            a1.Style.Font.Name = FontHeader;
            a1.Style.Font.Size = FontHeaderSize;
            a1.Style.Font.Bold = true;

            setLabel(ws, "A3", "Date:");
            setValue(ws, "B3", string.Format("{0} - {1}", FromDate.ToString("dd/MM/yyyy"), ToDate.ToString("dd/MM/yyyy")));

            if (typeOfHeader == ETypeOfHeader.Person)
            {
                setLabel(ws, "A5", "Employee ID:");
                setValue(ws, "B5", EmployeeID.ToString());

                setLabel(ws, "A6", "Position:");
                setValue(ws, "B6", Position);

                setLabel(ws, "E5", "Full Name:");
                setValue(ws, "F5", FullName);

                setLabel(ws, "E6", "Department:");
                setValue(ws, "F6", Department);
            }
            else if (typeOfHeader == ETypeOfHeader.Department)
            {
                setLabel(ws, "A5", "Department:");
                setValue(ws, "B5", Department);
            }
            else if (typeOfHeader == ETypeOfHeader.Project)
            {
                setLabel(ws, "A5", "Project Code:");
                setValue(ws, "B5", ProjectCode);

                setLabel(ws, "A6", "Project Name:");
                setValue(ws, "B6", ProjectName);
            }
            else if (typeOfHeader == ETypeOfHeader.AllProject)
            {
                
            }

        }
        protected void setLabel(ExcelWorksheet ws, string cell, object value)
        {
            var a3 = ws.Cells[cell];
            a3.Value = value;
            a3.Style.Font.Name = FontNormal;
            a3.Style.Font.Size = FontNormalSize;
            a3.Style.Font.Bold = true;
            a3.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            a3.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
            a3.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        }
        protected void setGridTitle(ExcelWorksheet ws, string title, string cell = "A8")
        {
            var a8 = ws.Cells[cell];
            a8.Value = title;
            a8.Style.Font.Name = FontHeader;
            a8.Style.Font.Size = FontHeaderSize;
            a8.Style.Font.Bold = true;
            a8.Style.Font.UnderLine = true;
        }
        protected void autoFitColumns(ExcelWorksheet ws)
        {
            if (ws.Dimension != null)
            {
                ws.Cells[ws.Dimension.Address].AutoFitColumns();
            }
        }
        protected void setGridHeader(ExcelWorksheet ws, string cell, string title)
        {
            var col = ws.Cells[cell];
            col.Value = title;
            col.Style.Font.Name = FontNormal;
            col.Style.Font.Size = FontNormalSize;
            col.Style.Font.Bold = true;
            col.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            col.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            col.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            col.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
        }
        protected void setValue(ExcelWorksheet ws, string cell, object val, string format = null, OfficeOpenXml.Style.ExcelHorizontalAlignment alignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.General)
        {
            var a = ws.Cells[cell];
            a.Value = val;
            a.Style.Font.Name = FontNormal;
            a.Style.Font.Size = FontNormalSize;
            a.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            a.Style.HorizontalAlignment = alignment;
            if (!string.IsNullOrEmpty(format))
            {
                a.Style.Numberformat.Format = format;
            }
        }
        protected void sumFormula(ExcelWorksheet ws, string cell, string fromRow, string toRow)
        {
            var sum = ws.Cells[cell];
            sum.Style.Font.Name = FontNormal;
            sum.Style.Font.Size = FontNormalSize;
            sum.Style.Numberformat.Format = "#,##0.00";
            sum.Formula = string.Format("=SUM({0}:{1})", fromRow, toRow);
            sum.Style.Font.Bold = true;
            sum.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        }
        protected void sumFormula(ExcelWorksheet ws, string cell, string formula)
        {
            var sum = ws.Cells[cell];
            sum.Style.Font.Name = FontNormal;
            sum.Style.Font.Size = FontNormalSize;
            sum.Style.Numberformat.Format = "#,##0.00";
            sum.Formula = formula;
            sum.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        }
    }
}
