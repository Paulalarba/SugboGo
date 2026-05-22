using SugboGo.Models;

namespace SugboGo.Services.BookingOptions;

public interface IBookingOptionsService
{
    List<BookingActivityOption> GetActivities(TravelSpot? destination = null);
    List<BookingAccommodationOption> GetAccommodations(TravelSpot? destination = null);
    List<BookingTransportOption> GetTransportOptions(TravelSpot? destination = null);
}

public sealed class BookingOptionsService : IBookingOptionsService
{
    public List<BookingActivityOption> GetActivities(TravelSpot? destination = null)
    {
        if (IsBantayan(destination))
        {
            return
            [
                Activity("bantayan-virgin-island", "Virgin Island hopping", 1600m, "Shared outrigger from Santa Fe with snorkeling stops and beach time."),
                Activity("bantayan-ogtong-kota", "Ogtong Cave and Kota Beach land tour", 650m, "Local guide, cave swim coordination, Kota sandbar stop, and island tricycle routing."),
                Activity("bantayan-snorkel", "Bantayan reef snorkeling", 500m, "Mask, vest, local boatman, and beginner-friendly reef guidance near Santa Fe."),
                Activity("bantayan-sunset-picnic", "Santa Fe sunset beach picnic", 350m, "Beach mat setup, local snacks, drinks, and golden-hour photo help."),
                Activity("bantayan-seafood", "Bantayan seafood grill add-on", 550m, "Fresh seafood dinner coordination with a trusted Santa Fe seaside cook.")
            ];
        }

        if (IsMalapascua(destination))
        {
            return
            [
                Activity("malapascua-thresher", "Thresher shark dive coordination", 2500m, "Monad Shoal dive slot with local dive shop coordination for certified divers."),
                Activity("malapascua-snorkel", "Coral garden snorkeling", 650m, "Boat-assisted snorkeling around reef spots near Malapascua."),
                Activity("malapascua-island-walk", "Bounty Beach to lighthouse walk", 350m, "Slow guided island walk with sunset timing and photo stops."),
                Activity("malapascua-kalanggaman", "Kalanggaman day trip assist", 2800m, "Shared boat coordination, registration support, and picnic lunch planning.")
            ];
        }

        if (IsCamotes(destination))
        {
            return
            [
                Activity("camotes-lake-kayak", "Lake Danao kayak session", 450m, "Kayak rental, lakeside guide, and relaxed freshwater sightseeing."),
                Activity("camotes-cave-loop", "Camotes cave and beach loop", 850m, "Bukilat or nearby cave visit paired with Santiago Bay or Mangodlong beach time."),
                Activity("camotes-cliff-swim", "Cliff and swim spots tour", 750m, "Local route through safe swimming viewpoints and coastal photo stops."),
                Activity("camotes-sunset", "Santiago Bay sunset picnic", 350m, "Simple beach picnic setup with local snacks and sunset timing.")
            ];
        }

        if (IsIslandOrBeach(destination))
        {
            return
            [
                Activity("local-island-hop", $"{ShortPlace(destination)} island-hopping route", 1500m, $"Shared boat route around {ShortPlace(destination)} with swim stops and seafood lunch planning."),
                Activity("shore-snorkel", "Guided shore snorkeling", 500m, "Beginner-friendly snorkeling with gear, safety briefing, and local reef guide."),
                Activity("beach-picnic", "Beach picnic setup", 350m, "Mat, drinks, local snacks, and shaded beach setup near the destination."),
                Activity("sunset-photo", "Sunset photo walk", 300m, "Local guide for the best beach corners and golden-hour timing.")
            ];
        }

        if (IsWaterfall(destination))
        {
            return
            [
                Activity("falls-guide", $"{ShortPlace(destination)} waterfall guide", 450m, "Local guide for trail access, safe swim areas, and timing."),
                Activity("river-trek", "River trek and swim assist", 650m, "Guided river approach with safety briefing and dry bag support."),
                Activity("canyoneering-lite", "Soft canyoneering add-on", 1200m, "Beginner-friendly jumps and stream route where locally permitted."),
                Activity("falls-picnic", "Waterfall picnic pack", 300m, "Packed local snacks and drinks for your waterfall stop.")
            ];
        }

        if (IsMountainOrViewpoint(destination))
        {
            return
            [
                Activity("trail-guide", $"{ShortPlace(destination)} trail guide", 450m, "Local trail guide with pacing support and weather-aware start time."),
                Activity("sunrise-hike", "Sunrise hike timing", 700m, "Early pickup coordination, headlamp support, and summit photo stops."),
                Activity("ridge-picnic", "Ridge picnic pack", 300m, "Light breakfast or snack pack for the viewpoint stop."),
                Activity("highland-photo", "Highland photography guide", 550m, "Guide focused on scenic viewpoints and group photos.")
            ];
        }

        if (IsCityHeritage(destination))
        {
            return
            [
                Activity("heritage-guide", $"{ShortPlace(destination)} heritage guide", 350m, "Context-rich walk through the destination and nearby heritage stops."),
                Activity("museum-pass", "Museum or shrine entry assist", 250m, "Entrance coordination and route timing for nearby cultural sites."),
                Activity("local-food-stop", "Nearby local food stop", 300m, "Short food detour matched to the area instead of a generic city crawl."),
                Activity("heritage-photo", "Heritage photo walk", 400m, "Best angles, quiet corners, and local story prompts for photos.")
            ];
        }

        if (IsResortOrPark(destination))
        {
            return
            [
                Activity("day-pass-assist", $"{ShortPlace(destination)} day-pass assist", 400m, "Entrance, cabana, or day-use coordination where available."),
                Activity("pool-cabana", "Cabana or table reservation", 700m, "Reserved base spot for bags, food, and group breaks."),
                Activity("family-photo", "Family photo session", 500m, "Short guided photo session inside the resort or park."),
                Activity("meal-voucher", "On-site meal coordination", 500m, "Meal slot or voucher assistance with the venue partner.")
            ];
        }

        return
        [
            Activity("local-guide", $"{ShortPlace(destination)} local guide", 400m, "Area-specific guide support for timing, photos, and local context."),
            Activity("nearby-food", "Nearby food stop", 300m, "Food stop selected near the booked destination."),
            Activity("photo-assist", "Photo and route assistant", 450m, "Help with best angles, pacing, and a smooth route around the area.")
        ];
    }

