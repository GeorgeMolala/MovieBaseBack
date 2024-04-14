using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBaseBack.Models
{
    public class Watched
    {
        [Key]
        public int WdFID { get; set; }

        public string Title { get; set; }

        public int Year { get; set; }

        public string Type { get; set; }

        public string ImbdID { get; set; }

        public string PosterImage { get; set; }
    }
}
