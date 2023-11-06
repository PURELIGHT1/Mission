using System.Web.Mvc;

namespace PMB.Models
{
    public class MstAngsuranView
    {
        public List<Jalur> JalurList { get; set; } = new List<Jalur>();
        public List<MstAngsuran> MstAngsuranList { get; set; } = new List<MstAngsuran>();
        public MstAngsuran MstAngsuran { get; set; } = new MstAngsuran();
        public List<SelectListItem> AllTahunAkademik { get; set; }
        public List<SelectListItem> getAllTahunAkademik()
        {
            List<SelectListItem> myList = new List<SelectListItem>();
            for (int i = DateTime.Now.Year; i >= 2009; i--)
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
    }
}