    public List<BookingAccommodationOption> GetAccommodations(TravelSpot? destination = null)
    {
        if (IsBantayan(destination))
        {
            return
            [
                Stay("bantayan-santa-fe-hostel", "Santa Fe beach hostel (Dorm)", "Hostel", 0m, "AC dorm, lockers, shared kitchen, walkable to beach", "Santa Fe, Bantayan Island", "4.6", "/images/153.jpg"),
                Stay("bantayan-cabin", "Bantayan native beach cabin", "Cabin", 900m, "Fan or AC room, private bath, porch, near local eateries", "Inside Bantayan Island", "4.7", "/images/456.jpg"),
                Stay("bantayan-kota-room", "Kota Beach area private room", "Beach Room", 1500m, "Private room, breakfast option, near the sandbar", "Kota Beach, Bantayan Island", "4.8", "/images/456.jpg"),
                Stay("bantayan-ogtong-resort", "Ogtong Cave resort stay", "Resort", 2300m, "Pool access, cave swim access, private room", "Bantayan Island", "4.7", "/images/457.jpg")
            ];
        }

        if (IsMalapascua(destination))
        {
            return
            [
                Stay("malapascua-bounty-hostel", "Bounty Beach dive hostel", "Dive Hostel", 0m, "Fan dorm, dive shop nearby, shared lounge", "Bounty Beach, Malapascua", "4.5", "/images/154.jpg"),
                Stay("malapascua-dive-room", "Malapascua dive lodge room", "Dive Lodge", 1500m, "Private room, gear rinse area, early dive breakfast", "Inside Malapascua Island", "4.7", "/images/161.jpg"),
                Stay("malapascua-beach-cottage", "Langub Beach cottage", "Cottage", 2100m, "Quiet cottage, private bath, beach access", "Langub Beach, Malapascua", "4.8", "/images/165.jpg")
            ];
        }

        if (IsCamotes(destination))
        {
            return
            [
                Stay("camotes-lakeside-inn", "Lake or bay-side inn", "Inn", 0m, "Simple private room, fan or AC, local breakfast option", "Inside Camotes Islands", "4.5", "/images/166.jpg"),
                Stay("camotes-santiago-room", "Santiago Bay beach room", "Beach Room", 1200m, "Private bath, beach access, family-friendly area", "Santiago Bay, Camotes", "4.7", "/images/166.jpg"),
                Stay("camotes-resort", "Camotes resort cottage", "Resort Cottage", 2100m, "Pool access, private room, near island tour pickup", "Camotes Islands", "4.8", "/images/458.jpg")
            ];
        }

        if (IsIslandOrBeach(destination))
        {
            return
            [
                Stay("beach-hostel", $"{ShortPlace(destination)} beach hostel", "Hostel", 0m, "Simple bed, lockers, shared bath, close to shore", $"Near {ShortPlace(destination)}", "4.5", destination?.ImageUrl ?? "/images/hero-bg.jpg"),
                Stay("beach-cabin", $"{ShortPlace(destination)} beach cabin", "Cabin", 1100m, "Private room, local host, beach access", $"Inside {ShortPlace(destination)} area", "4.7", destination?.ImageUrl ?? "/images/hero-bg.jpg"),
                Stay("beach-resort", $"{ShortPlace(destination)} resort room", "Resort", 2200m, "Private bath, pool or beach access, breakfast option", $"Near {ShortPlace(destination)}", "4.8", destination?.ImageUrl ?? "/images/hero-bg.jpg")
            ];
        }

        if (IsWaterfall(destination) || IsMountainOrViewpoint(destination))
        {
            return
            [
                Stay("trail-base-inn", $"{ShortPlace(destination)} local inn", "Local Inn", 0m, "Simple room, early breakfast, close to pickup route", $"Near {ShortPlace(destination)}", "4.5", destination?.ImageUrl ?? "/images/hero-bg.jpg"),
                Stay("nature-cabin", "Nature cabin stay", "Cabin", 1000m, "Private room, mountain or rural setting, guide pickup support", $"Within {destination?.Region ?? "Cebu"}", "4.7", destination?.ImageUrl ?? "/images/hero-bg.jpg"),
                Stay("eco-lodge", "Eco-lodge private room", "Eco-lodge", 1700m, "Private bath, quiet setting, breakfast option", $"Near {ShortPlace(destination)} route", "4.8", destination?.ImageUrl ?? "/images/hero-bg.jpg")
            ];
        }

        if (IsCityHeritage(destination))
        {
            return
            [
                Stay("city-hostel", "Cebu City heritage hostel", "Hostel", 0m, "AC dorm, lockers, walkable to downtown routes", "Downtown Cebu City", "4.5", "/images/15.jpg"),
                Stay("city-boutique", "Cebu City boutique room", "Boutique Hotel", 1500m, "Private room, ensuite, breakfast, near heritage district", "Cebu City", "4.7", "/images/421.jpg"),
                Stay("city-hotel", "Business hotel near route", "Hotel", 2300m, "Private room, elevator access, concierge support", "Cebu City", "4.8", "/images/421.jpg")
            ];
        }

        return
        [
            Stay("nearby-standard", $"Nearby stay for {ShortPlace(destination)}", "Standard Room", 0m, "Simple room selected near your booked destination", $"Near {ShortPlace(destination)}", "4.5", destination?.ImageUrl ?? "/images/hero-bg.jpg"),
            Stay("nearby-private", $"Private room near {ShortPlace(destination)}", "Private Room", 1100m, "Private bath, local host or hotel partner", $"Near {ShortPlace(destination)}", "4.7", destination?.ImageUrl ?? "/images/hero-bg.jpg"),
            Stay("nearby-comfort", $"Comfort stay near {ShortPlace(destination)}", "Comfort Hotel", 1900m, "Upgraded private room with breakfast option", $"Near {ShortPlace(destination)}", "4.8", destination?.ImageUrl ?? "/images/hero-bg.jpg")
        ];
    }

