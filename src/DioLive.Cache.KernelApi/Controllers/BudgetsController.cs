using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.KernelApi.Controllers
{
	[Route("api/[controller]")]
	public class BudgetsController : ControllerBase
	{
		// GET: api/Budgets
		[HttpGet]
		public IEnumerable<string> Get()
		{
			return new[] { "value1", "value2" };
		}

		// GET: api/Budgets/5
		[HttpGet("{id}", Name = "Get")]
		public string Get(int id)
		{
			return "value";
		}

		// POST: api/Budgets
		[HttpPost]
		public void Post([FromBody] string value)
		{
		}

		// PUT: api/Budgets/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE: api/ApiWithActions/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
	}
}