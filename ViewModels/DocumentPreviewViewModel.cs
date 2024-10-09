using RepositorioDocumentos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RepositorioDocumentos.ViewModels
{
    public class DocumentPreviewViewModel
    {
        public class DocumentContainer
        {
            public DocumentHeader DocumentHeader { get; set; }
            public DocumentDetail DocumentDetail { get; set; }
            public DocumentContent DocumentContent { get; set; }

            public List<DocumentGlossary> DocumentGlossary { get; set; }
            public List<DocumentGuideline> DocumentGuidelines { get; set; }
            public List<DocumentProcedure> DocumentProcedures { get; set; }
            public List<DocumentReference> DocumentReferences { get; set; }
            public List<DocumentChange> DocumentChanges { get; set; }
            public List<DocumentApproval> DocumentApprovals { get; set; }
        }
    }
}