    public List<BookingTransportOption> GetTransportOptions(TravelSpot? destination = null)
    {
        if (IsBantayan(destination))
        {
            return
            [
                Transport("bantayan-bus-ferry", "Cebu City to Hagnaya bus + ferry", 0m, "North Bus route to Hagnaya Port, ferry to Santa Fe, and arrival guidance."),
                Transport("bantayan-van-ferry", "Private van + Hagnaya ferry assist", 3200m, "Private Cebu pickup to Hagnaya plus ferry coordination to Bantayan Island."),
                Transport("bantayan-port-transfer", "Santa Fe port tricycle transfer", 200m, "Tricycle pickup from Santa Fe Port to your Bantayan stay."),
                Transport("bantayan-motorbike", "Bantayan motorbike rental (24h)", 400m, "Local scooter rental for beaches, Ogtong Cave, and Santa Fe food stops.")
            ];
        }

        if (IsMalapascua(destination))
        {
            return
            [
                Transport("malapascua-bus-boat", "Cebu City to Maya bus + boat", 0m, "North Bus route to Maya Port with boat transfer guidance to Malapascua."),
                Transport("malapascua-private-transfer", "Private car to Maya + boat assist", 3400m, "Private Cebu pickup to Maya Port plus island boat coordination."),
                Transport("malapascua-island-porter", "Island porter and beach transfer", 250m, "Arrival help from boat landing to Bounty Beach or your stay.")
            ];
        }

        if (IsCamotes(destination))
        {
            return
            [
                Transport("camotes-ferry", "Danao port ferry to Camotes", 0m, "Danao port routing, ferry timing, and arrival guidance."),
                Transport("camotes-private-port", "Private transfer to Danao port", 1800m, "Private Cebu pickup to Danao Port with ferry coordination."),
                Transport("camotes-motorbike", "Camotes motorbike rental (24h)", 450m, "Scooter rental for Lake Danao, caves, and beach routes.")
            ];
        }

        if (IsIslandOrBeach(destination))
        {
            return
            [
                Transport("coastal-shared", $"Shared transfer to {ShortPlace(destination)}", 0m, "Shared land or boat transfer matched to this coastal destination."),
                Transport("coastal-private", $"Private transfer to {ShortPlace(destination)}", 2200m, "Direct pickup with flexible departure and route support."),
                Transport("coastal-scooter", "Local scooter rental (24h)", 400m, "Independent transport for nearby beaches and food stops.")
            ];
        }

        if (IsCityHeritage(destination))
        {
            return
            [
                Transport("city-walk", "Self-guided arrival", 0m, "Meet at the destination or nearest public drop-off point."),
                Transport("city-grab-assist", "City ride-hailing assist", 150m, "Pickup pin, timing, and ride coordination inside Metro Cebu."),
                Transport("city-private-car", "Private city car loop", 1200m, "Private car for nearby Cebu City stops before or after checkout.")
            ];
        }

        return
        [
            Transport("standard-transfer", $"Standard transfer to {ShortPlace(destination)}", 0m, "Shared route or public transfer guidance matched to the destination."),
            Transport("private-transfer", $"Private transfer to {ShortPlace(destination)}", 1800m, "Direct pickup, flexible stopovers, and driver coordination."),
            Transport("local-rental", "Local motorbike rental (24h)", 400m, "Local scooter rental where available for nearby exploration.")
        ];
    }

