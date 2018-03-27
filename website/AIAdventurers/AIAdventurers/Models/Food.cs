using System;

namespace AIAdventurers.Model
{
    public class Food
    {



        public Food(double calorie, double fat, double carb, double protein)
        {
            Calories = calorie;
            Fat = fat;
            Carb = carb;
            Protein = protein;

        }

        public Food()
        {

        }

        public double Calories { get; set; }
        public double Fat { get; set; }
        public double Carb { get; set; }
        public double Protein { get; set; }
        public string Foodname { get; set; }
    }
}

