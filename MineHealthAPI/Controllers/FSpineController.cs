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
    [ApiController]
    [Route("api/[controller]")]
    public class FSpineController : ControllerBase
    {
        // GET

        //private readonly ILogger<FSpineController> _logger;
        //public FSpineController(ILogger<FSpineController> logger)
        //{
        //    _logger = logger;
        //}

        //[HttpGet]
        //public IEnumerable<FSpine> GetScore(string phone, string testdate)
        //{
        //    string[] score = SQLHelper.GetPostureScore(SQLHelper.PostureCategory.FrontSpine, phone, testdate);

        //    FSpine fSpine = new FSpine()
        //    {
        //        Phone = phone,
        //        TestDate = score[2],
        //        Score = score[0]
        //    };

        //    return new List<FSpine>() { fSpine };
        //}

    }
}
