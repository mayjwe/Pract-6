//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Pract_3.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Ordedrs
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Ordedrs()
        {
            this.Queue = new HashSet<Queue>();
        }
    
        public int ID_Orders { get; set; }
        public int ID_Staff { get; set; }
        public int ID_Clients { get; set; }
        public Nullable<int> ID_Models { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public string Cheque { get; set; }
        public string Warranty { get; set; }
    
        public virtual Clients Clients { get; set; }
        public virtual Models Models { get; set; }
        public virtual Staff Staff { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Queue> Queue { get; set; }
    }
}
