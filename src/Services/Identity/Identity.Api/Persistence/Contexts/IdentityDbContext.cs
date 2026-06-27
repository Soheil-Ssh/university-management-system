using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Persistence.Contexts;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    
}