using ConsoleApp2;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

var ip = IPAddress.Loopback;
var port = 27001;
var client = new TcpClient();
client.Connect(ip, port);
var stream = client.GetStream();
var br = new BinaryReader(stream);
var bw = new BinaryWriter(stream);
Command command = null;
string responce = null;

while (true)
{
	Console.WriteLine("Write Command or HELP");
    var str = Console.ReadLine().ToUpper();
	if (str == "HELP")
	{
		Console.WriteLine(Command.ProcessList);
		Console.WriteLine($"{Command.Run} <process_name>");
        Console.WriteLine($"{Command.Kill} <process_name>");
		Console.WriteLine("HELP");
		Console.ReadKey();
		Console.Clear();
		continue;
    }
	var input = str.Split(' ');
	switch (input[0])
	{
		case Command.ProcessList:
			command = new Command { Text = input[0] };
			bw.Write(JsonSerializer.Serialize(command));
			responce = br.ReadString();
			var processesName = JsonSerializer.Deserialize<string[]>(responce);
			foreach (var process in processesName)
			{
				Console.WriteLine(process);
			}
			Console.ReadKey();
			Console.Clear();
			break;
        
        case Command.Run:
            command = new Command { Text = input[0], Param = input[1] };
            bw.Write(JsonSerializer.Serialize(command));
            responce = br.ReadString();
            if (JsonSerializer.Deserialize<bool>(responce) == true)
                Console.WriteLine("Process run oldu");
            else
                Console.WriteLine("Process run olmadi");
            Console.ReadKey();
            Console.Clear();
            break;

        case Command.Kill:
            command = new Command { Text = input[0], Param = input[1].ToLower() };
            bw.Write(JsonSerializer.Serialize(command));
            responce = br.ReadString();
            if (JsonSerializer.Deserialize<bool>(responce) == true)
                Console.WriteLine("Process succeed");
            else
                Console.WriteLine("Process faild");
            Console.ReadKey();
            Console.Clear();
            break;
        default:
			break;
	}
}