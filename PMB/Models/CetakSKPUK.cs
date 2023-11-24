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
        public string kd_jalur { get; set; }
        public string nama_jalur { get; set; }
        public string nm_calon { get; set; }
        public string nm_fakultas { get; set; }
        public string nm_prodi { get; set; }
        public string ket_tgl_regis { get; set; }
        public string tgl_cetak { get; set; }
        public string kwrganegaraan { get; set; }
        public int jml_sebelum_potongan { get; set; }
        public int jml_potongan { get; set; }
        public int jml_setelah_potongan { get; set; }
        public string terbilangJmlStlhPotongan { get; set; }
        public int jml_angsuran { get; set; }
        public bool jaminan { get; set; }
        public string tanggal_jaminan { get; set; }
        public int uang_jaminan { get; set; }
    }

}
