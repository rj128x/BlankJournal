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
    
    public partial class FoldersTable
    {
        public FoldersTable()
        {
            this.TBPInfoTable = new HashSet<TBPInfoTable>();
        }
    
        public string Id { get; set; }
        public string Name { get; set; }
        public int Ident { get; set; }
        public string Zone { get; set; }
    
        public virtual ICollection<TBPInfoTable> TBPInfoTable { get; set; }
    }
}
