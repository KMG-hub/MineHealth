using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MineHealthAPI.Models;
using Microsoft.Extensions.Logging;

namespace MineHealthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepressedController : ControllerBase
    {
        //// GET

        //private readonly ILogger<DepressedController> _logger;
        //public DepressedController(ILogger<DepressedController> logger)
        //{
        //    _logger = logger;
        //}

        //[HttpGet]
        //public IEnumerable<Depressed> GetNeckScore(string phone, string testdate)
        //{
        //    string[] score = SQLHelper.GetEmotionScore(SQLHelper.EmotionCategory.Despressed, phone, testdate);

        //    Depressed depressed = new Depressed()
        //    {
        //        Phone = phone,
        //        TestDate = score[2],
        //        Score = score[0]
        //    };

        //    return new List<Depressed>() { depressed };
        //}
    }
}
