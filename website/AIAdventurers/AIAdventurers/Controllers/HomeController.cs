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

namespace AIAdventurers.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(VisionViewModel model)
        {
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




    }
}