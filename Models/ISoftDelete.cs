namespace PharmaBack.Models;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}
