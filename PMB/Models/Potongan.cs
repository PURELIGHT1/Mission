namespace PMB.Models
{
    public class Potongan
    {
        public int id_dtl_potongan { get; set; }
        public string id_rumus { get; set; }
        public string id_prodi { get; set; }
        public string kd_jalur { get; set; }
        public int id_tagihan { get; set; }
        public int nominal { get; set; }
        public string keterangan { get; set; }
        public string nama_tagihan { get; set; }
        public string nama_jalur { get; set; }
        public string nm_prodi { get; set; }
    }
}
