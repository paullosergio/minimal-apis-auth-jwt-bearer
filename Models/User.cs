using MongoDB.Bson;

namespace MinimalApiAuth.Models

{
    public record User (int Id, string Username, int Password);
}
