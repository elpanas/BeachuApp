using System;
using System.Text;

namespace BeachuApp
{
    public static class Funzioni
    {
        public static string CodificaId(string idu)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(idu));
        }
    }
}
