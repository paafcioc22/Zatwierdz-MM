using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Zatwierdz_MM.Droid;
using Zatwierdz_MM.Models;

[assembly: Dependency(typeof(AppVersionProvider))]
namespace Zatwierdz_MM.Droid
{
    public class AppVersionProvider: IAppVersionProvider
    {
        public string AppVersion
        {
            get
            {
                var context = Android.App.Application.Context;
                var info = context.PackageManager.GetPackageInfo(context.PackageName, 0);

                return $"{info.VersionName}";
            }
        }

        public int BuildVersion
        {
            get
            {
                var context = Android.App.Application.Context;
                var info = context.PackageManager.GetPackageInfo(context.PackageName, 0);

                return info.VersionCode;
            }
        }
        string _packageName => Android.App.Application.Context.PackageName;

        public Task OpenAppInStore()
        {
            return OpenAppInStore(_packageName);
        }

        public Task OpenAppInStore(string appName)
        {
            if (string.IsNullOrWhiteSpace(appName))
            {
                throw new ArgumentNullException(nameof(appName));
            }

            try
            {
                var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse($"market://details?id={appName}"));
                intent.SetPackage("com.android.vending");
                intent.SetFlags(ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(intent);
            }
            catch (ActivityNotFoundException)
            {
                var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse($"https://play.google.com/store/apps/details?id={appName}"));
                Android.App.Application.Context.StartActivity(intent);
            }

            return Task.FromResult(true);
        }
    }
}