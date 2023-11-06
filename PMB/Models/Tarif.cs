namespace PMB.Models
{
    public class Tarif
    {
        public string prodi { get; set; }
        public int tahun { get; set; }
        public string semester { get; set; }
        public List<int> biaya { get; set; }
        public List<String> tagihan { get; set; }
        public int pengali { get; set; }
    }

    public class TarifTagihan
    {
        public int id_tarif { get; set; }
        public string id_tagihan { get; set; }
    }

    public class ListTagihan
    {
        public int id_tagihan { get; set; }
        public string nama_tagihan { get; set; }
    }
}
