﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HipMobileUI.Navigation;
using HipMobileUI.ViewModels.Views;
using Xamarin.Forms;

namespace HipMobileUI.Views
{
    public partial class ExhibitsOverviewView : ContentView, IViewFor<ExhibitsOverviewViewModel>
    {
        public ExhibitsOverviewView()
        {
            InitializeComponent();
        }
    }
}
