namespace PMB.Models
{
    public class RmsAngsuranView
    {
        public List<Tagihan> TagihanList { get; set; } = new List<Tagihan>();
        public List<RmsAngsuran> RmsAngsuranList { get; set; } = new List<RmsAngsuran>();
        public RmsAngsuran RmsAngsuran { get; set; } = new RmsAngsuran();
        public int jlh_angsuran { get; set; }
    }
}
