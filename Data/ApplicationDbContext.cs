
using CinemaBooking.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDBContext: DbContext
{
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
    { 
    }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<Cinema> Cinemas { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<ShowTime> showTimes { get; set; }
    public DbSet<User> users { get; set; }
}