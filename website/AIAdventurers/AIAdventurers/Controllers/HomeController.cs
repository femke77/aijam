using AIAdventurers.Models;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Vision.V1;
using Grpc.Auth;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Firebase.Database;
using Firebase.Database.Query;
using System.Threading.Tasks;

namespace AIAdventurers.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(VisionViewModel model)
        {
            Imagseg seg = new Imagseg();
            seg.segmentation("");
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //https://stackoverflow.com/questions/16255882/uploading-displaying-images-in-mvc-4
        // https://googlecloudplatform.github.io/google-cloud-python/0.20.0/google-cloud-auth.html
        // https://cloud.google.com/docs/authentication/production
        // https://stackoverflow.com/questions/42865917/is-it-possible-to-manually-supply-a-googlecredential-to-speechclient-in-net-ap/42914395

        public ActionResult FileUpload(HttpPostedFileBase file)
        {
            string imageFilename = "";
            string filePath = "";
            if (file != null)
            {
                imageFilename = System.IO.Path.GetFileName(file.FileName);
                string imageDirectory = Server.MapPath("~/images/uploads");
                if (!Directory.Exists(imageDirectory))
                {
                    Directory.CreateDirectory(imageDirectory);
                }
                filePath = System.IO.Path.Combine(imageDirectory, imageFilename);
                // file is uploaded
                file.SaveAs(filePath);

                // save the image path path to the database or you can send image 
                // directly to database
                // in-case if you want to store byte[] ie. for DB
                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                }

            }
            string credentialFile = Server.MapPath("~") + "AIAdventurers-95702aa321b9.json";
            var credential = GoogleCredential.FromFile(credentialFile).CreateScoped(ImageAnnotatorClient.DefaultScopes);
            var channel = new Grpc.Core.Channel(ImageAnnotatorClient.DefaultEndpoint.ToString(), credential.ToChannelCredentials());

            var image = Google.Cloud.Vision.V1.Image.FromFile(filePath);

            var client = ImageAnnotatorClient.Create(channel);

            var response = client.DetectLabels(image);
            foreach (var annotation in response)
            {
                if (annotation.Description != null)
                    Console.WriteLine(annotation.Description);
            }

            VisionViewModel model = new VisionViewModel();
            model.imageUrl = "/images/uploads/" + imageFilename;
            List<Ingredient> ingredients = new List<Ingredient>();
            ingredients.Add(new Ingredient() { Name = "Orange", Calorie = 60 });
            ingredients.Add(new Ingredient() { Name = "Apple", Calorie = 95 });
            int totalCalories = 0;
            foreach(Ingredient i in ingredients)
            {
                totalCalories += i.Calorie;
            }
            model.TotalCalories = totalCalories;
            // model.GoogleVisionResults = response.ToString();
            //model.GoogleVisionResults = "Total calorie";
            // after successfully uploading redirect the user
            model.Ingredients = ingredients;
            return View("Index", model);
        }

        //volume of cylinder pi r squared height
        //volume of sphere = 4/3 pi r cubed
        private async Task CalculateNutrition(string foodname, double radius, double height = 1.0) //optional argument length for when not needed
        {
            Food fooditem = new Food();
            try
            {
                var firebase = new FirebaseClient("https://projec-7236f.firebaseio.com");
                double mass = 0;
                //pull data from firebase database as 1 food item
                var food_db = await firebase
                     .Child("Nutrition")
                     .OrderByKey()
                     .StartAt(foodname)
                     .LimitToFirst(1)
                     .OnceAsync<Food>();


                //pull out the children of the object for calculations
                foreach (var i in food_db)
                {
                    fooditem.Calories = i.Object.Calories;
                    fooditem.Fat = i.Object.Fat;
                    fooditem.Protein = i.Object.Protein;
                    fooditem.Carb = i.Object.Carb;
                    fooditem.Shape = i.Object.Shape;
                    fooditem.Density = i.Object.Density;
                }

                if (fooditem.Shape == "sphere")
                {
                    mass = 1.333 * 3.14 * (radius * radius * radius) * fooditem.Density;
                }

                if (fooditem.Shape == "cylinder")
                {
                    mass = 3.14 * radius * radius * height * fooditem.Density;

                }
                //update fooditem by the mass 
                fooditem.Calories *= mass;
                fooditem.Fat *= mass;
                fooditem.Carb *= mass;
                fooditem.Protein *= mass;

                //put back into database under the daily path 
                await firebase
                .Child("Daily")
                .Child(foodname)
                .PutAsync(new Food(fooditem.Calories, fooditem.Fat, fooditem.Carb, fooditem.Protein));

            }
            catch (Exception ex)
            {
                //need to add error handling here
            }
        }

        //retrieves every child under path "Daily" and creates/returns a Day of total values per each nutrient
        private async Task<Day> DayTotals()
        {
            Day day = new Day();
            try
            {
                var firebase = new FirebaseClient("https://projec-7236f.firebaseio.com");
                var food_db = await firebase
                    .Child("Daily")
                    .OnceAsync<Food>();

                foreach (var i in food_db)
                {
                    day.T_Calories += i.Object.Calories;
                    day.T_Fat += i.Object.Fat;
                    day.T_Protein = i.Object.Protein;
                    day.T_Carb = i.Object.Carb;
                }
            }
            catch (Exception ex)
            {
                //error handling
            }
            return day;
        }



    }
}