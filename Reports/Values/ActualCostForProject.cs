using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.Reports.Values
{
    public class ActualCostForProject
        : MemberDetail
    {
        public ActualCostForProject()
        {
            Title = "Actual Cost for Project";
        }

        public Dictionary<dynamic, dynamic> SummaryByProjectRole
        {
            get
            {
                var summary = new Dictionary<dynamic, dynamic>();
                var timesheetGroup = (from gm in Details
                                      orderby gm.ProjectRoleOrder
                                      group gm by new { gm.ProjectCode, gm.ProjectRole } into gmG
                                      select new
                                      {
                                          ProjectCode = gmG.Key.ProjectCode,
                                          ProjectRole = gmG.Key.ProjectRole,
                                          Timesheets = gmG.ToList()
                                      }).ToList();

                foreach (var grpTimesheet in timesheetGroup)
                {
                    var key = new
                    {
                        ProjectRole = grpTimesheet.ProjectRole,
                    };
                    var members = grpTimesheet.Timesheets.GroupBy(d => d.EmployeeID).Count();
                    dynamic values = new
                    {
                        Members = members,
                        Hours = grpTimesheet.Timesheets.Sum(t => t.Hours),
                        Cost = grpTimesheet.Timesheets.Sum(t => t.Cost),
                    };
                    summary[key] = values;
                }
                return summary;
            }
        }
        public Dictionary<dynamic, dynamic> SummaryByTaskType
        {
            get
            {
                var summary = new Dictionary<dynamic, dynamic>();
                var timesheetGroup = (from gm in Details
                                      orderby gm.PhaseOrder
                                      group gm by new { gm.Phase, gm.TaskType } into gmG
                                      select new
                                      {
                                          Phase = gmG.Key.Phase,
                                          TaskType = gmG.Key.TaskType,
                                          Timesheets = gmG.ToList()
                                      }).ToList();

                foreach (var grpTimesheet in timesheetGroup)
                {
                    var key = new
                    {
                        Phase = grpTimesheet.Phase,
                        TaskType = grpTimesheet.TaskType
                    };
                    dynamic values = new
                    {
                        Members = grpTimesheet.Timesheets.GroupBy(d => d.EmployeeID).Count(),
                        Hours = grpTimesheet.Timesheets.Sum(t => t.Hours),
                        Cost = grpTimesheet.Timesheets.Sum(t => t.Cost),
                    };
                    summary[key] = values;
                }
                return summary;
            }
        }
        public Dictionary<dynamic, dynamic> SummaryByPhase
        {
            get
            {
                var summary = new Dictionary<dynamic, dynamic>();
                var timesheetGroup = (from gm in Details
                                      orderby gm.PhaseOrder
                                      group gm by new { gm.Phase } into gmG
                                      select new
                                      {
                                          Phase = gmG.Key.Phase,
                                          Timesheets = gmG.ToList()
                                      }).ToList();

                foreach (var grpTimesheet in timesheetGroup)
                {
                    var key = new
                    {
                        Phase = grpTimesheet.Phase
                    };
                    dynamic values = new
                    {
                        Members = grpTimesheet.Timesheets.GroupBy(d => d.EmployeeID).Count(),
                        Hours = grpTimesheet.Timesheets.Sum(t => t.Hours),
                        Cost = grpTimesheet.Timesheets.Sum(t => t.Cost),
                    };
                    summary[key] = values;
                }
                return summary;
            }
        }
        public Dictionary<dynamic, dynamic> SummaryByWeek
        {
            get
            {
                Func<DateTime, int> weekProjector = 
                    d => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                        d,
                        CalendarWeekRule.FirstFourDayWeek,
                        DayOfWeek.Sunday);

                var summary = new Dictionary<dynamic, dynamic>();
                var timesheetGroup = (from gm in Details
                                      group gm by new { Week = weekProjector(gm.Date) } into gmG
                                      select new
                                      {
                                          Week = gmG.Key.Week,
                                          Timesheets = gmG.ToList()
                                      }).ToList();

                foreach (var grpTimesheet in timesheetGroup)
                {
                    var key = new
                    {
                        Week = grpTimesheet.Week,
                    };

                    var firstDayOfWeek = MemberDetail.FirstDayOfWeek(grpTimesheet.Timesheets.FirstOrDefault().Date);
                    var lastDayOfWeek = MemberDetail.LastDayOfWeek(grpTimesheet.Timesheets.FirstOrDefault().Date);

                    dynamic values = new
                    {
                        Period = string.Format("{0} - {1}",

                        firstDayOfWeek.ToString("dd/MM/yyyy"),
                        lastDayOfWeek.ToString("dd/MM/yyyy")
                        ),
                        Hours = grpTimesheet.Timesheets.Sum(t => t.Hours),
                        Cost = grpTimesheet.Timesheets.Sum(t => t.Cost),
                    };
                    summary[key] = values;
                }
                return summary;
            }
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
                // Add the Content sheet
                var summaryByPhaseTitle = "Summary by Phase";
                var wsSummaryByPhase = pck.Workbook.Worksheets.Add(summaryByPhaseTitle);
                setHeader(wsSummaryByPhase, ETypeOfHeader.Project);
                setGridTitle(wsSummaryByPhase, summaryByPhaseTitle);
                setSummaryByPhaseHeader(wsSummaryByPhase);
                setSummaryByPhaseDetails(wsSummaryByPhase);

                var summaryByTaskTypeTitle = "Summary by Type of Phase";
                var wsSummaryByType = pck.Workbook.Worksheets.Add(summaryByTaskTypeTitle);
                setHeader(wsSummaryByType, ETypeOfHeader.Project);
                setGridTitle(wsSummaryByType, summaryByTaskTypeTitle);
                setSummaryByTaskTypeHeader(wsSummaryByType);
                setSummaryByTaskTypeDetails(wsSummaryByType);

                var summaryByWeekTitle = "Summary by Week";
                var wsSummaryByWeek = pck.Workbook.Worksheets.Add(summaryByWeekTitle);
                setHeader(wsSummaryByWeek, ETypeOfHeader.Project);
                setGridTitle(wsSummaryByWeek, summaryByWeekTitle);
                setSummaryByWeekHeader(wsSummaryByWeek);
                setSummaryByWeekDetails(wsSummaryByWeek);

                var summaryByProjectRoleTitle = "Summary by Project Role";
                var wsSummaryByProjectRole = pck.Workbook.Worksheets.Add(summaryByProjectRoleTitle);
                setHeader(wsSummaryByProjectRole, ETypeOfHeader.Project);
                setGridTitle(wsSummaryByProjectRole, summaryByProjectRoleTitle);
                setSummaryByProjectRoleHeader(wsSummaryByProjectRole);
                setSummaryByProjectRoleDetails(wsSummaryByProjectRole);

                var detailTitle = "Detail";
                var wsDetail = pck.Workbook.Worksheets.Add(detailTitle);
                setHeader(wsDetail, ETypeOfHeader.Project);
                setGridTitle(wsDetail, detailTitle);
                PopulateDetail(wsDetail);

                autoFitColumns(wsSummaryByPhase);
                autoFitColumns(wsSummaryByType);
                autoFitColumns(wsSummaryByWeek);
                autoFitColumns(wsSummaryByProjectRole);
                autoFitColumns(wsDetail);

                var maxWidth = 13.5;
                wsSummaryByPhase.Column(1).Width = maxWidth;
                wsSummaryByType.Column(1).Width = maxWidth;
                wsSummaryByWeek.Column(1).Width = maxWidth;
                wsSummaryByProjectRole.Column(1).Width = maxWidth;
                wsDetail.Column(1).Width = maxWidth;
                //wsDetail.Column(11).Width = 11;

                wsSummaryByPhase.Column(2).Width = maxWidth + 15;
                wsSummaryByType.Column(2).Width = maxWidth + 15;
                wsSummaryByWeek.Column(2).Width = maxWidth + 10;
                wsSummaryByProjectRole.Column(2).Width = maxWidth;
                wsDetail.Column(2).Width = maxWidth;

                pck.Save();
            }
        }

        public int WriteSummaryHeader(ExcelWorksheet ws, int offset)
        {
            setLabel(ws, "A" + offset, "Project Code:");
            setValue(ws, "B" + offset, ProjectCode);
            ++offset;
            setLabel(ws, "A" + offset, "Project Name:");
            setValue(ws, "B" + offset, ProjectName);
            return offset;
        }

        private void setSummaryByPhaseHeader(ExcelWorksheet ws, int base_index = 10)
        {
            setGridHeader(ws, "A" + base_index, "No.");
            setGridHeader(ws, "B" + base_index, "Phase");
            setGridHeader(ws, "C" + base_index, "Member");
            setGridHeader(ws, "D" + base_index, "Hours");
            setGridHeader(ws, "E" + base_index, "% Hours");
            setGridHeader(ws, "F" + base_index, "Cost");
            setGridHeader(ws, "G" + base_index, "% Cost");
        }
        private int setSummaryByPhaseDetails(ExcelWorksheet ws, int base_index = 10)
        {
            var index = 0;
            var offset = base_index + index;
            //Populate Details
            foreach (var summary in SummaryByPhase)
            {
                ++index;
                offset = base_index + index;

                setValue(ws, "A" + offset, index, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                setValue(ws, "B" + offset, summary.Key.Phase);
                setValue(ws, "C" + offset, summary.Value.Members, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                setValue(ws, "D" + offset, summary.Value.Hours, "#,##0.00");
                setValue(ws, "F" + offset, summary.Value.Cost, "#,##0.00");
            }

            var last_offset = offset + 1;
            index = 0;
            foreach (var summary in SummaryByPhase)
            {
                ++index;
                offset = base_index + index;

                var e = ws.Cells["E" + offset];
                e.Style.Numberformat.Format = "#,##0.00";
                e.Style.Font.Size = FontNormalSize;
                e.Style.Font.Name = FontNormal;
                e.Formula = string.Format("=D{0}*100/$D${1}", offset, last_offset);
                e.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                var g = ws.Cells["G" + offset];
                g.Style.Numberformat.Format = "#,##0.00";
                g.Style.Font.Size = FontNormalSize;
                g.Style.Font.Name = FontNormal;
                g.Formula = string.Format("=F{0}*100/$F${1}", offset, last_offset);
                g.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            }

            //sumFormula(ws,
            //        string.Format("C{0}", offset + 1),
            //        string.Format("C{0}", 11),
            //        string.Format("C{0}", offset));

            sumFormula(ws,
                    string.Format("D{0}", offset + 1),
                    string.Format("D{0}", base_index + 1),
                    string.Format("D{0}", offset));

            sumFormula(ws,
                    string.Format("E{0}", offset + 1),
                    string.Format("E{0}", base_index + 1),
                    string.Format("E{0}", offset));

            sumFormula(ws,
                    string.Format("F{0}", offset + 1),
                    string.Format("F{0}", base_index + 1),
                    string.Format("F{0}", offset));

            sumFormula(ws,
                    string.Format("G{0}", offset + 1),
                    string.Format("G{0}", base_index + 1),
                    string.Format("G{0}", offset));

            return last_offset;
        }
        public int WriteSummaryByPhaseDetails(ExcelWorksheet ws, int offset)
        {
            setSummaryByPhaseHeader(ws, offset);
            //++offset;
            offset = setSummaryByPhaseDetails(ws, offset);
            return offset;
        }

        private void setSummaryByTaskTypeHeader(ExcelWorksheet ws, int base_index = 10)
        {
            setGridHeader(ws, "A" + base_index, "No.");
            setGridHeader(ws, "B" + base_index, "Phase");
            setGridHeader(ws, "C" + base_index, "Type");
            setGridHeader(ws, "D" + base_index, "Member");
            setGridHeader(ws, "E" + base_index, "Hours");
            setGridHeader(ws, "F" + base_index, "% Hours");
            setGridHeader(ws, "G" + base_index, "Cost");
            setGridHeader(ws, "H" + base_index, "% Cost");
        }
        private int setSummaryByTaskTypeDetails(ExcelWorksheet ws, int base_index = 10)
        {
            var index = 0;
            var offset = base_index + index;
            //Populate Details
            foreach (var summary in SummaryByTaskType)
            {
                ++index;
                offset = base_index + index;

                setValue(ws, "A" + offset, index, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                setValue(ws, "B" + offset, summary.Key.Phase);
                setValue(ws, "C" + offset, summary.Key.TaskType, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                setValue(ws, "D" + offset, summary.Value.Members, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                setValue(ws, "E" + offset, summary.Value.Hours, "#,##0.00");
                setValue(ws, "G" + offset, summary.Value.Cost, "#,##0.00");
            }

            var last_offset = offset + 1;
            index = 0;
            foreach (var summary in SummaryByTaskType)
            {
                ++index;
                offset = base_index + index;

                var e = ws.Cells["F" + offset];
                e.Style.Numberformat.Format = "#,##0.00";
                e.Style.Font.Name = FontNormal;
                e.Style.Font.Size = FontNormalSize;
                e.Formula = string.Format("=E{0}*100/$E${1}", offset, last_offset);
                e.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                var g = ws.Cells["H" + offset];
                g.Style.Numberformat.Format = "#,##0.00";
                g.Style.Font.Name = FontNormal;
                g.Style.Font.Size = FontNormalSize;
                g.Formula = string.Format("=G{0}*100/$G${1}", offset, last_offset);
                g.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            }

            //sumFormula(ws,
            //        string.Format("D{0}", offset + 1),
            //        string.Format("D{0}", 11),
            //        string.Format("D{0}", offset));

            sumFormula(ws,
                    string.Format("E{0}", offset + 1),
                    string.Format("E{0}", base_index + 1),
                    string.Format("E{0}", offset));

            sumFormula(ws,
                    string.Format("F{0}", offset + 1),
                    string.Format("F{0}", base_index + 1),
                    string.Format("F{0}", offset));

            sumFormula(ws,
                    string.Format("G{0}", offset + 1),
                    string.Format("G{0}", base_index + 1),
                    string.Format("G{0}", offset));

            sumFormula(ws,
                    string.Format("H{0}", offset + 1),
                    string.Format("H{0}", base_index + 1),
                    string.Format("H{0}", offset));

            return last_offset;
        }
        public int WriteSummaryByTaskTypeDetails(ExcelWorksheet ws, int offset)
        {
            setSummaryByTaskTypeHeader(ws, offset);
            //++offset;
            offset = setSummaryByTaskTypeDetails(ws, offset);
            return offset;
        }

        private void setSummaryByWeekHeader(ExcelWorksheet ws, int base_index = 10)
        {
            setGridHeader(ws, "A" + base_index, "No.");
            setGridHeader(ws, "B" + base_index, "Period");
            setGridHeader(ws, "C" + base_index, "Hours");
            setGridHeader(ws, "D" + base_index, "Cost");
            setGridHeader(ws, "E" + base_index, "Accumulated Cost");
        }
        private int setSummaryByWeekDetails(ExcelWorksheet ws, int base_index = 10)
        {
            var index = 0;
            var offset = base_index + index;
            //Populate Details
            foreach (var summary in SummaryByWeek)
            {
                ++index;
                offset = base_index + index;

                setValue(ws, "A" + offset, index, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                setValue(ws, "B" + offset, summary.Value.Period, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                setValue(ws, "C" + offset, summary.Value.Hours, "#,##0.00");
                setValue(ws, "D" + offset, summary.Value.Cost, "#,##0.00");
            }

            var last_offset = offset + 1;
            index = 0;
            foreach (var summary in SummaryByWeek)
            {
                ++index;
                offset = base_index + index;
                var e = ws.Cells["E" + offset];
                e.Style.Numberformat.Format = "#,##0.00";
                e.Style.Font.Name = FontNormal;
                e.Style.Font.Size = FontNormalSize;
                e.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                if (index == 1)
                {
                    e.Formula = string.Format("=D{0}", offset);
                    continue;
                }
                e.Formula = string.Format("=D{0}+E{1}", offset, offset - 1);
            }

            //sumFormula(ws,
            //        string.Format("D{0}", offset + 1),
            //        string.Format("D{0}", 11),
            //        string.Format("D{0}", offset));

            sumFormula(ws,
                    string.Format("C{0}", offset + 1),
                    string.Format("C{0}", base_index + 1),
                    string.Format("C{0}", offset));

            sumFormula(ws,
                    string.Format("D{0}", offset + 1),
                    string.Format("D{0}", base_index + 1),
                    string.Format("D{0}", offset));

            sumFormula(ws,
                    string.Format("E{0}", offset + 1),
                    string.Format("E{0}", base_index + 1),
                    string.Format("E{0}", offset));

            return last_offset;

        }
        public int WriteSummaryByWeekDetails(ExcelWorksheet ws, int offset)
        {
            setSummaryByWeekHeader(ws, offset);
            //++offset;
            offset = setSummaryByWeekDetails(ws, offset);
            return offset;
        }

        private void setSummaryByProjectRoleHeader(ExcelWorksheet ws, int base_index = 10)
        {
            setGridHeader(ws, "A" + base_index, "No.");
            setGridHeader(ws, "B" + base_index, "Project Role");
            setGridHeader(ws, "C" + base_index, "Member");
            setGridHeader(ws, "D" + base_index, "Hours");
            setGridHeader(ws, "E" + base_index, "% Hours");
            setGridHeader(ws, "F" + base_index, "Cost");
            setGridHeader(ws, "G" + base_index, "% Cost");
        }
        private int setSummaryByProjectRoleDetails(ExcelWorksheet ws, int base_index = 10)
        {
            var index = 0;
            var offset = base_index + index;
            //Populate Details
            foreach (var summary in SummaryByProjectRole)
            {
                ++index;
                offset = base_index + index;

                setValue(ws, "A" + offset, index, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                setValue(ws, "B" + offset, summary.Key.ProjectRole);
                setValue(ws, "C" + offset, summary.Value.Members, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                setValue(ws, "D" + offset, summary.Value.Hours, "#,##0.00");
                setValue(ws, "F" + offset, summary.Value.Cost, "#,##0.00");
            }

            var last_offset = offset + 1;
            index = 0;
            foreach (var summary in SummaryByProjectRole)
            {
                ++index;
                offset = base_index + index;

                var e = ws.Cells["E" + offset];
                e.Style.Numberformat.Format = "#,##0.00";
                e.Style.Font.Name = FontNormal;
                e.Style.Font.Size = FontNormalSize;
                e.Formula = string.Format("=D{0}*100/$D${1}", offset, last_offset);
                e.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                var g = ws.Cells["G" + offset];
                g.Style.Numberformat.Format = "#,##0.00";
                g.Style.Font.Name = FontNormal;
                g.Style.Font.Size = FontNormalSize;
                g.Formula = string.Format("=F{0}*100/$F${1}", offset, last_offset);
                g.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            }

            //sumFormula(ws,
            //        string.Format("C{0}", offset + 1),
            //        string.Format("C{0}", 11),
            //        string.Format("C{0}", offset));

            sumFormula(ws,
                    string.Format("D{0}", offset + 1),
                    string.Format("D{0}", base_index + 1),
                    string.Format("D{0}", offset));

            sumFormula(ws,
                    string.Format("E{0}", offset + 1),
                    string.Format("E{0}", base_index + 1),
                    string.Format("E{0}", offset));

            sumFormula(ws,
                    string.Format("F{0}", offset + 1),
                    string.Format("F{0}", base_index + 1),
                    string.Format("F{0}", offset));

            sumFormula(ws,
                    string.Format("G{0}", offset + 1),
                    string.Format("G{0}", base_index + 1),
                    string.Format("G{0}", offset));
            return last_offset;
        }
        public int WriteSummaryByProjectRoleDetails(ExcelWorksheet ws, int offset)
        {
            setSummaryByProjectRoleHeader(ws, offset);
            //++offset;
            offset = setSummaryByProjectRoleDetails(ws, offset);
            return offset;
        }

        public int PopulateDetail(ExcelWorksheet ws, int base_index = 10)
        {
            var index = -1;
            var offset = base_index + index;
            var index_detail = 0;

            var timesheetGroup = (from gm in Details
                                  orderby gm.PhaseOrder
                                  group gm by new { gm.Phase } into gmG
                                  select new
                                  {
                                      Phase = gmG.Key.Phase,
                                      Timesheets = gmG.ToList()
                                  }).ToList();

            foreach (var byPhase in timesheetGroup)
            {
                if (byPhase.Timesheets.Count > 0)
                {
                    // Header
                    ++index;
                    offset = base_index + index;
                    setLabel(ws, "A" + offset, "Phase:");
                    setValue(ws, "B" + offset, byPhase.Phase);

                    ++index;
                    offset = base_index + index;

                    ++index;
                    offset = base_index + index;

                    setGridHeader(ws, "A" + offset, "No.");
                    setGridHeader(ws, "B" + offset, "Date");
                    setGridHeader(ws, "C" + offset, "Employee ID");
                    setGridHeader(ws, "D" + offset, "Full Name");
                    setGridHeader(ws, "E" + offset, "Project Role");
                    setGridHeader(ws, "F" + offset, "Role Cost");
                    setGridHeader(ws, "G" + offset, "Type");
                    setGridHeader(ws, "H" + offset, "Main Task");
                    setGridHeader(ws, "I" + offset, "Sub Task");
                    setGridHeader(ws, "J" + offset, "Hours");
                    setGridHeader(ws, "K" + offset, "Cost");

                    ++index;
                    offset = base_index + index;

                    // Details Timesheet
                    var first_row = offset;
                    index_detail = 0;

                    foreach (var timesheet in byPhase.Timesheets)
                    {
                        ++index_detail;
                        setValue(ws, "A" + offset, index_detail, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                        setValue(ws, "B" + offset, timesheet.Date.ToString("dd/MM/yyyy"), alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                        setValue(ws, "C" + offset, timesheet.EmployeeID, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                        setValue(ws, "D" + offset, timesheet.FullName);
                        setValue(ws, "E" + offset, timesheet.ProjectRole);
                        setValue(ws, "F" + offset, timesheet.RoleCost, format: "#,##0.00");
                        setValue(ws, "G" + offset, timesheet.TaskType, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                        setValue(ws, "H" + offset, timesheet.MainTask);
                        setValue(ws, "I" + offset, timesheet.SubTask);
                        setValue(ws, "J" + offset, timesheet.Hours, format: "#,##0.00");

                        //sumFormula(ws, "K" + offset, string.Format("=J{0}*F{0}", offset));
                        setValue(ws, "K" + offset, timesheet.Cost, format: "#,##0.00");

                        ++index;
                        offset = base_index + index;
                    }

                    sumFormula(ws,
                        string.Format("J{0}", offset),
                        string.Format("J{0}", first_row),
                        string.Format("J{0}", offset - 1));

                    sumFormula(ws,
                        string.Format("K{0}", offset),
                        string.Format("K{0}", first_row),
                        string.Format("K{0}", offset - 1));

                    ++index;
                    offset = base_index + index;
                }
            }

            return offset;
        }
    }
}
