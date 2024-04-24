using Swed64;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Documents;


// Programmed By l0r ; github.com/rootL0r .

class Program
{
    [DllImport("user32.dll")]
    static extern short GetAsyncKeyState(Keys vKey);

    // Main addr
    static int Localplayer = 0x017361C8;
    static int entitylist7 = 0x01743C80;
    static int viewangelsY = 0x192F910;
    static int viewangelsX = 0x192F914;
    static int viewangels = 0x192F910;

    // Offsets
    static int health = 0x334;
    static int teamnumber = 0xEE8;
    static int position = 0xD60;

    class Entity
    {
        public IntPtr address;
        public int health;
        public int teamnumber;
        public Vector3 position;
    }

    static Swed swed = new Swed("cs2");
    static Entity localplayer = new Entity();
    static List<Entity> playerteam = new List<Entity>();
    static List<Entity> enemyteam = new List<Entity>();

    static IntPtr clientDLL = swed.GetModuleBase("client.dll");

    static void Main(string[] args)
    {
        if (IsProcessOpen("cs2"))
        {
            localplayer.address = swed.ReadPointer(clientDLL, Localplayer);

            while (true)
            {
                Console.Title = "L0rHaCkerCSaimBot";
                Console.WriteLine("N0w Try To Click on Z ...");


                Entitylist();
                if (enemyteam.Count > 0)
                {
                    aimBot(enemyteam[0]);
                }
            }
        }
        else
        {
            // Don't Ask Me About That >.<
            string message = " ====== Sorry, Start Game first . ======";
            PrintMessageWithChangingColor(message);
            
            Thread.Sleep(2000);
            
        }

        static void PrintMessageWithChangingColor(string message)
        {
            ConsoleColor[] colors = {
            ConsoleColor.Red,
            ConsoleColor.Yellow,
            ConsoleColor.Green,
            ConsoleColor.Blue,
            ConsoleColor.Magenta,
            ConsoleColor.Cyan
        };

            int colorIndex = 0;
            for (int i = 0; i < message.Length; i++)
            {
                Console.ForegroundColor = colors[colorIndex];
                Console.Write(message[i]);
                colorIndex = (colorIndex + 1) % colors.Length;
                Thread.Sleep(100); // Pause for 1 second
            }

            Thread.Sleep(100);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\n\nI love Astolfo");
            Thread.Sleep(200);
            Console.WriteLine("\n\nWtf... are u Still there ??");
            Thread.Sleep(10);
            Console.ResetColor();

        }
    }

    static void updateEntity(Entity entity)
    {
        entity.health = swed.ReadInt(entity.address, health);
        entity.teamnumber = swed.ReadInt(entity.address, teamnumber);
        entity.position = swed.ReadVec(entity.address, position);
    }

    static void Entitylist()
    {
        playerteam.Clear();
        enemyteam.Clear();

        // Distance between each entity is 0x8
        localplayer.address = swed.ReadPointer(clientDLL, Localplayer);
        updateEntity(localplayer);

        for (int i = 1; i < 32; i++)
        {
            Entity entity = new Entity();
            entity.address = swed.ReadPointer(clientDLL, entitylist7 + i * 0x8);
            updateEntity(entity);

            if (entity.health <= 0 || entity.health > 100)
                continue;


            if (localplayer.teamnumber == entity.teamnumber)
            {
                playerteam.Add(entity);
            }
            else
            {
                enemyteam.Add(entity);
            }
        }
    }

    static void aimBot(Entity entity)
    {
        localplayer.address = swed.ReadPointer(clientDLL, Localplayer);
        updateEntity(localplayer);

        float deltaX = entity.position.X - localplayer.position.X;
        float deltaY = entity.position.Y - localplayer.position.Y;
        float deltaZ = entity.position.Z - localplayer.position.Z;

        double distance = Math.Sqrt(Math.Pow(deltaY, 2) + Math.Pow(deltaX, 2));

        float X = (float)(Math.Atan2(deltaY, deltaX) * 180 / Math.PI);
        float Y = -(float)(Math.Atan2(deltaZ - 10f, distance) * 180 / Math.PI);

        Vector3 Aimbot = new Vector3(Y, X, 0);

        var view4angles = swed.ReadVec(clientDLL, viewangels);

        var Z = GetAsyncKeyState(Keys.Z);

        if (0 > Z)
        {
            swed.WriteVec(clientDLL, viewangels, Aimbot);
        }
    }

    static bool IsProcessOpen(string name)
    {
        foreach (Process process in Process.GetProcesses())
        {
            if (process.ProcessName.Contains(name))
            {
                return true;
            }
        }

        return false;
        
    }
}
