namespace PMB.Models
{
    public class CetakSKPUK
    {
        public string thnakademik { get; set; }
        public string kd_jalur { get; set; }
        public string nama_jalur { get; set; }
        public string nm_calon { get; set; }
        public string kd_calon { get; set; }
        public string nm_fakultas { get; set; }
        public string nm_prodi { get; set; }
        public int jml_sebelum_potongan { get; set; }
        public int jml_potongan { get; set; }
        public int jml_setelah_potongan { get; set; }
    }

    public class CetakSKPUKMhs
    {
        public string kd_calon { get; set; }
        public string thnakademik { get; set; }
        public string nama_jalur { get; set; }
        public string nm_calon { get; set; }
        public string nm_fakultas { get; set; }
        public string nm_prodi { get; set; }
        public int jml_sebelum_potongan { get; set; }
        public int jml_potongan { get; set; }
        public int jml_setelah_potongan { get; set; }
        public string terbilangJmlStlhPotongan { get; set; }
        public int jml_angsuran { get; set; }
    }

}
