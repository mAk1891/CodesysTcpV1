using testt.NewFolder;
using SuperSimpleTcp;
using System.Text;
using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;
using System.Text.Json;

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
        SimpleTcpClient client = new SimpleTcpClient("127.0.0.1:4545");
        client.Connect();
        

        while (!stoppingToken.IsCancellationRequested)
        {
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
        //  Isto:"throw new NotImplementedException();" se este metodo for chamado, da uma exception e programa crasha. 
        // É uma forma de te lembrares que nao esta implementado. Podes comentar se quiseres a linha debaixo ate ter logica para isto feita.
        // ou apagas, your choice. Começa a experimentar estas coisas.
        throw new NotImplementedException();
    }

    private void DataReceived(object? sender, DataReceivedEventArgs received)
    {
        // Vais buscar o teu json em "serie". Basicamente é uma string. "GeString" é um metodo de sistema e encoding esta como UTF8.
        // Explora Econding..tens Encoding.ASCII, Encoding.UTF7 etc.
        // PS: Se quiseres, podes colocar isso dentro "try". Assim, nao crasha se por alguma razao "GetString" nao funcionar.
        string jsonParaDesreliazar = Encoding.UTF8.GetString(received.Data.Array, 0, received.Data.Count);

        // Tenta desrelizar. Se der asneira, escreve erro na terminal.
        try
        {
            //Magia aqui. Faz debug a esta linha e "Explora" "deserializedObjectPedro"
            EstruturaDoPedro deserializedObjectPedro = JsonSerializer.Deserialize<EstruturaDoPedro>(jsonParaDesreliazar);
            // Ex
            Console.WriteLine(deserializedObjectPedro.var1.ToString());
            

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }


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

public class EstruturaDoPedro
{
    // O mais improtante aqui para ti, é colocar os tipos de var iguais. Se no json var2 é int, aqui tambem tem que ser.
    // BTW. Meti isto aqui a toa...devia estar num directorio que faça sentido no projeto.
    public string var1 { get; set; }
    public int var2 { get; set; }
    public string var3 { get; set; }
    public string var4 { get; set; }
    public string var5 { get; set; }
    public string var6 { get; set; }
    public string var7 { get; set; }
}