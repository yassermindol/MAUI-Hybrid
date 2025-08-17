using Android.Webkit;
using ExpenseTracker.Models;
using ExpenseTracker.Models.DbEntities;
using SQLite;

namespace ExpenseTracker.Database;
public class ExpenseCategoryTable
{
    SQLiteConnection _connection;
    public ExpenseCategoryTable(SQLiteConnection sQLiteAsyncConnection)
    {
        _connection = sQLiteAsyncConnection;
    }

    public void Add(List<ExpenseCategoryEntity> entities)
    {
        _connection.InsertAll(entities);
    }

    public void Add(ExpenseCategoryEntity entity)
    {
        _connection.Insert(entity);
    }

    public void Add(string category)
    {
        _connection.Insert(new ExpenseCategoryEntity { Name = category });
    }

    public async Task<ExpenseCategoryEntity> Get(int primaryKey)
    {
        ExpenseCategoryEntity item = _connection.Get<ExpenseCategoryEntity>(primaryKey);
        return item;
    }

    public async Task<ExpenseCategoryEntity> Update(string name, string newName)
    {
        ExpenseCategoryEntity item = _connection.Get<ExpenseCategoryEntity>(x => x.Name == name);
        item.Name = newName;

        //Consider categories that was deleted and new categories created with the same name of the categories that was deleted.
        //Note that we dont delete the categories but only mark it as deleted.
        List<ExpenseCategoryHistory> history = new List<ExpenseCategoryHistory>();
        if (string.IsNullOrWhiteSpace(item.History))
        {

        }
        else
        {

        }
        return item;
    }

    /// <summary>
    /// Gets all categories that are not deleted.
    /// </summary>
    public List<ExpenseCategoryEntity> GetAllNotDeleted()
    {
        List<ExpenseCategoryEntity> list = _connection.Table<ExpenseCategoryEntity>().Where(x => !x.IsDeleted).ToList();
        return list ?? new List<ExpenseCategoryEntity>();
    }

    public List<ExpenseCategoryEntity> Where(Func<ExpenseCategoryEntity, bool> predicate)
    {
        List<ExpenseCategoryEntity> list = _connection.Table<ExpenseCategoryEntity>().Where(predicate).ToList();
        return list;
    }

    public void Update(ExpenseCategoryEntity entity)
    {
        _connection.Update(entity);
    }

    public void Delete(List<string> categories)
    {
        var categoryEntities = GetAllNotDeleted();

        List<ExpenseCategoryEntity> categoriesTodelete = new();
        foreach (var name in categories)
        {
            var entity = categoryEntities.First(x => x.Name == name);
            entity.IsDeleted = true;
            categoriesTodelete.Add(entity);
        }
        _connection.UpdateAll(categoriesTodelete);
    }

    public void Restore(List<string> categories)
    {
        var categoryEntities = _connection.Table<ExpenseCategoryEntity>().Where(x => x.IsDeleted).ToList();

        List<ExpenseCategoryEntity> categoriesTodelete = new();
        foreach (var name in categories)
        {
            var entity = categoryEntities.First(x => x.Name == name);
            entity.IsDeleted = false;
            categoriesTodelete.Add(entity);
        }
        _connection.UpdateAll(categoriesTodelete);
    }
}
