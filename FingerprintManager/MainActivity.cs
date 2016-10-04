using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Hardware.Fingerprints;

using Android;
using System;

namespace FingerprintManagerTest
{
    [Activity(Label = "FingerprintManager", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private CancellationSignal _cancellationSignal;

        protected override void OnCreate(Bundle bundle)
        {
           
            base.OnCreate(bundle);
            this.SetContentView(Resource.Layout.Main);
            var txt = this.FindViewById<TextView>(Resource.Id.txt);
            txt.Text = "請輸入您的指紋進行辨識";
            var fingerprintManager = this.GetSystemService(Context.FingerprintService) as FingerprintManager;
            var  keyguardManager = (KeyguardManager)GetSystemService(KeyguardService);
            var myFingerprintAuthCallback = new MyFingerprintAuthCallback();

            if (!fingerprintManager.IsHardwareDetected)
            {
                txt.Text = "您的裝置不支援指紋辨識";
                return;
            }

            if (!keyguardManager.IsKeyguardSecure)
            {
                txt.Text = "您的裝置未設定銀幕鎖定解鎖功能";
                return;
            }

            if (!fingerprintManager.HasEnrolledFingerprints)
            {
                txt.Text = "您的裝置未設定任何一組指紋可辨識";
                return;
            }

            if(this.CheckSelfPermission(Manifest.Permission.UseFingerprint)== Android.Content.PM.Permission.Granted)
            {
                myFingerprintAuthCallback.OnResult += (sender, e) =>
                {
                    txt.Text = e;
                };


                this._cancellationSignal=new CancellationSignal();
                fingerprintManager.Authenticate(
                    null,
                    this._cancellationSignal,
                    FingerprintAuthenticationFlags.None,
                    myFingerprintAuthCallback,
                    null);
            }
            else
            {
                txt.Text = "您未允許使用指紋辨識權限";
                return;
            }
        }

    }



    public class MyFingerprintAuthCallback : FingerprintManager.AuthenticationCallback
    {
        public event EventHandler<string> OnResult;
        public override void OnAuthenticationError(FingerprintState errorCode, Java.Lang.ICharSequence errString)
        {
            if (this.OnResult != null)
                this.OnResult(this, "辨識指紋錯誤");
        }

        public override void OnAuthenticationFailed()
        {
            if (this.OnResult != null)
                this.OnResult(this, "辨識指紋失敗");
        }

        public override void OnAuthenticationSucceeded(FingerprintManager.AuthenticationResult result)
        {
            if (this.OnResult != null)
                this.OnResult(this, "辨識指紋通過");
        }
    }
}

