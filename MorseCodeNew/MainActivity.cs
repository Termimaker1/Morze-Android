using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System.Collections.Generic;
using Android.Support.V4.App;
using System;
using Android;
using Android.Support.V4.Content;
using System.Threading;
using Xamarin.Essentials;

namespace MorseCodeNew // Leon Rubin TA-18V
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        IDictionary<string, string> morze = new Dictionary<string, string>();
        int delay = 100;
        bool flashAllowed = true;
        TextView morseOutput;
        CheckBox light, vibration;
        bool threadRunning = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            morze.Add("B", "-...");
            morze.Add("C", "-.-.");
            morze.Add("D", "-..");
            morze.Add("E", ".");
            morze.Add("F", "..-.");
            morze.Add("G", "--.");
            morze.Add("H", "....");
            morze.Add("I", "..");
            morze.Add("J", ".---");
            morze.Add("K", "-.-");
            morze.Add("L", ".-..");
            morze.Add("M", "--");
            morze.Add("N", "-.");
            morze.Add("O", "---");
            morze.Add("P", ".--.");
            morze.Add("Q", "--.-");
            morze.Add("R", ".-.");
            morze.Add("S", "...");
            morze.Add("T", "-");
            morze.Add("U", "..-");
            morze.Add("V", "...-");
            morze.Add("W", ".--");
            morze.Add("X", "-..-");
            morze.Add("Y", "-.--");
            morze.Add("Z", "--..");
            morze.Add("1", ".----");
            morze.Add("2", "..---");
            morze.Add("3", "...--");
            morze.Add("4", "....-");
            morze.Add("5", ".....");
            morze.Add("6", "-....");
            morze.Add("7", "--...");
            morze.Add("8", "---..");
            morze.Add("9", "----.");
            morze.Add("0", "-----");
            morze.Add(" ", ";");
            FindViewById<ImageButton>(Resource.Id.start).SetImageResource(Resource.Drawable.start);
            EditText editText = FindViewById<EditText>(Resource.Id.textInput);
            Button clear = FindViewById<Button>(Resource.Id.clear);
            morseOutput = FindViewById<TextView>(Resource.Id.morseOutput);
            RadioButton smallDist = FindViewById<RadioButton>(Resource.Id.smallDist);
            RadioButton mediumDist = FindViewById<RadioButton>(Resource.Id.mediumDist);
            RadioButton longDist = FindViewById<RadioButton>(Resource.Id.longDist);
            ImageButton start = FindViewById<ImageButton>(Resource.Id.start);
            Button stop = FindViewById<Button>(Resource.Id.stop);
            Button exit = FindViewById<Button>(Resource.Id.exit);
            light = FindViewById<CheckBox>(Resource.Id.light);
            vibration = FindViewById<CheckBox>(Resource.Id.vibration);
            editText.TextChanged += (sender, e) =>
            {
                string[] temp = editText.Text.Split(' ');
                string tempText = "";
                string[] additional = new string[] { "..-..", ".---.", ".--..", "....--.." };
                Dictionary<string, string> unknown = new Dictionary<string, string>();
                foreach (string item in temp)
                {
                    foreach (char character in item)
                    {
                        if (!morze.ContainsKey(character.ToString()))
                        {
                            if (!unknown.ContainsKey(character.ToString()))
                            {
                                if (unknown.Count == 4)
                                    unknown.Clear();
                                foreach (string add in additional)
                                {
                                    if (!unknown.ContainsValue(add))
                                    {
                                        unknown.Add(character.ToString(), add);
                                        break;
                                    }
                                }
                                tempText += " " + unknown[character.ToString()];
                            }
                            else
                            {
                                tempText += " " + unknown[character.ToString()];
                            }
                        }
                        else
                        {
                            tempText += " " + GetMorse(character.ToString());
                        }

                    }
                    unknown.Clear();
                    tempText += ";";
                }
                morseOutput.Text = tempText.ToString();
            };
            start.Click += (sender, e) => { if (!threadRunning) { Thread startFl = new Thread(new ThreadStart(startBlink)); startFl.Start(); flashAllowed = true; } };
            stop.Click += (sender, e) => { flashAllowed = false; };
            smallDist.Click += (sender, e) => { delay = 400; };
            mediumDist.Click += (sender, e) => { delay = 800; };
            longDist.Click += (sender, e) => { delay = 1200; };
            clear.Click += (sender, e) => { editText.Text = ""; };
            light.Click += (sender, e) => { if (!light.Checked && !vibration.Checked) { light.Checked = true; } };
            vibration.Click += (sender, e) => { if (!light.Checked && !vibration.Checked) { light.Checked = true; } };
            exit.Click += (sender, e) => { Android.OS.Process.KillProcess(Android.OS.Process.MyPid()); };
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera) != (int)Android.Content.PM.Permission.Granted) 
            {
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.Camera }, 1);
            }
            
        }
        public async void startBlink()
        {
            threadRunning = true;
            if (!light.Checked && !vibration.Checked)
            {
                light.Checked = true;
            }
            foreach (char item in morseOutput.Text)
            {
                if (!flashAllowed)
                    break;
                if (light.Checked)
                {
                    if (item == '.')
                    {
                        Flash();
                    }
                    else if (item == '-')
                    {
                        Flash();
                    }
                }
                if (vibration.Checked)
                {
                    if (flashAllowed)
                    {
                        if (item == '.')
                        {
                            Vibrate(delay);
                        }
                        else if (item == '-')
                        {
                            Vibrate(delay * 3);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                if (item == '.')
                {
                    Thread.Sleep(delay);
                }
                else if (item == '-')
                {
                    Thread.Sleep(delay * 3);
                }

                await Flashlight.TurnOffAsync();

                if (item == ' ')
                {
                    Thread.Sleep(delay * 3);
                }
                else if (item == ';')
                {
                    Thread.Sleep(delay * 4);
                }
                else
                {
                    Thread.Sleep(delay);
                }


            }
            threadRunning = false;
        }
        public async void Flash()
        {


            // We have permission, go ahead and use the camera.
            try
            {

                await Flashlight.TurnOnAsync();

            }
            catch (FeatureNotSupportedException fnsEx)
            {


            }
            catch (PermissionException pEx)
            {

            }
            catch (Exception ex)
            {
                // Unable to turn on/off flashlight
            }

        }
        public void Vibrate(int del)
        {

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Vibrate) == (int)Android.Content.PM.Permission.Granted)
            {
                // We have permission, go ahead and use the camera.
                try
                {

                    Vibration.Vibrate(del);
                }
                catch (FeatureNotSupportedException fnsEx)
                {


                }
                catch (PermissionException pEx)
                {

                }
                catch (Exception ex)
                {
                    // Unable to turn on/off flashlight
                }
            }
            else
            {
                // Camera permission is not granted. If necessary display rationale & request.
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.Vibrate }, 1);
                Vibrate(del);
            }
        }

        string GetMorse(string character)
        {
            character = character.ToUpper();
            if (morze.ContainsKey(character))
            {
                return morze[character];
            }
            return "";
        }
    }
}
