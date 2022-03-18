using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MineHealthAPI.Models;

namespace MineHealthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnxietyController : ControllerBase
    {
        // GET

        //private readonly ILogger<AnxietyController> _logger;
        //public AnxietyController(ILogger<AnxietyController> logger)
        //{
        //    _logger = logger;
        //}

        //[HttpGet]
        //public IEnumerable<Anxiety> GetNeckScore(string phone, string testdate)
        //{
        //    string[] score = SQLHelper.GetEmotionScore(SQLHelper.EmotionCategory.Anxeity, phone, testdate);

        //    Anxiety anxiety = new Anxiety()
        //    {
        //        Phone = phone,
        //        TestDate = score[2],
        //        Score = score[0]
        //    };

        //    return new List<Anxiety>() { anxiety };
        //}
    }
}
