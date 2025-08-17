using System;
using System.Reflection;
using ExpenseTracker.Helpers;

namespace ExpenseTracker.ExtensionMethods;

public static class ListViewExtensions
{
    static object _lock = new object();

    private static ITemplatedItemsList<Cell> GetTemplatedList(ListView listView)
    {
        ITemplatedItemsList<Cell> cells = null;
        IEnumerable<PropertyInfo> pInfos = listView.GetType().GetRuntimeProperties();
        var templatedItems = pInfos.FirstOrDefault(info => info.Name == "TemplatedItems");
        if (templatedItems != null)
        {
            var templatedList = templatedItems.GetValue(listView);
            cells = templatedList as ITemplatedItemsList<Cell>;
        }
        return cells;
    }

    /// <summary>
    /// The method content is invoked on main thread. We may need to include additional height on top of the total height due to xamarin bug.
    /// This method will resize according to visible items in the listview.
    /// </summary>
    /// <param name="listView"></param>
    /// <param name="additonalHeight">Addtional listview height may be needed due to bug shown by xamarin.</param>
    public static void ResizeByTotalRowHeight(this ListView listView, double totalItemsCount, double additonalHeight = 0)
    {
        const int dividerSpacing = 1;
        double totalRowHeight = 0;
        int totalDividerSpacing = 0;

        lock (_lock)
        {
            DebugHelper.WriteLine($"Resizing by total row height ({listView.AutomationId})");
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ITemplatedItemsList<Cell> cells = GetTemplatedList(listView);

                if (cells == null)
                {
                    DebugHelper.WriteLine("Templated list is null.");
                    return;
                }

                int visibleRowsCount = 0;
                int count = cells.Count;
                if (count > 0)
                {
                    int itemNumber = 0;
                    foreach (ViewCell cell in cells)
                    {
                        var context = cell.BindingContext;
                        itemNumber++;
                        StackLayout target = cell.View as StackLayout;
                        if (target != null)
                        {
                            totalDividerSpacing += dividerSpacing;
                            double height = target.Height;
                            if (height < 1)
                            {
                                DebugHelper.WriteLine($"Skipping to add the height for item {itemNumber}. This item is not visible because height is: {height}");
                            }
                            else
                            {
                                visibleRowsCount++;
                                totalRowHeight += height;
                            }
                        }
                    }
                }

                //if (totalItemsCount > visibleRowsCount)
                //{
                //    DebugHelper.WriteLine($"TotalItemsCount:{totalItemsCount} | VisibleRowsCount:{visibleRowsCount} => Maximizing height");
                //    listView.HeightRequest = -1;
                //}
                //else
                //{
                listView.HeightRequest = totalRowHeight + totalDividerSpacing + additonalHeight;
                DebugHelper.WriteLine($"ListViewHeight({listView.AutomationId})={totalRowHeight}|DividerSpacing={dividerSpacing}" +
                        $"|TotalDividerSpacing={totalDividerSpacing}|AdditonalHeight={additonalHeight}|TotalHeight={listView.HeightRequest}");
                //}
            });
        }
    }

    public static int GetNumberOfVisibleItems(this ListView listView)
    {
        int visibleItemsCount = 0;
        lock (_lock)
        {
            DebugHelper.WriteLine("Getting number of visible items.");
            ITemplatedItemsList<Cell> Rows = GetTemplatedList(listView);
            if (Rows == null)
            {
                DebugHelper.WriteLine("Templated list is null.");
                return 0;
            }
            int rowCount = Rows.Count;
            if (rowCount > 0)
            {
                foreach (ViewCell cell in Rows)
                {
                    StackLayout view = cell.View as StackLayout;
                    if (view != null)
                    {
                        double height = view.Height;
                        if (height > 0) //height of -1 means not visible as observed during debugging.
                        {
                            visibleItemsCount++;
                        }
                    }
                }
            }
        }

        return visibleItemsCount;
    }

    public static void SetHeight(this ListView listView, int numberOfItems)
    {
        lock (_lock)
        {
            DebugHelper.WriteLine("Getting number of visible items.");
            ITemplatedItemsList<Cell> Rows = GetTemplatedList(listView);
            if (Rows == null)
            {
                DebugHelper.WriteLine("Templated list is null.");
                return;
            }

            int rowCount = Rows.Count;
            if (rowCount > 0)
            {
                foreach (ViewCell cell in Rows)
                {
                    if (cell.View is StackLayout view)
                    {
                        //StackLayout view = cell.View as StackLayout;
                        if (view != null)
                        {
                            double height = view.Height;
                            if (height > 0) //height of -1 means not visible as observed during debugging.
                            {
                                double heigthRequest = height * numberOfItems;
                                listView.HeightRequest = heigthRequest;
                                return;
                            }
                        }
                    }
                    else
                    {
                        double height = cell.Height;
                        if (height > 0) //height of -1 means not visible as observed during debugging.
                        {
                            double heigthRequest = height * numberOfItems;
                            listView.HeightRequest = heigthRequest;
                            return;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Resizes the listview height to occupy the whole space available by setting the HeightRequest to -1.
    /// This is invoked on the main thread. Note that if there are 2 listviews, xamarin will equally allocate the space No matter how many
    /// the records are, it will only provide tha allocated space as the maximum. E.g. listview 1 has 50 records and listview 2 has 3 records.
    /// Each of them still get half of the allocated screen even if we set the HeightRequest more than that.
    /// </summary>
    /// <param name="listView"></param>
    public static void MaximizeHeight(this ListView listView)
    {
        lock (_lock)
        {
            DebugHelper.WriteLine("Maximizing height");
            MainThread.BeginInvokeOnMainThread(() => listView.HeightRequest = -1);
        }
    }

    /// <summary>
    /// Resizes the listview to occupy the whole space available by setting the HeightRequest to -1. This is invoked on the main thread.
    /// </summary>
    /// <param name="listView"></param>
    public static void ResizeHeight(this ListView listView, double height)
    {
        lock (_lock)
        {
            DebugHelper.WriteLine($"Resizing height with {height}");
            MainThread.BeginInvokeOnMainThread(() => listView.HeightRequest = height);
        }
    }
}
