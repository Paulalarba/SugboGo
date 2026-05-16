using SugboGo.Models;

namespace SugboGo.Services.BookingOptions;

public interface IBookingOptionsService
{
    List<BookingActivityOption> GetActivities();
    List<BookingAccommodationOption> GetAccommodations();
    List<BookingTransportOption> GetTransportOptions();
}

public sealed class BookingOptionsService : IBookingOptionsService
{
    public List<BookingActivityOption> GetActivities() =>
    [
        new() { Id = "canyoneering", Name = "Guided canyoneering adventure", Price = 2100m, Description = "Full-day Badian route with cliff jumps, lunch, and certified guides." },
        new() { Id = "food-crawl", Name = "Local street food crawl", Price = 800m, Description = "Evening Carbon market & Parian tour with a local food expert." },
        new() { Id = "island-hopping", Name = "Experience-driven island hopping", Price = 3500m, Description = "Private boat to Olango & Gilutungan with snorkeling gear and fresh seafood lunch." },
        new() { Id = "heritage-walk", Name = "Cultural heritage walk", Price = 600m, Description = "3-hour guided walk through Colon, Fort San Pedro, and old museums." },
        new() { Id = "private-guide", Name = "Private photography guide", Price = 1800m, Description = "A local guide focused on the best angles, timing, and local stories." }
    ];

    public List<BookingAccommodationOption> GetAccommodations() =>
    [
        new() { Id = "flash-hostel", Name = "Flashpacker hostel (Dorm)", Type = "Hostel", PricePerNight = 0m, Amenities = "AC dorm, lockers, high-speed WiFi, social lounge", Distance = "Near central transport", Rating = "4.6", ImageUrl = "/images/hero-bg.jpg" },
        new() { Id = "boutique-hostel", Name = "Boutique hostel (Private Room)", Type = "Boutique Hostel", PricePerNight = 2200m, Amenities = "Private AC room, ensuite, WiFi, breakfast", Distance = "10 min from pickup", Rating = "4.8", ImageUrl = "/images/hero-bg.jpg" },
        new() { Id = "glamping", Name = "Eco-glamping tent", Type = "Glamping", PricePerNight = 2800m, Amenities = "Luxury tent, mountain views, shared bath, bonfire site", Distance = "On-site at destination", Rating = "4.7", ImageUrl = "/images/hero-bg.jpg" },
        new() { Id = "coliving", Name = "Cebu Co-living space", Type = "Co-living", PricePerNight = 1500m, Amenities = "Private room, dedicated workspace, community kitchen", Distance = "Urban hub access", Rating = "4.5", ImageUrl = "/images/hero-bg.jpg" }
    ];

    public List<BookingTransportOption> GetTransportOptions() =>
    [
        new() { Id = "p2p-bus", Name = "Premium P2P Bus", Price = 0m, Details = "Guaranteed seat in an air-conditioned express coach." },
        new() { Id = "shared-van", Name = "Shared van transfer", Price = 400m, Details = "Door-to-door pickup shared with other travelers." },
        new() { Id = "ferry", Name = "Ferry & port coordination", Price = 1500m, Details = "Fast-craft tickets and terminal fee support." },
        new() { Id = "motorbike", Name = "Motorbike rental (24h)", Price = 600m, Details = "Fuel-efficient scooter for independent exploration." },
        new() { Id = "private-car", Name = "Private car (Speed focus)", Price = 2500m, Details = "Fastest route, direct drop-off, flexible schedule." }
    ];
}
