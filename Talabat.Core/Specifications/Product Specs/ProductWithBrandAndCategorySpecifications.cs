using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Product;

namespace Talabat.Core.Specifications.Product_Specs
{
    public class ProductWithBrandAndCategorySpecifications : BaseSpecifications<Product>
    {
        // This Ctor will be Used for Creating an Object, That Will be Used to Get All Products
        public ProductWithBrandAndCategorySpecifications(ProductSpecParams specParams)
             : base(P => 
                        (string.IsNullOrEmpty(specParams.Search) ||P.Name.ToLower().Contains(specParams.Search))
                        &&
                        (!specParams.BrandId.HasValue    || P.BrandId == specParams.BrandId.Value) 
                        &&
                        (!specParams.CategoryId.HasValue || P.CategoryId == specParams.CategoryId.Value)
             )
        {
            //ApplyProduct();
            Includes.Add(P => P.Brand);
            Includes.Add(P => P.Category);

            if(!string.IsNullOrEmpty(specParams.Sort))
            {
                switch (specParams.Sort)
                {
                    case "priceAsc":
                        //OrderBy = P => P.Price;
                        AddOrderBy ( P => P.Price);
                        break;
                    case "priceDesc":
                        //OrderByDesc = P => P.Price;
                        AddOrderByDesc(P => P.Price);
                        break;
                    default:
                        OrderBy = P => P.Name;
                        break;
                }
            }

            // In case 'sort' Is Null Or Empty !!
            else 
                //OrderBy = P => P.Name;  
                AddOrderBy( P => P.Name);

            // Products = 18 ~ 20 
            // PageSize = 5
            // PageIndex = 4

            

            ApplyPagination((specParams.PageIndex - 1) * (specParams.PageSize), specParams.PageSize);
        }

       // This Constructor will be Used for Creating an Object, That Will be Used to GetProductById
        public ProductWithBrandAndCategorySpecifications(int id)
                :base(P => P.Id == id)
        {
            // ApplyProduct();
            Includes.Add(P => P.Brand);
            Includes.Add(P => P.Category);
            
        }

      //public void ApplyProduct()
      //{
      //    Includes.Add(P => P.Brand);
      //    Includes.Add(P => P.Category);
      //}


    }
}
