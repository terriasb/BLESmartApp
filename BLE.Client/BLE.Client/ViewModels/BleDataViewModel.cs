using Acr.UserDialogs;
using MvvmCross.ViewModels;
using Plugin.BLE.Abstractions.Contracts;
using System;
using Plugin.BLE.Abstractions.EventArgs;
using System.Collections.ObjectModel;
using Microcharts;
using SkiaSharp;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Linq;
using System.Diagnostics;
using MvvmCross.Commands;
using System.Collections.Generic;

namespace BLE.Client.ViewModels
{
    public class BleDataViewModel : BaseViewModel
    {
        private readonly IUserDialogs _userDialogs;
        private bool _dataisSet = false;
        public bool DataIsSet
        {
            get => _dataisSet;
            set => SetProperty(ref _dataisSet, value);
        }
        public string CharacteristicValue => _characteristic?.StringValue;
        private int _weightSet;
        public int WeightSet
        {
            get => _weightSet;
            set => SetProperty(ref _weightSet, value);
        }
        private int _complianceSet;
        public int ComplianceSet
        {
            get => _complianceSet;
            set => SetProperty(ref _complianceSet, value);
        }
        private float _currentCompliancePercentage;
        public float CurrentCompliancePercentage
        {
            get => _currentCompliancePercentage;
            set => SetProperty(ref _currentCompliancePercentage, value);
        }
        /*        public Chart VisualTestChart = new RadialGaugeChart()
                {
                    Entries = entries(),
                    BackgroundColor = SKColor.Parse("#77d065"),
                    LabelTextSize = 60,
                    LineSize = 30,
                    LineAreaAlpha = 3,
                };
                private dynamic ChartEntry[] entries = new ChartEntry[]
                {

                    new ChartEntry(_complianceSet)
                    {
                        Color = SKColor.Parse("#749065"),
                    },
                    new ChartEntry(52)
                    {
                        Color = SKColor.Parse("#749065"),
                    }


                };*/
        public ObservableCollection<BleListItemViewModel> SmartCastRawData { get; set; } = new ObservableCollection<BleListItemViewModel>();
        public IDevice _device { get; private set;}
        public IService _service { get; private set; }
        public ICharacteristic _characteristic { get; private set; }
        public ObservableCollection<int> bleValue { get; set; } = new ObservableCollection<int>();
        public Dictionary<int, int> BleDataTime { get; set; } = new Dictionary<int, int>();
        public BleListItemViewModel data;
        public override void Prepare(MvxBundle parameters)
        {
            base.Prepare(parameters);
            _device = GetDeviceFromBundle(parameters);
        }
        public override void ViewAppeared()
        {
            base.ViewAppeared();
            LoadService();
            
        }        
        private async void LoadService()
        {
                _userDialogs.ShowLoading("Loading characteristics...");
            try
            {
                _service = await _device.GetServiceAsync(Guid.Parse("0000ffe0-0000-1000-8000-00805f9b34fb"));
                _userDialogs.HideLoading();
                
            }
            catch (Exception ex)
            {
                _userDialogs.HideLoading();
                await _userDialogs.AlertAsync(ex.Message);
            }
            if (_service != null)
            {
                LoadCharacteristic();
            }
        }
        private async void LoadCharacteristic()
        {
            try
            {
                _characteristic = await _service.GetCharacteristicAsync(Guid.Parse("0000ffe1-0000-1000-8000-00805f9b34fb"));
                
            }
            catch(Exception ex)
            {
                await _userDialogs.AlertAsync(ex.Message);
            }
            if (_characteristic!=null)
            {
                LoadValue();
            }
        }
        private async void LoadValue()
        {
            try
            {
                await _characteristic.StartUpdatesAsync();
                _characteristic.ValueUpdated -= CharacteristicOnValueUpdated;
                _characteristic.ValueUpdated += CharacteristicOnValueUpdated;
            }
            catch(Exception ex)
            {
                await _userDialogs.AlertAsync(ex.Message);
            }
        }
        public BleDataViewModel(IAdapter adapter, IUserDialogs userDialogs) : base(adapter)
        {
            _userDialogs = userDialogs;
        }     
        private void CharacteristicOnValueUpdated(object sender, CharacteristicUpdatedEventArgs characteristicUpdatedEventArgs)
        {
            //bleValue.Add();
/*            var intCharVal = Int32.Parse(CharacteristicValue);
            
            data.SetForce(intCharVal);
            SmartCastRawData.Add(data);*/
            
            if ((_weightSet > 0) && (_weightSet <=500) )
            {
                _currentCompliancePercentage = (Int32.Parse(CharacteristicValue) / WeightSet)*100;
                
                if (_complianceSet == 25 && _currentCompliancePercentage >= _complianceSet) { }
                if (_complianceSet == 50 && _currentCompliancePercentage >= _complianceSet) { }
                if (_complianceSet == 75 && _currentCompliancePercentage >= _complianceSet) { }
                if (_complianceSet == 100 && _currentCompliancePercentage >= _complianceSet) { }
                RaisePropertyChanged(() => CurrentCompliancePercentage);
            }
            RaisePropertyChanged(() => CharacteristicValue);
        }
        public MvxCommand SetWeight => new MvxCommand(SettingWeight);
        private async void SettingWeight()
        {
            try
            {
                var r = await _userDialogs.PromptAsync(new PromptConfig
                {
                    Title = "Weight Input",
                    Placeholder = "Enter your Current Weight",
                    InputType = InputType.Number,
                    OkText = "Submit",
                    MaxLength = 3,

                });
                _weightSet = int.Parse(r.Text);
                if (!(_weightSet > 0 && _weightSet <= 500)) 
                {
                    await _userDialogs.AlertAsync("Weight Input is Invalid");
                }
                else
                {
                    await RaisePropertyChanged(() => WeightSet);
                }
                
            }
            catch (Exception ex) 
            {
                await _userDialogs.AlertAsync(ex.Message);
            }
        }
        public MvxCommand SetCompliance => new MvxCommand(SettingCompliance);
        private async void SettingCompliance()
        {
            try
            {
                var r = await _userDialogs.PromptAsync(new PromptConfig
                {
                    Title = "Compliance Percentage",
                    Placeholder = "Valid Input Values: 25, 50, 75, 100",
                    InputType = InputType.Number,
                    OkText = "Submit",
                    MaxLength = 3,

                });
                _complianceSet = Int32.Parse(r.Text);
                if ((_complianceSet != 25) && (_complianceSet != 50) && (_complianceSet != 75) && (_complianceSet != 100))
                {
                    _complianceSet = 100;
                    await _userDialogs.AlertAsync("Compliance Input is not Valid, autoSet to 100");
                    await RaisePropertyChanged(() => ComplianceSet);
                }
                else
                {
                    await RaisePropertyChanged(() => ComplianceSet);
                }

            }
            catch (Exception ex) 
            {
                await _userDialogs.AlertAsync(ex.Message);
            }
        }
    }
}
