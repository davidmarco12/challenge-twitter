namespace TwitterAPI.Domain.Abstractions
{
    public abstract class Entity
    {
        protected Entity()
        {
        }

        public int Id { get; init; }

        public DateTime CreationDate { get; init; } = DateTime.Now;
    }
}