    private static BookingActivityOption Activity(string id, string name, decimal price, string description) =>
        new() { Id = id, Name = name, Price = price, Description = description };

    private static BookingAccommodationOption Stay(
        string id,
        string name,
        string type,
        decimal price,
        string amenities,
        string distance,
        string rating,
        string imageUrl) =>
        new() { Id = id, Name = name, Type = type, PricePerNight = price, Amenities = amenities, Distance = distance, Rating = rating, ImageUrl = imageUrl };

    private static BookingTransportOption Transport(string id, string name, decimal price, string details) =>
        new() { Id = id, Name = name, Price = price, Details = details };

    private static bool IsBantayan(TravelSpot? destination) =>
        Contains(destination, "Bantayan") || Contains(destination, "Santa Fe") || Contains(destination, "Kota Beach") || Contains(destination, "Ogtong");

    private static bool IsMalapascua(TravelSpot? destination) =>
        Contains(destination, "Malapascua") || Contains(destination, "Bounty Beach") || Contains(destination, "Langub Beach");

    private static bool IsCamotes(TravelSpot? destination) =>
        Contains(destination, "Camotes") || Contains(destination, "Lake Danao") || Contains(destination, "Bukilat") || Contains(destination, "Santiago Bay");

    private static bool IsIslandOrBeach(TravelSpot? destination) =>
        IsCategory(destination, "Island") || IsCategory(destination, "Beach") || IsCategory(destination, "Marine Sanctuary") || IsCategory(destination, "Diving");

    private static bool IsWaterfall(TravelSpot? destination) =>
        IsCategory(destination, "Waterfall") || IsCategory(destination, "Spring");

    private static bool IsMountainOrViewpoint(TravelSpot? destination) =>
        IsCategory(destination, "Mountain") || IsCategory(destination, "Viewpoint") || IsCategory(destination, "Nature Park");

    private static bool IsCityHeritage(TravelSpot? destination) =>
        IsCategory(destination, "Historical") ||
        IsCategory(destination, "Religious") ||
        IsCategory(destination, "Museum") ||
        IsCategory(destination, "Monument") ||
        IsCategory(destination, "Landmark") ||
        IsCategory(destination, "Market") ||
        IsCategory(destination, "Street") ||
        Contains(destination, "Cebu City");

    private static bool IsResortOrPark(TravelSpot? destination) =>
        IsCategory(destination, "Resort") ||
        IsCategory(destination, "Park") ||
        IsCategory(destination, "Theme Park") ||
        IsCategory(destination, "Water Park") ||
        IsCategory(destination, "Wildlife") ||
        IsCategory(destination, "Adventure");

    private static bool IsCategory(TravelSpot? destination, string category) =>
        string.Equals(destination?.Category, category, StringComparison.OrdinalIgnoreCase);

    private static bool Contains(TravelSpot? destination, string text)
    {
        return (destination?.Name?.Contains(text, StringComparison.OrdinalIgnoreCase) == true) ||
            (destination?.Location?.Contains(text, StringComparison.OrdinalIgnoreCase) == true) ||
            (destination?.Region?.Contains(text, StringComparison.OrdinalIgnoreCase) == true);
    }

    private static string ShortPlace(TravelSpot? destination)
    {
        var name = destination?.Name;
        return string.IsNullOrWhiteSpace(name) ? "your destination" : name;
    }
}
