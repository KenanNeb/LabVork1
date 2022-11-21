using ConsoleApp1;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

var ip = IPAddress.Loopback;
var port = 27001;
var listener = new TcpListener(ip, port);
listener.Start();

while (true)
{
    var client = listener.AcceptTcpClient();
    var stream = client.GetStream();
    var br = new BinaryReader(stream);
    var bw = new BinaryWriter(stream);
    while (true)
    {
        var input = br.ReadString();
        var command = JsonSerializer.Deserialize<Command>(input);
        if (command == null) continue;

        Console.WriteLine(command.Text);
        Console.WriteLine(command.Param);
        switch (command.Text)
        {
            case Command.ProcessList:
                var processes = Process.GetProcesses();
                var processesNames = JsonSerializer.Serialize(processes.Select(p => p.ProcessName));
                bw.Write(processesNames);
                break;
            case Command.Run:
                bool check  = false;
                try
                {
                    Process.Start(command.Param);
                }
                catch (Exception)
                {
                    check = true;                    
                }
                if (check == true)
                {
                    bw.Write(JsonSerializer.Serialize<bool>(false));
                }
                else 
                {
                    bw.Write(JsonSerializer.Serialize<bool>(true));
                }
                break;
            case Command.Kill:
                int ch = 0;
                Process[] pcs = Process.GetProcesses();
                foreach (var item in pcs)
                {
                    if (item.ProcessName == command.Param)
                    {
                        item.Kill();
                        ch = 1;
                    }
                }
                if (ch == 0)
                    bw.Write(JsonSerializer.Serialize<bool>(false));
                else if (ch == 1)
                    bw.Write(JsonSerializer.Serialize<bool>(true));
                break;

            default:
                break;
        }
    }
}