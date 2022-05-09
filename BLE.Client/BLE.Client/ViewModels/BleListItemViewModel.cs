using System;
using MvvmCross.ViewModels;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;

namespace BLE.Client.ViewModels
{
    public class BleListItemViewModel : MvxNotifyPropertyChanged
    {
        public int BleDataVals { get; private set; }
        public string _timeString;
        public int _force;
        public int _time;
        public float _angle;


        public BleListItemViewModel(int Force, int Time, float Angle)
        {
            _force = Force;
            _time = Time;
            _angle = Angle;
        }

        public string GetTime()
        {
            return _timeString;
        }
        public int GetForce()
        {
            return _force;
        }
        public float GetAngle()
        {
            return _angle;
        }
        public string GetTimeAsString()
        {
            return _timeString;
        }
        public void SetForce(int Force)
        {
            _force = Force;
        }
        public void SetTime(int Time)
        {
            _time = Time;
            _timeString = Time.ToString();
        }
        public void SetAngle(float Angle)
        {
            _angle = Angle;
        }

    }
}