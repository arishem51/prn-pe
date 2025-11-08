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
                var buffer = new byte[1024];
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                
                if (bytesRead > 0)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Hello World");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling client: {ex.Message}");
        }
    });
}
