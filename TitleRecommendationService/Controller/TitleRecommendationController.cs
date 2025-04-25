using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TitleRecommendationService.DbModels;
using TitleRecommendationService.Models;
using System.Linq;

namespace TitleRecommendationService.Controllers;

[ApiController]
[Route("[controller]")]
public class TitleRecommendationController : Controller
{
	private readonly ApplicationDbContext _context;
	public TitleRecommendationController(ApplicationDbContext context)
	{
		_context = context;
	}
	[HttpGet("test")]
	public string Test()
	{
		var item = _context.TitleRatings.Find(1); // »щем по idSearch
		return item?.IdTitle.ToString() ?? "таблица не найдена или запись пуста€";
	}

	[HttpGet("top")]
	public async Task<IActionResult> GetTopTitles(int limit = 6)
	{
		var topAnime = await _context.TitleRatings
			.OrderByDescending(t => t.Rating)
			.ThenByDescending(t => t.CountReviews)
			.Take(limit)
			.ToListAsync();
		return Ok(topAnime);
	}

	[HttpPost("addNewTitle")]
	public async Task<IActionResult> AddNewTitle([FromBody] int idTitle)
	{
		var newTitle = new TitleRating
		{
			IdTitle = idTitle,
			Rating = null,
			CountReviews = 0
		};
		_context.TitleRatings.Add(newTitle);
		await _context.SaveChangesAsync();

		return Ok();
	}

	[HttpPost("addReview")]
	public async Task<IActionResult> AddReview([FromBody] ReviewDTO reviewDto)
	{
        var titleRating = await _context.TitleRatings
        .FirstOrDefaultAsync(tr => tr.IdTitle == reviewDto.IdTitle);

        if (titleRating == null)
        {
            titleRating = new TitleRating 
			{ 
				IdTitle = reviewDto.IdTitle,
				Rating = 0,
				CountReviews = 0
			};
        }

        titleRating.CountReviews += 1;
        titleRating.Rating = (titleRating.Rating * (titleRating.CountReviews - 1) + reviewDto.Rating)
            / titleRating.CountReviews;

        await _context.SaveChangesAsync();

        return Ok();
    }
}
