﻿using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.Reports.Values
{
    public class ActualCostForPerson
        : MemberDetail
    {
        public ActualCostForPerson()
        {
            Title = "Actual Cost for Person";
        }

        public Dictionary<dynamic, dynamic> SummaryByProjectRole
        {
            get
            {
                var summary = new Dictionary<dynamic, dynamic>();
                var timesheetGroup = (from gm in Details
                                      orderby gm.ProjectCode, gm.ProjectRoleOrder
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
                        ProjectCode = grpTimesheet.ProjectCode,
                        ProjectRole = grpTimesheet.ProjectRole
                    };
                    dynamic values = new
                    {
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
                        TaskType = grpTimesheet.TaskType
                    };
                    dynamic values = new
                    {
                        Hours = grpTimesheet.Timesheets.Sum(t => t.Hours),
                        Cost = grpTimesheet.Timesheets.Sum(t => t.Cost),
                    };
                    summary[key] = values;
                }
                return summary;
            }
        }
        public Dictionary<dynamic, dynamic> SummaryByProject
        {
            get
            {
                var summary = new Dictionary<dynamic, dynamic>();
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
                        Phase = grpTimesheet.Phase
                    };
                    dynamic values = new
                    {
                        Hours = grpTimesheet.Timesheets.Sum(t => t.Hours),
                        Cost = grpTimesheet.Timesheets.Sum(t => t.Cost),
                    };
                    summary[key] = values;
                }
                return summary;
            }
        }
        public Dictionary<dynamic, dynamic> SummaryByDate
        {
            get
            {

                var summary = new Dictionary<dynamic, dynamic>();
                var timesheetGroup = (from gm in Details
                                      group gm by new { gm.Date } into gmG
                                      select new
                                      {
                                          Date = gmG.Key.Date,
                                          Timesheets = gmG.ToList()
                                      }).ToList();

                foreach (var grpTimesheet in timesheetGroup)
                {
                    var key = new
                    {
                        Date = grpTimesheet.Date,
                    };
                    dynamic values = new
                    {
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
            setGridHeader(ws, "D10", "Cost");
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
                setValue(ws, "C" + offset, summary.Value.Hours, "#,##0.00");
                setValue(ws, "D" + offset, summary.Value.Cost, "#,##0.00");
            }

            if (SummaryByDate.Count > 0)
            {
                sumFormula(ws,
                        string.Format("C{0}", offset + 1),
                        string.Format("C{0}", 11),
                        string.Format("C{0}", offset));

                sumFormula(ws,
                        string.Format("D{0}", offset + 1),
                        string.Format("D{0}", 11),
                        string.Format("D{0}", offset));
            }
        }

        private void setSummaryByProjectHeader(ExcelWorksheet ws)
        {
            setGridHeader(ws, "A10", "No.");
            setGridHeader(ws, "B10", "Project Code");
            setGridHeader(ws, "C10", "Phase");
            setGridHeader(ws, "D10", "Hours");
            setGridHeader(ws, "E10", "% Hours");
            setGridHeader(ws, "F10", "Cost");
            setGridHeader(ws, "G10", "% Cost");
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
                setValue(ws, "D" + offset, summary.Value.Hours, "#,##0.00");
                setValue(ws, "F" + offset, summary.Value.Cost, "#,##0.00");
            }

            var last_offset = offset + 1;
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

                var g = ws.Cells["G" + offset];
                g.Style.Numberformat.Format = "#,##0.00";
                g.Style.Font.Name = FontNormal;
                g.Style.Font.Size = FontNormalSize;
                g.Formula = string.Format("=F{0}*100/$F${1}", offset, last_offset);
                g.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            }

            if (SummaryByProject.Count > 0)
            {
                sumFormula(ws,
                        string.Format("D{0}", offset + 1),
                        string.Format("D{0}", 11),
                        string.Format("D{0}", offset));

                sumFormula(ws,
                        string.Format("E{0}", offset + 1),
                        string.Format("E{0}", 11),
                        string.Format("E{0}", offset));

                sumFormula(ws,
                        string.Format("F{0}", offset + 1),
                        string.Format("F{0}", 11),
                        string.Format("F{0}", offset));

                sumFormula(ws,
                        string.Format("G{0}", offset + 1),
                        string.Format("G{0}", 11),
                        string.Format("G{0}", offset));
            }
        }

        private void setSummaryByTaskTypeHeader(ExcelWorksheet ws)
        {
            setGridHeader(ws, "A10", "No.");
            setGridHeader(ws, "B10", "Project Code");
            setGridHeader(ws, "C10", "Type");
            setGridHeader(ws, "D10", "Hours");
            setGridHeader(ws, "E10", "% Hours");
            setGridHeader(ws, "F10", "Cost");
            setGridHeader(ws, "G10", "% Cost");
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
                setValue(ws, "C" + offset, summary.Key.TaskType);
                setValue(ws, "D" + offset, summary.Value.Hours, "#,##0.00");
                setValue(ws, "F" + offset, summary.Value.Cost, "#,##0.00");
            }

            var last_offset = offset + 1;
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

                var g = ws.Cells["G" + offset];
                g.Style.Numberformat.Format = "#,##0.00";
                g.Style.Font.Name = FontNormal;
                g.Style.Font.Size = FontNormalSize;
                g.Formula = string.Format("=F{0}*100/$F${1}", offset, last_offset);
                g.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            }

            if (SummaryByTaskType.Count > 0)
            {
                sumFormula(ws,
                        string.Format("D{0}", offset + 1),
                        string.Format("D{0}", 11),
                        string.Format("D{0}", offset));

                sumFormula(ws,
                        string.Format("E{0}", offset + 1),
                        string.Format("E{0}", 11),
                        string.Format("E{0}", offset));

                sumFormula(ws,
                        string.Format("F{0}", offset + 1),
                        string.Format("F{0}", 11),
                        string.Format("F{0}", offset));

                sumFormula(ws,
                        string.Format("G{0}", offset + 1),
                        string.Format("G{0}", 11),
                        string.Format("G{0}", offset));
            }
        }

        private void setSummaryByProjectRoleHeader(ExcelWorksheet ws)
        {
            setGridHeader(ws, "A10", "No.");
            setGridHeader(ws, "B10", "Project Code");
            setGridHeader(ws, "C10", "Project Role");
            setGridHeader(ws, "D10", "Hours");
            setGridHeader(ws, "E10", "% Hours");
            setGridHeader(ws, "F10", "Cost");
            setGridHeader(ws, "G10", "% Cost");
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
                setValue(ws, "B" + offset, summary.Key.ProjectCode);
                setValue(ws, "C" + offset, summary.Key.ProjectRole);
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

            if (SummaryByProjectRole.Count > 0)
            {
                sumFormula(ws,
                        string.Format("D{0}", offset + 1),
                        string.Format("D{0}", 11),
                        string.Format("D{0}", offset));

                sumFormula(ws,
                        string.Format("E{0}", offset + 1),
                        string.Format("E{0}", 11),
                        string.Format("E{0}", offset));

                sumFormula(ws,
                        string.Format("F{0}", offset + 1),
                        string.Format("F{0}", 11),
                        string.Format("F{0}", offset));

                sumFormula(ws,
                        string.Format("G{0}", offset + 1),
                        string.Format("G{0}", 11),
                        string.Format("G{0}", offset));
            }
        }

        private void populateDetail(ExcelWorksheet ws)
        {
            var index = -1;
            var base_index = 10;
            var offset = base_index + index;
            var index_detail = 0;
            foreach (var project in DetailHeaders.OrderBy( p => p.ProjectCode ))
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
                    setGridHeader(ws, "H" + offset, "Role Cost");
                    setGridHeader(ws, "I" + offset, "Hours");
                    setGridHeader(ws, "J" + offset, "Cost");

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
                        setValue(ws, "H" + offset, timesheet.RoleCost, format: "#,##0.00");
                        setValue(ws, "I" + offset, timesheet.Hours, format: "#,##0.00");
                        setValue(ws, "J" + offset, timesheet.Cost, format: "#,##0.00");

                        ++index;
                        offset = base_index + index;
                    }

                    sumFormula(ws,
                            string.Format("I{0}", offset),
                            string.Format("I{0}", first_row),
                            string.Format("I{0}", offset - 1));

                    sumFormula(ws,
                        string.Format("J{0}", offset),
                        string.Format("J{0}", first_row),
                        string.Format("J{0}", offset - 1));

                    ++index;
                    offset = base_index + index;
                }
            }
        }
    }
}
