using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Repository.Data;
using TalabatPractising.APIs.Controllers;

namespace Talabat.APIs.Controllers
{

    public class BuggyController : BaseApiController
    {
        private readonly StoreContext _dbContext;

        public BuggyController(StoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("notfound")] // Get : api/Buggy/notfound
        public ActionResult GetNotFoundRequest()
        {
            var product = _dbContext.Products.Find(100);
            if(product is null)
                return NotFound(new ApiResponse(404));

            return Ok(product); 
        }

        [HttpGet("servererror")] // Get : api/Buggy/servererror
        public ActionResult GetServerRequest()  
        {
            var product = _dbContext.Products.Find(100);

            var productToReturn = product.ToString();  // Will Throw Exception [NullReferenceException
        
            return Ok(productToReturn);
        }

        [HttpGet("badrequest")] // Get : api/Buggy/badrequest
        public ActionResult GetBadRequest()
        {
            return BadRequest(new ApiResponse(400));
        }

        [HttpGet("badrequest/{id}")] // Get : api/Buggy/badrequest/five
        public ActionResult GetValidationError(int id) // Validation Error
        {
            return Ok();
        }

        [HttpGet("unauthorized")] // Get : api/Buggy/unauthorized
        public ActionResult GetUnauthorizedError()
        {
            return Unauthorized(new ApiResponse(401));
        }
    }
}
