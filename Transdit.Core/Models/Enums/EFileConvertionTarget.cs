using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transdit.Core.Models.Enums
{
    public enum EFileConvertionTarget
    {
        [Description("Pdf")]
        PDF = 1,
        [Description("Txt")]
        TXT = 2,
        [Description("Docx")]
        DOCX = 3,
    }
}
