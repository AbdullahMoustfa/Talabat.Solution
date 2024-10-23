using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Controllers;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Entities.Product;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Specifications.Product_Specs;

namespace TalabatPractising.APIs.Controllers
{

    public class ProductsController : BaseApiController

    {
        //private readonly IGenericRepositories<Product> _productRepo;
        //private readonly IGenericRepositories<ProductBrand> _brandsRepo;
        //private readonly IGenericRepositories<ProductCategory> _categoriesRepo;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ProductsController(IMapper mapper,
            IUnitOfWork unitOfWork
            //IGenericRepositories<Product> productRepo,
            //IGenericRepositories<ProductBrand> brandsRepo,
            //IGenericRepositories<ProductCategory> categoriesRepo
            )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            //_productRepo = productRepo;
            //_brandsRepo = brandsRepo;
            //_categoriesRepo = categoriesRepo;
        }

        [CashedAttribute(600)]
       // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]    //GET: /api/Products
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery] ProductSpecParams specParams)
        {
            var spec = new ProductWithBrandAndCategorySpecifications(specParams);

            var products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);

            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

            var CountSpec = new ProductWithFiltrationForCountAsync(specParams);

            var Count = await _unitOfWork.Repository<Product>().GetCountWithSpecAsync(CountSpec);

            return Ok(new Pagination<ProductToReturnDto>(specParams.PageIndex, specParams.PageSize, data, Count));
        }

		[CashedAttribute(600)]
		[ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]    // BaseUrl/api/Products/1
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var spec = new ProductWithBrandAndCategorySpecifications(id);   

            var products = await _unitOfWork.Repository<Product>().GetEntityWithSpecAsync(spec);
            if (products is null)
                return NotFound(new ApiResponse(404));

            var MappedProduct = _mapper.Map<Product, ProductToReturnDto>(products);
            return Ok(MappedProduct);
        }


		[CashedAttribute(600)]
		[HttpGet("brands")] // Get: /api/products/brands
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            var brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();   

            return Ok(brands);
        }

		[CashedAttribute(600)]
		[HttpGet("categories")]  // Get: /api/products/categories
        public async Task<ActionResult<IReadOnlyList<ProductCategory>>> GetCategories()
        {
            var categories = await _unitOfWork.Repository<ProductCategory>().GetAllAsync();

            return Ok(categories);
        }
    }
}
