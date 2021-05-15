using System;

namespace DinoRun
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new DinoRun())
                game.Run();
        }
    }
}
