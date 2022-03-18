using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MineHealthAPI.Models
{
    public class Posture
    {
        public string Phone { get; set; }
        public string TestDate { get; set; }
        public string NeckScore { get; set; }
        public string FrontSpineScore { get; set; }
        public string SideSpineScore { get; set; }
    }
}
