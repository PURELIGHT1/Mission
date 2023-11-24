using System.ComponentModel.DataAnnotations;

namespace PMB.Models
{
    public class SPUMhs
    {
        [Required(ErrorMessage = "Nama calon pendaftar wajib dipilih!")]
        public string kd_calon { get; set; }
        public string? nm_calon { get; set; }
        public string? thnakademik { get; set; }
        public int spu { get; set; }
        public string? username { get; set; }
        public DateTime? tanggal { get; set; }
        public DateTime? tgl_cetak { get; set; }
        public string? is_matrikulasi { get; set; }
        public string? masuk { get; set; }
        public string? periode { get; set; }
        public string? kd_jalur { get; set; }
    }

    public class SPUMhs2
    {
        public string kd_calon { get; set; }
        public string nm_calon { get; set; }
        public string masuk { get; set; }
        public string prodi_diterima { get; set; }
        public string kd_jalur { get; set; }
        public string nama_jalur { get; set; }
        public string periode { get; set; }
    }

    public class CekProdiSPUMhs
    {
        public string kode_calon_awal { get; set; }
        public string kode_calon_akhir { get; set; }
        public string id_prodi { get; set; }
    }

    public class StoreSPUMhs
    {
        public string tgl_cetak { get; set; }
        public string kode_calon_awal { get; set; }
        public string kode_calon_akhir { get; set; }
        public string kd_jalur { get; set; }
        public string id_prodi { get; set; }
        public string username { get; set; }
        public int spu { get; set; }
        public int ptg_konsesi { get; set; }
    }

    public class DetailRmsAgs
    {
        public int id_tagihan { get; set; }
        public string nama_tagihan { get; set; }
        public int angsuran_ke { get; set; }
        public int presentase { get; set; }
        public DateTime tgl_buka { get; set; }
        public DateTime tgl_tutup { get; set; }
        public bool is_jaminan { get; set; }
    }

    public class DetailRmsPtg
    {
        public int nominal { get; set; }
        public string keterangan { get; set; }
        public int id_tagihan { get; set; }
    }

    public class DetailPtg
    {
        public string jenis { get; set; }
        public int jlh_total { get; set; }
    }

    public class AngsuranMhs2
    {
        public string kd_calon { get; set; }
        public int angsuranke { get; set; }
        public string ket_angsuran { get; set; }
        public int potongan { get; set; }
        public int jumlah { get; set; }
    }
}
