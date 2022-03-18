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
    public class SSpineController : ControllerBase
    {
        // GET
        //private readonly ILogger<SSpineController> _logger;
        //public SSpineController(ILogger<SSpineController> logger)
        //{
        //    _logger = logger;
        //}

        //[HttpGet]
        //public IEnumerable<SSpine> GetNeckScore(string phone, string testdate)
        //{
        //    string[] score = SQLHelper.GetPostureScore(SQLHelper.PostureCategory.SideSpine, phone, testdate);

        //    SSpine sspine = new SSpine()
        //    {
        //        Phone = phone,
        //        TestDate = score[2],
        //        Score = score[0]
        //    };

        //    return new List<SSpine>() { sspine };
        //}

    }
}
