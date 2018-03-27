using System;

namespace AIAdventurers.Models
{

    public class Day
    {



        public Day(double tcalorie, double tfat, double tcarb, double tprotein)
        {
            T_Calories = tcalorie;
            T_Fat = tfat;
            T_Carb = tcarb;
            T_Protein = tprotein;
        }


        public Day()
        {

        }

        public double T_Calories { get; set; }
        public double T_Fat { get; set; }
        public double T_Carb { get; set; }
        public double T_Protein { get; set; }

    }










}
