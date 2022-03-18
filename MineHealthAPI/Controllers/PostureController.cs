using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MineHealthAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MineHealthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostureController : ControllerBase
    {
        // GET

        private readonly ILogger<PostureController> _logger;
        public PostureController(ILogger<PostureController> logger)
        {
            _logger = logger;
        }


        [HttpGet("Neck")]
        public IEnumerable<Neck> GetNeckScore(string phone, string testdate)
        {
            string[] score = SQLHelper.GetPostureScore(SQLHelper.PostureCategory.SideNeck, phone, testdate);

            Neck neck = new Neck()
            {
                Phone = phone,
                TestDate = score[2],
                Score = score[0]
            };

            return new List<Neck>() { neck };
        }

        [HttpGet("FrontSpine")]
        public IEnumerable<FSpine> GetFSpineScore(string phone, string testdate)
        {
            string[] score = SQLHelper.GetPostureScore(SQLHelper.PostureCategory.FrontSpine, phone, testdate);
            FSpine fSpine = new FSpine()
            {
                Phone = phone,
                TestDate = score[2],
                Score = score[0]
            };
            return new List<FSpine>() { fSpine };
        }

        [HttpGet("SideSpine")]
        public IEnumerable<SSpine> GetSSPineScore(string phone, string testdate)
        {
            string[] score = SQLHelper.GetPostureScore(SQLHelper.PostureCategory.SideSpine, phone, testdate);

            SSpine sspine = new SSpine()
            {
                Phone = phone,
                TestDate = score[2],
                Score = score[0]
            };

            return new List<SSpine>() { sspine };
        }

        [HttpGet("All")]
        public IEnumerable<Posture> GetAllScore(string phone, string testdate)
        {
            string[] scoreSN = SQLHelper.GetPostureScore(SQLHelper.PostureCategory.SideNeck, phone, testdate);
            string[] scoreSS = SQLHelper.GetPostureScore(SQLHelper.PostureCategory.SideSpine, phone, testdate);
            string[] scoreFS = SQLHelper.GetPostureScore(SQLHelper.PostureCategory.FrontSpine, phone, testdate);

            Posture posture = new Posture()
            {
                Phone = phone,
                TestDate = scoreSN[2],
                NeckScore = scoreSN[0],
                SideSpineScore = scoreSS[0],
                FrontSpineScore = scoreFS[0],
            };


            return new List<Posture>() { posture };
        }
    }
}
