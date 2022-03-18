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
    public class NeckController : ControllerBase
    {
        //// GET

        //private readonly ILogger<NeckController> _logger;
        //public NeckController(ILogger<NeckController> logger)
        //{
        //    _logger = logger;
        //}

        //[HttpGet]
        //public IEnumerable<Neck> GetNeckScore(string phone, string testdate)
        //{
        //    string[] score = SQLHelper.GetPostureScore(SQLHelper.PostureCategory.SideNeck, phone, testdate);

        //    Neck neck = new Neck()
        //    {
        //        Phone = phone, TestDate = score[2], Score = score[0]
        //    };

        //    return new List<Neck>() { neck };
        //}
        
    }
}
