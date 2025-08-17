using ExpenseTracker.Models.DbEntities;
using SQLite;
using System.Linq.Expressions;

namespace ExpenseTracker.Database;

public class ExpenseTable
{
    SQLiteConnection _connection;

    public ExpenseTable(SQLiteConnection sQLiteAsyncConnection)
    {
        _connection = sQLiteAsyncConnection;
    }

    public void Add(List<ExpenseEntity> entities)
    {
        _connection.InsertAll(entities);
    }

    public void Add(ExpenseEntity entity)
    {
        _connection.Insert(entity);
    }

    public List<ExpenseEntity> GetAll()
    {
        var list = _connection.Table<ExpenseEntity>().ToList();
        return list;
    }

    public List<ExpenseEntity> GetAllDeleted()
    {
        var list = _connection.Table<ExpenseEntity>().Where(x=>x.IsDeleted && !x.IsCategoryDeleted).ToList();
        return list;
    }

    public async Task<List<ExpenseEntity>> Get(DateTime startDate, DateTime endDate)
    {
        var list = _connection.Table<ExpenseEntity>().Where(x => x.Date >= startDate && x.Date <= endDate && !x.IsDeleted && !x.IsCategoryDeleted).ToList();
        return list;
    }

    public async Task<List<ExpenseEntity>> Where(Expression<Func<ExpenseEntity, bool>> predicate)
    {
        var list = _connection.Table<ExpenseEntity>().Where(predicate).ToList();
        return list;
    }

    public async Task<ExpenseEntity> Get(long ID)
    {
        var item = _connection.Table<ExpenseEntity>().First(x => x.ID == ID);
        return item;
    }

    public async Task Update(ExpenseEntity entity)
    {
        _connection.Update(entity);
    }

    public async Task UpdateAll(List<ExpenseEntity> entity)
    {
        _connection.UpdateAll(entity);
    }

    public void SoftDelete(ExpenseEntity entity)
    {
        entity.IsDeleted = true;
        _connection.Update(entity);
    }

    public List<ExpenseEntity> Get(string categoryName)
    {
        var list = _connection.Table<ExpenseEntity>().Where(x => x.Category == categoryName).ToList();
        return list;
    }
}
