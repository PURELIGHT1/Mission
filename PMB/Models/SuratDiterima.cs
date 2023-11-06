using System.ComponentModel.DataAnnotations;

namespace PMB.Models
{
    public class SuratDiterima
    {
        [Required(ErrorMessage = "Nomor surat wajib diisi!")]
        public string No_Surat { get; set; }

        [Required(ErrorMessage = "Jalur masuk wajib dipilih!")]
        public string Kd_Jalur { get; set; }
        public string? Nama_Jalur { get; set; }

        [Required(ErrorMessage = "Periode wajib diisi!")]
        public string? Periode { get; set; }

        [Required(ErrorMessage = "Periode mulai perkuliahan wajib diisi!")]
        public string? Perkuliahan { get; set; }

        [Required(ErrorMessage = "Periode registrasi ulang wajib diisi!")]
        public string? Periode_Regis { get; set; }

        [Required(ErrorMessage = "Tanggal registrasi ulang wajib diisi!")]
        public string? Tgl_Regis { get; set; }

        public string? Periode_Inisiasi { get; set; }
        public string? Tgl_Inisiasi { get; set; }
    }
}
