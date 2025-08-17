using CommunityToolkit.Mvvm.ComponentModel;
using ExpenseTracker.Features.OnBoarding.Models;
using System.Collections.ObjectModel;

namespace ExpenseTracker.Features.OnBoarding;

public partial class OnBoardingViewModel : ObservableObject
{
    public OnBoardingViewModel()
    {
        Slides = new ObservableCollection<SlideModel>
        {
            new SlideModel{ Order = 1 },
            new SlideModel{ Order = 2 },
            new CategorySlideModel { Order = 3 } 
        };
    }

    [ObservableProperty]
    ObservableCollection<SlideModel> slides = new ObservableCollection<SlideModel>();
}