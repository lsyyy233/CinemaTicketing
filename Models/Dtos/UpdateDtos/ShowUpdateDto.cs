using CinemaTicketing.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketing.Models.Dtos.UpdateDtos
{
    public class ShowUpdateDto
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public ShowNum ShowNum { get; set; }
        public int MovieId { get; set; }
        public int HallId { get; set; }
        public double Price { get; set; }
    }
}
