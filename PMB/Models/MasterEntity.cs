using System.ComponentModel.DataAnnotations;

namespace PMB.Models
{
    public class Provinsi
    {
        public string KD_PROP { get; set; }
        public string NAMA_PROP { get; set; }
    }

    public class StoreProvinsi
    {
        public string kd_prop { get; set; }
        public string nama_prop { get; set; }
        public int status { get; set; }
    }

    public class Kota
    {
        public int ID_KABUPATEN { get; set; }
        public string NAMA_KABUPATEN { get; set; }
    }

    public class StoreKota
    {
        public string KD_PROP { get; set; }
        public int ID_KABUPATEN { get; set; }
        public string NAMA_KABUPATEN { get; set; }
        public int status { get; set; }
    }
    public class Kecamatan
    {
        public int id_kecamatan { get; set; }
        public string nama_kecamatan { get; set; }
    }

    public class Jas
    {
        public string id { get; set; }
        public string ukuran { get; set; }
    }
}
