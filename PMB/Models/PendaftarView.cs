using MessagePack;
using System.Web.Mvc;

namespace PMB.Models
{
    public class PendaftarView
    {
        public List<Pendaftar> CalonMHS { get; set; } = new List<Pendaftar>();
        public List<AngsuranMhs> AngsuranCalonMhs { get; set; } = new List<AngsuranMhs>();
        public Pendaftar Pendaftar { get; set; } = new Pendaftar();
        public List<Nilai> ListNilai { get; set; } = new List<Nilai>(); 
        public Prestasi Prestasi { get; set; } = new Prestasi();
        public int cekDokumen { get; set; }
        public string kd_calon { get; set; }
        public int prestasi_id { get; set; }
        public string title { get; set; }
        //public DataDokumen DataDokumen { get; set; } = new DataDokumen();
        public List<Dokumen> ListDokumen { get; set; } = new List<Dokumen>();
        public List<Prestasi> ListPrestasi { get; set; } = new List<Prestasi>();
        public StorePendaftar2 StorePendaftar { get; set; } = new StorePendaftar2();
        public AngsuranMhs Angsuran { get; set; } = new AngsuranMhs();
        public String thnAkademik { get; set; } = new SelectedItem().thnAkademik;
        public String search { get; set; }
        public List<SelectListItem> AllTahunAkademik { get; set; }
        public List<SelectListItem> getAllTahunAkademik()
        {
            List<SelectListItem> myList = new List<SelectListItem>();
            for(int i = DateTime.Now.Year + 1; i >= 2009; i--)
            {
                String tahun = i + "/" + (i + 1);
                var data = new SelectListItem
                {
                    Value = tahun,
                    Text = "TA " + tahun,
                };
                myList.Add(data);
            }
            return myList;
        }

        public List<SelectListItem> getAllTahun()
        {
            List<SelectListItem> myList = new List<SelectListItem>();
            for (int i = DateTime.Now.Year+1; i >= 2013; i--)
            {
                var data = new SelectListItem
                {
                    Value = i.ToString(),
                    Text = i.ToString(),
                };
                myList.Add(data);
            }
            return myList;
        }
        public List<Provinsi> ProvinsiList { get; set; }
        public List<Kota> KotaList { get; set; }
        public List<Kecamatan> KecamatanList { get; set; }
        public List<Jas> JasList { get; set; }
        public List<dynamic> SMAList { get; set; }
        public List<Kategori> ListKategori { get; set; }
        public List<dynamic> ProdiList { get; set; }
        public List<dynamic> JalurList { get; set; }
    }
}
