using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PJ_CWN019.TM.Web.Models
{
    public class CumulativeItemByDateView
    {
        public int Group { get; set; }
        public int Count { get; set; }
        public long ID { get; set; }
        public string Date { get; set; }
        public decimal PreSale { get; set; }
        public decimal Implement { get; set; }
        public decimal Warranty { get; set; }
        public decimal Operation { get; set; }

    }
}