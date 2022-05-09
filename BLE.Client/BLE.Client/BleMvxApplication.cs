using System;
using BLE.Client.ViewModels;
using MvvmCross;
using MvvmCross.Forms.Core;
using MvvmCross.IoC;
using MvvmCross.Localization;
using MvvmCross.ViewModels;
using Xamarin.Forms;

namespace BLE.Client
{
    public class BleMvxApplication : MvxApplication //baseviewmodel
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            RegisterAppStart<DeviceListViewModel>();
        }


    }
}
