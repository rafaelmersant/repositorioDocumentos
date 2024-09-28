//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RepositorioDocumentos.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DocumentHeader
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DocumentHeader()
        {
            this.DocumentApprovals = new HashSet<DocumentApproval>();
            this.DocumentChanges = new HashSet<DocumentChange>();
            this.DocumentContents = new HashSet<DocumentContent>();
            this.DocumentDetails = new HashSet<DocumentDetail>();
            this.DocumentGlossaries = new HashSet<DocumentGlossary>();
            this.DocumentGuidelines = new HashSet<DocumentGuideline>();
            this.DocumentProcedures = new HashSet<DocumentProcedure>();
            this.DocumentReferences = new HashSet<DocumentReference>();
            this.DocumentPermissions = new HashSet<DocumentPermission>();
        }
    
        public int Id { get; set; }
        public int DocumentTypeId { get; set; }
        public string Status { get; set; }
        public string Image { get; set; }
        public string Code { get; set; }
        public short Revision { get; set; }
        public System.DateTime Date { get; set; }
        public string Title { get; set; }
        public short DirectorateId { get; set; }
        public int DepartmentCode { get; set; }
        public short AreaId { get; set; }
        public int MacroprocessId { get; set; }
        public int ProcessId { get; set; }
        public string Objective { get; set; }
        public Nullable<bool> AttachmentType { get; set; }
        public bool IsPublic { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
    
        public virtual Area Area { get; set; }
        public virtual Department Department { get; set; }
        public virtual Directorate Directorate { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentApproval> DocumentApprovals { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentChange> DocumentChanges { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentContent> DocumentContents { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentDetail> DocumentDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentGlossary> DocumentGlossaries { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentGuideline> DocumentGuidelines { get; set; }
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentProcedure> DocumentProcedures { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentReference> DocumentReferences { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentPermission> DocumentPermissions { get; set; }
        public virtual DocumentType DocumentType { get; set; }
        public virtual Macroprocess Macroprocess { get; set; }
        public virtual Process Process { get; set; }
    }
}
