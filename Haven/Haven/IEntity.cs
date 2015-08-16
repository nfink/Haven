namespace Haven
{
    public interface IEntity
    {
        int Id { get; set; }

        IRepository Repository { set; }
    }
}
