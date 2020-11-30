namespace OpenTPW
{
    class Program
    {
        public static TPWGame game;

        static void Main(string[] args)
        {
            game = new TPWGame("GameProperties.json");
            game.Run();
            while (game.isRunning) ;
        }
    }
}
