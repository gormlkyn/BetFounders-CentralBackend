namespace BetFounders.CentralBackend.Common.Models;

public class ServiceResult<T>
{
    public bool IsSuccess { get; set; }

    public string ErrorMessage { get; set; }

    public T Data { get; set; }

    public static ServiceResult<T> Success(T data)
    {
        return new() { IsSuccess = true, Data = data };
    } 

    public static ServiceResult<T> Failure(string error)
    {
        return new() { IsSuccess = false, ErrorMessage = error };
    }
}