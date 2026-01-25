
namespace CinemaBooking.Data;
using CinemaBooking.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext: DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { 
    }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<Cinema> Cinemas { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<ShowTime> ShowTimes { get; set; }
    public DbSet<User> users { get; set; }
}