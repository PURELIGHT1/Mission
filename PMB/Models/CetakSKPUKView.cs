namespace PMB.Models
{
    public class CetakSKPUKView
    {
        public CetakSKPUKMhs DataMhs { get; set; } = new CetakSKPUKMhs();
        public List<PembayaranSKPUK> JenisPembayaran { get; set; } = new List<PembayaranSKPUK>();
        public List<PotonganSKPUK> Potongan { get; set; } = new List<PotonganSKPUK>();
        public List<AngsuranSKPUK> ListAngsuranMhs = new List<AngsuranSKPUK>();
        public bool jaminan { get; set; }
        public dynamic Pejabat { get; set; }
        public bool wna { get; set; }
    }
}
