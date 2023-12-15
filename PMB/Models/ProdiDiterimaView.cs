using System.Web.Mvc;

namespace PMB.Models
{
    public class ProdiDiterimaView
    {
        public List<ProdiDiterimaExcel> ListDataProdiExcel { get; set; } = new List<ProdiDiterimaExcel>();
        public List<SelectListItem> AllTahunAkademik { get; set; }
        public List<SelectListItem> getAllTahunAkademik()
        {
            List<SelectListItem> myList = new List<SelectListItem>();
            for (int i = DateTime.Now.Year + 1; i >= 2009; i--)
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
        public List<dynamic> JalurList { get; set; }
        public List<dynamic> ProdiList { get; set; }
    }
}
