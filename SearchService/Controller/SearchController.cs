using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SearchService.Controllers;

[ApiController]
[Route("[controller]")]
public class SearchController : Controller
{
    private readonly ApplicationDbContext _contex;
    public SearchController(ApplicationDbContext context)
    {
        _contex = context;
    }
    [HttpGet("test")]
    public string Test()
    {
        var item = _contex.SearchTable.Find(1); // »щем по idSearch
        return item?.AnimeTag ?? "таблица не найдена или запись пуста€";
    }
}
