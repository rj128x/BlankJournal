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
    
    public partial class UsersTable
    {
        public UsersTable()
        {
            this.BPJournalTable = new HashSet<BPJournalTable>();
            this.DataTable = new HashSet<DataTable>();
            this.TBPCommentsTable = new HashSet<TBPCommentsTable>();
            this.TBPCommentsTable1 = new HashSet<TBPCommentsTable>();
            this.TBPHistoryTable = new HashSet<TBPHistoryTable>();
        }
    
        public string Login { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public bool CanEditTBP { get; set; }
        public bool CanDoOper { get; set; }
        public bool CanCommentTBP { get; set; }
        public bool SendMailComments { get; set; }
        public bool CanEditUsers { get; set; }
        public string AvailEditFolders { get; set; }
        public bool Admin { get; set; }
    
        public virtual ICollection<BPJournalTable> BPJournalTable { get; set; }
        public virtual ICollection<DataTable> DataTable { get; set; }
        public virtual ICollection<TBPCommentsTable> TBPCommentsTable { get; set; }
        public virtual ICollection<TBPCommentsTable> TBPCommentsTable1 { get; set; }
        public virtual ICollection<TBPHistoryTable> TBPHistoryTable { get; set; }
    }
}
