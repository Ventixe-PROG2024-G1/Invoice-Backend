using Invoice.Business.Models;
using Invoice.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController(IInvoiceServices _invoiceServices) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _invoiceServices.GetAllInvoicesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _invoiceServices.GetInvoiceByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateInvoice data)
        {
            if (!ModelState.IsValid)
                return BadRequest(data);

            var result = await _invoiceServices.CreateInvoiceAsync(data);
            return result ? Ok() : BadRequest();
        }
    }
}
