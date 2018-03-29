using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIAdventurers.Models
{
    public class VisionViewModel
    {
        public string imageUrl { get; set; }

        public string GoogleVisionResults { get; set; }

        public List<Ingredient> Ingredients { get; set; }

        public int TotalCalories { get; set; }
    }

    public class Ingredient
    {
       public string Name { get; set; }
            public int Calorie { get; set; }

    }
}