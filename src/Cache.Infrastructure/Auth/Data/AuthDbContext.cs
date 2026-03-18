using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DioRed.Cache.Infrastructure.Auth.Data;

public class AuthDbContext(
    DbContextOptions<AuthDbContext> options
) : IdentityDbContext(options)
{
}