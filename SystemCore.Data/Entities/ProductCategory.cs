using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using SystemCore.Data.Enums;
using SystemCore.Data.Interfaces;
using SystemCore.Infrastructure.SharedKernel;

namespace SystemCore.Data.Entities
{
    [Table("ProductCategories")]
    public class ProductCategory : DomainEntity<int>, IDateTracking, IHasSeoMetaData, ISwitchable, ISortable
    {
        public ProductCategory()
        {
            Products = new List<Product>();
        }

        public ProductCategory (
            string Name, string Description, int? ParentId, 
            int? HomeOrder, string Image, bool? HomeFlag, DateTime DateCreated, 
            DateTime DateModified, string SeoPageTitle,
            string SeoAlias, string SeoKeywords, string SeoDescription, Status Status, int SortOrder 
            )
        {
            this.Name = Name;
            this.Description = Description;
            this.ParentId = ParentId;
            this.HomeFlag = HomeFlag;
            this.HomeOrder = HomeOrder;
            this.Image = Image;
            this.DateCreated = DateCreated;
            this.DateModified = DateModified;
            this.SeoPageTitle = SeoPageTitle;
            this.SeoAlias = SeoAlias;
            this.SeoKeywords = SeoKeywords;
            this.SeoDescription = SeoDescription;
            this.Status = Status;
            this.SortOrder = SortOrder;
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public int? ParentId { get; set; }

        public int? HomeOrder { get; set; }

        public string Image { get; set; }

        public bool? HomeFlag { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public string SeoPageTitle { get; set; }

        public string SeoAlias { get; set; }

        public string SeoKeywords { get; set; }

        public string SeoDescription { get; set; }

        public Status Status { get; set; }

        public int SortOrder { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
