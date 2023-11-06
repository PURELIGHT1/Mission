using System.Web.Mvc;

namespace PMB.Models
{
    public class MstReferensiView
    {
        public List<MstReferensi> ReferensiList { get; set; } = new List<MstReferensi>();
        public string SelectedProvinsiId { get; set; }
        public int SelectedKotaId { get; set; }
        public int SelectedKecamatanId { get; set; }
        public String title { get; set; }
        public List<Provinsi> ProvinsiList { get; set; }
        public List<Kota> KotaList { get; set; }
        public List<Kecamatan> KecamatanList { get; set; }
        
        public MstReferensi MstReferensi { get; set; }  = new MstReferensi();
        
    }
}
