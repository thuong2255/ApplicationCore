using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SystemCore.Infrastructure.SharedKernel;

namespace SystemCore.Data.Entities
{
    [Table("Permissions")]
    public class Permission : DomainEntity<int>
    {
        public Permission() { }

        public Permission(Guid RoleId, string FunctionId, bool CanCreate, bool CanRead, bool CanUpdate, bool CanDelete)
        {
            this.RoleId = RoleId;
            this.FunctionId = FunctionId;
            this.CanCreate = CanCreate;
            this.CanRead = CanRead;
            this.CanUpdate = CanUpdate;
            this.CanDelete = CanDelete;
        }

        [Required]
        public Guid RoleId { get; set; }

        public string FunctionId { get; set; }

        public bool CanCreate { set; get; }
        public bool CanRead { set; get; }

        public bool CanUpdate { set; get; }
        public bool CanDelete { set; get; }

        [ForeignKey("RoleId")]
        public virtual AppRole AppRole { get; set; }

        [ForeignKey("FunctionId")]
        public virtual Function Function { get; set; }
    }
}