namespace PMB.Models
{
    public class SuratDiterimaView
    {
        public List<Jalur> JalurList { get; set; } = new List<Jalur>();
        public List<SuratDiterima> KetDiterimaList { get; set; } = new List<SuratDiterima>();
        public SuratDiterima KetDiterima { get; set; } = new SuratDiterima();
    }
}
