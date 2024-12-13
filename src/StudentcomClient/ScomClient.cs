using ZeroQL;

namespace StudentcomClient;

public class StudentcomClient : ScomClient, IDisposable
{
    public StudentcomClient(string token) : base(new HttpHandler(new HttpClient()
    {
        DefaultRequestHeaders =
        {
            { "x-api-token", token }
        },
        BaseAddress = new Uri("https://public-gateway.dandythrust.com/graphql")
    }, true))
    {
    }
}
