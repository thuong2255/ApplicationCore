using System;

namespace SystemCore.Service.ViewModels.System
{
    public class PermissionVm
    {
        public int Id { get; set; }

        public Guid RoleId { get; set; }

        public string FunctionId { get; set; }

        public bool CanCreate { set; get; }

        public bool CanRead { set; get; }

        public bool CanUpdate { set; get; }

        public bool CanDelete { set; get; }

        public RoleVm Role { get; set; }

        public FunctionVm Function { get; set; }
    }
}