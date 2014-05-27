using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.Reports.Values
{
    public class ActualCostForAllProject
        : MemberDetail
    {
        public ActualCostForAllProject()
        {
            Title = "Actual Cost for All Project";
        }

        public void WriteExcel(string path)
        {
            var newFile = new FileInfo(path);
            if (newFile.Exists)
            {
                newFile.Delete();
            }

            using (var pck = new ExcelPackage(newFile))
            {
                var detailTitle = "Detail";
                var wsDetail = pck.Workbook.Worksheets.Add(detailTitle);
                setHeader(wsDetail, ETypeOfHeader.AllProject);
                setGridTitle(wsDetail, detailTitle, cell: "A5");
                populateDetail(wsDetail);

                autoFitColumns(wsDetail);

                var maxWidth = 12.5;
                wsDetail.Column(1).Width = maxWidth;

                pck.Save();
            }
        }

        private void populateDetail(ExcelWorksheet ws)
        {
            var index = -1;
            var base_index = 7;
            var offset = base_index + index;

            var timesheetGroup = (from gm in Details
                                  group gm by new { gm.ProjectCode } into gmG
                                  select new
                                  {
                                      ProjectCode = gmG.Key.ProjectCode,
                                      StartDate = gmG.ToList().Min(d => d.Date),
                                      EndDate = gmG.ToList().Max(d => d.Date),
                                      Timesheets = gmG.ToList()
                                  }).ToList();


            // Header
            ++index;
            offset = base_index + index;

            setGridHeader(ws, "A" + offset, "No.");
            setGridHeader(ws, "B" + offset, "Date");
            setGridHeader(ws, "C" + offset, "Days");
            setGridHeader(ws, "D" + offset, "Project Code");
            setGridHeader(ws, "E" + offset, "Member");
            setGridHeader(ws, "F" + offset, "Hours");
            setGridHeader(ws, "G" + offset, "% Hours");
            setGridHeader(ws, "H" + offset, "Cost");
            setGridHeader(ws, "I" + offset, "% Cost");

            var first_row = offset + 1;
            foreach (var byPhase in timesheetGroup)
            {
                ++index;
                offset = base_index + index;

                //var members = byPhase.Timesheets.GroupBy(d => d.EmployeeID).Count();
                var members = DetailHeaders
                    .Where(h => h.ProjectCode == byPhase.ProjectCode)
                    .Select(p => p.Members)
                    .FirstOrDefault();

                // Details Timesheet
                var startDate = byPhase.Timesheets.Min(d => d.Date);
                var endDate = byPhase.Timesheets.Max(d => d.Date);
                var days = (endDate - startDate).TotalDays + 1;
                var dateBetween = string.Format("{0} - {1}", startDate.ToString("dd/MM/yyyy"), endDate.ToString("dd/MM/yyyy"));

                setValue(ws, "A" + offset, index, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                setValue(ws, "B" + offset, dateBetween, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                setValue(ws, "C" + offset, days, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                setValue(ws, "D" + offset, byPhase.ProjectCode);

                setValue(ws, "E" + offset, members, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                setValue(ws, "F" + offset, byPhase.Timesheets.Sum(h => h.Hours), format: "#,##0.00");
                setValue(ws, "H" + offset, byPhase.Timesheets.Sum(h => h.Cost), format: "#,##0.00");
            }

            var last_offset = offset + 1;
            index = 0;
            foreach (var byPhase in timesheetGroup)
            {
                ++index;
                offset = base_index + index;

                var e = ws.Cells["G" + offset];
                e.Style.Numberformat.Format = "#,##0.00";
                e.Style.Font.Name = FontNormal;
                e.Style.Font.Size = FontNormalSize;
                e.Formula = string.Format("=F{0}*100/$F${1}", offset, last_offset);
                e.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                var g = ws.Cells["I" + offset];
                g.Style.Numberformat.Format = "#,##0.00";
                g.Style.Font.Name = FontNormal;
                g.Style.Font.Size = FontNormalSize;
                g.Formula = string.Format("=H{0}*100/$H${1}", offset, last_offset);
                g.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            }

            sumFormula(ws,
                string.Format("F{0}", offset + 1),
                string.Format("F{0}", first_row),
                string.Format("F{0}", offset));

            sumFormula(ws,
                string.Format("G{0}", offset + 1),
                string.Format("G{0}", first_row),
                string.Format("G{0}", offset));

            sumFormula(ws,
                string.Format("H{0}", offset + 1),
                string.Format("H{0}", first_row),
                string.Format("H{0}", offset));

            sumFormula(ws,
                string.Format("I{0}", offset + 1),
                string.Format("I{0}", first_row),
                string.Format("I{0}", offset));
        }
    }
}