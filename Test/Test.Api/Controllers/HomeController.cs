using Amazon.Runtime.Internal.Util;
using Common.Application;
using Common.Domain;
using Common.Domain.Repository;
using Microsoft.AspNetCore.Mvc;
using ILogger = Amazon.Runtime.Internal.Util.ILogger;


namespace Test.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ITestRepository _repository;

        public HomeController(ITestRepository repository)
        {
            _repository = repository;
        }

        // GET: api/<HomeController>
        [HttpGet]
        public IActionResult Get()
        {
            _repository.AddAsync(new Test()
            {
                Title = "rtrtr",
                Cost = 5000
            });
            return null;
        }

    }

    public class Test : BaseEntity
    {
        public string Title { get; set; }
        public int Cost { get; set; }
    }

    public class TestRepository : DapperBaseRepository<Test> , ITestRepository
    {
        public TestRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }

    public interface ITestRepository : IDapperBaseRepository<Test>
    {
    }



}
