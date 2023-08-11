using testt.NewFolder;
using SuperSimpleTcp;
using System.Text;
using System;

namespace testt;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private bool _isFirstBoot = false;
    private int reconnectionAttempts = 0;
    private int maxtimeout = 10000;
    private int maxtimeoutSeconds = 0;
    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }
    // vou primeiro fazer aqui, depois metemos numa classe
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //tirar do while...nem precisa dequele firstboot.
        SimpleTcpClient client = new SimpleTcpClient("127.0.0.1:4545");
        client.Connect();
        
        //Console.ReadKey();

        while (!stoppingToken.IsCancellationRequested)
        {
            //melhor
            if (client.IsConnected ==true  && _isFirstBoot == false)
            {
                //em teoria, vai esta sempre a mandar
                client.Send("Client Connected!");
                Console.WriteLine("Client Connected!");
            }

            if (client.IsConnected == false)
            {
                Console.WriteLine("Client Disconnected!");
                try
                {
                    client.ConnectWithRetries(maxtimeout);
                }
                catch  
                {
                    maxtimeoutSeconds = maxtimeout / 1000;
                    Console.WriteLine($"Connection Retry Timeout {maxtimeoutSeconds}s reached");
                }
               
            }         
            if (_isFirstBoot == false)
            {
                //eventos sao tipo links, tu fazes associação uma vez. E ele fica tipo: Ah, quando acontece esta merda...ele quer correr aqueles metodos...ta bem ta bem.
                client.Events.Connected += ConnectedEvent;
                client.Events.Disconnected += Disconnected;
                client.Events.DataReceived += DataReceived;
                client.Events.DataSent += DataSent;
                _isFirstBoot = true;

            } 
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            //delay porque..sim
            await Task.Delay(2000, stoppingToken);
        }
    }

    private void DataSent(object? sender, DataSentEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void DataReceived(object? sender, DataReceivedEventArgs received)
    {
        //explora este evento depois
        //nao vai fazer nada por enquanto
        //ver se isto da
        Console.WriteLine($"[{received.IpPort}] {Encoding.UTF8.GetString(received.Data.Array, 0, received.Data.Count)}");
    }

    private void Disconnected(object? sender, ConnectionEventArgs e)
    {
        
        //Console.WriteLine(e.Reason);
        Console.WriteLine(e.IpPort);
        //nao vai fazer nada por enquanto

    }

    private void ConnectedEvent(object? sender, ConnectionEventArgs feedback)
    {
        
        //Console.WriteLine(feedback.Reason);
        Console.WriteLine(feedback.IpPort);
        reconnectionAttempts++;
        Console.WriteLine($"Number of reconnections {reconnectionAttempts}");



    }
}
