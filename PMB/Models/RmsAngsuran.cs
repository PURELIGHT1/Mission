using System.ComponentModel.DataAnnotations;

namespace PMB.Models
{
    public class RmsAngsuran
    {
        public int nomor { get; set; }
        public int id_detail { get; set; }
        public int id_rumus { get; set; }

        [Required(ErrorMessage = "Jenis tagihan wajib dipilih!")]
        public int id_tagihan { get; set; }
        
        public string jenis_tagihan { get; set; }

        [Required(ErrorMessage = "Termasuk dalam angsuran ke berapa wajib dipilih!")]
        public int angsuran_ke { get; set; }
        
        public int jml_persentase { get; set; }
        //[DataType(DataType.Date)]
        [Required(ErrorMessage = "Batas awal wajib diisi!")]
        public string tgl_buka { get; set; }

        //[DataType(DataType.Date)]
        [Required(ErrorMessage = "Batas akhir wajib diisi!")]
        public string tgl_tutup { get; set; }
        public string keterangan { get; set; }
        public string is_jaminan { get; set; }
        public int? jlh_angsuran { get; set; }

        [Required(ErrorMessage = "Persentase wajib diisi!")]
        [Range(1, 100, ErrorMessage = "Persentase wajib bernilai 1-100!")]
        public int prosentase { get; set; }
        public int? jumlah_detail_rumus { get; set; }
    }

    public class StoreRmsAngsuran
    {
        public int id_detail { get; set; }
        public int id_rumus { get; set; }
        public int id_tagihan { get; set; }
        public int angsuran_ke { get; set; }
        public int jml_persentase { get; set; }
        public string tgl_buka { get; set; }
        public string tgl_tutup { get; set; }
        public bool jaminan { get; set; }
    }
}
