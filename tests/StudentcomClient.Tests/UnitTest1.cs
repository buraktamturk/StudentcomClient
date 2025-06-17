using Microsoft.Extensions.Configuration;
using ZeroQL;

namespace StudentcomClient.Tests;

public sealed class StudentcomClientTests : IDisposable
{
    private readonly ScomClient _client;

    public StudentcomClientTests()
    {
        var Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        
        _client = new StudentcomClient(Configuration["ApiKey"]);
    }
    
    [Fact]
    public async Task TestGetCountries()
    {
        var results = await _client.Query(a => a.GetCountries(1, 20, a => new
        {
            countries = a.Counties(b => new { b.Id, b.Name }),
            pageInfo = a.PageInfo(a => new { a.CurrentPage, a.PageSize, a.TotalPages, a.Total })
        }));
        
        Assert.NotNull(results);

        var countryId = results.Data.countries[0].Id;
        var cities = await _client.Query(a => a.GetCities(countryId, 1, 20, b => new { cities = b.Cities(b => new { b.Id, b.Name }) }));

        var facilities = await _client.Query(a =>
            a.GetFacilities(b => new
            {
                Bills = b.Bills(c => new { c.Name, c.Group, c.Slug, c.Label, c.Type }),
                Features = b.Features(c => new { c.Name, c.Group, c.Slug, c.Label, c.Type }),
                PropertyRules = b.PropertyRules(c => new {  c.Name, c.Group, c.Slug, c.Label, c.Type }),
                SecurityAndSafety = b.SecurityAndSafety(c => new { c.Name, c.Group, c.Slug, c.Label, c.Type })
            }));
        
        
        
    }

    [Fact]
    public async Task TestCreateProperty()
    {
        var request = new CreatePropertyInput()
        {
            ApartmentType = ApartmentType.StudentAccommodation,
            FreeCancellationPeriod = FreeCancellationPeriod.Hours24,
            BookingType = PropertyBookingJourney.MessageProperty,
            
        };

        var property = await _client.Mutation(a => a.CreateProperty(request, a => new { PropertyId = a.Property(b => b.Id) }));
        var propertyId = property.Data.PropertyId.Value;


        var roomRequest = new CreateRoomInput()
        {
            PropertyId = propertyId,
            SmokingPreference = SmokingPreference.Smoking,
            Category = RoomCategory.PrivateRoom,
            RoomSize = new RoomSizesInput()
            {
                
            },
            BedCount = 1,
            DualOccupancy = DualOccupancy.DualOccupancyNotAllowed,
            KitchenArrangement = KitchenArrangement.Shared,
            Facilities = [RoomFacility.Bathroom, RoomFacility.Wifi],
            
            

        };
        var room = await _client.Mutation(a => a.CreateRoom(roomRequest, a => new { RoomId = a.Room(b => b.Id) }));
        var roomId = room.Data.RoomId.Value;



        var imageCreate = new CreatePropertyImageInput()
        {
            File = new Upload("", Stream.Null),
            Category = ImageCategory.Room,
            PropertyId = propertyId,
            
        };
        
        var image = await _client.Mutation(a => a.CreatePropertyImage(imageCreate, b => new { imageId = b.Image(c => c.Id) }));
       
        
        // var updateRateAvailablity = new UpdateRateAvailabilityInput()
        // {
        //     
        // }
        //
        // await _client.Mutation(a => a.Update())


    }

    [Fact]
    public async Task TestPagePaymentPlans()
    {
        int pageNumber = 1;
        var query = await _client.Query(a => a.PagePaymentPlans(pageNumber, selector: a => new
        {
            PaymentPlans = a.PaymentPlans(b => new
            {
                b.Name
            })
        }));
        
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}