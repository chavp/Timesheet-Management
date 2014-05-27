using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.Reports.Values
{
    public class ActualEffortForPerson
        : MemberDetail
    {
        public ActualEffortForPerson()
        {
            Title = "Actual Effort for Person";
        }

        public Dictionary<dynamic, decimal> SummaryByProjectRole 
        {
            get
            {
                var summary = new Dictionary<dynamic, decimal>();
                var timesheetGroup = (from gm in Details
                                      orderby gm.ProjectRoleOrder
                                      group gm by new { gm.ProjectRole } into gmG
                                      select new
                                      {
                                          ProjectRole = gmG.Key.ProjectRole,
                                          Timesheets = gmG.ToList()
                                      }).ToList();

                foreach (var grpTimesheet in timesheetGroup)
                {
                    var key = new
                    {
                        ProjectRole = grpTimesheet.ProjectRole,
                    };

                    summary[key] = grpTimesheet.Timesheets.Sum(t => t.Hours);
                }
                return summary;
            }
        }
        public Dictionary<dynamic, decimal> SummaryByTaskType
        {
            get
            {
                var summary = new Dictionary<dynamic, decimal>();

                var timesheetGroup = (from gm in Details
                                      orderby gm.ProjectCode
                                      group gm by new { gm.ProjectCode, gm.TaskType } into gmG
                                      select new
                                      {
                                          ProjectCode = gmG.Key.ProjectCode,
                                          TaskType = gmG.Key.TaskType,
                                          Timesheets = gmG.ToList()
                                      }).ToList();

                foreach (var grpTimesheet in timesheetGroup)
                {
                    var key = new
                    {
                        ProjectCode = grpTimesheet.ProjectCode,
                        TaskType = grpTimesheet.TaskType,
                    };

                    summary[key] = grpTimesheet.Timesheets.Sum(t => t.Hours);
                }

                return summary;
            }
        }
        public Dictionary<dynamic, decimal> SummaryByProject
        {
            get
            {
                var summary = new Dictionary<dynamic, decimal>();
                var timesheetGroup = (from gm in Details
                                      orderby gm.ProjectCode, gm.PhaseOrder
                                      group gm by new { gm.ProjectCode, gm.Phase } into gmG
                                      select new
                                      {
                                          ProjectCode = gmG.Key.ProjectCode,
                                          Phase = gmG.Key.Phase,
                                          Timesheets = gmG.ToList()
                                      }).ToList();

                foreach (var grpTimesheet in timesheetGroup)
                {
                    var key = new
                    {
                        ProjectCode = grpTimesheet.ProjectCode,
                        Phase = grpTimesheet.Phase,
                    };

                    summary[key] = grpTimesheet.Timesheets.Sum(t => t.Hours);
                }

                return summary;
            }
        }
        public Dictionary<dynamic, decimal> SummaryByDate
        {
            get
            {
                var summary = new Dictionary<dynamic, decimal>();
                var timesheetGroup = (from gm in Details
                                                          group gm by gm.Date into gmG
                                                          select new
                                                          {
                                                              Date = gmG.Key,
                                                              Timesheets = gmG.ToList()
                                                          }).ToList();

                foreach (var grpTimesheet in timesheetGroup)
                {
                    var key = new
                    {
                        Date = grpTimesheet.Date
                    };
                    summary[key] = grpTimesheet.Timesheets.Sum(t => t.Hours);
                }

                return summary;
            }
        }

        public void DisplayConsole()
        {
            Console.WriteLine(Title);
            Console.WriteLine();
            Console.WriteLine(
                string.Format("Date: {0} - {1}", 
                FromDate.ToString("dd/MM/yyyy"),
                ToDate.ToString("dd/MM/yyyy")));
            Console.WriteLine();
            Console.WriteLine("Employee ID: " + EmployeeID);
            Console.WriteLine("Full Name: " + FullName);
            Console.WriteLine("Position: " + Position);
            Console.WriteLine("Department: " + Department);
            Console.WriteLine();
            Console.WriteLine("Detail-------------------------------------------");
            Console.WriteLine();
            int index = 0;
            foreach (var header in DetailHeaders)
            {
                index = 0;
                Console.WriteLine("Project Code: " + header.ProjectCode);
                Console.WriteLine("Project Name: " + header.ProjectName);
                Console.WriteLine("No.|Date|Project Role|Phase|Type|Main Task|Sub Task|Hours");
                foreach (var detail in header.Details)
                {
                    ++index;
                    Console.WriteLine(
                        string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}",
                        index, detail.Date.ToString("dd/MM/yyyy"), detail.ProjectRole, detail.Phase, detail.TaskType, detail.MainTask, detail.SubTask, detail.Hours));
                }
                Console.WriteLine("Total: " + header.Hours);
                Console.WriteLine();
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
                var summaryByDateTitle = "Summary by Date";
                var wsSummaryByDate = pck.Workbook.Worksheets.Add(summaryByDateTitle);
                setHeader(wsSummaryByDate);
                setGridTitle(wsSummaryByDate, summaryByDateTitle);
                setSummaryByDateHeader(wsSummaryByDate);
                setSummaryByDateDetails(wsSummaryByDate);

                var summaryByProjectTitle = "Summary by Project";
                var wsSummaryByProject = pck.Workbook.Worksheets.Add(summaryByProjectTitle);
                //wsSummaryByProject.Workbook.CalcMode = ExcelCalcMode.Automatic;
                setHeader(wsSummaryByProject);
                setGridTitle(wsSummaryByProject, summaryByProjectTitle);
                setSummaryByProjectHeader(wsSummaryByProject);
                setSummaryByProjectDetails(wsSummaryByProject);

                var summaryByTaskTypeTitle = "Summary by Type (New, Rework)";
                var wsSummaryByType = pck.Workbook.Worksheets.Add(summaryByTaskTypeTitle);
                setHeader(wsSummaryByType);
                setGridTitle(wsSummaryByType, summaryByTaskTypeTitle);
                setSummaryByTaskTypeHeader(wsSummaryByType);
                setSummaryByTaskTypeDetails(wsSummaryByType);

                var summaryByProjectRoleTitle = "Summary by Project Role";
                var wsSummaryByProjectRole = pck.Workbook.Worksheets.Add(summaryByProjectRoleTitle);
                setHeader(wsSummaryByProjectRole);
                setGridTitle(wsSummaryByProjectRole, summaryByProjectRoleTitle);
                setSummaryByProjectRoleHeader(wsSummaryByProjectRole);
                setSummaryByProjectRoleDetails(wsSummaryByProjectRole);

                var detailTitle = "Detail";
                var wsDetail = pck.Workbook.Worksheets.Add(detailTitle);
                setHeader(wsDetail);
                setGridTitle(wsDetail, detailTitle);
                populateDetail(wsDetail);

                autoFitColumns(wsSummaryByDate);
                autoFitColumns(wsSummaryByProject);
                autoFitColumns(wsSummaryByType);
                autoFitColumns(wsSummaryByProjectRole);
                autoFitColumns(wsDetail);

                var maxWidth = 13.5;
                wsSummaryByDate.Column(1).Width = maxWidth;
                wsSummaryByProject.Column(1).Width = maxWidth;
                wsSummaryByType.Column(1).Width = maxWidth;
                wsSummaryByProjectRole.Column(1).Width = maxWidth;
                wsDetail.Column(1).Width = maxWidth;

                wsSummaryByDate.Column(2).Width = maxWidth + 1.5;
                wsSummaryByProject.Column(2).Width = maxWidth + 1.5;
                wsSummaryByType.Column(2).Width = maxWidth + 1.5;
                wsSummaryByProjectRole.Column(2).Width = maxWidth + 1.5;
                wsDetail.Column(2).Width = maxWidth + 1.5;

                pck.Save();
            }
        }

        private void setSummaryByDateHeader(ExcelWorksheet ws)
        {
            setGridHeader(ws, "A10", "No.");
            setGridHeader(ws, "B10", "Date");
            setGridHeader(ws, "C10", "Hours");
        }
        private void setSummaryByDateDetails(ExcelWorksheet ws)
        {
            var index = 0;
            var base_index = 10;
            var offset = base_index + index;
            //Populate Details
            foreach (var summary in SummaryByDate)
            {
                ++index;
                offset = base_index + index;

                setValue(ws, "A" + offset, index, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                setValue(ws, "B" + offset, summary.Key.Date.ToString("dd/MM/yyyy"), alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                setValue(ws, "C" + offset, summary.Value, "#,##0.00");
            }
            if (SummaryByDate.Count > 0)
            {
                sumFormula(ws,
                        string.Format("C{0}", offset + 1),
                        string.Format("C{0}", 11),
                        string.Format("C{0}", offset));
            }
        }

        private void setSummaryByProjectHeader(ExcelWorksheet ws)
        {
            setGridHeader(ws, "A10", "No.");
            setGridHeader(ws, "B10", "Project Code");
            setGridHeader(ws, "C10", "Phase");
            setGridHeader(ws, "D10", "Hours");
            setGridHeader(ws, "E10", "%");
        }
        private void setSummaryByProjectDetails(ExcelWorksheet ws)
        {
            var index = 0;
            var base_index = 10;
            var offset = base_index + index;
            //Populate Details
            foreach (var summary in SummaryByProject)
            {
                ++index;
                offset = base_index + index;

                setValue(ws, "A" + offset, index, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                setValue(ws, "B" + offset, summary.Key.ProjectCode);
                setValue(ws, "C" + offset, summary.Key.Phase);
                setValue(ws, "D" + offset, summary.Value, "#,##0.00");
            }

            var last_offset = offset + 1;
            //var d_total = ws.Cells["D" + last_offset];
            //d_total.Style.Numberformat.Format = "#,##0.00";
            //d_total.Style.Font.Name = FontNormal;
            //d_total.Style.Font.Size = FontNormalSize;
            //d_total.Formula = string.Format("=SUM(D11:D{0})", last_offset - 1);
            //d_total.Style.Font.Bold = true;
            //d_total.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            if (SummaryByProject.Count > 0)
            {
                sumFormula(ws,
                        string.Format("D{0}", offset + 1),
                        string.Format("D{0}", 11),
                        string.Format("D{0}", offset));
            }

            index = 0;
            foreach (var summary in SummaryByProject)
            {
                ++index;
                offset = base_index + index;
                var e = ws.Cells["E" + offset];
                e.Style.Numberformat.Format = "#,##0.00";
                e.Style.Font.Name = FontNormal;
                e.Style.Font.Size = FontNormalSize;
                e.Formula = string.Format("=D{0}*100/$D${1}", offset, last_offset);
                e.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            }

            if (SummaryByProject.Count > 0)
            {
                sumFormula(ws,
                        string.Format("E{0}", offset + 1),
                        string.Format("E{0}", 11),
                        string.Format("E{0}", offset));
            }
        }

        private void setSummaryByTaskTypeHeader(ExcelWorksheet ws)
        {
            setGridHeader(ws, "A10", "No.");
            setGridHeader(ws, "B10", "Project Code");
            setGridHeader(ws, "C10", "Type");
            setGridHeader(ws, "D10", "Hours");
            setGridHeader(ws, "E10", "%");
        }
        private void setSummaryByTaskTypeDetails(ExcelWorksheet ws)
        {
            var index = 0;
            var base_index = 10;
            var offset = base_index + index;
            //Populate Details
            foreach (var summary in SummaryByTaskType)
            {
                ++index;
                offset = base_index + index;

                setValue(ws, "A" + offset, index, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                setValue(ws, "B" + offset, summary.Key.ProjectCode);
                setValue(ws, "C" + offset, summary.Key.TaskType, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                setValue(ws, "D" + offset, summary.Value, "#,##0.00");
            }

            var last_offset = offset + 1;
            //var d_total = ws.Cells["D" + last_offset];
            //d_total.Style.Numberformat.Format = "#,##0.00";
            //d_total.Style.Font.Name = FontNormal;
            //d_total.Style.Font.Size = FontNormalSize;
            //d_total.Formula = string.Format("=SUM(D11:D{0})", last_offset - 1);
            //d_total.Style.Font.Bold = true;
            //d_total.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

            if (SummaryByTaskType.Count > 0)
            {
                sumFormula(ws,
                        string.Format("D{0}", offset + 1),
                        string.Format("D{0}", 11),
                        string.Format("D{0}", offset));
            }

            index = 0;
            foreach (var summary in SummaryByTaskType)
            {
                ++index;
                offset = base_index + index;
                var e = ws.Cells["E" + offset];
                e.Style.Numberformat.Format = "#,##0.00";
                e.Style.Font.Name = FontNormal;
                e.Style.Font.Size = FontNormalSize;
                e.Formula = string.Format("=D{0}*100/$D${1}", offset, last_offset);
                e.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            }

            if (SummaryByTaskType.Count > 0)
            {
                sumFormula(ws,
                        string.Format("E{0}", offset + 1),
                        string.Format("E{0}", 11),
                        string.Format("E{0}", offset));
            }
        }

        private void setSummaryByProjectRoleHeader(ExcelWorksheet ws)
        {
            setGridHeader(ws, "A10", "No.");
            setGridHeader(ws, "B10", "Project Role");
            setGridHeader(ws, "C10", "Hours");
            setGridHeader(ws, "D10", "%");
        }
        private void setSummaryByProjectRoleDetails(ExcelWorksheet ws)
        {
            var index = 0;
            var base_index = 10;
            var offset = base_index + index;
            //Populate Details
            foreach (var summary in SummaryByProjectRole)
            {
                ++index;
                offset = base_index + index;

                setValue(ws, "A" + offset, index, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                setValue(ws, "B" + offset, summary.Key.ProjectRole);
                setValue(ws, "C" + offset, summary.Value, "#,##0.00");
            }

            var last_offset = offset + 1;
            //var d_total = ws.Cells["C" + last_offset];
            //d_total.Style.Numberformat.Format = "#,##0.00";
            //d_total.Style.Font.Name = FontNormal;
            //d_total.Style.Font.Size = FontNormalSize;
            //d_total.Formula = string.Format("=SUM(C11:C{0})", last_offset - 1);
            //d_total.Style.Font.Bold = true;
            //d_total.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

            if (SummaryByProjectRole.Count > 0)
            {
                sumFormula(ws,
                            string.Format("C{0}", offset + 1),
                            string.Format("C{0}", 11),
                            string.Format("C{0}", offset));
            }

            index = 0;
            foreach (var summary in SummaryByProjectRole)
            {
                ++index;
                offset = base_index + index;
                var e = ws.Cells["D" + offset];
                e.Style.Font.Name = FontNormal;
                e.Style.Font.Size = FontNormalSize;
                e.Style.Numberformat.Format = "#,##0.00";
                e.Formula = string.Format("=C{0}*100/$C${1}", offset, last_offset);
                e.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            }

            if (SummaryByProjectRole.Count > 0)
            {
                sumFormula(ws,
                        string.Format("D{0}", offset + 1),
                        string.Format("D{0}", 11),
                        string.Format("D{0}", offset));
            }
        }

        private void populateDetail(ExcelWorksheet ws)
        {
            var index = -1;
            var base_index = 10;
            var offset = base_index + index;
            var index_detail = 0;

            //var timesheetGroup = (from gm in Details
            //                      orderby gm.ProjectCode, gm.ProjectRole
            //                      group gm by new { gm.ProjectCode, gm.ProjectRole } into gmG
            //                      select new
            //                      {
            //                          ProjectCode = gmG.Key.ProjectCode,
            //                          ProjectRole = gmG.Key.ProjectRole,
            //                          Timesheets = gmG.ToList()
            //                      }).ToList();

            foreach (var project in DetailHeaders.OrderBy( h => h.ProjectCode ))
            {
                var details = from d in project.Details
                              orderby d.Date, d.PhaseOrder
                              select d;

                if (details.Count() > 0)
                {
                    // Header
                    ++index;
                    offset = base_index + index;
                    setLabel(ws, "A" + offset, "Project Code:");
                    setValue(ws, "B" + offset, project.ProjectCode);

                    ++index;
                    offset = base_index + index;
                    setLabel(ws, "A" + offset, "Project Name:");
                    setValue(ws, "B" + offset, project.ProjectName);

                    //++index;
                    //offset = base_index + index;
                    //setLabel(ws, "A" + offset, "Project Role:");
                    //setValue(ws, "B" + offset, project.CurrentProjectRole);

                    ++index;
                    offset = base_index + index;

                    ++index;
                    offset = base_index + index;

                    setGridHeader(ws, "A" + offset, "No.");
                    setGridHeader(ws, "B" + offset, "Date");
                    setGridHeader(ws, "C" + offset, "Phase");
                    setGridHeader(ws, "D" + offset, "Type");
                    setGridHeader(ws, "E" + offset, "Main Task");
                    setGridHeader(ws, "F" + offset, "Sub Task");
                    setGridHeader(ws, "G" + offset, "Project Role");
                    setGridHeader(ws, "H" + offset, "Hours");

                    ++index;
                    offset = base_index + index;

                    // Details Timesheet
                    var first_row = offset;
                    index_detail = 0;

                    foreach (var timesheet in details)
                    {
                        ++index_detail;
                        setValue(ws, "A" + offset, index_detail, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                        setValue(ws, "B" + offset, timesheet.Date.ToString("dd/MM/yyyy"), alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                        setValue(ws, "C" + offset, timesheet.Phase);
                        setValue(ws, "D" + offset, timesheet.TaskType, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                        setValue(ws, "E" + offset, timesheet.MainTask);
                        setValue(ws, "F" + offset, timesheet.SubTask);
                        setValue(ws, "G" + offset, timesheet.ProjectRole);
                        setValue(ws, "H" + offset, timesheet.Hours, format: "#,##0.00");

                        ++index;
                        offset = base_index + index;
                    }


                    sumFormula(ws,
                        string.Format("H{0}", offset),
                        string.Format("H{0}", first_row),
                        string.Format("H{0}", offset - 1));


                    ++index;
                    offset = base_index + index;
                }
            }
        }
    }
}
