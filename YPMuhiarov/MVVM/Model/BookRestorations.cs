//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace YPMuhiarov.MVVM.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class BookRestorations
    {
        public int RestorationId { get; set; }
        public Nullable<int> BookId { get; set; }
        public string Reason { get; set; }
        public System.DateTime StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public string Executor { get; set; }
        public string Status { get; set; }
    
        public virtual Books Books { get; set; }
    }
}
