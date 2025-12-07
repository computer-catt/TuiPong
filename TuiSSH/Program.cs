using FxSsh;
using FxSsh.Services;
using System.Net;
using System.Text;
using TuiCommon;
using TuiCommon.Applications;
using TuiPongWeb;

namespace SshServerLoader;

static class Program {
    static void Main(string[] args) {
        var ecdsap521Pem = @"-----BEGIN PRIVATE KEY-----
                        MIHuAgEAMBAGByqGSM49AgEGBSuBBAAjBIHWMIHTAgEBBEIAGXO87cgkpAPLIWoc
                        kZirXguaO7WeAFtO+z5TtfHyTLEgSUWlGhP1PZ3ZbyLf0ht6t4X46TQQn7Eyqkuy
                        XgXZ0RihgYkDgYYABAAa9hQJavg/gAqUEIVoL1TucLMu1gCElMvX68BrJQoYdoNe
                        gbR4mS/oiOdvU5zm4H2ABo6gDYo2Pl4W80lqL3nGdgAwdNN7udRi/A5wc39KvZ5w
                        bbDmx/ly7kvagszIWafjG8Hzg5v5kKBbdYw9A+9pN2cbhWXug41xR1rLDOI6hFSn
                        TA==
                        -----END PRIVATE KEY-----";

        var server = new SshServer(new StartingInfo(IPAddress.Any, 2223, "SSH-2.0-FxSsh"));
        server.AddHostKey("ecdsa-sha2-nistp521", ecdsap521Pem);
        server.ConnectionAccepted += server_ConnectionAccepted;

        server.Start();
        Console.WriteLine($"Server started.\nRunning on {server.StartingInfo.LocalAddress}:{server.StartingInfo.Port} with the {server.StartingInfo.ServerBanner} banner");
        Task.Delay(-1).Wait();
    }

    static void server_ConnectionAccepted(object? sender, Session e) {
        Console.WriteLine("Accepted a client.");
        e.ServiceRegistered += e_ServiceRegistered;
        e.KeysExchanged += e_KeysExchanged;
    }

    private static void e_KeysExchanged(object? sender, KeyExchangeArgs e) {
        foreach (var keyExchangeAlg in e.KeyExchangeAlgorithms)
            Console.WriteLine("Key exchange algorithm: {0}", keyExchangeAlg);
    }

    static void e_ServiceRegistered(object? sender, SshService e) {
        if (sender == null) {
            Console.WriteLine("Sender is null. ignoring.");
            return;
        }
        
        var session = (Session)sender;
        Console.WriteLine("Session {0} requesting {1}.",
            BitConverter.ToString(session.SessionId).Replace("-", ""), e.GetType().Name);
        switch (e) {
            case UserAuthService service: { 
                service.UserAuth += (o, args) => args.Result = true;
                break;
            }
            case ConnectionService service: {
                SshScreen screen = new SshScreen([]);
                service.CommandOpened += (o, e) => {
                    Console.WriteLine($"Channel {e.Channel.ServerChannelId} runs {e.ShellType}: \"{e.CommandText}\", client key SHA256:{e.AttachedUserAuthArgs.Fingerprint}.");
                    e.Agreed = true;
                    if (e.ShellType != "shell") return;
                    
                    screen.SetCallback(s => {
                        e.Channel.SendData("\e[H"u8.ToArray());
                        e.Channel.SendData(Encoding.UTF8.GetBytes(s));
                    });

                    screen.SetApplication(new Pong(screen));
                    e.Channel.DataReceived += (o, bytes) => {
                        foreach (var byted in bytes) Console.Write($"{byted} ");
                        Console.WriteLine();
                        string input = Encoding.UTF8.GetString(bytes);
                        Console.WriteLine(input);
                        screen.SendKey(new TuiKey(input, ""));
                    };

                    e.Channel.CloseReceived += (_, _) => screen.StopScreen();
        
                    Task.Run(screen.StartScreen);
                };
                service.EnvReceived += service_EnvReceived;
                service.PtyReceived += (o, args) => screen.SetScreenBounds((int)args.WidthChars, (int)args.HeightRows);
                service.TcpForwardRequest += service_TcpForwardRequest;
                service.WindowChange += (o, args) => screen.SetScreenBounds((int)args.WidthColumns, (int)args.HeightRows);
                break;
            }
        }
    }

    static void service_TcpForwardRequest(object? sender, TcpRequestArgs e) =>
        Console.WriteLine("Received a request to forward data to {0}:{1}", e.Host, e.Port);
    // TODO: see if i need to do something here

    static void service_EnvReceived(object? sender, EnvironmentArgs e) =>
        Console.WriteLine("Received environment variable {0}:{1}", e.Name, e.Value);
}
