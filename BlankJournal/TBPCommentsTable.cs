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
    
    public partial class TBPCommentsTable
    {
        public string Id { get; set; }
        public string TBPNumber { get; set; }
        public string Author { get; set; }
        public string Comment { get; set; }
        public string WordData { get; set; }
        public string Performer { get; set; }
        public string CommentPerform { get; set; }
        public System.DateTime DateCreate { get; set; }
        public Nullable<System.DateTime> DatePerform { get; set; }
    
        public virtual DataTable DataTable { get; set; }
        public virtual TBPInfoTable TBPInfoTable { get; set; }
        public virtual UsersTable UsersTable { get; set; }
        public virtual UsersTable UsersTable1 { get; set; }
    }
}
