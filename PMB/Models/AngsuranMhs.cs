namespace PMB.Models
{
    public class AngsuranMhs
    {
        public string? Kd_Calon { get; set; }
        public string? Nm_Calon { get; set; }
        public int AngsuranKe { get; set; }
        public int Jml_Uang { get; set; }
        public string? Tgl_Buka { get; set; }
        public string? Batas_Waktu { get; set; }
        public string? Batas_Pembayaran { get; set; }
        public string? Ket_Angsuran { get; set; }
        public string? Status { get; set; }
        public string? Is_Jaminan { get; set; }
        public int Jumlah { get; set; }
        public int Potongan { get; set; }
        public string? Sks { get; set; }
        public int Id_detail_ags { get; set; }


        public string? Masuk { get; set; }
        public string? Periode { get; set; }
        public string? Kd_Jalur { get; set; }
        public string? Thnakademik { get; set; }
    }

    public class JadwalAngsuran
    {
        public string kd_calon { get; set; }
        public int id_ags { get; set; }
        public string tgl_buka { get; set; }
        public string tgl_tutup { get; set; }
        public int banyak { get; set; }
        public List<int> split_uang { get; set; }
    }
}
