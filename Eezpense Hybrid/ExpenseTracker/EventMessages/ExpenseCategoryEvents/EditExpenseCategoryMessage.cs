using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ExpenseTracker.EventMessages.ExpenseCategoryEvents
{
    public class EditExpenseCategoryMessage : ValueChangedMessage<EditedCategoryName>
    {
        public EditExpenseCategoryMessage(EditedCategoryName editedCategory) : base(editedCategory)
        {
        }
    }

    public class EditedCategoryName
    {
        public EditedCategoryName(string previousName, string newName)
        {
            PreviousName = previousName;
            NewName = newName;
        }

        public string PreviousName { get; set; }
        public string NewName { get; set; }
    }
}
