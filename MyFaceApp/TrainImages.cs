using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System.IO;

namespace MyFaceApp
{
    class TrainImages
    {
       

        public async void train(IFaceServiceClient faceServiceClient)
        {
            //await faceServiceClient.DeletePersonAsync(personGroupId, "My Friends");
            //CreatePersonGroupAsync(personGroupId, "My Friends");
            // Create an empty person group
            string personGroupId = "myfriends";
           // await faceServiceClient.DeletePersonGroupAsync(personGroupId);
            
            try
            {
                await faceServiceClient.CreatePersonGroupAsync(personGroupId, "My Friends");
            }
            catch
            {

            }

            // Define Anna
            CreatePersonResult friend1 = await faceServiceClient.CreatePersonAsync(
                // Id of the person group that the person belonged to
                personGroupId,
                // Name of the person
                "Anna"
            );
            // Define Bill
            CreatePersonResult friend2 = await faceServiceClient.CreatePersonAsync(
                // Id of the person group that the person belonged to
                personGroupId,
                // Name of the person
                "Bill"
            );
            //Define Beyonce 
            CreatePersonResult friend3 = await faceServiceClient.CreatePersonAsync(
               // Id of the person group that the person belonged to
               personGroupId,
               // Name of the person
               "Beyonce"
           );
            /**
            //Define Obama
            CreatePersonResult friend4 = await faceServiceClient.CreatePersonAsync(
               // Id of the person group that the person belonged to
               personGroupId,
               // Name of the person
               "Obama"
           );
            //Define Nelson
            CreatePersonResult friend5 = await faceServiceClient.CreatePersonAsync(
               // Id of the person group that the person belonged to
               personGroupId,
               // Name of the person
               "Nelson"
           );**/

            // Define Bill and Clare in the same way

            // Directory contains image files of Anna
            const string friend1ImageDir = @"C:\Temp\Family1-Mom";

            foreach (string imagePath in Directory.GetFiles(friend1ImageDir, "*.jpg"))
            {
                using (Stream s = File.OpenRead(imagePath))
                {
                    // Detect faces in the image and add to Anna
                    await faceServiceClient.AddPersonFaceAsync(
                        personGroupId, friend1.PersonId, s);
                }
            }
            // Do the same for Bill and Clare

            // Directory contains image files of Anna
            const string friend2ImageDir = @"C:\Temp\Family2-Man";

            foreach (string imagePath in Directory.GetFiles(friend2ImageDir, "*.jpg"))
            {
                using (Stream s = File.OpenRead(imagePath))
                {
                    // Detect faces in the image and add to Anna
                    await faceServiceClient.AddPersonFaceAsync(
                        personGroupId, friend2.PersonId, s);
                }
            }

            // Directory contains image files of Beyonce
            const string friend3ImageDir = @"C:\Temp\Beyonce";

            foreach (string imagePath in Directory.GetFiles(friend3ImageDir, "*.jpg"))
            {
                using (Stream s = File.OpenRead(imagePath))
                {
                    // Detect faces in the image and add to Anna
                    await faceServiceClient.AddPersonFaceAsync(
                        personGroupId, friend3.PersonId, s);
                }
            }
            /**
            // Directory contains image files of Obama
            const string friend4ImageDir = @"C:\Temp\Obama";

            foreach (string imagePath in Directory.GetFiles(friend4ImageDir, "*.jpg"))
            {
                using (Stream s = File.OpenRead(imagePath))
                {
                    // Detect faces in the image and add to Anna
                    await faceServiceClient.AddPersonFaceAsync(
                        personGroupId, friend4.PersonId, s);
                }
            }

            // Directory contains image files of Nelson
            const string friend5ImageDir = @"C:\Temp\Nelson";

            foreach (string imagePath in Directory.GetFiles(friend5ImageDir, "*.jpg"))
            {
                using (Stream s = File.OpenRead(imagePath))
                {
                    // Detect faces in the image and add to Anna
                    await faceServiceClient.AddPersonFaceAsync(
                        personGroupId, friend5.PersonId, s);
                }
            }
    **/

            //Lets Train them
            await faceServiceClient.TrainPersonGroupAsync(personGroupId);
            TrainingStatus trainingStatus = null;
            while (true)
            {
                trainingStatus = await faceServiceClient.GetPersonGroupTrainingStatusAsync(personGroupId);

                if (trainingStatus.Status.Equals("running"))
                {
                    break;
                }

                await Task.Delay(1000);
            }
    
        }
   
    }
    
}
