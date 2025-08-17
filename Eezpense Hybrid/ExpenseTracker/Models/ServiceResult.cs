using System;

namespace ExpenseTracker.Models;

public class ServiceResult
{
    public ServiceResult(bool isError = false)
    {
        this.isError = isError;
    }

    bool isError;
    public bool IsError { get => isError; }
}

public class ServiceResult<T> : ServiceResult
{
    public ServiceResult(T value, bool isError = false) : base(isError)
    {
        this.value = value;
    }

    T value;
    public T Value { get => this.value; }
}
