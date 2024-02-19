namespace SimpleWebApiAuth.Infrastructure
{
    public interface ICrudService<T>
    {
        Task<IEnumerable<T>> GetAsync();
        Task<T?> GetAsync(string id);
        Task CreateAsync(T newObject);
        Task UpdateAsync(string id, T updatedObject);
        Task RemoveAsync(string id);
    }
}
