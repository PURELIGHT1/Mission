using System.ComponentModel.DataAnnotations;

namespace PMB.Models
{
    public class SKPUK
    {
        public int id_skpu { get; set; }

        [Required(ErrorMessage = "Program studi wajib dipilih!")]
        public string id_prodi { get; set; }

        [Required(ErrorMessage = "Tahun akademik wajib diisi!")]
        [RegularExpression(@"^([1-9]{1}[0-9]{3})+[-/.]+([1-9]{1}[0-9]{3})$", ErrorMessage = "Format tahun akademik wajib YYYY/YYYY!")]
        public string thnakademik { get; set; }

        [Required(ErrorMessage = "Jenis tagihan wajib dipilih!")]
        public int id_tagihan { get; set; }
        public int? nominal { get; set; }
        public int? jaket { get; set; }
        public int? prm { get; set; }
        public int? kemahasiswaan { get; set; }
        public string? nama_prodi { get; set; }
        public string? nama_tagihan { get; set; }
    }

    public class PembayaranSKPUK
    {
        public bool is_jaminan { get; set; }
        public string jenis { get; set; }
        public int jumlah { get; set; }
        public int potongan { get; set; }
        public int total { get; set; }
        public string ket_angsuran { get; set; }
        public string tgl_buka { get; set; }
        public string batas_waktu { get; set; }
        public string batas_bayar { get; set; }

    }

    public class AngsuranSKPUK
    {
        public int jmluang { get; set; }
        public int angsuranke { get; set; }
        public string ket_angsuran { get; set; }
        public string batas_bayar { get; set; }
    }

    public class PotonganSKPUK
    {
        public string jns_potongan { get; set; }
        public string jenis_potongan { get; set; }
        public int jlh_total { get; set; }
    }

    public class MhsPendaftar2
    {
        public string thnakademik { get; set; }
        public string kd_jalur { get; set; }
    }
}
