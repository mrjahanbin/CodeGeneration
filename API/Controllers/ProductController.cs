using Microsoft.AspNetCore.Mvc;
using CodeGeneration.Domains;
using CodeGeneration.UnitOfWork;

namespace CodeGeneration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var items = _unitOfWork.Products.GetAll();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = _unitOfWork.Products.GetById(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public IActionResult Add(Product item)
        {
            _unitOfWork.Products.Add(item);
            _unitOfWork.Complete();
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Product item)
        {
            if (id != item.Id) return BadRequest();
            _unitOfWork.Products.Update(item);
            _unitOfWork.Complete();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _unitOfWork.Products.Delete(id);
            _unitOfWork.Complete();
            return NoContent();
        }
    }
}
