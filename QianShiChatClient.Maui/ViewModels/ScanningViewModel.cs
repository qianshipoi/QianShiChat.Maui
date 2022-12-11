using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class ScanningViewModel : ViewModelBase
{
    public ScanningViewModel(INavigationService navigationService, IStringLocalizer<MyStrings> stringLocalizer) : base(navigationService, stringLocalizer)
    {
    }
}
