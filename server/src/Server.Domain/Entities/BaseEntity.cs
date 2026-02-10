namespace Server.Domain.Entities;

public class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDeleted => DeletedAt.HasValue;

    public virtual void SoftDelete()
    {
        DeletedAt = DateTime.UtcNow;
        IsActive = false;
    }

    public virtual void Restore()
    {
        DeletedAt = null;
        IsActive = true;
    }
}
