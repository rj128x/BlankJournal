//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BlankJournal
{
    using System;
    using System.Collections.Generic;
    
    public partial class BPJournalTable
    {
        public string Id { get; set; }
        public double Number { get; set; }
        public string TBPNumber { get; set; }
        public string Name { get; set; }
        public System.DateTime DateCreate { get; set; }
        public string Author { get; set; }
        public System.DateTime DateStart { get; set; }
        public System.DateTime DateEnd { get; set; }
        public string Comment { get; set; }
        public string WordData { get; set; }
        public string PDFData { get; set; }
        public bool isOBP { get; set; }
    
        public virtual DataTable DataTable { get; set; }
        public virtual DataTable DataTable1 { get; set; }
        public virtual TBPInfoTable TBPInfoTable { get; set; }
        public virtual UsersTable UsersTable { get; set; }
    }
}
