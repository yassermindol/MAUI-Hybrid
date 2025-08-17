using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.EventMessages
{
    public class BottomSheetViewIsPresentedMessage : ValueChangedMessage<bool>
    {
        public BottomSheetViewIsPresentedMessage(bool value) : base(value)
        {
        }
    }
}
