using ExpenseTracker.Models.UI;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ExpenseTracker.Services;

public class CollectionChanged
{
    NotifyCollectionChangedEventHandler prevous_UiExpenses_CollectionChanged = null;
    public void Monitor(ObservableCollection<UiExpenseItem> uiExpenses, int newCount, Action NotBusy, Action InvalidateMeasure)
    {
        if (newCount == 0)
        {
            NotBusy();
            return;
        }

        int counter = 0;
        NotifyCollectionChangedEventHandler UiExpenses_CollectionChanged = (sender, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is UiExpenseItem uiExpenseItem)
                    {
                        counter++;
                        if (newCount == counter)
                        {
                            newCount = 0;
                            counter = 0;
                            MainThread.InvokeOnMainThreadAsync(async () =>
                            {
                                await Task.Delay(100);                                                               
                                NotBusy();
                                InvalidateMeasure();
                            });
                        }
                    }
                }
            }
        };

        if (prevous_UiExpenses_CollectionChanged != null)
            uiExpenses.CollectionChanged -= prevous_UiExpenses_CollectionChanged;
        uiExpenses.CollectionChanged += UiExpenses_CollectionChanged;
        prevous_UiExpenses_CollectionChanged = UiExpenses_CollectionChanged;
    }


    NotifyCollectionChangedEventHandler prevous_UiWeeks_CollectionChanged = null;
    public void Monitor(ObservableCollection<UiWeekItem> uiWeeks, int newCount, Action NotBusy, Action InvalidateMeasure)
    {
        if (newCount == 0)
        {
            NotBusy();
            return;
        }

        int counter = 0;
        NotifyCollectionChangedEventHandler UiWeeks_CollectionChanged = (sender, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is UiWeekItem uiWeekItem)
                    {
                        counter++;
                        if (newCount == counter)
                        {
                            newCount = 0;
                            counter = 0;
                            MainThread.InvokeOnMainThreadAsync(async () =>
                            {
                                await Task.Delay(100);
                                NotBusy();
                                InvalidateMeasure?.Invoke();
                            });
                        }
                    }
                }
            }
        };
        if (prevous_UiWeeks_CollectionChanged != null)
            uiWeeks.CollectionChanged -= prevous_UiWeeks_CollectionChanged;
        uiWeeks.CollectionChanged += UiWeeks_CollectionChanged;
        prevous_UiWeeks_CollectionChanged = UiWeeks_CollectionChanged;
    }

    NotifyCollectionChangedEventHandler prevous_UiMonths_CollectionChanged = null;
    public void Monitor(ObservableCollection<UiMonthItem> uiMonths, int newCount, Action NotBusy, Action InvalidateMeasure)
    {
        if (newCount == 0)
        {
            NotBusy();
            return;
        }

        int counter = 0;
        NotifyCollectionChangedEventHandler UiMonths_CollectionChanged = (sender, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is UiMonthItem uiMonthItem)
                    {
                        counter++;
                        if (newCount == counter)
                        {
                            newCount = 0;
                            counter = 0;
                            MainThread.InvokeOnMainThreadAsync(async () =>
                            {
                                await Task.Delay(100);
                                NotBusy();
                                InvalidateMeasure?.Invoke();
                            });
                        }
                    }
                }
            }
        };

        if (prevous_UiMonths_CollectionChanged != null)
            uiMonths.CollectionChanged -= prevous_UiMonths_CollectionChanged;
        uiMonths.CollectionChanged += UiMonths_CollectionChanged;
        prevous_UiMonths_CollectionChanged = UiMonths_CollectionChanged;
    }

    NotifyCollectionChangedEventHandler prevous_UiDays_CollectionChanged = null;
    public void Monitor(ObservableCollection<UiDayItem> uiDays, int newCount, Action NotBusy, Action InvalidateMeasure)
    {
        if (newCount == 0)
        {
            NotBusy();
            return;
        }

        int counter = 0;
        NotifyCollectionChangedEventHandler UiDays_CollectionChanged = (sender, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is UiDayItem uiDayItem)
                    {
                        counter++;
                        if (newCount == counter)
                        {
                            newCount = 0;
                            counter = 0;
                            MainThread.InvokeOnMainThreadAsync(async () =>
                            {
                                await Task.Delay(100);
                                NotBusy();
                                InvalidateMeasure?.Invoke();
                            });
                        }
                    }
                }
            }
        };

        if (prevous_UiDays_CollectionChanged != null)
            uiDays.CollectionChanged -= prevous_UiDays_CollectionChanged;
        uiDays.CollectionChanged += UiDays_CollectionChanged;
        prevous_UiDays_CollectionChanged = UiDays_CollectionChanged;
    }
}
