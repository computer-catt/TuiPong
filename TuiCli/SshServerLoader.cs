using System.Net;
using System.Text;
using FxSsh;
using FxSsh.Services;
using TuiCommon;
using TuiCommon.Applications;

namespace TuiCli;

public static class SshServerLoader {
    public static void Init(string[] args) {
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
        
        server.ConnectionAccepted += (sender, session) => {
            Console.WriteLine("Accepted a client.");
            session.ServiceRegistered += e_ServiceRegistered;
        };

        server.Start();
        Console.WriteLine($"SSH server started.\nRunning on {server.StartingInfo.LocalAddress}:{server.StartingInfo.Port} with the {server.StartingInfo.ServerBanner} banner");
        Task.Delay(-1).Wait();
    }

    private static void e_ServiceRegistered(object? sender, SshService e) {
        if (sender == null) {
            Console.WriteLine("Sender is null. ignoring.");
            return;
        }
        
        var session = (Session)sender;
        Console.WriteLine("Session {0} requesting {1}.",
            BitConverter.ToString(session.SessionId).Replace("-", ""), e.GetType().Name);
        switch (e) {
            case UserAuthService service: { 
                service.UserAuth += (_, args) => args.Result = true;
                break;
            }
            case ConnectionService service: {
                try {
                    SshScreen screen = new SshScreen();
                    PtyArgs? ptyArgs = null;
                service.CommandOpened += (_, e) => {
                    Console.WriteLine($"Channel {e.Channel.ServerChannelId} runs {e.ShellType}: \"{e.CommandText}\", client key SHA256:{e.AttachedUserAuthArgs.Fingerprint}.");
                    if (e.ShellType is not "shell") {
                        e.Channel.SendData(
                            Encoding.UTF8.GetBytes(
                                ptyArgs?.Terminal == "xterm-kitty" ? 
                                "\r\n\nSorry. using ssh kitten is not supported, \n\r" +
                                "Ssh with a real ssh client.\r\n\n" : 
                                $"\r\n\nSorry. {e.ShellType} is not supported.\r\n\n"));
                        e.Channel.SendClose();
                        return;
                    }
                    e.Agreed = true;
                    
                    // e.Channel.SendData("\e[?1003h"u8.ToArray());
                    /*e.Channel.SendData("\e[38:5:0m\e[48:5:255m"u8.ToArray());*/
                    
                    /*e.Channel.DataReceived += (o, bytes) => {
                        string meow = "";
                        foreach (var byted in bytes) meow += $"{byted} ";
                        e.Channel.SendData(Encoding.UTF8.GetBytes($"{meow.Trim()}\n\r"));
                    };*/

                    /*screen.SetArgs(e.CommandText.Split(' ', StringSplitOptions.RemoveEmptyEntries));*/
                    
                    screen.SetCallback(s => {
                        if (e.Channel.ClientClosed || e.Channel.ServerClosed) return;
                        e.Channel.SendData("\e[H"u8.ToArray());
                        e.Channel.SendData(Encoding.UTF8.GetBytes(s.ToString()!));
                    });

                    screen.SetApplication(new MainMenu(screen));
                    e.Channel.DataReceived += (_, bytes) => {
                        if (bytes.Length == 1 && bytes[0] is 3) {
                            screen.StopScreen();
                            return;
                        }

                        /*foreach (var byted in bytes) Console.Write($"{byted} ");
                        Console.WriteLine();*/
                        
                        TuiKey[] keys = BytesToTuiKeys(bytes);
                        foreach (var key in keys) screen.SendKey(key);
                    };

                    screen.Stopping += () => {
                        e.Channel.SendData("\e[2J\e[H"u8.ToArray());
                        e.Channel.SendClose();
                    };
                    
                    Console.WriteLine($"Welcome {e.AttachedUserAuthArgs.Username} onto the server!");
                    session.Disconnected += (_,_) => {
                        Console.WriteLine($"Goodbye {e.AttachedUserAuthArgs.Username}");
                        screen.StopScreen();
                    };

                    Task.Run(screen.StartScreen);
                };
                service.EnvReceived += service_EnvReceived;
                service.PtyReceived += (_, args) => screen.SetScreenBounds((int)(ptyArgs = args).WidthChars, (int)args.HeightRows);
                service.TcpForwardRequest += service_TcpForwardRequest;
                service.WindowChange += (_, args) => screen.SetScreenBounds((int)args.WidthColumns, (int)args.HeightRows);
                }
                catch (Exception exception) {
                    Console.WriteLine(exception);
                    throw;
                }
                break;
            }
        }
    }

    private static void service_TcpForwardRequest(object? sender, TcpRequestArgs e) =>
        Console.WriteLine("Received a request to forward data to {0}:{1}", e.Host, e.Port);
    // TODO: see if i need to do something here

    private static void service_EnvReceived(object? sender, EnvironmentArgs e) =>
        Console.WriteLine("Received environment variable {0}:{1}", e.Name, e.Value);

    private static TuiKey[] BytesToTuiKeys(byte[] inputBytes) {
        List<TuiKey> keys = [];
        
        string inputString = Encoding.UTF8.GetString(inputBytes);
        string[] splitKeys = inputString.Split("\e");
        
        foreach (var character in splitKeys[0])
            keys.Add(ParseTuiChar(character));
        for (int i = 1; i < splitKeys.Length; i++)
            keys.Add(ParseTuiEscSeq($"\e{splitKeys[i]}"));

        return keys.ToArray();
    }

    private static TuiKey ParseTuiChar(char character) => character switch {
        '\r' => new TuiKey("Enter", '\r'),
        '\t' => new TuiKey("Tab", '\t'),
        '\b' or (char)127 => new TuiKey("Backspace", '\b'),
        ' ' => new TuiKey("Space", ' '),
        _ => new TuiKey(character.ToString(), character)
    };

    private static TuiKey ParseTuiEscSeq(string seq) => seq switch {
        "\e[A" => new TuiKey("UpArrow", (char?)null),
        "\e[B" => new TuiKey("DownArrow", (char?)null),
        "\e[C" => new TuiKey("RightArrow", (char?)null),
        "\e[D" => new TuiKey("LeftArrow", (char?)null),
        "\e" => new TuiKey("Escape", (char?)null),
        _ => new TuiKey(seq, (char?)null)
    };
}