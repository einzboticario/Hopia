using System;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using Emgu.CV;
using Emgu.CV.Structure;

namespace FaceDetectionCamera
{
    public partial class Form1 : Form
    {
        SpeechSynthesizer s = new SpeechSynthesizer();
        Choices list = new Choices();
        public Form1()
        {
            SpeechRecognitionEngine rec = new SpeechRecognitionEngine();

            list.Add(new String[] { "hello", "how are you", "what is your name", "what are you", "what time is it", "what is the date today" });

            Grammar gr = new Grammar(new GrammarBuilder(list));

            try
            {
                rec.RequestRecognizerUpdate();
                rec.LoadGrammar(gr);
                rec.SpeechRecognized += rec_SpeechRecognized;
                rec.SetInputToDefaultAudioDevice();
                rec.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch
            {
                return;
            }

            //SET VOICE, TEST
            s.SelectVoiceByHints(VoiceGender.Female);
            s.Speak("Hello! My name is Hopia");
            InitializeComponent();
        }
        public void say(string h)
        {
            s.Speak(h);
        }
        private void rec_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            String r = e.Result.Text;

            /* Here are some pre-made responses. 
             * I know it's few. 
             * Feel free to add more...
             * 
             What you say */
            if (r == "hello")
            { //What it says
                say("hi");
                return;
            }
            //What you say
            if (r == "how are you")
            { //What it says
                say("great, how about you");
                return;
            }
            if (r == "what time is it")
            {
                say(DateTime.Now.ToString("h:mm tt"));
                return;
            }
            if (r == "what is the date today")
            {
                say(DateTime.Now.ToString("M/d/yyyy"));
                return;
            }
            //What you say
            if (r == "what is your name")
            { //What it says
                say("my name is Hopia");
                return;
            }
            //What you say
            if (r == "what are you")
            { //What it says
                say("im a program, please feed me more information");
                return;
            }

        }
        FilterInfoCollection filter;
        VideoCaptureDevice device;

        private void Form1_Load(object sender, EventArgs e)
        {
            filter = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in filter)
                cbxDevice.Items.Add(device.Name);
            cbxDevice.SelectedIndex = 0;
            device = new VideoCaptureDevice();
        }

        private void BtnDetect_Click(object sender, EventArgs e)
        {
            device = new VideoCaptureDevice(filter[cbxDevice.SelectedIndex].MonikerString);
            device.NewFrame += Device_NewFrame;
            device.Start();
        }

        static readonly CascadeClassifier cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_alt_tree.xml");
        private void Device_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            Image<Bgr, byte> grayImage = new Image<Bgr, byte>(bitmap);
            Rectangle[] rectangles = cascadeClassifier.DetectMultiScale(grayImage, 1.2, 1);
            foreach(Rectangle rectangle in rectangles)
            {
                using(Graphics graphics = Graphics.FromImage(bitmap))
                {
                    using (Pen pen = new Pen(Color.Red, 5))
                    {
                        graphics.DrawRectangle(pen, rectangle);
                    }
                }
            }
            pic.Image = bitmap;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (device.IsRunning)
                device.Stop();
        }
    }
}
