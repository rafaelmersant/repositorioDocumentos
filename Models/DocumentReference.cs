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
    
    public partial class DocumentReference
    {
        public int Id { get; set; }
        public int DocumentHeaderId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
    
        public virtual User User { get; set; }
        public virtual DocumentHeader DocumentHeader { get; set; }
    }
}
