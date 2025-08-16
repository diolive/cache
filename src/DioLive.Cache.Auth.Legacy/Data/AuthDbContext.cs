using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DioLive.Cache.Auth.Legacy.Data;

public class AuthDbContext(
    DbContextOptions<AuthDbContext> options
) : IdentityDbContext(options)
{
}