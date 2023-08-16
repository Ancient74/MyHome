using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Content;
using Java.Net;

namespace MyHomeApp.Droid
{
    [Activity(Label = "My Home", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true)]
    [IntentFilter(new[] { Intent.ActionSend }, Categories = new[] {
    Intent.CategoryDefault,
    Intent.CategoryBrowsable,
}, DataMimeType = "text/plain")]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            OnNewIntent(Intent);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            string url = "";
            if (intent != null && intent.Type == "text/plain" && intent.Action == Intent.ActionSend)
                url = intent.GetStringExtra(Intent.ExtraText);
            LoadApplication(new App(url));
            if (!string.IsNullOrEmpty(url))
                FinishAffinity();
        }

        protected override void OnResume()
        {
            base.OnResume();
            OnNewIntent(Intent);
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}