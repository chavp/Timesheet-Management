using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.Reports.Values
{
    public class ActualCostForDepartment
        : MemberDetail
    {
        public ActualCostForDepartment()
        {
            Title = "Actual Cost for Department";
            IsDisplayCost = true;
        }

        public bool IsDisplayCost { get; set; }

        public Dictionary<dynamic, dynamic> SummaryByProjectRole
        {
            get
            {
                var summary = new Dictionary<dynamic, dynamic>();
                var timesheetOnlyDepartmentGroupByProjectRole = (from gm in Details
                                                                 orderby gm.ProjectRoleOrder
                                                                 group gm by gm.ProjectRole into gmG
                                                                 select new
                                                                 {
                                                                     ProjectRole = gmG.Key,
                                                                     Timesheets = gmG.ToList()
                                                                 }).ToList();

                foreach (var grpTimesheet in timesheetOnlyDepartmentGroupByProjectRole)
                {
                    var key = new
                    {
                        ProjectRole = grpTimesheet.ProjectRole
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
        public Dictionary<dynamic, dynamic> SummaryByTaskType
        {
            get
            {
                var summary = new Dictionary<dynamic, dynamic>();
                var timesheetGroupByTaskType = (from gm in Details
                                                group gm by new { gm.ProjectCode, gm.TaskType } into gmG
                                                select new
                                                {
                                                    Key = gmG.Key,
                                                    Timesheets = gmG.ToList()
                                                }).ToList();

                foreach (var grpTimesheet in timesheetGroupByTaskType)
                {
                    var key = new
                    {
                        ProjectCode = grpTimesheet.Key.ProjectCode,
                        TaskType = grpTimesheet.Key.TaskType,
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
        public Dictionary<dynamic, dynamic> SummaryByProject
        {
            get
            {
                var summary = new Dictionary<dynamic, dynamic>();
                var timesheetGroupBy = (from gm in Details
                                        orderby gm.ProjectCode, gm.PhaseOrder
                                        group gm by new { gm.ProjectCode, gm.Phase } into gmG
                                        select new
                                        {
                                            ProjectCode = gmG.Key.ProjectCode,
                                            Phase = gmG.Key.Phase,
                                            Timesheets = gmG.ToList()
                                        }).ToList();

                foreach (var grpTimesheet in timesheetGroupBy)
                {
                    var key = new
                    {
                        ProjectCode = grpTimesheet.ProjectCode,
                        Phase = grpTimesheet.Phase,
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
        public Dictionary<dynamic, dynamic> SummaryByPerson
        {
            get
            {
                var summary = new Dictionary<dynamic, dynamic>();

                var timesheetGroup = (from gm in Details
                                      orderby gm.ProjectCode
                                      group gm by new 
                                      {
                                          gm.EmployeeID,
                                          gm.FullName,
                                          gm.ProjectCode,
                                          gm.Phase,
                                          gm.ProjectRole,
                                      } into gmG
                                      select new
                                      {
                                          EmployeeID = gmG.Key.EmployeeID,
                                          FullName = gmG.Key.FullName,
                                          ProjectCode = gmG.Key.ProjectCode,
                                          Phase = gmG.Key.Phase,
                                          ProjectRole = gmG.Key.ProjectRole,
                                          Timesheets = gmG.ToList()
                                      }).ToList();

                foreach (var grpTimesheet in timesheetGroup)
                {
                    var key = new
                    {
                        EmployeeID = grpTimesheet.EmployeeID,
                        ProjectCode = grpTimesheet.ProjectCode,
                        ProjectRole = grpTimesheet.ProjectRole,
                    };
                    dynamic values = new
                    {
                        Hours = grpTimesheet.Timesheets.Sum(t => t.Hours),
                        Cost = grpTimesheet.Timesheets.Sum(t => t.Cost),
                        Phase = grpTimesheet.Phase,
                        FullName = grpTimesheet.FullName,
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
                var timesheetOnlyDepartmentGroupByDate = (from gm in Details
                                                          orderby gm.ProjectCode
                                                          group gm by gm.Date into gmG
                                                          select new
                                                          {
                                                              Date = gmG.Key,
                                                              Timesheets = gmG.ToList()
                                                          }).ToList();

                foreach (var grpTimesheet in timesheetOnlyDepartmentGroupByDate)
                {
                    var key = new
                    {
                        Date = grpTimesheet.Date
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

        public void WriteExcel(string path)
        {
            var newFile = new FileInfo(path);
            if (newFile.Exists)
            {
                newFile.Delete();
            }

            using (var pck = new ExcelPackage(newFile))
            {
                var summaryByDateTitle = "Summary by Date";
                var wsSummaryByDate = pck.Workbook.Worksheets.Add(summaryByDateTitle);
                setHeader(wsSummaryByDate, ETypeOfHeader.Department);
                setGridTitle(wsSummaryByDate, summaryByDateTitle);
                setSummaryByDateHeader(wsSummaryByDate);
                setSummaryByDateDetails(wsSummaryByDate);

                var summaryByProjectTitle = "Summary by Project";
                var wsSummaryByProject = pck.Workbook.Worksheets.Add(summaryByProjectTitle);
                setHeader(wsSummaryByProject, ETypeOfHeader.Department);
                setGridTitle(wsSummaryByProject, summaryByProjectTitle);
                setSummaryByProjectHeader(wsSummaryByProject);
                setSummaryByProjectDetails(wsSummaryByProject);

                var summaryByProjectRoleTitle = "Summary by Project Role";
                var wsSummaryByProjectRole = pck.Workbook.Worksheets.Add(summaryByProjectRoleTitle);
                setHeader(wsSummaryByProjectRole, ETypeOfHeader.Department);
                setGridTitle(wsSummaryByProjectRole, summaryByProjectRoleTitle);
                setSummaryByProjectRoleHeader(wsSummaryByProjectRole);
                setSummaryByProjectRoleDetails(wsSummaryByProjectRole);

                var summaryByTaskTypeTitle = "Summary by Type (New, Rework)";
                var wsSummaryByType = pck.Workbook.Worksheets.Add(summaryByTaskTypeTitle);
                setHeader(wsSummaryByType, ETypeOfHeader.Department);
                setGridTitle(wsSummaryByType, summaryByTaskTypeTitle);
                setSummaryByTaskTypeHeader(wsSummaryByType);
                setSummaryByTaskTypeDetails(wsSummaryByType);

                var summaryByPersonTitle = "Summary by Person";
                var wsSummaryByPerson = pck.Workbook.Worksheets.Add(summaryByPersonTitle);
                setHeader(wsSummaryByPerson, ETypeOfHeader.Department);
                setGridTitle(wsSummaryByPerson, summaryByPersonTitle);
                setSummaryByPersonHeader(wsSummaryByPerson);
                setSummaryByPersonDetails(wsSummaryByPerson);

                var detailTitle = "Detail";
                var wsDetail = pck.Workbook.Worksheets.Add(detailTitle);
                setHeader(wsDetail, ETypeOfHeader.Department);
                setGridTitle(wsDetail, detailTitle);
                populateDetail(wsDetail);

                autoFitColumns(wsSummaryByDate);
                autoFitColumns(wsSummaryByProject);
                autoFitColumns(wsSummaryByType);
                autoFitColumns(wsSummaryByProjectRole);
                autoFitColumns(wsSummaryByPerson);
                autoFitColumns(wsDetail);

                var maxWidth = 12.5;
                wsSummaryByDate.Column(1).Width = maxWidth;
                wsSummaryByProject.Column(1).Width = maxWidth;
                wsSummaryByType.Column(1).Width = maxWidth;
                wsSummaryByProjectRole.Column(1).Width = maxWidth;
                wsSummaryByPerson.Column(1).Width = maxWidth;
                wsDetail.Column(1).Width = maxWidth;

                wsSummaryByDate.Column(2).Width = maxWidth + 2.5;
                wsSummaryByProject.Column(2).Width = maxWidth + 2.5;
                wsSummaryByType.Column(2).Width = maxWidth + 2.5;
                wsSummaryByProjectRole.Column(2).Width = maxWidth + 2.5;
                wsSummaryByPerson.Column(2).Width = maxWidth + 2.5;
                wsDetail.Column(2).Width = maxWidth + 2.5;

                pck.Save();
            }
        }

        private void setSummaryByDateHeader(ExcelWorksheet ws)
        {
            setGridHeader(ws, "A10", "No.");
            setGridHeader(ws, "B10", "Date");
            setGridHeader(ws, "C10", "Member");
            setGridHeader(ws, "D10", "Hours");
            if(IsDisplayCost)
            {
                setGridHeader(ws, "E10", "Cost");
            }
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
                setValue(ws, "C" + offset, summary.Value.Members, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                setValue(ws, "D" + offset, summary.Value.Hours, "#,##0.00");
                if (IsDisplayCost)
                {
                    setValue(ws, "E" + offset, summary.Value.Cost, "#,##0.00");
                }
            }

            if (SummaryByDate.Count > 0)
            {
                sumFormula(ws,
                        string.Format("D{0}", offset + 1),
                        string.Format("D{0}", 11),
                        string.Format("D{0}", offset));

                if (IsDisplayCost)
                {
                    sumFormula(ws,
                            string.Format("E{0}", offset + 1),
                            string.Format("E{0}", 11),
                            string.Format("E{0}", offset));
                }
            }
        }

        private void setSummaryByProjectHeader(ExcelWorksheet ws)
        {
            setGridHeader(ws, "A10", "No.");
            setGridHeader(ws, "B10", "Project Code");
            setGridHeader(ws, "C10", "Phase");
            setGridHeader(ws, "D10", "Member");
            setGridHeader(ws, "E10", "Hours");
            setGridHeader(ws, "F10", "% Hours");
            if (IsDisplayCost)
            {
                setGridHeader(ws, "G10", "Cost");
                setGridHeader(ws, "H10", "% Cost");
            }
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
                setValue(ws, "D" + offset, summary.Value.Members, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                setValue(ws, "E" + offset, summary.Value.Hours, "#,##0.00");
                if (IsDisplayCost)
                {
                    setValue(ws, "G" + offset, summary.Value.Cost, "#,##0.00");
                }
            }

            var last_offset = offset + 1;
            index = 0;
            foreach (var summary in SummaryByProject)
            {
                ++index;
                offset = base_index + index;

                var e = ws.Cells["F" + offset];
                e.Style.Numberformat.Format = "#,##0.00";
                e.Style.Font.Name = FontNormal;
                e.Style.Font.Size = FontNormalSize;
                e.Formula = string.Format("=E{0}*100/$E${1}", offset, last_offset);
                e.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                if (IsDisplayCost)
                {
                    var g = ws.Cells["H" + offset];
                    g.Style.Numberformat.Format = "#,##0.00";
                    g.Style.Font.Name = FontNormal;
                    g.Style.Font.Size = FontNormalSize;
                    g.Formula = string.Format("=G{0}*100/$G${1}", offset, last_offset);
                    g.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                }
            }

            if (SummaryByProject.Count > 0)
            {
                sumFormula(ws,
                        string.Format("E{0}", offset + 1),
                        string.Format("E{0}", 11),
                        string.Format("E{0}", offset));

                sumFormula(ws,
                        string.Format("F{0}", offset + 1),
                        string.Format("F{0}", 11),
                        string.Format("F{0}", offset));

                if (IsDisplayCost)
                {
                    sumFormula(ws,
                        string.Format("G{0}", offset + 1),
                        string.Format("G{0}", 11),
                        string.Format("G{0}", offset));

                    sumFormula(ws,
                            string.Format("H{0}", offset + 1),
                            string.Format("H{0}", 11),
                            string.Format("H{0}", offset));
                }
            }
        }

        private void setSummaryByTaskTypeHeader(ExcelWorksheet ws)
        {
            setGridHeader(ws, "A10", "No.");
            setGridHeader(ws, "B10", "Project Code");
            setGridHeader(ws, "C10", "Type");
            setGridHeader(ws, "D10", "Member");
            setGridHeader(ws, "E10", "Hours");
            setGridHeader(ws, "F10", "% Hours");
            if (IsDisplayCost)
            {
                setGridHeader(ws, "G10", "Cost");
                setGridHeader(ws, "H10", "% Cost");
            }
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
                setValue(ws, "D" + offset, summary.Value.Members, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                setValue(ws, "E" + offset, summary.Value.Hours, "#,##0.00");
                if (IsDisplayCost)
                {
                    setValue(ws, "G" + offset, summary.Value.Cost, "#,##0.00");
                }
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

                if (IsDisplayCost)
                {
                    var g = ws.Cells["H" + offset];
                    g.Style.Numberformat.Format = "#,##0.00";
                    g.Style.Font.Name = FontNormal;
                    g.Style.Font.Size = FontNormalSize;
                    g.Formula = string.Format("=G{0}*100/$G${1}", offset, last_offset);
                    g.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                }
            }

            if (SummaryByTaskType.Count > 0)
            {
                //sumFormula(ws,
                //        string.Format("D{0}", offset + 1),
                //        string.Format("D{0}", 11),
                //        string.Format("D{0}", offset));

                sumFormula(ws,
                        string.Format("E{0}", offset + 1),
                        string.Format("E{0}", 11),
                        string.Format("E{0}", offset));

                sumFormula(ws,
                        string.Format("F{0}", offset + 1),
                        string.Format("F{0}", 11),
                        string.Format("F{0}", offset));

                if (IsDisplayCost)
                {
                    sumFormula(ws,
                            string.Format("G{0}", offset + 1),
                            string.Format("G{0}", 11),
                            string.Format("G{0}", offset));

                    sumFormula(ws,
                            string.Format("H{0}", offset + 1),
                            string.Format("H{0}", 11),
                            string.Format("H{0}", offset));
                }
            }
        }

        private void setSummaryByProjectRoleHeader(ExcelWorksheet ws)
        {
            setGridHeader(ws, "A10", "No.");
            setGridHeader(ws, "B10", "Project Role");
            setGridHeader(ws, "C10", "Member");
            setGridHeader(ws, "D10", "Hours");
            setGridHeader(ws, "E10", "% Hours");
            if (IsDisplayCost)
            {
                setGridHeader(ws, "F10", "Cost");
                setGridHeader(ws, "G10", "% Cost");
            }
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
                setValue(ws, "C" + offset, summary.Value.Members, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                setValue(ws, "D" + offset, summary.Value.Hours, "#,##0.00");
                if (IsDisplayCost)
                {
                    setValue(ws, "F" + offset, summary.Value.Cost, "#,##0.00");
                }
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

                if (IsDisplayCost)
                {
                    var g = ws.Cells["G" + offset];
                    g.Style.Numberformat.Format = "#,##0.00";
                    g.Style.Font.Name = FontNormal;
                    g.Style.Font.Size = FontNormalSize;
                    g.Formula = string.Format("=F{0}*100/$F${1}", offset, last_offset);
                    g.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                }
            }

            if (SummaryByProjectRole.Count > 0)
            {
                //sumFormula(ws,
                //        string.Format("C{0}", offset + 1),
                //        string.Format("C{0}", 11),
                //        string.Format("C{0}", offset));

                sumFormula(ws,
                        string.Format("D{0}", offset + 1),
                        string.Format("D{0}", 11),
                        string.Format("D{0}", offset));

                sumFormula(ws,
                        string.Format("E{0}", offset + 1),
                        string.Format("E{0}", 11),
                        string.Format("E{0}", offset));

                if (IsDisplayCost)
                {
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
        }

        private void setSummaryByPersonHeader(ExcelWorksheet ws)
        {
            setGridHeader(ws, "A10", "No.");
            setGridHeader(ws, "B10", "Employee ID");
            setGridHeader(ws, "C10", "Full Name");
            setGridHeader(ws, "D10", "Project Code");
            setGridHeader(ws, "E10", "Phase");
            setGridHeader(ws, "F10", "Project Role");
            setGridHeader(ws, "G10", "Hours");
            setGridHeader(ws, "H10", "% Hours");
            if (IsDisplayCost)
            {
                setGridHeader(ws, "I10", "Cost");
                setGridHeader(ws, "J10", "% Cost");
            }
        }
        private void setSummaryByPersonDetails(ExcelWorksheet ws)
        {
            var index = 0;
            var base_index = 10;
            var offset = base_index + index;
            //Populate Details
            foreach (var summary in SummaryByPerson)
            {
                ++index;
                offset = base_index + index;

                setValue(ws, "A" + offset, index, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                setValue(ws, "B" + offset, summary.Key.EmployeeID, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                setValue(ws, "C" + offset, summary.Value.FullName);
                setValue(ws, "D" + offset, summary.Key.ProjectCode, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                setValue(ws, "E" + offset, summary.Value.Phase);
                setValue(ws, "F" + offset, summary.Key.ProjectRole);
                setValue(ws, "G" + offset, summary.Value.Hours, "#,##0.00");
                if (IsDisplayCost)
                {
                    setValue(ws, "I" + offset, summary.Value.Cost, "#,##0.00");
                }
            }

            var last_offset = offset + 1;
            index = 0;
            foreach (var summary in SummaryByPerson)
            {
                ++index;
                offset = base_index + index;

                var e = ws.Cells["H" + offset];
                e.Style.Numberformat.Format = "#,##0.00";
                e.Style.Font.Name = FontNormal;
                e.Style.Font.Size = FontNormalSize;
                e.Formula = string.Format("=G{0}*100/$G${1}", offset, last_offset);
                e.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                if (IsDisplayCost)
                {
                    var g = ws.Cells["J" + offset];
                    g.Style.Numberformat.Format = "#,##0.00";
                    g.Style.Font.Name = FontNormal;
                    g.Style.Font.Size = FontNormalSize;
                    g.Formula = string.Format("=I{0}*100/$I${1}", offset, last_offset);
                    g.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                }
            }

            if (SummaryByPerson.Count > 0)
            {
                sumFormula(ws,
                        string.Format("G{0}", offset + 1),
                        string.Format("G{0}", 11),
                        string.Format("G{0}", offset));

                sumFormula(ws,
                        string.Format("H{0}", offset + 1),
                        string.Format("H{0}", 11),
                        string.Format("H{0}", offset));

                if (IsDisplayCost)
                {
                    sumFormula(ws,
                            string.Format("I{0}", offset + 1),
                            string.Format("I{0}", 11),
                            string.Format("I{0}", offset));

                    sumFormula(ws,
                            string.Format("J{0}", offset + 1),
                            string.Format("J{0}", 11),
                            string.Format("J{0}", offset));
                }
            }

        }

        private void populateDetail(ExcelWorksheet ws)
        {
            var index = -1;
            var base_index = 10;
            var offset = base_index + index;
            var index_detail = 0;

            var timesheetGroup = (from gm in Details
                                  group gm by new
                                  {
                                      gm.EmployeeID,
                                      gm.FullName,
                                  } into gmG
                                  select new
                                  {
                                      EmployeeID = gmG.Key.EmployeeID,
                                      FullName = gmG.Key.FullName,
                                      Timesheets = gmG.ToList()
                                  }).ToList();

            foreach (var byMember in timesheetGroup)
            {
                var details = from d in byMember.Timesheets
                              orderby d.Date, d.ProjectCode, d.PhaseOrder
                              select d;

                if (details.Count() > 0)
                {
                    // Header
                    ++index;
                    offset = base_index + index;
                    setLabel(ws, "A" + offset, "Employee ID:");
                    setValue(ws, "B" + offset, byMember.EmployeeID, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);

                    ++index;
                    offset = base_index + index;
                    setLabel(ws, "A" + offset, "Full Name:");
                    setValue(ws, "B" + offset, byMember.FullName, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);

                    ++index;
                    offset = base_index + index;

                    ++index;
                    offset = base_index + index;

                    setGridHeader(ws, "A" + offset, "No.");
                    setGridHeader(ws, "B" + offset, "Date");
                    setGridHeader(ws, "C" + offset, "Project Code");
                    setGridHeader(ws, "D" + offset, "Phase");
                    setGridHeader(ws, "E" + offset, "Type");
                    setGridHeader(ws, "F" + offset, "Main Task");
                    setGridHeader(ws, "G" + offset, "Sub Task");
                    setGridHeader(ws, "H" + offset, "Project Role");
                    if (IsDisplayCost)
                    {
                        setGridHeader(ws, "I" + offset, "Role Cost");
                        setGridHeader(ws, "J" + offset, "Hours");
                        setGridHeader(ws, "K" + offset, "Cost");
                    }
                    else
                    {
                        setGridHeader(ws, "I" + offset, "Hours");
                    }

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
                        setValue(ws, "C" + offset, timesheet.ProjectCode);
                        setValue(ws, "D" + offset, timesheet.Phase);
                        setValue(ws, "E" + offset, timesheet.TaskType, alignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                        setValue(ws, "F" + offset, timesheet.MainTask);
                        setValue(ws, "G" + offset, timesheet.SubTask);
                        setValue(ws, "H" + offset, timesheet.ProjectRole);
                        if (IsDisplayCost)
                        {
                            setValue(ws, "I" + offset, timesheet.RoleCost, format: "#,##0.00");
                            setValue(ws, "J" + offset, timesheet.Hours, format: "#,##0.00");
                            setValue(ws, "K" + offset, timesheet.Cost, format: "#,##0.00");
                        }
                        else
                        {
                            setValue(ws, "I" + offset, timesheet.Hours, format: "#,##0.00");
                        }

                        ++index;
                        offset = base_index + index;
                    }

                    if (IsDisplayCost)
                    {
                        sumFormula(ws,
                            string.Format("J{0}", offset),
                            string.Format("J{0}", first_row),
                            string.Format("J{0}", offset - 1));

                        sumFormula(ws,
                            string.Format("K{0}", offset),
                            string.Format("K{0}", first_row),
                            string.Format("K{0}", offset - 1));
                    }
                    else
                    {
                        sumFormula(ws,
                            string.Format("I{0}", offset),
                            string.Format("I{0}", first_row),
                            string.Format("I{0}", offset - 1));
                    }
                }
                ++index;
                offset = base_index + index;
            }
        }
    }
}
