using System.Collections.Generic;

namespace BeachuApp
{
    public class GeoLocation
    {
        public string type { get; set; }
        public List<float> coordinates { get; set; }
    }

    public class Stabilimento
    {
        public string _id { get; set; }
        public string nome { get; set; }
        public string localita { get; set; }
        public string provincia { get; set; }
        public GeoLocation location { get; set; }
        public int ombrelloni { get; set; }
        public int disponibili { get; set; }
        public string idutente { get; set; }
        public string telefono { get; set; }
        public string email { get; set; }
        public string web { get; set; }
    }
}
