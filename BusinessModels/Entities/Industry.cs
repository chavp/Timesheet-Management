using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwn.PM.BusinessModels.Entities
{
    /// <summary>
    /// Detailed structure and explanatory notes ISIC Rev.4
    /// -- http://unstats.un.org/unsd/cr/registry/regcst.asp?Cl=27
    /// กระทรวงพานิชย์: file:///C:/Users/admin/Downloads/book_business_man.pdf
    /// </summary>
    public class Industry : Entity
    {
        protected Industry() { }
        public Industry(string code, string name)
            : this()
        {
            Code = code;
            Name = name;
        }

        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual string NameTH { get; set; }
    }
}
