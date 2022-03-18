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
    /// <summary>
    /// 스트레스 점수 조회
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class StressController : ControllerBase
    {
        //private readonly ILogger<StressController> _logger;
        //public StressController(ILogger<StressController> logger)
        //{
        //    _logger = logger;
        //}

        //[HttpGet]
        //public IEnumerable<Stress> GetNeckScore(string phone, string testdate)
        //{
        //    string[] score = SQLHelper.GetEmotionScore(SQLHelper.EmotionCategory.Stress, phone, testdate);

        //    Stress stress = new Stress()
        //    {
        //        Phone = phone,
        //        TestDate = score[2],
        //        Score = score[0]
        //    };

        //    return new List<Stress>() { stress };
        //}
    }
}
