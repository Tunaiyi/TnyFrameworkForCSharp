// See https://aka.ms/new-console-template for more information

using System;
using System.Threading.Tasks;


Console.WriteLine("Hello, World!");


await Task.Run(async () => {

    while (true)
    {
        await Task.Yield();
    }
});