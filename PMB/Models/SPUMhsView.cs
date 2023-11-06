namespace PMB.Models
{
    public class SPUMhsView
    {
        public List<Pendaftar> PendaftarList { get; set; } = new List<Pendaftar>();
        public List<SPUMhs2> SPUMhsList { get; set; } = new List<SPUMhs2>();
        public SPUMhs SPUCalonMhs { get; set; } = new SPUMhs();

    }
}
