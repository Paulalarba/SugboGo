using SugboGo.Models;

namespace SugboGo.Data;

public static class TravelSpotSeedData
{
    public static IReadOnlyList<TravelSpot> GetTravelSpots()
    {
        TravelSpot[] spots =
        [
        new() { Id = 1, Name = "Magellan's Cross", Location = "Cebu City", Description = "Historical cross planted by Ferdinand Magellan in 1521.", Category = "Historical", Region = "Cebu City", IsPopular = true },
        new() { Id = 2, Name = "Basilica del Santo Nino", Location = "Cebu City", Description = "Oldest Roman Catholic church in the Philippines.", Category = "Religious", Region = "Cebu City", IsPopular = true },
        new() { Id = 3, Name = "Fort San Pedro", Location = "Cebu City", Description = "Smallest triangular fort in the Philippines.", Category = "Historical", Region = "Cebu City", IsPopular = true },
        new() { Id = 4, Name = "Cebu Taoist Temple", Location = "Beverly Hills, Cebu City", Description = "Scenic Taoist temple with city views.", Category = "Religious", Region = "Cebu City", IsPopular = true },
        new() { Id = 5, Name = "Temple of Leah", Location = "Busay, Cebu City", Description = "Grand monument of love with European architecture.", Category = "Monument", Region = "Cebu City", IsPopular = true },
        new() { Id = 6, Name = "Sirao Garden", Location = "Busay, Cebu City", Description = "Flower garden known as Little Amsterdam.", Category = "Garden", Region = "Cebu City", IsPopular = true },
        new() { Id = 7, Name = "Tops Lookout", Location = "Busay, Cebu City", Description = "Panoramic viewpoint of Cebu City.", Category = "Viewpoint", Region = "Cebu City", IsPopular = true },
        new() { Id = 8, Name = "Heritage of Cebu Monument", Location = "Parian, Cebu City", Description = "Sculptural tableau of Cebu history.", Category = "Monument", Region = "Cebu City" },
        new() { Id = 9, Name = "Yap-Sandiego Ancestral House", Location = "Cebu City", Description = "Oldest ancestral house in Cebu.", Category = "Museum", Region = "Cebu City" },
        new() { Id = 10, Name = "Casa Gorordo Museum", Location = "Cebu City", Description = "Restored 19th-century house museum.", Category = "Museum", Region = "Cebu City" },
        new() { Id = 11, Name = "Cebu Metropolitan Cathedral", Location = "Cebu City", Description = "Historic cathedral in the city center.", Category = "Religious", Region = "Cebu City" },
        new() { Id = 12, Name = "Fuente Osmena Circle", Location = "Cebu City", Description = "Iconic roundabout and park.", Category = "Park", Region = "Cebu City" },
        new() { Id = 13, Name = "Colon Street", Location = "Cebu City", Description = "Oldest street in the Philippines.", Category = "Street", Region = "Cebu City" },
        new() { Id = 14, Name = "Mactan Shrine", Location = "Lapu-Lapu City", Description = "Monument to Lapu-Lapu.", Category = "Historical", Region = "Mactan" },
        new() { Id = 15, Name = "National Museum of the Philippines - Cebu", Location = "Cebu City", Description = "Showcases Cebu heritage and artifacts.", Category = "Museum", Region = "Cebu City" },
        new() { Id = 16, Name = "Cebu Provincial Capitol", Location = "Cebu City", Description = "Neoclassical government building.", Category = "Landmark", Region = "Cebu City" },
        new() { Id = 17, Name = "Ayala Center Cebu", Location = "Cebu City", Description = "Major shopping mall.", Category = "Shopping", Region = "Cebu City" },
        new() { Id = 18, Name = "SM Seaside City Cebu", Location = "Cebu City", Description = "Large mall and entertainment complex.", Category = "Shopping", Region = "Cebu City" },
        new() { Id = 19, Name = "Cebu IT Park", Location = "Cebu City", Description = "Modern business and lifestyle district.", Category = "Urban", Region = "Cebu City" },
        new() { Id = 20, Name = "Cebu Ocean Park", Location = "Cebu City", Description = "Marine theme park.", Category = "Wildlife", Region = "Cebu City" },
        new() { Id = 21, Name = "Carbon Market", Location = "Cebu City", Description = "Historic public market and local food stop.", Category = "Market", Region = "Cebu City" },
        new() { Id = 22, Name = "10,000 Roses Cafe", Location = "Cordova", Description = "Seaside cafe known for illuminated white rose installations.", Category = "Cafe", Region = "Mactan" },
        new() { Id = 23, Name = "Anjo World Theme Park", Location = "Minglanilla", Description = "Theme park with rides and family attractions.", Category = "Theme Park", Region = "Metro Cebu" },

        new() { Id = 51, Name = "Kawasan Falls", Location = "Badian", Description = "Iconic turquoise multi-tiered waterfalls.", Category = "Waterfall", Region = "South Cebu", IsPopular = true },
        new() { Id = 52, Name = "Inambakan Falls", Location = "Alegria", Description = "Multi-level falls with swimming and trekking.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 53, Name = "Mantayupan Falls", Location = "Barangay Mantayupan", Description = "Tall powerful waterfall with pools.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 54, Name = "Tumalog Falls", Location = "Oslob", Description = "Curtain-like waterfall.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 55, Name = "Aguinid Falls", Location = "Samboan", Description = "Series of cascading falls.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 56, Name = "Kabutongan Falls", Location = "South Cebu", Description = "Deep blue pool with cave and cliff jump.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 57, Name = "Dao Falls", Location = "South Cebu", Description = "Epic canyon hike waterfall.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 58, Name = "Cambais Falls", Location = "South Cebu", Description = "Scenic falls for swimming.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 59, Name = "Binalayan Hidden Falls", Location = "South Cebu", Description = "Claw-like cascades for cliff diving.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 60, Name = "Cancalanog Falls", Location = "South Cebu", Description = "Off-the-beaten-path stunning falls.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 61, Name = "Lusno Falls", Location = "Ronda", Description = "Local waterfall known for swimming and a quieter rural setting.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 62, Name = "Tinubdan Falls", Location = "Catmon", Description = "Northern Cebu waterfall with natural pools.", Category = "Waterfall", Region = "North Cebu" },
        new() { Id = 63, Name = "Kanlaob River Falls", Location = "Alegria", Description = "River canyon waterfall route used for trekking and water activities.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 64, Name = "Bugnawan Falls", Location = "South Cebu", Description = "Local waterfall stop for swimming and nature trips.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 65, Name = "Kampael Falls", Location = "South Cebu", Description = "Scenic local waterfall for adventure itineraries.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 66, Name = "Bucawe Falls", Location = "South Cebu", Description = "Hidden waterfall suited for local-guided routes.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 67, Name = "Montpeller Falls", Location = "Alegria", Description = "Waterfall and natural pool experience in South Cebu.", Category = "Waterfall", Region = "South Cebu" },
        new() { Id = 68, Name = "Dau Falls", Location = "Samboan", Description = "Tall waterfall reached by a scenic river trek.", Category = "Waterfall", Region = "South Cebu" },

        new() { Id = 151, Name = "Panagsama Beach", Location = "Moalboal", Description = "Famous for sardine run and diving.", Category = "Beach", Region = "South Cebu", IsPopular = true },
        new() { Id = 152, Name = "Sumilon Island", Location = "Oslob", Description = "White sandbar and marine sanctuary.", Category = "Island", Region = "South Cebu", IsPopular = true },
        new() { Id = 153, Name = "Bantayan Island", Location = "North Cebu", Description = "Pristine beaches and relaxed vibe.", Category = "Island", Region = "North Cebu" },
        new() { Id = 154, Name = "Malapascua Island", Location = "North Cebu", Description = "Diving paradise with thresher sharks.", Category = "Island", Region = "North Cebu", IsPopular = true },
        new() { Id = 155, Name = "Lambug Beach", Location = "Badian", Description = "Quiet white sand beach.", Category = "Beach", Region = "South Cebu" },
        new() { Id = 156, Name = "Tingko Beach", Location = "Alcoy", Description = "Scenic beach in south Cebu.", Category = "Beach", Region = "South Cebu" },
        new() { Id = 157, Name = "Hermit's Cove", Location = "Aloguinsan", Description = "Hidden picturesque cove.", Category = "Beach", Region = "South Cebu" },
        new() { Id = 158, Name = "Basdaku Beach", Location = "Moalboal", Description = "White sand beach known for longer stays and group trips.", Category = "Beach", Region = "South Cebu" },
        new() { Id = 159, Name = "Carnaza Island", Location = "North Cebu", Description = "Untouched island beauty.", Category = "Island", Region = "North Cebu" },
        new() { Id = 160, Name = "Pescador Island", Location = "Moalboal", Description = "Great for snorkeling and diving.", Category = "Island", Region = "South Cebu" },
        new() { Id = 161, Name = "Bounty Beach", Location = "Malapascua", Description = "Beachfront area popular with divers visiting Malapascua.", Category = "Beach", Region = "North Cebu" },
        new() { Id = 162, Name = "Paradise Beach", Location = "Bantayan Island", Description = "Quiet beach stop on Bantayan Island.", Category = "Beach", Region = "North Cebu" },
        new() { Id = 163, Name = "White Beach Moalboal", Location = "Moalboal", Description = "Long beach area used for swimming and sunsets.", Category = "Beach", Region = "South Cebu" },
        new() { Id = 164, Name = "Vano Beach", Location = "Mactan", Description = "Accessible beach area in Mactan.", Category = "Beach", Region = "Mactan" },
        new() { Id = 165, Name = "Langub Beach", Location = "Malapascua", Description = "Quiet beach area on Malapascua Island.", Category = "Beach", Region = "North Cebu" },
        new() { Id = 166, Name = "Santiago Bay", Location = "Camotes Islands", Description = "Wide beach bay in Camotes.", Category = "Beach", Region = "Camotes" },
        new() { Id = 167, Name = "Olango Island", Location = "Lapu-Lapu City", Description = "Island known for bird sanctuary visits and coastal routes.", Category = "Island", Region = "Mactan" },

        new() { Id = 301, Name = "Osmena Peak", Location = "Dalaguete", Description = "Highest point in Cebu with dramatic karst views.", Category = "Mountain", Region = "South Cebu", IsPopular = true },
        new() { Id = 302, Name = "Casino Peak (Lugsangan Peak)", Location = "Dalaguete", Description = "Chocolate Hills-like limestone formations.", Category = "Viewpoint", Region = "South Cebu" },
        new() { Id = 303, Name = "Kandungaw Peak", Location = "South Cebu", Description = "Lesser-known hiking peak.", Category = "Mountain", Region = "South Cebu" },
        new() { Id = 304, Name = "Mt. Naupa", Location = "Naga", Description = "Accessible mountain trail with ridge views.", Category = "Mountain", Region = "South Cebu" },
        new() { Id = 305, Name = "Mt. Kan-Irag (Sirao Peak)", Location = "Cebu City", Description = "Highland hiking route near Sirao.", Category = "Mountain", Region = "Cebu City" },
        new() { Id = 306, Name = "Mt. Babag", Location = "Cebu City", Description = "Mountain trail and viewpoint near the city.", Category = "Mountain", Region = "Cebu City" },
        new() { Id = 307, Name = "Bocaue Peak", Location = "Cebu", Description = "Viewpoint and local hiking destination.", Category = "Viewpoint", Region = "Cebu" },
        new() { Id = 308, Name = "Dalas-ag Peak", Location = "Cebu", Description = "Scenic mountain peak for guided hikes.", Category = "Mountain", Region = "Cebu" },

        new() { Id = 401, Name = "Oslob Whale Sharks", Location = "Oslob", Description = "Swim with gentle whale sharks.", Category = "Wildlife", Region = "South Cebu", IsPopular = true },
        new() { Id = 402, Name = "Cebu Safari and Adventure Park", Location = "Carmen", Description = "Wildlife park with adventure activities.", Category = "Wildlife", Region = "North Cebu" },
        new() { Id = 403, Name = "Simala Shrine", Location = "Sibonga", Description = "Castle-like pilgrimage church and shrine.", Category = "Religious", Region = "South Cebu", IsPopular = true },
        new() { Id = 404, Name = "Bojo River Eco Tour", Location = "Aloguinsan", Description = "Community-led river cruise and eco-tour experience.", Category = "Eco Tour", Region = "South Cebu" },
        new() { Id = 405, Name = "Don Emilio Botanical Garden", Location = "Busay, Cebu City", Description = "Botanical garden and mountain leisure stop.", Category = "Garden", Region = "Cebu City" },
        new() { Id = 406, Name = "Carcar Heritage Houses", Location = "Carcar", Description = "Ancestral houses and heritage district in Carcar.", Category = "Historical", Region = "South Cebu" },
        new() { Id = 407, Name = "Argao Church", Location = "Argao", Description = "Historic church in southern Cebu.", Category = "Religious", Region = "South Cebu" },
        new() { Id = 408, Name = "Boljoon Church", Location = "Boljoon", Description = "Historic church and heritage landmark.", Category = "Religious", Region = "South Cebu" },

        new() { Id = 409, Name = "Sky Adventure Cebu", Location = "Cebu City", Description = "Rooftop amusement facility atop Crown Regency Hotel featuring Sky Walk, Edge Coaster, Tower Zip, and 6D cinema — 127 meters above the city.", Category = "Theme Park", Region = "Cebu City" },
        new() { Id = 410, Name = "Mountain View Nature Park", Location = "Cebu", Description = "Highland nature park in Busay with panoramic city views, swimming pools, rope courses, mini zoo, botanical garden, and camping facilities.", Category = "Nature Park", Region = "Cebu" },
        new() { Id = 411, Name = "Neri's Ville", Location = "Cebu", Description = "Bali-inspired selfie park in Barangay Lusaran featuring recreated Wanagiri Bird's Nest and Hobbit House set against scenic mountain backdrops.", Category = "Resort", Region = "Cebu" },
        new() { Id = 412, Name = "Strawberry de Cantipla Eco Farm", Location = "Cebu City", Description = "Strawberry farm in Barangay Tabunan along the Transcentral Highway offering pick-your-own strawberry experience and strawberry-flavored treats.", Category = "Farm", Region = "Cebu City" },
        new() { Id = 413, Name = "Terrazas de Flores", Location = "Cebu", Description = "Flower terraces botanical garden in Malubog featuring 127 tropical flora species, cabanas, viewing decks, and a hillside café.", Category = "Garden", Region = "Cebu" },
        new() { Id = 414, Name = "Museo Sugbo", Location = "Cebu City", Description = "Cebu's provincial museum in the restored 1870 Spanish colonial jail, with 14 galleries covering Pre-Colonial through WWII Cebuano history.", Category = "Museum", Region = "Cebu City" },
        new() { Id = 415, Name = "1730 Jesuit House", Location = "Cebu City", Description = "One of the oldest houses in the Philippines, hidden inside a Parian hardware compound. Built in 1730 as the Jesuit headquarters, now a heritage museum.", Category = "Historical", Region = "Cebu City" },
        new() { Id = 416, Name = "Sugbo Mercado", Location = "Cebu City", Description = "Cebu's largest weekly open-air food market inside IT Park featuring 40+ rotating stalls of local and international street food with live music nightly.", Category = "Market", Region = "Cebu City" },
        
        //closed due to covid
        new() { Id = 417, Name = "The Pyramid", Location = "Cebu City", Description = "(closed)", Category = "Landmark", Region = "Cebu City" },
       
        new() { Id = 418, Name = "La Vie Parisienne", Location = "Cebu", Description = "French-European bistro and wine bar on Gorordo Avenue known for Parisian interiors, freshly baked pastries, gelato, and a romantic al fresco wine cellar.", Category = "Cafe", Region = "Cebu" },
        new() { Id = 419, Name = "La Vie in the Sky", Location = "Cebu", Description = "French-inspired mountain restaurant and winery in Busay with sweeping panoramic views of Cebu City. Now operating as La Parisienne Sky.", Category = "Restaurant", Region = "Cebu" },
        new() { Id = 420, Name = "Mist Mountain Resort", Location = "Cebu", Description = "Eco-tourism resort in Taptap within the Central Cebu Protected Landscape featuring an infinity pool, forest camping, nature trails, and mountain views.", Category = "Resort", Region = "Cebu" },
        new() { Id = 421, Name = "Waterfront Hotel", Location = "Cebu City", Description = "Landmark five-star hotel and casino complex on Salinas Drive in Lahug, offering luxury accommodations, multiple dining outlets, and a full-service casino.", Category = "Hotel", Region = "Cebu City" },
        new() { Id = 422, Name = "Top of Cebu", Location = "Cebu", Description = "Filipino restaurant and viewpoint in Busay at 2,000 feet elevation, serving contemporary Filipino dishes alongside panoramic views of the Cebu skyline.", Category = "Viewpoint", Region = "Cebu" },
        new() { Id = 423, Name = "Lantaw Native Restaurant", Location = "Cordova", Description = "Floating native restaurant on stilts above the sea in Barangay Day-as, Cordova, serving fresh Filipino seafood with views of the Mactan Channel at sunset.", Category = "Restaurant", Region = "Mactan" },
        new() { Id = 424, Name = "Kabang Falls (Budlaan)", Location = "Cebu City", Description = "Scenic waterfall in Barangay Budlaan accessible via a river trek, popular for swimming and hiking in the Cebu City highlands.", Category = "Waterfall", Region = "Cebu City" },
        new() { Id = 425, Name = "Himbabawod Falls (Bonbon)", Location = "Cebu", Description = "Hidden tiered waterfall in Barangay Bonbon nestled in the Transcentral highlands, reached by a short forest hike through unspoiled terrain.", Category = "Waterfall", Region = "Cebu" },
        new() { Id = 426, Name = "Busay Lut-od Falls", Location = "Cebu", Description = "Natural waterfall in the Busay highlands along the Transcentral Highway corridor, a local off-the-beaten-path escape in the Cebu City mountains.", Category = "Waterfall", Region = "Cebu" },
        new() { Id = 427, Name = "Kawa Falls (Toong Pardo)", Location = "Cebu", Description = "Small waterfall and natural pool in Barangay Toong, Pardo, a locally popular urban-fringe destination for a cool day-trip within Cebu City.", Category = "Waterfall", Region = "Cebu" },
        new() { Id = 428, Name = "NUSTAR Resort & Casino Cebu", Location = "Cebu City", Description = "Five-star integrated resort on Kawit Point, SRP, with the region's largest casino, two luxury hotels, The Mall at NUSTAR, 30+ dining outlets, and waterfront views.", Category = "Resort", Region = "Cebu City" },
        new() { Id = 429, Name = "SM Seaside Sky Park", Location = "Cebu City", Description = "20,000 sqm open-air rooftop park at SM Seaside City featuring Cobonpue playgrounds, glass-floor Skywalk Adventure, amphitheaters, and panoramic sea views.", Category = "Park", Region = "Cebu City" },
        new() { Id = 430, Name = "Il Corso Lifemalls", Location = "Cebu City", Description = "10-hectare al fresco waterfront lifestyle mall by Filinvest in City di Mare, SRP, with open-air dining, a sea-view boardwalk, and Cebu's first dancing light fountain.", Category = "Shopping", Region = "Cebu City" },
        new() { Id = 431, Name = "SRP Boardwalk", Location = "Cebu City", Description = "Scenic seaside promenade along South Road Properties popular for jogging, cycling, and sunset viewing with CCLEX bridge and Mactan Channel vistas.", Category = "Park", Region = "Cebu City" },
        new() { Id = 432, Name = "Cebu Fun Park", Location = "Cebu", Description = "Outdoor family theme park beside SM Seaside City in SRP offering 30+ rides, colorful attractions, food stalls, and live entertainment for all ages.", Category = "Theme Park", Region = "Cebu" },
        new() { Id = 433, Name = "ICON Cebu", Location = "Cebu City", Description = "One of Cebu City's premier nightlife venues on F. Cabahug Street, featuring top DJs, live performances, and a high-energy club atmosphere.", Category = "Shopping", Region = "Cebu City" },
        // === NEW ENTRIES (All missing spots from your list) ===
        // Lapu-Lapu City
        new() { Id = 434, Name = "Caohagan Island", Location = "Lapu-Lapu City", Description = "A small island in the Olango Island Group with powdery white sand, crystal-clear waters, and a marine sanctuary. Popular island-hopping stop offering snorkeling, fresh seafood, and a glimpse into local village life.", Category = "Island", Region = "Mactan" },
        new() { Id = 435, Name = "Nalusuan Island", Location = "Lapu-Lapu City", Description = "A privately managed marine sanctuary island in the Olango Group, famous for its iconic long wooden pier, vibrant coral reefs, and crystal-clear waters. A highlight of Mactan island-hopping tours for snorkeling and diving.", Category = "Island", Region = "Mactan" },
        new() { Id = 436, Name = "Pangan-an Island", Location = "Lapu-Lapu City", Description = "A barangay island east of Mactan next to Olango Island, uniquely accessible by tricycle across the reef flat at low tide. Known for its peaceful atmosphere, fresh seafood, and coastal marine sanctuaries.", Category = "Island", Region = "Mactan" },
        new() { Id = 437, Name = "Sulpa Islet", Location = "Lapu-Lapu City", Description = "An uninhabited hectare-wide islet near Olango Island, part of the Olango Island Group. Features rocky terrain, shallow clear waters for snorkeling, cottages for rent, and a serene off-the-beaten-path tropical atmosphere.", Category = "Island", Region = "Mactan" },
        new() { Id = 438, Name = "San Vicente Marine Sanctuary", Location = "Lapu-Lapu City", Description = "A marine protected area on Olango Island featuring a 500-meter bamboo boardwalk through mangroves, healthy coral reefs, and diverse marine life. Offers swimming, snorkeling, kayaking, and overnight camping.", Category = "Marine Sanctuary", Region = "Mactan" },
        new() { Id = 439, Name = "Yellow Submarine Dive Center", Location = "Mactan", Description = "A semi-submarine tourist attraction at JPark Resort in Maribago, Mactan, offering a 45-minute underwater viewing ride descending to about 40 feet to observe local marine life through glass windows.", Category = "Diving", Region = "Mactan" },
        new() { Id = 440, Name = "Pawod Spring", Location = "Lapu-Lapu City", Description = "A natural freshwater spring and cave system in Barangay Agus, Mactan Island. A popular local swimming spot and cave diving training site, accessible within 15–20 minutes from Mactan's resort area.", Category = "Spring", Region = "Mactan" },
        new() { Id = 441, Name = "Shangri-La's Mactan Resort & Spa", Location = "Mactan", Description = "An award-winning five-star beachfront resort on 13 hectares in Punta Engaño, featuring 541 rooms, a private beach cove, a 6-hectare marine sanctuary with 160+ fish species, two pools, CHI Spa, and multiple restaurants.", Category = "Resort", Region = "Mactan" },
        new() { Id = 442, Name = "Crimson Resort and Spa Mactan", Location = "Mactan", Description = "A five-star beachfront resort featuring 40 private pool villas, 250 guest rooms, a cascading three-tiered infinity pool, 14-room Aum Spa, private beach, and Azure Beach Club. Just 15 minutes from Mactan airport.", Category = "Resort", Region = "Mactan" },
        new() { Id = 443, Name = "Plantation Bay Resort & Spa", Location = "Mactan", Description = "An expansive 11-hectare resort in Marigondon known for the largest privately-owned saltwater lagoon (2.3 hectares) in the region, four freshwater pools, 255 rooms, a Japanese-inspired spa, and extensive activity offerings.", Category = "Resort", Region = "Mactan" },
        new() { Id = 444, Name = "Dusit Thani Mactan Cebu", Location = "Mactan", Description = "A five-star resort on the Punta Engaño Peninsula blending Thai and Filipino hospitality, featuring a 100-meter infinity pool overlooking historic Magellan Bay, 272 rooms, Benjarong Thai restaurant, and panoramic sunset views.", Category = "Resort", Region = "Mactan" },
        new() { Id = 445, Name = "JPark Island Resort & Waterpark", Location = "Mactan", Description = "A 16.5-hectare five-star integrated resort in Maribago featuring 568 rooms, 6 themed pools, 3 waterslides, a lazy river, 10 dining outlets, a casino, Pororo indoor theme park, spa, and private beach.", Category = "Resort", Region = "Mactan" },
        
       // Mandaue, Talisay, Naga, Carcar, Danao, etc.
        new() { Id = 446, Name = "Waterworld Cebu", Location = "Mandaue City", Description = "The biggest water park in Central Visayas featuring numerous slides and a giant wave pool.", Category = "Water Park", Region = "Mandaue" },
        new() { Id = 447, Name = "Upside Down World", Location = "Mandaue City", Description = "A unique museum featuring fully furnished rooms designed to be viewed from an inverted perspective.", Category = "Theme Park", Region = "Mandaue" },
        new() { Id = 448, Name = "Westown Lagoon", Location = "Mandaue City", Description = "A popular urban resort featuring refreshing pools, waterslides, and a relaxing man-made lagoon.", Category = "Park", Region = "Mandaue" },
        new() { Id = 449, Name = "Mandaue Heritage Plaza", Location = "Mandaue City", Description = "A historical landmark home to the National Shrine of St. Joseph and the Presidencia administration building.", Category = "Historical", Region = "Mandaue" },
        new() { Id = 450, Name = "Hidden Valley Mountain Resort", Location = "Talisay City", Description = "A serene mountain getaway offering natural spring pools and lush tropical landscapes.", Category = "Resort", Region = "Talisay" },
        new() { Id = 451, Name = "Crocolandia", Location = "Talisay City", Description = "A conservation center and nature park dedicated to crocodiles, birds, and other local wildlife.", Category = "Wildlife", Region = "Talisay" },
        new() { Id = 452, Name = "Naga Boardwalk / Coastal Boulevard", Location = "Naga City", Description = "A scenic seaside park featuring a long promenade, dining stalls, and a vibrant night atmosphere.", Category = "Park", Region = "Naga" },
        new() { Id = 453, Name = "Carcar Heritage Town", Location = "Carcar City", Description = "Famous for its Spanish-era architecture, preserved ancestral houses, and historic St. Catherine of Alexandria Church.", Category = "Historical", Region = "Carcar" },
        new() { Id = 454, Name = "Danao Adventure Park", Location = "Danao City", Description = "An eco-tourism destination offering thrilling activities like sky-dropping, zip-lining, and rock climbing.", Category = "Adventure", Region = "Danao" },
        new() { Id = 455, Name = "Danasan Eco Adventure Park", Location = "Danao City", Description = "A massive outdoor park with waterfalls, wakeboarding, trekking, and unique high-altitude challenges.", Category = "Adventure", Region = "Danao" },

        // More key additions (Bantayan, Camotes, etc.)
        new() { Id = 456, Name = "Kota Beach", Location = "Bantayan Island", Description = "Renowned for its fine white sand, crystal clear waters, and a picturesque sandbar that appears during low tide.", Category = "Beach", Region = "Bantayan" },
        new() { Id = 457, Name = "Ogtong Cave", Location = "Bantayan Island", Description = "A natural limestone cave within a resort that features a cool, refreshing freshwater pool for swimming.", Category = "Cave", Region = "Bantayan" },
        new() { Id = 458, Name = "Lake Danao", Location = "Camotes Island", Description = "A guitar-shaped freshwater lake surrounded by greenery, perfect for kayaking and sightseeing.", Category = "Lake", Region = "Camotes" },
        new() { Id = 459, Name = "Bukilat Cave", Location = "Camotes Island", Description = "One of the island's most famous caves, known for its natural skylights and impressive stalactite formations.", Category = "Cave", Region = "Camotes" },
        new() { Id = 460, Name = "Gilutungan Island", Location = "Cordova", Description = "A premier marine sanctuary famous for its vibrant coral reefs and abundant tropical fish, ideal for snorkeling.", Category = "Island", Region = "Mactan" },
    ];

        foreach (var spot in spots)
        {
            spot.ImageUrl = $"/images/{spot.Id}.jpg";
            
            // Assign Adventure Level based on Category
            spot.AdventureLevel = spot.Category switch
            {
                "Waterfall" => 4,
                "Island" => 3,
                "Mountain" => 5,
                "Beach" => 2,
                "Historical" => 1,
                "Religious" => 1,
                "Wildlife" => 3,
                "Theme Park" => 3,
                _ => 2
            };

            // Assign Base Price based on Category
            spot.BasePrice = spot.Category switch
            {
                "Island" => 3500m,
                "Waterfall" => 1800m,
                "Mountain" => 1200m,
                "Wildlife" => 2500m,
                "Theme Park" => 1500m,
                "Historical" => 800m,
                "Religious" => 500m,
                _ => 2000m
            };
        }

        return spots;
    }
}
