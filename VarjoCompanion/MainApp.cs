using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using VarjoInterface;

namespace VarjoCompanion
{
    class MainApp
    {
        static void Main(string[] args)
        {
            VarjoApp.SessionInit();

            if (!VarjoApp.IsGazeAllowed())
            {
                Console.WriteLine("Gaze tracking is not allowed! Please enable it in the Varjo Base!");
                return;
            }

            VarjoApp.GazeInit();
            VarjoApp.SyncProperties();

            using (var memMapFile = MemoryMappedFile.CreateNew("VarjoApp", Marshal.SizeOf(VarjoApp.varjoData)))
            {
                using (var accessor = memMapFile.CreateViewAccessor())
                {
                    Console.WriteLine("Eye tracking session has started!");
                    while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter))
                    {
                        VarjoApp.GetGazeData();
                        accessor.Write(0, ref VarjoApp.varjoData);
                    }
                }
            }

            VarjoApp.Shutdown();
        }
    }
}