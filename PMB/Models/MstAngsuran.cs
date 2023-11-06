using System.ComponentModel.DataAnnotations;

namespace PMB.Models
{
    public class MstAngsuran
    {
        public string id_rumus { get; set; }
        public int? jlh_angsuran { get; set; }

        [Required(ErrorMessage = "Tahun akademik wajib diisi!")]
        [RegularExpression(@"^([1-9]{1}[0-9]{3})+[-/.]+([1-9]{1}[0-9]{3})$", ErrorMessage = "Format tahun akademik wajib YYYY/YYYY!")]
        public string thnakademik { get; set; }

        [Required(ErrorMessage = "Nama jalur wajib dipilih!")]
        public string kd_jalur { get; set; }
        public string? nama_jalur { get; set; }
        public int periode { get; set; }

        // Punya rumus angsuran
        public int? nomor { get; set; }
        public int? id_detail { get; set; }
        public int? id_tagihan { get; set; }
        public string? jenis_tagihan { get; set; }
        public int? angsuran_ke { get; set; }
        public string? prosentase { get; set; }
        public string? tgl_buka { get; set; }
        public string? tgl_tutup { get; set; }
        public string? keterangan { get; set; }
        public string? is_jaminan { get; set; }
    }

    public class StoreMstAngsuran
    {
        public int id_rumus { get; set; }
        public string ta { get; set; }
        public string kd_jalur { get; set; }
        public int jumlah { get; set; }
        public int periode { get; set; }
    }
}
