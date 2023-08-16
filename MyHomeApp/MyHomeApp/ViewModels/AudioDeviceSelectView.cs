using System;
using System.Collections.Generic;
using System.Text;

namespace MyHomeApp.ViewModels
{

    public enum SelectState
    {
        Normal, Selected, Active
    }

    internal class AudioDeviceSelectView : ViewModelBase
    {
        private string name;
        private string id;
        private SelectState selectState;

        public AudioDeviceSelectView(string name, string id, SelectState selectState)
        {
            Name = name;
            Id = id;
            SelectState = selectState;
        }

        public string Name { get => name; set { 
                if (name == value)
                    return;
                name = value;
                OnPropertyChanged();
            } }

        public string Id
        {
            get => id; 
            set
            {
                if (id == value)
                    return;
                id = value;
                OnPropertyChanged();
            }
        }

        public SelectState SelectState
        {
            get => selectState; set
            {
                if (selectState == value)
                    return;
                selectState = value;
                OnPropertyChanged();
            }
        }
    }
}
