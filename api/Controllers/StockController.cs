
using api.Data;
using Microsoft.AspNetCore.Mvc;
using api.Mappers;
using api.Dtos.Stock;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public StockController(ApplicationDBContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var stocksTask = await _context.Stocks.ToListAsync();
            var stocks = stocksTask.Select(s => s.ToStockDto());
            return Ok(stocks);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                return NotFound();
            }
            return Ok(stock.ToStockDto());
        }
        [HttpPost]
        public async Task<IActionResult> create([FromBody] createStockDto stockDto)
        {
            var stock = stockDto.toStockFromCreateDto();
            await _context.AddAsync(stock);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = stock.Id }, stock);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> update([FromRoute] int id, [FromBody] updateStockDto stockDto)
        {
            var stock = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);
            if (stock == null)
            {
                return NotFound();
            }
            stock.Symbol = stockDto.Symbol;
            stock.CompanyName = stockDto.CompanyName;
            stock.Purchase = stockDto.Purchase;
            stock.LastDiv = stockDto.LastDiv;
            stock.Industry = stockDto.Industry;
            stock.MarketCap = stockDto.MarketCap;

            await _context.SaveChangesAsync();
            return Ok(stock.ToStockDto());

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> delete([FromRoute] int id)
        {
            var stock = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);
            if (stock == null)
            {
                return NotFound();
            }
            _context.Stocks.Remove(stock);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}