using DNXtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

using FooRecord = DNXtensions.Record<DnxSampleApi.FooType>;

namespace DnxSampleApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FooController : ControllerBase
    {
        private readonly ILogger<FooController> _logger;
        readonly FooContext _context;

        public FooController(ILogger<FooController> logger, FooContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IEnumerable<FooRecord> Get() => _context.Foos;

        [HttpPost]
        async public Task<FooRecord> Put(FooType newFoo)
        {
            var record = new FooRecord { Detail = newFoo };
            _context.Add(record);
            await _context.SaveChangesAsync();
            return record;
        }

        /// <summary>Updates a foo</summary>
        /// <param name="id">Id of the foo to update</param>
        /// <param name="updatedFoo" cref="DnxSampleApi.FooType">Partial foo with any property values to update</param>
        /// <returns>FooRecord</returns>
        [HttpPost("{id}")]
        async public Task<FooRecord> Post(int id, JsonElement updatedFoo)
        {
            var record = await _context.Foos.FindAsync(id);
            record.Modified = DateTimeOffset.UtcNow;
            var detail = record.Detail;
            record.Detail = updatedFoo.DeserializeInto(detail);
            await _context.SaveChangesAsync();
            return record;
        }
    }
}
