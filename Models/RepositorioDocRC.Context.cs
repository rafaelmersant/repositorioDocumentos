﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class RepositorioDocRCEntities : DbContext
    {
        public RepositorioDocRCEntities()
            : base("name=RepositorioDocRCEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<LoginHistory> LoginHistories { get; set; }
        public virtual DbSet<Directorate> Directorates { get; set; }
        public virtual DbSet<DocumentApproval> DocumentApprovals { get; set; }
        public virtual DbSet<DocumentChange> DocumentChanges { get; set; }
        public virtual DbSet<DocumentContent> DocumentContents { get; set; }
        public virtual DbSet<DocumentGlossary> DocumentGlossaries { get; set; }
        public virtual DbSet<DocumentGuideline> DocumentGuidelines { get; set; }
        public virtual DbSet<DocumentProcedure> DocumentProcedures { get; set; }
        public virtual DbSet<DocumentType> DocumentTypes { get; set; }
        public virtual DbSet<Macroprocess> Macroprocesses { get; set; }
        public virtual DbSet<Process> Processes { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<DocumentCode> DocumentCodes { get; set; }
        public virtual DbSet<DocumentHeader> DocumentHeaders { get; set; }
        public virtual DbSet<DocumentDetail> DocumentDetails { get; set; }
        public virtual DbSet<DocumentReference> DocumentReferences { get; set; }
    }
}
