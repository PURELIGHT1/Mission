namespace PMB.Models
{
    public class SKPUKView
    {
        public List<Prodi> ProdiList { get; set; } = new List<Prodi>();
        public List<Tagihan> TagihanList { get; set; } = new List<Tagihan>();
        public List<SKPUK> SKPUKList { get; set; } = new List<SKPUK>();
        public SKPUK SKPUK { get; set; } = new SKPUK();
    }
}
