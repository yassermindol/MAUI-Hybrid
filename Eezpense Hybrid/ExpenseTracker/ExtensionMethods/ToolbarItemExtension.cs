namespace ExpenseTracker.ExtensionMethods
{
    public static class ToolbarItemExtension
    {
        public static ToolbarItem Clone(this ToolbarItem item)
        {
            return new ToolbarItem
            {
                Order = item.Order,
                Priority = item.Priority,
            };
        }
    }
}
