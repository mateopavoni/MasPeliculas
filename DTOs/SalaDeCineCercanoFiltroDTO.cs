using System.ComponentModel.DataAnnotations;

namespace MasPelículasAPI.DTOs
{
    public class SalaDeCineCercanoFiltroDTO
    {
        [Range(-90, 90)]
        public double Latitud { get; set; }

        [Range(-180, 180)]
        public double Longitud { get; set; }

        private int distanciaEnKms = 10;
        private int distanciaMaximaEnKms = 50;
        public int DistanciaEnKms
        {
            get => distanciaEnKms;
            set
            {
                if (value < 0)
                {
                    distanciaEnKms = 0;
                }
                else if (value > distanciaMaximaEnKms)
                {
                    distanciaEnKms = distanciaMaximaEnKms;
                }
                else
                {
                    distanciaEnKms = value;
                }
            }
        }
    }
}
