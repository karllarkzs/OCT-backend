namespace PharmaBack.WebApi.Models;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}
