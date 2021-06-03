namespace Firenet
{
    public interface IFireCollection<TEntity> :
        ICommandSync<TEntity>, ICommandAsync<TEntity>, IQuery<TEntity>
        where TEntity : class
    { }
}
