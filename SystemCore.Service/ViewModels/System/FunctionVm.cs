using System.ComponentModel.DataAnnotations;
using SystemCore.Data.Enums;

namespace SystemCore.Service.ViewModels.System
{
    public class FunctionVm
    {
        public string Id { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { set; get; }

        [Required]
        [StringLength(250)]
        public string URL { set; get; }

        [StringLength(128)]
        public string ParentId { set; get; }

        public string IconCss { get; set; }

        public int SortOrder { set; get; }

        public Status Status { set; get; }
    }
}