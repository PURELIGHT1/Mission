using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PMB.Models
{
    public class Pendaftar
    {
        public string Kd_Calon { get; set; }
        public string Nm_Calon { get; set; }
        public string Jns_Kel { get; set; }
        public string Tmp_Lahir { get; set; }
        public string Tgl_Lahir { get; set; }
        public string Agama { get; set; }
        public string Kwrganegaraan { get; set; }
        public string Masuk { get; set; }
        public string Nm_Prodi { get; set; }
        public string Thnakademik { get; set; }
        public string Periode { get; set; }
        public string Kd_Jalur { get; set; }
        public string Id_Ortu { get; set; }
        public string Keterangan { get; set; }
        public string Jenjang { get; set; }

        //Angsuran
        public int AngsuranKe { get; set; }
        public int? Jml_Uang { get; set; }
        public string? Tgl_Buka { get; set; }
        public string? Batas_Waktu { get; set; }
        public string Ket_Angsuran { get; set; }
        public string? Status { get; set; }
        public string? Is_Jaminan { get; set; }
        public int? Jumlah { get; set; }
        public int? Potongan { get; set; }
        public string? Sks { get; set; }
    }

    public class SelectedItem
    {
        public string? thnAkademik { get; set; }
    }   

    public class StorePendaftar2
    {
        public string Kd_Calon { get; set; }
        public string Nm_Calon { get; set; }
        public string Kd_Jalur{ get; set; }
        public string? Id_Ortu { get; set; }
        public string Id_Sma { get; set; }
        public int Id_Calon_Online { get; set; }
        public int? Id_Calon_Online_Pasca { get; set; }
        public string jenjang { get; set; }
        public string email { get; set; }
        public string nama_jalur { get; set; }
        public int id_template_nilai { get; set; }
        public string berkas { get; set; }
    }

    public class DataDiri
    {
        public string Kd_Calon { get; set; }
        public string Nm_Calon { get; set; }
        //public string Nik { get; set; }
        //public string No_kk { get; set; }
        public string Tmp_Lahir { get; set; }
        public string Tgl_Lahir { get; set; }
        public string Jns_Kel { get; set; }
        public string Agama { get; set; }
        public string Kwrganegaraan { get; set; }
        public string Negara { get; set; }
        public string Nama_ibu { get; set; }
        public string Id_ortu { get; set; }
        public string Hp_pendaftar { get; set; }
        public string Email { get; set; }
        public string Periode { get; set; }
        public string Alamat { get; set; }
        public string? No_kps { get; set; }
        public string Jas { get; set; }
        public string Nisn { get; set; }
        public string Kode_pos { get; set; }
        public int Kec_asal { get; set; }
        [Required(ErrorMessage = "Kabupaten wajib diisi!")]
        public int Kota_asal { get; set; }
        public string Kebutuhan_khusus_mhs { get; set; }

    }

    public class DataSMA
    {
        public string Kd_calon { get; set; }
        public string Id_sma { get; set; }
        public string Jur_sma_smk { get; set; }
    }

    public class DataProdi
    {
        public string Kd_calon { get; set; }
        public string pil_1 { get; set; }
        public string pil_2 { get; set; }
        public string? pil_3 { get; set; }
    }

    public class UTBK
    {
        public string Kd_calon { get; set; }
        public int id_nilai_utbk { get; set; }
        public int id_ref_nilai_utbk { get; set; }
        public float nilai { get; set; }
    }

    public class Nilai
    {
        public int id_prestasi { get; set; }
        public string kd_calon { get; set; }
        public string kelas { get; set; }
        public string rangking { get; set; }
        public string semester { get; set; }
        public decimal matematika { get; set; }
        public decimal kkm_matematika { get; set; }
        public decimal bhs_inggris { get; set; }
        public decimal kkm_inggris { get; set; }
        public decimal bahasa { get; set; }
        public decimal kkm_indonesia { get; set; }

    }

    public class Prestasi
    {
        public int id_prestasi { get; set; }
        public string kd_calon { get; set ; }
        public string jns_keg { get; set; }
        public string kelompok { get; set; }
        public string tingkat { get; set; }
        public string prestasi { get; set; }
        public string tahun { get; set; }
        public string ket_prestasi { get; set; }
        public int id_ref_prestasi { get; set; }
        public string? deskripsi { get; set; }
        public int id { get; set; }
    }

    public class Kategori
    {
        public int id_ref_prestasi { get; set; }
        public string deskripsi { get; set; }
    }

    public class Dokumen
    {
        public string kd_calon;
        public int? id_calon_online;
        public string kd_jalur;
        public string nama_jalur;
        public int id_ref_jenis_dokumen;
        public string jenis_dokumen;
    }

    public class UploadDokumen
    {
        public string kd_calon { get; set; }
        public int id_calon_online { get; set; }
        public int id_ref_jenis_dokumen { get; set; }
        public string jalur { get; set; }
        public byte[] pdf { get; set; }
    }

    public class Verifikasi
    {
        public string kd_calon { get; set; }
        public string content { get; set; }
    }
}
