﻿using System;
using System.ComponentModel.DataAnnotations;

namespace ParkyWeb.Models
{
    public class Trail
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public double Distance { get; set; }
        [Required]
        public double Elevation { get; set; }
        public enum DifficultyType { Easy, Moderator, Difficult, Expert }
        public DifficultyType Difficulty { get; set; }
        [Required]
        public int NationalParkId { get; set; }

        public NationalPark NationalPark { get; set; }
    }
}
