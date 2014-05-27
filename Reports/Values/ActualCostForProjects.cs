using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.Reports.Values
{
    public class ActualCostForProjects
        : MemberDetail
    {
        public ActualCostForProjects()
        {
            Title = "Actual Cost for Project";
            ActualCostForProjectList = new List<ActualCostForProject>();
        }

        public List<ActualCostForProject> ActualCostForProjectList { get; set; }

        public void WriteExcel(string path)
        {
            var newFile = new FileInfo(path);
            if (newFile.Exists)
            {
                newFile.Delete();
            }

            using (var pck = new ExcelPackage(newFile))
            {
                int offset = 5;
    
                // Add the Content sheet
                var summaryByPhaseTitle = "Summary by Phase";
                var wsSummaryByPhase = pck.Workbook.Worksheets.Add(summaryByPhaseTitle);
                setHeader(wsSummaryByPhase, ETypeOfHeader.AllProject);
                foreach (var summary in ActualCostForProjectList)
                {
                    
                    offset = summary.WriteSummaryHeader(wsSummaryByPhase, offset);
                    ++offset;
                    ++offset;
                    setGridTitle(wsSummaryByPhase, summaryByPhaseTitle, cell: "A" + offset);
                    ++offset;
                    ++offset;
                    //write detail
                    offset = summary.WriteSummaryByPhaseDetails(wsSummaryByPhase, offset);

                    ++offset;
                    ++offset;
                }

                offset = 5;
                var summaryByTaskTypeTitle = "Summary by Type of Phase";
                var wsSummaryByType = pck.Workbook.Worksheets.Add(summaryByTaskTypeTitle);
                setHeader(wsSummaryByType, ETypeOfHeader.AllProject);
                foreach (var summary in ActualCostForProjectList)
                {
                    offset = summary.WriteSummaryHeader(wsSummaryByType, offset);
                    ++offset;
                    ++offset;
                    setGridTitle(wsSummaryByType, summaryByTaskTypeTitle, cell: "A" + offset);
                    ++offset;
                    ++offset;
                    //write detail
                    offset = summary.WriteSummaryByTaskTypeDetails(wsSummaryByType, offset);

                    ++offset;
                    ++offset;
                }

                offset = 5;
                var summaryByWeekTitle = "Summary by Week";
                var wsSummaryByWeek = pck.Workbook.Worksheets.Add(summaryByWeekTitle);
                setHeader(wsSummaryByWeek, ETypeOfHeader.AllProject);
                foreach (var summary in ActualCostForProjectList)
                {
                    offset = summary.WriteSummaryHeader(wsSummaryByWeek, offset);
                    ++offset;
                    ++offset;
                    setGridTitle(wsSummaryByWeek, summaryByWeekTitle, cell: "A" + offset);
                    ++offset;
                    ++offset;
                    //write detail
                    offset = summary.WriteSummaryByWeekDetails(wsSummaryByWeek, offset);

                    ++offset;
                    ++offset;
                }

                offset = 5;
                var summaryByProjectRoleTitle = "Summary by Project Role";
                var wsSummaryByProjectRole = pck.Workbook.Worksheets.Add(summaryByProjectRoleTitle);
                setHeader(wsSummaryByProjectRole, ETypeOfHeader.AllProject);
                foreach (var summary in ActualCostForProjectList)
                {
                    offset = summary.WriteSummaryHeader(wsSummaryByProjectRole, offset);
                    ++offset;
                    ++offset;
                    setGridTitle(wsSummaryByProjectRole, summaryByProjectRoleTitle, cell: "A" + offset);
                    ++offset;
                    ++offset;
                    //write detail
                    offset = summary.WriteSummaryByProjectRoleDetails(wsSummaryByProjectRole, offset);

                    ++offset;
                    ++offset;
                }

                offset = 5;
                var detailTitle = "Detail";
                var wsDetail = pck.Workbook.Worksheets.Add(detailTitle);
                setHeader(wsDetail, ETypeOfHeader.AllProject);
                foreach (var summary in ActualCostForProjectList)
                {
                    offset = summary.WriteSummaryHeader(wsDetail, offset);
                    ++offset;
                    ++offset;
                    setGridTitle(wsDetail, detailTitle, cell: "A" + offset);
                    ++offset;
                    ++offset;
                    //write detail
                    offset = summary.PopulateDetail(wsDetail, offset);

                    ++offset;
                    ++offset;
                }

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

                wsSummaryByPhase.Column(2).Width = maxWidth + 15;
                wsSummaryByType.Column(2).Width = maxWidth + 15;
                wsSummaryByWeek.Column(2).Width = maxWidth + 10;
                wsSummaryByProjectRole.Column(2).Width = maxWidth;
                wsDetail.Column(2).Width = maxWidth;

                pck.Save();
            }
        }
    }
}
