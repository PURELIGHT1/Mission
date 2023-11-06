using Dapper;
using PMB.Models;
using System.Data.SqlClient;

namespace PMB.DAO
{
    public class AccountDAO
    {
        public UserModel getKaryawan(string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP (1) NPP, NAMA_LENGKAP_GELAR AS NAMA, PASSWORD
                                    FROM [simka].[MST_KARYAWAN]
                                    WHERE NPP = @npp";
                                    //JOIN [siatmax.TBL_USER_ROLE] b ON a.NPP = b.NPP
                                    //JOIN [siatmax.REF_ROLE] c ON b.ID_ROLE = c.ID_ROLE
                    var param = new { npp = npp };
                    var data = conn.QueryFirstOrDefault<UserModel>(query, param);

                    return data;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public List<MDLMENU> GetMenuKaryawan(string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT DISTINCT b.*
                                    FROM [siatmax.TBL_SISTEM_INFORMASI] a
                                    JOIN [siatmax.TBL_SI_MENU] b ON a.ID_SISTEM_INFORMASI = b.ID_SISTEM_INFORMASI
                                    JOIN [siatmax.TBL_SI_SUBMENU] c ON b.ID_SI_MENU = c.ID_SI_MENU
                                    JOIN [siatmax.TBL_ROLE_SUBMENU] d ON d.ID_SI_SUBMENU = c.ID_SI_SUBMENU
                                    JOIN [siatmax.REF_ROLE] e ON e.ID_ROLE = d.ID_ROLE
                                    JOIN [siatmax.TBL_USER_ROLE] f ON f.ID_ROLE = e.ID_ROLE
                                    JOIN [simka.MST_KARYAWAN] g ON g.NPP = f.NPP
                                    WHERE a.ID_SISTEM_INFORMASI = 2 AND e.ID_ROLE = 7";
                    var param = new { username = npp };
                    var data = conn.Query<MDLMENU>(query, param).AsList();

                    return data;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public List<MDLSUBMENU> GetSubMenuKaryawan(string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT DISTINCT c.*
                                    FROM [siatmax.TBL_SISTEM_INFORMASI] a
                                    JOIN [siatmax.TBL_SI_MENU] b ON a.ID_SISTEM_INFORMASI = b.ID_SISTEM_INFORMASI
                                    JOIN [siatmax.TBL_SI_SUBMENU] c ON b.ID_SI_MENU = c.ID_SI_MENU
                                    JOIN [siatmax.TBL_ROLE_SUBMENU] d ON d.ID_SI_SUBMENU = c.ID_SI_SUBMENU
                                    JOIN [siatmax.REF_ROLE] e ON e.ID_ROLE = d.ID_ROLE
                                    JOIN [siatmax.TBL_USER_ROLE] f ON f.ID_ROLE = e.ID_ROLE
                                    JOIN [simka.MST_KARYAWAN] g ON g.NPP = f.NPP
                                    WHERE a.ID_SISTEM_INFORMASI = 2 AND e.ID_ROLE = 7";
                    var param = new { username = npp };
                    var data = conn.Query<MDLSUBMENU>(query, param).AsList();

                    return data;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }
    }
}
