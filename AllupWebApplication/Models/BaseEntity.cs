using System;

namespace AllupWebApplication.Models;

public class BaseEntity
{
    public int Id { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; }

    /// <summary>
    /// Updates the ModifiedDate to the current UTC datetime.
    /// This should be called only when an update occurs.
    /// </summary>
    public void UpdateModifiedDate()
    {
        ModifiedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the CreatedDate to the current UTC datetime.
    /// This should only be called at the time of creation of the entity.
    /// </summary>
    public void SetCreatedDate()
    {
        if (CreatedDate == default)
        {
            CreatedDate = DateTime.UtcNow;
        }
    }
}
