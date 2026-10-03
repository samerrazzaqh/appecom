using AppProject.API.Helper;
using AutoMapper;
using Core.DTO;
using Core.interfaces;
using Core.Services;
using Core.Sharing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppProject.API.Controllers
{
    [Authorize]
    public class ProductsController : BaseController
    {
        private readonly IImageManagementService service;

        public ProductsController(IUnitOfWork work, IMapper mapper, IImageManagementService service) : base(work, mapper)
        {
            this.service = service;
        }
        
        [AllowAnonymous]
        [HttpGet("get-all")]
        public async Task<IActionResult> get([FromQuery] ProductParams productParams)
        {
            try
            {
                var Product = await work.productRepositry
                    .GetAllAsync(productParams);

                return Ok(new Pagination<ProductDTO>(productParams.PageNumber, productParams.pageSize, productParams.TotatlCount, Product));
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> getById(int id)
        {
            try
            {
                var product = await work.productRepositry.GetByIdAsync(id,
                    x => x.Category, x => x.Photos);

                var result = mapper.Map<ProductDTO>(product);

                if (product is null) return BadRequest(new ResponseAPI(400));


                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [HttpPost("Add-Product")]
        public async Task<IActionResult> add(AddProductDTO productDTO)
        {
            try
            {
                await work.productRepositry.AddAsync(productDTO);
                return Ok(new ResponseAPI(200));
            }
            catch (Exception ex)
            {

                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }
        [HttpPut("Update-Product")]
        public async Task<IActionResult> Update(UpdateProductDTO updateProductDTO)
        {
            try
            {
                await work.productRepositry.UpdateAsync(updateProductDTO);
                return Ok(new ResponseAPI(200));
            }
            catch (Exception ex)
            {

                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }
        [HttpDelete("Delete-Product/{Id}")]
        public async Task<IActionResult> delete(int Id)
        {
            try
            {
                var product = await work.productRepositry
                    .GetByIdAsync(Id, x => x.Photos, x => x.Category);

                await work.productRepositry.DeleteAsync(product);

                return Ok(new ResponseAPI(200));

            }
            catch (Exception ex)
            {

                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }
    }
}
