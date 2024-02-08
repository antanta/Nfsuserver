using System.Diagnostics;

namespace Nfsserver;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private Timer? _timer = null;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.CompletedTask;
        }
    }

    public override async Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        //_timer = new Timer(DoWork, null, TimeSpan.Zero,
        //    TimeSpan.FromMinutes(1));


        await Task.Run(() =>
        {
            Process pr = new Process();
            ProcessStartInfo prs = new ProcessStartInfo();
            prs.FileName = "nfsuserver.1.0.1.linux.static";
            prs.RedirectStandardOutput = true;
            prs.RedirectStandardError = true;

            prs.UseShellExecute = false;//??

            pr.StartInfo = prs;

            pr.OutputDataReceived += new DataReceivedEventHandler((a, b) =>
            {
                _logger.LogInformation($"{nameof(Worker)}: {a}");
            });

            pr.EnableRaisingEvents = true;


            //ThreadStart ths = new ThreadStart(() => pr.Start());
            //Thread th = new Thread(ths);
            //th.Start();
            _logger.LogInformation("-------------Starting process-------------");
            pr.Start();
            _logger.LogInformation("-------------Process started-------------");
        }, stoppingToken);

        //return processTask;

        //return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        //var count = Interlocked.Increment(ref executionCount);
        _logger.LogInformation("Timed Hosted Service is working.");
    }

    public override Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _timer?.Dispose();
    }
}
