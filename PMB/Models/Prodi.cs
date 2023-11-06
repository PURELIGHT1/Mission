namespace PMB.Models
{
    public class Prodi
    {
        public string id_prodi { get; set; }
        public string nama_prodi { get; set; }
        public string id_fakultas { get; set; }
        public string kampus { get; set; }
        public double rata2_raport { get; set; }
        public string singkatan { get; set; }
        public bool is_internasional { get; set; }
        public bool is_cutoff { get; set; }
    }
}
