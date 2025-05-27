using ZeroQL;

namespace StudentcomClient;

public class StudentcomClient : ScomClient 
{
    public StudentcomClient(string token, bool isTest = false) : base(new HttpHandler(new HttpClient()
    {
        DefaultRequestHeaders =
        {
            { "x-api-token", token }
        },
        BaseAddress = new Uri($"https://public-gateway.{(isTest ? "dandythrust.com" : "student.com")}/graphql")
    }, true))
    {
        
    }
}
