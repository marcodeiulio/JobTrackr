using JobTrackr.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JobTrackr.Application.Tests.Common;

public static class Helpers
{
    public static ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid()
                .ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}