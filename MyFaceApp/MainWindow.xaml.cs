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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private readonly IFaceServiceClient faceServiceClient = new FaceServiceClient("82a75f7a5c9243f0ad9c46fd12c4f102");
   

        private async void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            
            var openDlg = new Microsoft.Win32.OpenFileDialog();

            openDlg.Filter = "JPEG Image(*.jpg)|*.jpg";
            bool? result = openDlg.ShowDialog(this);

            if (!(bool)result)
            {
                return;
            }

            string filePath = openDlg.FileName;

            Uri fileUri = new Uri(filePath);
            BitmapImage bitmapSource = new BitmapImage();

            bitmapSource.BeginInit();
            bitmapSource.CacheOption = BitmapCacheOption.None;
            bitmapSource.UriSource = fileUri;
            bitmapSource.EndInit();

            FacePhoto.Source = bitmapSource;

            Title = "Detecting...";
           // FaceRectangle[] faceRects = await UploadAndDetectFaces(filePath);
            //train_pictures();
            string personGroupId = "myfriends";

        
            string testImageFile = filePath;
            using (Stream s = File.OpenRead(testImageFile))
       
            {
              
                try
                {
                    var faces = await faceServiceClient.DetectAsync(s);
                    var faceIds = faces.Select(face => face.FaceId).ToArray();
                    var results = await faceServiceClient.IdentifyAsync(personGroupId, faceIds);

                    foreach (var identifyResult in results)
                    {
                        Console.WriteLine("Result of face: {0}", identifyResult.FaceId);
                        if (identifyResult.Candidates.Length == 0)
                        {
                            Console.WriteLine("No one identified");
                            Title = String.Format("I don't know who this is");
                        }
                        else
                        {
                            // Get top 1 among all candidates returned
                            var candidateId = identifyResult.Candidates[0].PersonId;
                            var person = await faceServiceClient.GetPersonAsync(personGroupId, candidateId);
                            Console.WriteLine("Identified as {0}", person.Name);
                            Title = String.Format("Hello, this is {0} ", person.Name);
                        }
                    }
                }
                catch {
                    Title = String.Format("I don't know who this is");
                }
                }
            /**
                        if (faceRects.Length > 0)
                        {
                            DrawingVisual visual = new DrawingVisual();
                            DrawingContext drawingContext = visual.RenderOpen();
                            drawingContext.DrawImage(bitmapSource,
                                new Rect(0, 0, bitmapSource.Width, bitmapSource.Height));
                            double dpi = bitmapSource.DpiX;
                            double resizeFactor = 96 / dpi;

                            foreach (var faceRect in faceRects)
                            {
                                drawingContext.DrawRectangle(
                                    Brushes.Transparent,
                                    new Pen(Brushes.Red, 2),
                                    new Rect(
                                        faceRect.Left * resizeFactor,
                                        faceRect.Top * resizeFactor,
                                        faceRect.Width * resizeFactor,
                                        faceRect.Height * resizeFactor
                                        )
                                );
                            }

                            drawingContext.Close();
                            RenderTargetBitmap faceWithRectBitmap = new RenderTargetBitmap(
                                (int)(bitmapSource.PixelWidth * resizeFactor),
                                (int)(bitmapSource.PixelHeight * resizeFactor),
                                96,
                                96,
                                PixelFormats.Pbgra32);

                            faceWithRectBitmap.Render(visual);
                            FacePhoto.Source = faceWithRectBitmap;
                        }

                **/
        }

        private async void train_pictures()
        {
            // Create an empty person group
            string personGroupId = "myfriends";
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


        private async Task<FaceRectangle[]> UploadAndDetectFaces(string imageFilePath)
        {
            try
            {
                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    var faces = await faceServiceClient.DetectAsync(imageFileStream);
                    var faceRects = faces.Select(face => face.FaceRectangle);
                    return faceRects.ToArray();
                }
            }
            catch (Exception)
            {
                return new FaceRectangle[0];
            }
        }
    }


}
