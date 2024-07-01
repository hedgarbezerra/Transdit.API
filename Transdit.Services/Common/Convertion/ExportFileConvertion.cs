using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Contracts;
using Transdit.Core.Domain;
using Transdit.Core.Models.Enums;
using Transdit.Core.Models.Transcription;

namespace Transdit.Services.Common.Convertion
{
    public partial class ExportFileConvertion : IFileConverter
    {
        private readonly ExportFileConverter _pdf;
        private readonly ExportFileConverter _docx;
        private readonly ExportFileConverter _txt;
        public ExportFileConvertion(string fontsPath, IEmailFromFileGenerator mailGenerator)
        {
            try
            {
                var exportPath = Path.Combine(fontsPath, "Temp", "Export");
                Directory.CreateDirectory(exportPath);
            }
            catch{ }

            _pdf = new Pdf(mailGenerator);
            _docx = new Docx(fontsPath);
            _txt = new Txt();
        }

        public MemoryStream Convert(string content, EFileConvertionTarget format, bool detailed = false)
        {
            switch (format)
            {
                case EFileConvertionTarget.PDF:
                    return _pdf.Convert(content);
                case EFileConvertionTarget.TXT:
                    return _txt.Convert(content);
                case EFileConvertionTarget.DOCX:
                    return _docx.Convert(content);
                default:
                    return _txt.Convert(content);
            }
        }

        public MemoryStream Convert(TranscriptionOperationResult transcription, EFileConvertionTarget format, bool detailed = false)
        {
            switch (format)
            {
                case EFileConvertionTarget.PDF:
                    return _pdf.Convert(transcription, detailed);
                case EFileConvertionTarget.TXT:
                    return _txt.Convert(transcription, detailed);
                case EFileConvertionTarget.DOCX:
                    return _docx.Convert(transcription, detailed);
                default:
                    return _txt.Convert(transcription, detailed);
            }
        }
    }
}
