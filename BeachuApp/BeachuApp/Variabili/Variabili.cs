namespace BeachuApp
{
    public static class Variabili
    {
        private const string url = "https://beachunjs.herokuapp.com/api/";
        public static string UrlUser { get; } = url + "user/";
        public static string UrlStab { get; } = url + "stab/";
    }
}
