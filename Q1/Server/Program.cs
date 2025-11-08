using System.Net;
using System.Net.Sockets;
using System.Text;

const int port = 5000;

var listener = new TcpListener(IPAddress.Any, port);
listener.Start();

Console.WriteLine($"Server listening on port {port}...");

while (true)
{
    var client = await listener.AcceptTcpClientAsync();
    Console.WriteLine("Client connected.");

    _ = Task.Run(async () =>
    {
        try
        {
            using (client)
            using (var stream = client.GetStream())
            {
                // Read the JSON data
                var buffer = new byte[4096];
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                
                if (bytesRead > 0)
                {
                    var json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    
                    // Deserialize the Start array
                    var stars = System.Text.Json.JsonSerializer.Deserialize<List<Start>>(json);
                    
                    if (stars != null && stars.Count > 0)
                    {
                        Console.WriteLine($"Received {stars.Count} star(s):");
                        foreach (var star in stars)
                        {
                            Console.WriteLine($"  - Name: {star.Name}, DOB: {star.Dob?.ToString("yyyy-MM-dd") ?? "N/A"}, " +
                                            $"Male: {star.Male?.ToString() ?? "N/A"}, Nationality: {star.Nationality ?? "N/A"}");
                        }
                    }
                    
                    // Send true response back to client
                    var response = Encoding.UTF8.GetBytes("true");
                    await stream.WriteAsync(response, 0, response.Length);
                    Console.WriteLine("Response sent: true");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling client: {ex.Message}");
        }
    });
}
