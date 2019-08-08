using System;
using System.Threading.Tasks;

namespace NonWebIntegrationDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ConsoleKeyInfo keyInfo = new ConsoleKeyInfo();

            var someClass = new SomeClass();

            while (keyInfo.Key != ConsoleKey.Q)
            {
                var result = await someClass.SayHello("World");
                Console.WriteLine($"Output: {result}");

                Console.WriteLine("Press any key to restart or Q to quit.");
                keyInfo = Console.ReadKey();
            }
        }
    }
}
