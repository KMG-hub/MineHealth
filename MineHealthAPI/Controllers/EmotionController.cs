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
    public class EmotionController : ControllerBase
    {
        // GET

        private readonly ILogger<EmotionController> _logger;
        public EmotionController(ILogger<EmotionController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Depressed")]
        public IEnumerable<Depressed> GetDepressedScore(string phone, string testdate)
        {
            string[] score = SQLHelper.GetEmotionScore(SQLHelper.EmotionCategory.Despressed, phone, testdate);

            Depressed depressed = new Depressed()
            {
                Phone = phone,
                TestDate = score[2],
                Score = score[0]
            };

            return new List<Depressed>() { depressed };
        }

        [HttpGet("Anxiety")]
        public IEnumerable<Anxiety> GetAnxietyScore(string phone, string testdate)
        {
            string[] score = SQLHelper.GetEmotionScore(SQLHelper.EmotionCategory.Anxeity, phone, testdate);

            Anxiety anxiety = new Anxiety()
            {
                Phone = phone,
                TestDate = score[2],
                Score = score[0]
            };

            return new List<Anxiety>() { anxiety };
        }

        [HttpGet("Stress")]
        public IEnumerable<Stress> GetStressScore(string phone, string testdate)
        {
            string[] score = SQLHelper.GetEmotionScore(SQLHelper.EmotionCategory.Stress, phone, testdate);

            Stress stress = new Stress()
            {
                Phone = phone,
                TestDate = score[2],
                Score = score[0]
            };

            return new List<Stress>() { stress };
        }

        [HttpGet("All")]
        public IEnumerable<Emotion> GetAllScore(string phone, string testdate)
        {
            string[] scoreD = SQLHelper.GetEmotionScore(SQLHelper.EmotionCategory.Despressed, phone, testdate);
            string[] scoreA = SQLHelper.GetEmotionScore(SQLHelper.EmotionCategory.Anxeity, phone, testdate);
            string[] scoreS = SQLHelper.GetEmotionScore(SQLHelper.EmotionCategory.Stress, phone, testdate);

            Emotion stress = new Emotion()
            {
                Phone = phone,
                TestDate = scoreD[2],
                DepressedScore = scoreD[0],
                AnxeityScore = scoreA[0],
                StressScore = scoreS[0],
            };

            return new List<Emotion>() { stress };
        }
    }
}
