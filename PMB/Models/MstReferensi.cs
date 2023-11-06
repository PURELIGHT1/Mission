using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.Xml;

namespace PMB.Models
{
    public class MstReferensi
    {
        public int ID_SMA { get; set; }

        [Required(ErrorMessage = "Nama wajib diisi!")]
        public string NAMA_SMA { get; set; }
        [Required(ErrorMessage = "Alamat wajib diisi!")]
        public string ALAMAT { get; set; }
        public string? KODE_POS { get; set; }
        public string? TELEPON { get; set; }
        public string? FAKSIMILI { get; set; }
        [Required(ErrorMessage = "Provinsi wajib dipilih!")]
        public string KD_PROP { get; set; }

        //[Required(ErrorMessage = "Kabupaten/Kota wajib dipilih!")]
        public int ID_KABUPATEN { get; set; }
        //[Required(ErrorMessage = "Kecamatan wajib dipilih!")]
        public int ID_KECAMATAN { get; set; }
    }
}
