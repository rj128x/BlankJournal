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
    
    public partial class TBPInfoTable
    {
        public TBPInfoTable()
        {
            this.BPJournalTable = new HashSet<BPJournalTable>();
            this.TBPCommentsTable = new HashSet<TBPCommentsTable>();
            this.TBPHistoryTable = new HashSet<TBPHistoryTable>();
        }
    
        public string Number { get; set; }
        public int Folder { get; set; }
        public string Name { get; set; }
        public string DataPDF { get; set; }
        public string DataWord { get; set; }
    
        public virtual ICollection<BPJournalTable> BPJournalTable { get; set; }
        public virtual DataTable DataTable { get; set; }
        public virtual DataTable DataTable1 { get; set; }
        public virtual FoldersTable FoldersTable { get; set; }
        public virtual ICollection<TBPCommentsTable> TBPCommentsTable { get; set; }
        public virtual ICollection<TBPHistoryTable> TBPHistoryTable { get; set; }
    }
}