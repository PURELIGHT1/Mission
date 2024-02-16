namespace PMB.Models
{
    public class ProdiDiterima
    {
        public List<string> kode_calon { get; set; }
        public string id_prodi { get; set; }
        public string nm_prodi { get; set; }
    }

    public class ProdiDiterimaExcel
    {
        public string kd_calon { get; set; }
        public string pil_1 { get; set; }
        public string pil_2 { get; set; }
        public string pil_3 { get; set; }
        public string pilihan_1 { get; set; }
        public string pilihan_2 { get; set; }
        public string pilihan_3 { get; set; }
        public string masuk { get; set; }
        public string id_masuk { get; set; }
    }
}
