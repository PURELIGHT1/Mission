using Dapper;
using Microsoft.CSharp.RuntimeBinder;
using PMB.Models;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PMB.DAO
{
    public class SPUMhsDAO
    {
        //Tabel SPU
        public List<SPUMhs2> GetAllSPU()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT
		                                A.kd_calon,
		                                B.nm_calon,
		                                B.masuk,
		                                CONCAT(CASE WHEN c.jenjang = 's1' THEN 'S1'
													WHEN c.jenjang = 's2' THEN 'S2'
													WHEN c.jenjang = 's3' THEN 'S3'
												ELSE null 
												END, ' - ',c.nm_prodi) prodi_diterima,
		                                B.kd_jalur,
		                                D.nama_jalur,
		                                B.periode
                                  FROM spu A
                                  JOIN mhs_pendaftar B ON A.kd_calon = B.kd_calon
                                  JOIN ref_prodi C ON B.masuk = C.id_prodi
                                  JOIN ref_jalur D ON B.kd_jalur = D.kd_jalur
                                  WHERE b.masuk is not null and b.masuk not in ('', '00')
                                  ORDER BY kd_calon DESC";
                    
                    var data = conn.Query<SPUMhs2>(query).AsList();

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

        public dynamic GetDetailSPU(string id)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP(1) 
									    a.kd_calon,
									    b.kd_jalur,
									    c.nama_jalur,
									    b.masuk,
									    CONCAT(CASE WHEN d.jenjang = 's1' THEN 'S1'
										    WHEN d.jenjang = 's2' THEN 'S2'
										    WHEN d.jenjang = 's3' THEN 'S3'
										    ELSE null 
									    END, ' - ', d.nm_prodi) as prodi_diterima,
										CONVERT(varchar, a.tgl_cetak, 20) as tgl_cetak,
									    a.spu
								      FROM spu a
								      JOIN mhs_pendaftar b ON a.kd_calon = b.kd_calon
								      JOIN ref_jalur c ON b.kd_jalur = c.kd_jalur
								      JOIN ref_prodi d ON b.masuk = d.id_prodi
								      WHERE a.kd_calon = @kd_calon";

                    var data = conn.QueryFirstOrDefault<dynamic>(query, new { kd_calon = id });
                    return data;
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public bool SimpanSPU(SPUMhsView simpan)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"INSERT INTO [dbo].[SPU]
	                                    ([KD_CALON]
	                                    ,[SPU]
	                                    ,[USERNAME]
	                                    ,[TANGGAL]
                                        ,[TGL_CETAK]
	                                    ,[IS_MATRIKULASI])
                                    VALUES
                                        (@kd_calon, @spu, @username, GETDATE(), NULL, @is_matrikulasi)";

                    var data = conn.Execute(query, simpan.SPUCalonMhs);

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public List<string> CekProdiTarifSPU(CekProdiSPUMhs cekProdiSPUMhs)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    List<string> list = new List<string>();

                    string queryProdiAda = @"SELECT DISTINCT b.id_prodi
                                            FROM mhs_pendaftar a
                                            INNER JOIN ref_prodi b ON a.masuk = b.id_prodi
                                            INNER JOIN tr_tarif c ON a.masuk = c.id_prodi
                                            AND a.th_masuk = c.thmasuk
                                            WHERE kd_calon BETWEEN @calon1 AND @calon2
                                            AND b.id_prodi NOT IN ('', 00)";

                    string queryProdiTidakAda = @"SELECT DISTINCT CONCAT(UPPER(b.jenjang),' - ' , b.nm_prodi) as prodi
                                                FROM mhs_pendaftar a
                                                INNER JOIN ref_prodi b ON a.masuk = b.id_prodi
                                                LEFT OUTER JOIN tr_tarif c ON a.masuk = c.id_prodi AND a.th_masuk = c.thmasuk
                                                WHERE kd_calon BETWEEN @calon1 AND @calon2 AND b.id_prodi NOT IN ('', 00)
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM tr_tarif d
                                                    WHERE a.masuk = d.id_prodi AND a.th_masuk = d.thmasuk
                                                )";
                    var param = new { calon1 = cekProdiSPUMhs.kode_calon_awal, calon2 = cekProdiSPUMhs .kode_calon_akhir};

                    var ListProdiTidakAda = conn.Query<string>(queryProdiTidakAda, param).AsList();
                    var ListProdiAda = conn.Query<string>(queryProdiAda, param).AsList();
                    if(cekProdiSPUMhs.id_prodi == "All")
                    {
                        return ListProdiTidakAda;
                    }
                    else
                    {
                        foreach (string prodi in ListProdiAda)
                        {
                            
                            if (!prodi.Equals(cekProdiSPUMhs.id_prodi))
                            {
                                List<string> data = new List<string>
                                {
                                    "Prodi Tidak memiliki tarif SPU"
                                };
                                return data;
                            } 
                            else
                            {
                                List<string> data = new List<string>
                                {
                                    "Lengkap"
                                };
                                return data;
                            }
                        }
                    }
                    return null;
                }
                catch(Exception ex)
                {
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public bool SimpanDataSPU(StoreSPUMhs simpan)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string procedurePtgSPU = @"--declare @kdCalon varchar(10), @is_konsesi varchar(10)
                                            --set @kdCalon = '22200375'
                                            --set @is_konsesi = 'true'
                                            PRINT 'potongan unggulan'
		                                        INSERT INTO POTONGAN(KD_CALON,JNS_POTONGAN,JLH_TOTAL, JENIS, ID_TAGIHAN)
		                                        SELECT distinct kd_calon,JNS_POTONGAN ,JUMLAHPOTONGAN ,'SPU', e.id_tagihan
		                                        FROM  MHS_PENDAFTAR INNER JOIN
		                                        REF_POTONGAN ON MHS_PENDAFTAR.KD_JALUR = REF_POTONGAN.id_jalur
		                                        INNER JOIN MST_ANGSURAN c ON MHS_PENDAFTAR.KD_JALUR = c.KD_JALUR AND MHS_PENDAFTAR.THNAKADEMIK = c.THNAKADEMIK 
		                                        INNER JOIN detail_rumus_angsuran d ON c.ID_RUMUS = d.id_rumus 
		                                        INNER JOIN REF_TAGIHAN e ON d.id_tagihan = e.id_tagihan
		                                        where  th_masuk between tahun_masuk_awal and tahun_masuk_akhir
		                                        and kd_calon = @kdCalon 
		                                        and UPPER(e.nama_tagihan) LIKE '%SPU%'
		                                        and iscurrent = 1 and JNS_POTONGAN not in ('PI','PN','PL')

		                                        PRINT 'potongan prestasi'
	                                        IF exists (SELECT KD_CALON FROM DT_PRESTASI where DT_PRESTASI.KD_CALON = @kdCalon and 
	                                        DT_PRESTASI.TINGKAT = 'Internasional')
		                                        INSERT INTO POTONGAN (KD_CALON,JNS_POTONGAN,JLH_TOTAL, jenis, ID_TAGIHAN)
		                                        select distinct kd_Calon,JNS_POTONGAN,JUMLAHPOTONGAN, 'SPU', e.id_tagihan
		                                        FROM  MHS_PENDAFTAR INNER JOIN
		                                        REF_POTONGAN ON MHS_PENDAFTAR.KD_JALUR = REF_POTONGAN.id_jalur_new
		                                        INNER JOIN MST_ANGSURAN c ON MHS_PENDAFTAR.KD_JALUR = c.KD_JALUR AND MHS_PENDAFTAR.THNAKADEMIK = c.THNAKADEMIK 
		                                        INNER JOIN detail_rumus_angsuran d ON c.ID_RUMUS = d.id_rumus 
		                                        INNER JOIN REF_TAGIHAN e ON d.id_tagihan = e.id_tagihan
		                                        where th_masuk between tahun_masuk_awal and tahun_masuk_akhir
		                                        and UPPER(e.nama_tagihan) LIKE '%SPU%'
		                                        and kd_calon = @kdcalon  and  JNS_POTONGAN='PI' and iscurrent = 1

	                                        else if exists (SELECT KD_CALON FROM DT_PRESTASI 
		                                        where DT_PRESTASI.KD_CALON = @kdCalon and 
		                                        DT_PRESTASI.TINGKAT = 'Nasional')
		                                        INSERT INTO POTONGAN (KD_CALON,JNS_POTONGAN,JLH_TOTAL, jenis, ID_TAGIHAN)
		                                        select distinct kd_Calon,JNS_POTONGAN,JUMLAHPOTONGAN, 'SPU', e.id_tagihan
		                                        FROM  MHS_PENDAFTAR INNER JOIN
		                                        REF_POTONGAN ON MHS_PENDAFTAR.KD_JALUR = REF_POTONGAN.id_jalur_new
		                                        INNER JOIN MST_ANGSURAN c ON MHS_PENDAFTAR.KD_JALUR = c.KD_JALUR AND MHS_PENDAFTAR.THNAKADEMIK = c.THNAKADEMIK 
		                                        INNER JOIN detail_rumus_angsuran d ON c.ID_RUMUS = d.id_rumus 
		                                        INNER JOIN REF_TAGIHAN e ON d.id_tagihan = e.id_tagihan
		                                        where th_masuk between tahun_masuk_awal and tahun_masuk_akhir
		                                        and UPPER(e.nama_tagihan) LIKE '%SPU%'
		                                        and kd_calon = @kdcalon  and JNS_POTONGAN='PN' and iscurrent = 1

	                                        else if exists (SELECT KD_CALON FROM DT_PRESTASI 
	                                        where DT_PRESTASI.KD_CALON = @kdCalon and 
	                                        DT_PRESTASI.TINGKAT = 'Lokal')
		                                        INSERT INTO POTONGAN (KD_CALON,JNS_POTONGAN,JLH_TOTAL, jenis, ID_TAGIHAN)
		                                        select distinct kd_Calon,JNS_POTONGAN,JUMLAHPOTONGAN , 'SPU', e.id_tagihan
		                                        FROM  MHS_PENDAFTAR INNER JOIN
		                                        REF_POTONGAN ON MHS_PENDAFTAR.KD_JALUR = REF_POTONGAN.id_jalur_new
		                                        INNER JOIN MST_ANGSURAN c ON MHS_PENDAFTAR.KD_JALUR = c.KD_JALUR AND MHS_PENDAFTAR.THNAKADEMIK = c.THNAKADEMIK 
		                                        INNER JOIN detail_rumus_angsuran d ON c.ID_RUMUS = d.id_rumus 
		                                        INNER JOIN REF_TAGIHAN e ON d.id_tagihan = e.id_tagihan
		                                        where th_masuk between tahun_masuk_awal and tahun_masuk_akhir
		                                        and UPPER(e.nama_tagihan) LIKE '%SPU%'
		                                        and kd_calon = @kdcalon  and JNS_POTONGAN='PL' and iscurrent = 1

                                        PRINT 'potongan keluarga'
                                        IF exists (SELECT KD_CALON FROM DT_POTONGAN_KELUARGA 
	                                        where DT_POTONGAN_KELUARGA.KD_CALON = @kdCalon)
	                                        begin
		                                        INSERT INTO POTONGAN (KD_CALON,JNS_POTONGAN,JLH_TOTAL, jenis, ID_TAGIHAN)
		                                        select distinct @kdCalon,JNS_POTONGAN,JUMLAHPOTONGAN, 'SPU', e.id_tagihan
		                                        from dbo.REF_POTONGAN 
		                                        INNER JOIN MST_ANGSURAN c ON REF_POTONGAN.id_jalur_new = c.KD_JALUR
		                                        INNER JOIN detail_rumus_angsuran d ON c.ID_RUMUS = d.id_rumus 
		                                        INNER JOIN REF_TAGIHAN e ON d.id_tagihan = e.id_tagihan
		                                        where JNS_POTONGAN='SORTU' and iscurrent = 1
		                                        and UPPER(e.nama_tagihan) LIKE '%SPU%'

		                                        update DT_POTONGAN_KELUARGA
		                                        set status = 'true'
		                                        where kd_calon = @kdCalon
	                                        end		

	                                        PRINT 'cek konsesi/anak karyawan uajy!!'
	                                        if(@is_konsesi = 'true')
	                                        begin
		
		                                        INSERT INTO POTONGAN (KD_CALON,JNS_POTONGAN,JLH_TOTAL, jenis, ID_TAGIHAN)
		                                        SELECT TOP (1) MHS_PENDAFTAR.KD_CALON, 'konsesi' AS Expr1, SKPU_detail.JUMLAH, 'Spp Tetap', e.id_tagihan
		                                        FROM SKPU_detail INNER JOIN
		                                        MHS_PENDAFTAR ON SKPU_detail.ID_PRODI = MHS_PENDAFTAR.MASUK AND SKPU_detail.THNAKADEMIK = MHS_PENDAFTAR.THNAKADEMIK
		                                        INNER JOIN MST_ANGSURAN c ON MHS_PENDAFTAR.KD_JALUR = c.KD_JALUR AND MHS_PENDAFTAR.THNAKADEMIK = c.THNAKADEMIK 
		                                        INNER JOIN detail_rumus_angsuran d ON c.ID_RUMUS = d.id_rumus 
		                                        INNER JOIN REF_TAGIHAN e ON d.id_tagihan = e.id_tagihan
		                                        WHERE (SKPU_detail.Jenis = 'Spp Tetap') AND (MHS_PENDAFTAR.KD_CALON = @kdCalon)
		                                        and UPPER(e.nama_tagihan) LIKE '%SPP TETAP%'
		
		                                        update dbo.MHS_PENDAFTAR 
		                                        set IS_KONSESI  = 'true'
		                                        where kd_calon = @kdCalon

		                                        delete from potongan where KD_CALON= @kdcalon and JNS_POTONGAN in ('RAPORT','RANGKING')

	                                        end
                                            IF EXISTS (
                                                SELECT MHS_PENDAFTAR.MASUK
                                                FROM MHS_PENDAFTAR
                                                WHERE masuk < 90 AND KD_CALON = @kdcalon
                                            )
                                            BEGIN
                                                PRINT 'INSERT POTONGAN PASCASARJANA'

                                                IF NOT EXISTS (
                                                    SELECT id_jalur
                                                    FROM MHS_PENDAFTAR
                                                    INNER JOIN REF_POTONGAN_BY_IPK ON COALESCE(MHS_PENDAFTAR.IS_ALUMNI, 0) = REF_POTONGAN_BY_IPK.isAlumni
                                                        AND MHS_PENDAFTAR.KD_JALUR = REF_POTONGAN_BY_IPK.id_jalur 
                                                    WHERE MHS_PENDAFTAR.KD_CALON = @kdcalon
                                                )
                                                BEGIN
                                                    INSERT INTO POTONGAN (KD_CALON, JNS_POTONGAN, JLH_TOTAL, jenis, ID_TAGIHAN)
                                                    SELECT
                                                        MHS_PENDAFTAR.KD_CALON,
                                                        'IPK' AS JNS_POTONGAN,
                                                        COALESCE(SPU, 0) * potongan / 100 AS potongan,
                                                        e.nama_tagihan,
                                                        e.id_tagihan
                                                    FROM
                                                        MHS_PENDAFTAR
                                                    INNER JOIN REF_POTONGAN_BY_IPK ON COALESCE(MHS_PENDAFTAR.IS_ALUMNI, 0) = REF_POTONGAN_BY_IPK.isAlumni 
                                                    LEFT OUTER JOIN SPU ON MHS_PENDAFTAR.KD_CALON = SPU.KD_CALON
                                                    INNER JOIN MST_ANGSURAN c ON MHS_PENDAFTAR.KD_JALUR = c.KD_JALUR AND MHS_PENDAFTAR.THNAKADEMIK = c.THNAKADEMIK 
                                                    INNER JOIN detail_rumus_angsuran d ON c.ID_RUMUS = d.id_rumus 
                                                    INNER JOIN REF_TAGIHAN e ON d.id_tagihan = e.id_tagihan
                                                    WHERE
                                                        MHS_PENDAFTAR.KD_CALON = @kdCalon
                                                        AND IPK BETWEEN BATAS_MIN AND BATAS_MAX
                                                        AND ISCURRENT = 1
                                                        AND COALESCE(SPU, 0) > 0
                                                        AND id_jalur IS NULL
                                                        AND UPPER(e.nama_tagihan) LIKE '%SPP TETAP%' and UPPER(e.jenjang) LIKE '%S2%'
		                                            UNION ALL
		                                            SELECT 
			                                            MHS_PENDAFTAR.KD_CALON,
			                                            'SPP Tetap' JNS_POTONGAN,
			                                            COALESCE(JUMLAH,0)* REF_POTONGAN_BY_IPK.SPP_TETAP /100 potongan,
			                                            e.nama_tagihan, 
			                                            e.id_tagihan
		                                            FROM 
			                                            MHS_PENDAFTAR 
		                                            INNER JOIN REF_POTONGAN_BY_IPK ON coalesce(MHS_PENDAFTAR.IS_ALUMNI,0) = REF_POTONGAN_BY_IPK.isAlumni  
		                                            LEFT OUTER JOIN MST_ANGSURAN ON MHS_PENDAFTAR.THNAKADEMIK = MST_ANGSURAN.THNAKADEMIK 
			                                            AND MHS_PENDAFTAR.periode = MST_ANGSURAN.PERIODE 
			                                            AND MHS_PENDAFTAR.KD_JALUR = MST_ANGSURAN.KD_JALUR  
		                                            LEFT OUTER JOIN SKPU_detail ON MHS_PENDAFTAR.MASUK = SKPU_detail.ID_PRODI 
			                                            AND MHS_PENDAFTAR.THNAKADEMIK = SKPU_detail.THNAKADEMIK 
		                                            INNER JOIN MST_ANGSURAN c ON MHS_PENDAFTAR.KD_JALUR = c.KD_JALUR 
			                                            AND MHS_PENDAFTAR.THNAKADEMIK = c.THNAKADEMIK 
		                                            INNER JOIN detail_rumus_angsuran d ON c.ID_RUMUS = d.id_rumus 
		                                            INNER JOIN REF_TAGIHAN e ON d.id_tagihan = e.id_tagihan
		                                            WHERE 
			                                            (MHS_PENDAFTAR.KD_CALON = @kdCalon) 
			                                            AND SKPU_detail.Jenis ='SPP Tetap' 
			                                            AND IPK BETWEEN BATAS_MIN AND BATAS_MAX
			                                            AND iscurrent = 1 
			                                            AND id_jalur is null
                                                        AND UPPER(e.nama_tagihan) LIKE '%SPP TETAP%' and UPPER(e.jenjang) LIKE '%S2%'
		                                            UNION ALL
		                                            --begin  ( potongan heregistrasi + Spp T pasca)
		                                            SELECT 
			                                            MHS_PENDAFTAR.KD_CALON,'Biaya Herregistrasi' JNS_POTONGAN, 
			                                            COALESCE(JUMLAH*pct/100,0)* REF_POTONGAN_BY_IPK.BIAYA_REGIS /100 potongan, 
			                                            e.nama_tagihan, 
			                                            e.id_tagihan
		                                            FROM 
			                                            MHS_PENDAFTAR 
		                                            INNER JOIN REF_POTONGAN_BY_IPK ON COALESCE(MHS_PENDAFTAR.IS_ALUMNI,0) = REF_POTONGAN_BY_IPK.isAlumni  
		                                            LEFT OUTER JOIN MST_ANGSURAN ON MHS_PENDAFTAR.THNAKADEMIK = MST_ANGSURAN.THNAKADEMIK 
			                                            AND MHS_PENDAFTAR.periode = MST_ANGSURAN.PERIODE  and MHS_PENDAFTAR.KD_JALUR = MST_ANGSURAN.KD_JALUR  
		                                            LEFT OUTER JOIN RUMUS_ANGSURAN_detail ON MST_ANGSURAN.ID_RUMUS = RUMUS_ANGSURAN_detail.ID_RUMUS 
			                                            AND jenis ='Biaya Herregistrasi'
		                                            LEFT OUTER JOIN SKPU_detail ON MHS_PENDAFTAR.MASUK = SKPU_detail.ID_PRODI 
			                                            AND MHS_PENDAFTAR.THNAKADEMIK = SKPU_detail.THNAKADEMIK 
		                                            INNER JOIN MST_ANGSURAN c ON MHS_PENDAFTAR.KD_JALUR = c.KD_JALUR 
			                                            AND MHS_PENDAFTAR.THNAKADEMIK = c.THNAKADEMIK 
		                                            INNER JOIN detail_rumus_angsuran d ON c.ID_RUMUS = d.id_rumus 
		                                            INNER JOIN REF_TAGIHAN e ON d.id_tagihan = e.id_tagihan
		                                            WHERE 
			                                            (MHS_PENDAFTAR.KD_CALON = @kdCalon) 
			                                            AND SKPU_detail.Jenis ='Biaya Herregistrasi' 
			                                            AND IPK BETWEEN BATAS_MIN AND BATAS_MAX
			                                            AND iscurrent = 1 
			                                            AND id_jalur is null
                                                        AND UPPER(e.nama_tagihan) LIKE '%SPP TETAP%' and UPPER(e.jenjang) LIKE '%S2%'
		                                            UNION
		                                            SELECT DISTINCT 
			                                            kd_calon,
			                                            JNS_POTONGAN,
			                                            JUMLAHPOTONGAN,
			                                            e.nama_tagihan, 
			                                            e.id_tagihan
		                                            FROM 
			                                            REF_POTONGAN 
		                                            INNER JOIN MHS_PENDAFTAR ON REF_POTONGAN.id_prodi = MHS_PENDAFTAR.MASUK
		                                            INNER JOIN MST_ANGSURAN c ON MHS_PENDAFTAR.KD_JALUR = c.KD_JALUR 
			                                            AND MHS_PENDAFTAR.THNAKADEMIK = c.THNAKADEMIK 
		                                            INNER JOIN detail_rumus_angsuran d ON c.ID_RUMUS = d.id_rumus 
		                                            INNER JOIN REF_TAGIHAN e ON d.id_tagihan = e.id_tagihan
			                                            AND th_masuk between tahun_masuk_awal and tahun_masuk_akhir
		                                            WHERE 
			                                            kd_calon = @kdcalon 
			                                            AND iscurrent = 1 
			                                            AND id_jalur is null 
                                                        AND UPPER(e.nama_tagihan) LIKE '%SPP TETAP%' and UPPER(e.jenjang) LIKE '%S2%'
	                                            END
	                                            ELSE
	                                            BEGIN
		                                            INSERT INTO POTONGAN (KD_CALON,JNS_POTONGAN,JLH_TOTAL, jenis, ID_TAGIHAN)
		                                            --begin  ( potongan heregistrasi + Spp T pasca)
		                                            SELECT 
			                                            MHS_PENDAFTAR.KD_CALON,
			                                            'SPP Tetap' JNS_POTONGAN, 
			                                            COALESCE(JUMLAH,0)* REF_POTONGAN_BY_IPK.SPP_TETAP /100 potongan, 
			                                            e.nama_tagihan, 
			                                            e.id_tagihan
		                                            FROM 
			                                            MHS_PENDAFTAR 
		                                            INNER JOIN REF_POTONGAN_BY_IPK ON COALESCE(MHS_PENDAFTAR.IS_ALUMNI,0) = REF_POTONGAN_BY_IPK.isAlumni
		                                            AND MHS_PENDAFTAR.KD_JALUR = REF_POTONGAN_BY_IPK.id_jalur 
		                                            LEFT OUTER JOIN MST_ANGSURAN ON MHS_PENDAFTAR.THNAKADEMIK = MST_ANGSURAN.THNAKADEMIK 
			                                            AND MHS_PENDAFTAR.periode = MST_ANGSURAN.PERIODE  and MHS_PENDAFTAR.KD_JALUR = MST_ANGSURAN.KD_JALUR
		                                            LEFT OUTER JOIN SKPU_detail ON MHS_PENDAFTAR.MASUK = SKPU_detail.ID_PRODI 
			                                            AND MHS_PENDAFTAR.THNAKADEMIK = SKPU_detail.THNAKADEMIK 

		                                            INNER JOIN MST_ANGSURAN c ON MHS_PENDAFTAR.KD_JALUR = c.KD_JALUR 
			                                            AND MHS_PENDAFTAR.THNAKADEMIK = c.THNAKADEMIK 
		                                            INNER JOIN detail_rumus_angsuran d ON c.ID_RUMUS = d.id_rumus 
		                                            INNER JOIN REF_TAGIHAN e ON d.id_tagihan = e.id_tagihan

		                                            WHERE
			                                            (MHS_PENDAFTAR.KD_CALON = @kdCalon) 
			                                            AND SKPU_detail.Jenis ='SPP Tetap' 
			                                            AND  IPK BETWEEN BATAS_MIN AND BATAS_MAX
			                                            AND iscurrent = 1 
			                                            AND id_jalur is not null
                                                        AND UPPER(e.nama_tagihan) LIKE '%SPP TETAP%' and UPPER(e.jenjang) LIKE '%S2%'
		                                            UNION ALL
		                                            --begin  ( potongan heregistrasi + Spp T pasca)
		                                            SELECT 
			                                            MHS_PENDAFTAR.KD_CALON,
			                                            'Biaya Herregistrasi' JNS_POTONGAN, 
			                                            COALESCE(JUMLAH*pct/100,0)* REF_POTONGAN_BY_IPK.BIAYA_REGIS /100 potongan, 
			                                            e.nama_tagihan, 
			                                            e.id_tagihan
		                                            FROM 
			                                            MHS_PENDAFTAR 
		                                            INNER JOIN REF_POTONGAN_BY_IPK ON COALESCE(MHS_PENDAFTAR.IS_ALUMNI,0) = REF_POTONGAN_BY_IPK.isAlumni
			                                            AND MHS_PENDAFTAR.KD_JALUR = REF_POTONGAN_BY_IPK.id_jalur 
		                                            LEFT OUTER JOIN MST_ANGSURAN ON MHS_PENDAFTAR.THNAKADEMIK = MST_ANGSURAN.THNAKADEMIK 
			                                            AND MHS_PENDAFTAR.periode = MST_ANGSURAN.PERIODE 
			                                            AND MHS_PENDAFTAR.KD_JALUR = MST_ANGSURAN.KD_JALUR  
		                                            LEFT OUTER JOIN RUMUS_ANGSURAN_detail ON MST_ANGSURAN.ID_RUMUS = RUMUS_ANGSURAN_detail.ID_RUMUS 
			                                            AND jenis ='Biaya Herregistrasi'
		                                            LEFT OUTER JOIN SKPU_detail ON MHS_PENDAFTAR.MASUK = SKPU_detail.ID_PRODI 
			                                            AND MHS_PENDAFTAR.THNAKADEMIK = SKPU_detail.THNAKADEMIK  
		                                            INNER JOIN MST_ANGSURAN c ON MHS_PENDAFTAR.KD_JALUR = c.KD_JALUR 
			                                            AND MHS_PENDAFTAR.THNAKADEMIK = c.THNAKADEMIK 
		                                            INNER JOIN detail_rumus_angsuran d ON c.ID_RUMUS = d.id_rumus 
		                                            INNER JOIN REF_TAGIHAN e ON d.id_tagihan = e.id_tagihan
		                                            WHERE 
			                                            (MHS_PENDAFTAR.KD_CALON = @kdCalon) 
			                                            AND SKPU_detail.Jenis ='Biaya Herregistrasi' 
			                                            AND IPK BETWEEN BATAS_MIN AND BATAS_MAX 
			                                            AND iscurrent = 1 
			                                            AND id_jalur is not null 
                                                        AND UPPER(e.nama_tagihan) LIKE '%SPP TETAP%' and UPPER(e.jenjang) LIKE '%S2%'
		                                            UNION 
		                                            SELECT distinct 
			                                            kd_calon,
			                                            JNS_POTONGAN,
			                                            JUMLAHPOTONGAN, 
			                                            e.nama_tagihan, 
			                                            e.id_tagihan
		                                            FROM 
			                                            REF_POTONGAN 
		                                            INNER JOIN MHS_PENDAFTAR ON REF_POTONGAN.id_prodi = MHS_PENDAFTAR.MASUK
			                                            AND th_masuk between tahun_masuk_awal and tahun_masuk_akhir
		                                            INNER JOIN MST_ANGSURAN c ON MHS_PENDAFTAR.KD_JALUR = c.KD_JALUR 
			                                            AND MHS_PENDAFTAR.THNAKADEMIK = c.THNAKADEMIK 
		                                            INNER JOIN detail_rumus_angsuran d ON c.ID_RUMUS = d.id_rumus 
		                                            INNER JOIN REF_TAGIHAN e ON d.id_tagihan = e.id_tagihan

		                                            WHERE 
			                                            kd_calon = @kdcalon 
			                                            AND iscurrent = 1 
			                                            AND id_jalur is null 
                                                        AND UPPER(e.nama_tagihan) LIKE '%SPP TETAP%' and UPPER(e.jenjang) LIKE '%S2%'
	                                            END
                                            END";

                    string procedureInsertRmsPtg = @"INSERT INTO potongan([KD_CALON],[JNS_POTONGAN],[JLH_TOTAL],[JENIS],[id_tagihan])
                                                    SELECT mhs.KD_CALON, ptg.keterangan, ptg.nominal, tgh.nama_tagihan, tgh.id_tagihan
                                                    FROM detail_rumus_potongan ptg 
                                                    INNER JOIN MHS_PENDAFTAR mhs ON ptg.kd_jalur = mhs.KD_JALUR AND ptg.id_prodi = mhs.MASUK 
                                                    INNER JOIN MST_ANGSURAN ags ON ptg.id_rumus = ags.ID_RUMUS AND mhs.KD_JALUR = ags.KD_JALUR AND mhs.THNAKADEMIK = ags.THNAKADEMIK 
                                                    INNER JOIN ref_tagihan tgh ON ptg.id_tagihan = tgh.id_tagihan
                                                    WHERE mhs.KD_CALON between @kode_calon_awal and @kode_calon_akhir and mhs.KD_JALUR = @kd_jalur ";

                    string procedureInsertSPU = @"IF EXISTS (SELECT 1 FROM [dbo].[spu] WHERE kd_calon = @kdCalon)
                                                    BEGIN
                                                        -- Update existing data
                                                        PRINT 'update spu'
                                                        UPDATE [dbo].[spu]
                                                        SET
		                                                    username = @username,
                                                            spu =  CASE
                                                                        WHEN (select jenjang from ref_jalur where kd_jalur = @kd_jalur) LIKE 'S1' THEN tr.biaya
                                                                        ELSE 0
                                                                    END,
                                                            tanggal = GETDATE(),
                                                            tgl_cetak = @tgl_cetak
                                                        FROM
                                                            mhs_pendaftar mhs
                                                            INNER JOIN tr_tarif tr ON mhs.masuk = tr.id_prodi AND mhs.th_masuk = tr.thmasuk 
                                                            INNER JOIN ref_tagihan tgh ON tr.id_tagihan = tgh.id_tagihan 
                                                            INNER JOIN mst_angsuran ags ON mhs.kd_jalur = ags.kd_jalur AND mhs.thnakademik = ags.thnakademik 
                                                            INNER JOIN detail_rumus_angsuran d_ags ON tgh.id_tagihan = d_ags.id_tagihan AND ags.id_rumus = d_ags.id_rumus
                                                        WHERE
                                                            mhs.KD_JALUR = @kd_jalur AND
                                                            tgh.nama_tagihan LIKE '%spu%' AND
                                                            mhs.kd_calon = @kdCalon;
                                                    END
                                                ELSE
                                                    BEGIN
                                                        -- Insert new data
                                                        PRINT 'insert spu'
                                                        INSERT INTO [dbo].[spu] (kd_calon, username, spu, tanggal, tgl_cetak)
                                                        SELECT
                                                            mhs.kd_calon,
                                                            @username,
                                                             CASE
                                                                WHEN (select jenjang from ref_jalur where kd_jalur = @kd_jalur) LIKE 'S1' THEN tr.biaya
                                                                ELSE 0
                                                            END,
                                                            GETDATE(),
                                                            @tgl_cetak
                                                        FROM
                                                            mhs_pendaftar mhs
                                                            INNER JOIN tr_tarif tr ON mhs.masuk = tr.id_prodi AND mhs.th_masuk = tr.thmasuk 
                                                            INNER JOIN ref_tagihan tgh ON tr.id_tagihan = tgh.id_tagihan 
                                                            INNER JOIN mst_angsuran ags ON mhs.kd_jalur = ags.kd_jalur AND mhs.thnakademik = ags.thnakademik 
                                                            INNER JOIN detail_rumus_angsuran d_ags ON tgh.id_tagihan = d_ags.id_tagihan AND ags.id_rumus = d_ags.id_rumus
                                                        WHERE
                                                            mhs.KD_JALUR = @kd_jalur AND
                                                            tgh.nama_tagihan LIKE '%spu%' AND
                                                            mhs.kd_calon = @kdCalon
                                                        GROUP BY
                                                            mhs.kd_calon,
                                                            tr.biaya,
                                                            tr.id_tagihan,
                                                            mhs.th_masuk
                                                        ORDER BY
                                                            mhs.th_masuk DESC,
                                                            mhs.kd_calon;
                                                    END";

                    string procedureInsertAngsuran = @"insert into [dbo].[ANGSURAN_MHS]([KD_CALON],[ANGSURANKE],[TGL_BUKA],[BATAS_WAKTU],
                                                        [IS_JAMINAN],[ID_REF_TAGIHAN],[SKS],[JUMLAH],[JMLUANG],[POTONGAN],[KET_ANGSURAN], [status])
                                                        SELECT mhs.KD_CALON , d_ags.angsuran_ke, d_ags.tgl_buka, d_ags.tgl_tutup, 
                                                        d_ags.is_jaminan, tgh.id_tagihan, trf.jumlah_pengali AS sks, 
                                                        d_ags.prosentase * trf.jumlah_pengali * trf.biaya/100 jmlh,
                                                        case when coalesce(JLH_Potongan,0)= 0 then
                                                        d_ags.prosentase * trf.jumlah_pengali * trf.biaya/100
                                                        else
                                                        d_ags.prosentase * ((trf.jumlah_pengali * trf.biaya) - coalesce(JLH_Potongan,0)) /100
                                                        end AS jumlah_uang, 
                                                        d_ags.prosentase *   coalesce(JLH_Potongan,0) /100 potongan,
                                                        cast(prosentase as varchar(5))+'% '+nama_tagihan keterangan,
                                                        '0'

                                                        FROM MHS_PENDAFTAR mhs 
                                                        INNER JOIN MST_ANGSURAN ags ON mhs.KD_JALUR = ags.KD_JALUR AND mhs.THNAKADEMIK = ags.THNAKADEMIK AND mhs.periode = ags.periode
                                                        INNER JOIN detail_rumus_angsuran d_ags ON ags.ID_RUMUS = d_ags.id_rumus 
                                                        INNER JOIN ref_tagihan tgh ON d_ags.id_tagihan = tgh.id_tagihan 
                                                        INNER JOIN tr_tarif trf ON mhs.MASUK = trf.id_prodi AND mhs.TH_MASUK = trf.thmasuk AND tgh.id_tagihan = trf.id_tagihan
                                                        LEFT OUTER JOIN (select KD_CALON, id_tagihan, sum(JLH_TOTAL) JLH_Potongan from dbo.POTONGAN group by KD_CALON,id_tagihan) POTONGAN   
                                                        ON POTONGAN.KD_CALON = mhs.KD_CALON AND tgh.id_tagihan = POTONGAN.id_tagihan
                                                        WHERE mhs.KD_CALON between @calon1 and @calon2
                                                        AND mhs.KD_JALUR = @kd_jalur ";

                    string updateAgsBySPU = @"DELETE FROM angsuran_mhs
                                            WHERE kd_calon = @kdCalon AND ket_angsuran LIKE '%SPU%' and status = '0';

                                            INSERT INTO [dbo].[ANGSURAN_MHS] (
                                                [KD_CALON], [ANGSURANKE], [TGL_BUKA], [BATAS_WAKTU],
                                                [IS_JAMINAN], [ID_REF_TAGIHAN], [SKS], [JUMLAH], 
	                                            [JMLUANG], [POTONGAN], [KET_ANGSURAN], [status]
                                            )
                                            SELECT
                                                mhs.KD_CALON,
                                                d_ags.angsuran_ke,
                                                d_ags.tgl_buka,
                                                d_ags.tgl_tutup,
                                                d_ags.is_jaminan,
                                                tgh.id_tagihan,
                                                trf.jumlah_pengali AS sks,
                                                d_ags.prosentase * trf.jumlah_pengali * @spu_new / 100 AS jmlh,
                                                CASE
                                                    WHEN COALESCE(JLH_Potongan, 0) = 0 THEN d_ags.prosentase * trf.jumlah_pengali * @spu_new / 100
                                                    ELSE d_ags.prosentase * ((trf.jumlah_pengali * @spu_new) - COALESCE(JLH_Potongan, 0)) / 100
                                                END AS jumlah_uang,
                                                d_ags.prosentase * COALESCE(JLH_Potongan, 0) / 100 AS potongan,
                                                CAST(prosentase AS VARCHAR(5)) + '% ' + nama_tagihan AS keterangan,
                                                '0'
                                            FROM
	                                            MHS_PENDAFTAR mhs
                                                INNER JOIN MST_ANGSURAN ags ON mhs.KD_JALUR = ags.KD_JALUR AND mhs.THNAKADEMIK = ags.THNAKADEMIK
                                                INNER JOIN detail_rumus_angsuran d_ags ON ags.ID_RUMUS = d_ags.id_rumus
                                                INNER JOIN ref_tagihan tgh ON d_ags.id_tagihan = tgh.id_tagihan
                                                INNER JOIN tr_tarif trf ON mhs.MASUK = trf.id_prodi AND mhs.TH_MASUK = trf.thmasuk AND tgh.id_tagihan = trf.id_tagihan
                                                LEFT OUTER JOIN (
                                                    SELECT KD_CALON, id_tagihan, SUM(JLH_TOTAL) AS JLH_Potongan
                                                    FROM dbo.POTONGAN
                                                    GROUP BY KD_CALON, id_tagihan
                                                ) POTONGAN ON POTONGAN.KD_CALON = mhs.KD_CALON AND tgh.id_tagihan = POTONGAN.id_tagihan
                                            WHERE
                                                mhs.kd_calon = @kdCalon AND
                                                tgh.nama_tagihan LIKE '%SPU%'";

                    string queryProdiAda = @"SELECT DISTINCT b.id_prodi
                                            FROM mhs_pendaftar a
                                            INNER JOIN ref_prodi b ON a.masuk = b.id_prodi
                                            INNER JOIN tr_tarif c ON a.masuk = c.id_prodi
                                            AND a.th_masuk = c.thmasuk
                                            WHERE kd_calon BETWEEN @calon1 AND @calon2
                                            AND b.id_prodi NOT IN ('', 00)";

                    string deletePotongan = @"DELETE a
                                            FROM potongan a
                                            JOIN mhs_pendaftar b ON a.kd_calon = b.kd_calon
                                            where a.kd_calon between @calon1 and @calon2 
                                            and b.kd_jalur = @kd_jalur ";
                    string deleteRmsAgs = @"DELETE a
                                            FROM angsuran_mhs a 
                                            JOIN mhs_pendaftar b ON a.kd_calon = b.kd_calon
                                            WHERE a.kd_calon between @calon1 and @calon2
                                            and b.kd_jalur = @kd_jalur and status = '0'";

                    if (simpan.kode_calon_akhir.Equals("ubah"))
                    {
                        var deleteAllPtg = conn.Execute(deletePotongan, new
                        {
                            calon1 = simpan.kode_calon_awal,
                            calon2 = simpan.kode_calon_awal,
                            kd_jalur = simpan.kd_jalur
                        });

                        var deleteAgs = conn.Execute(deleteRmsAgs, new
                        {
                            calon1 = simpan.kode_calon_awal,
                            calon2 = simpan.kode_calon_awal,
                            kd_jalur = simpan.kd_jalur
                        });

                        //insert Rumus Potongan
                        var insertRmsPtg = conn.Execute(procedureInsertRmsPtg, new
                        {
                            kode_calon_awal = simpan.kode_calon_awal,
                            kode_calon_akhir = simpan.kode_calon_awal,
                            kd_jalur = simpan.kd_jalur,
                        });

                        //insert Angusuran
                        var insertAgs = conn.Execute(procedureInsertAngsuran, new
                        {
                            calon1 = simpan.kode_calon_awal,
                            calon2 = simpan.kode_calon_awal,
                            kd_jalur = simpan.kd_jalur
                        });

                        //update Angusuran SPU
                        var updateAgs = conn.Execute(updateAgsBySPU, new
                        {
                            kdCalon = simpan.kode_calon_awal,
                            spu_new = simpan.spu
                        });

                        var insertSPU = conn.Execute(procedureInsertSPU, new
                        {
                            kdCalon = simpan.kode_calon_awal,
                            username = simpan.username,
                            tgl_cetak = simpan.tgl_cetak,
                            kd_jalur = simpan.kd_jalur
                        });
                        return true;
                    }
                    else
                    {
                        if (simpan.id_prodi.Equals("All"))
                        {
                            var param = new { calon1 = simpan.kode_calon_awal, calon2 = simpan.kode_calon_akhir };
                            var ListProdiAda = conn.Query<string>(queryProdiAda, param).AsList();

                            string cariKdCalon = @"select kd_calon,
                                                    case 
	                                                    when is_konsesi = '1' then 'true'
	                                                    else 'false'
                                                    end as konsesi
                                                    from mhs_pendaftar 
                                                    where kd_calon between @calon1 AND @calon2 
                                                    and kd_jalur = @kd_jalur
                                                    and masuk in @listProdi";
                            var kdCalon = conn.Query<dynamic>(cariKdCalon, new
                            {
                                calon1 = simpan.kode_calon_awal,
                                calon2 = simpan.kode_calon_akhir,
                                kd_jalur = simpan.kd_jalur,
                                listProdi = ListProdiAda
                            }).AsList();


                            deletePotongan = deletePotongan + @" and b.masuk in @listProdi";
                            var deleteAllPtg = conn.Execute(deletePotongan, new {
                                calon1 = simpan.kode_calon_awal,
                                calon2 = simpan.kode_calon_akhir,
                                kd_jalur = simpan.kd_jalur,
                                listProdi = ListProdiAda
                            });


                            deleteRmsAgs = deleteRmsAgs + @" and b.masuk in @listProdi";
                            var deleteAgs = conn.Execute(deleteRmsAgs, new
                            {
                                calon1 = simpan.kode_calon_awal,
                                calon2 = simpan.kode_calon_akhir,
                                kd_jalur = simpan.kd_jalur,
                                listProdi = ListProdiAda
                            });

                            foreach (var items in kdCalon)
                            {
                                var insertPtgSPU = conn.Execute(procedurePtgSPU, new { kdCalon = items.kd_calon, is_konsesi = items.konsesi });
                            }

                            //insert Rumus Potongan
                            procedureInsertRmsPtg = procedureInsertRmsPtg + @" and mhs.masuk in @listProdi";
                            var insertRmsPtg = conn.Execute(procedureInsertRmsPtg, new
                            {
                                kode_calon_awal = simpan.kode_calon_awal,
                                kode_calon_akhir = simpan.kode_calon_akhir,
                                kd_jalur = simpan.kd_jalur,
                                listProdi = ListProdiAda
                            });

                            //insert Rumus Angsuran
                            procedureInsertAngsuran = procedureInsertAngsuran + @" and mhs.masuk in @listProdi";
                            var insertAgs = conn.Execute(procedureInsertAngsuran, new { 
                                calon1 = simpan.kode_calon_awal, 
                                calon2 = simpan.kode_calon_akhir ,
                                kd_jalur = simpan.kd_jalur,
                                listProdi = ListProdiAda
                            });

                            //insert SPU
                            foreach (var items in kdCalon)
                            {
                                var insertSPU = conn.Execute(procedureInsertSPU, new {
                                    kdCalon = items.kd_calon,
                                    username = simpan.username,
                                    tgl_cetak = simpan.tgl_cetak,
                                    kd_jalur = simpan.kd_jalur
                                });
                            }
                            return true;
                        }
                        else
                        {
                            var param = new { calon1 = simpan.kode_calon_awal, calon2 = simpan.kode_calon_akhir };
                            var ListProdiAda = conn.Query<string>(queryProdiAda, param).AsList();

                            string cariKdCalon = @"select kd_calon,
                                                    case 
	                                                    when is_konsesi = '1' then 'true'
	                                                    else 'false'
                                                    end as konsesi
                                                    from mhs_pendaftar 
                                                    where kd_calon between @calon1 AND @calon2 
                                                    and kd_jalur = @kd_jalur
                                                    and masuk = @listProdi";
                            var kdCalon = conn.Query<dynamic>(cariKdCalon, new
                            {
                                calon1 = simpan.kode_calon_awal,
                                calon2 = simpan.kode_calon_akhir,
                                kd_jalur = simpan.kd_jalur,
                                listProdi = simpan.id_prodi
                            }).AsList();

                            deletePotongan = deletePotongan + @" and b.masuk in @listProdi";
                            var deleteAllPtg = conn.Execute(deletePotongan, new
                            {
                                calon1 = simpan.kode_calon_awal,
                                calon2 = simpan.kode_calon_akhir,
                                kd_jalur = simpan.kd_jalur,
                                listProdi = ListProdiAda
                            });


                            deleteRmsAgs = deleteRmsAgs + @" and b.masuk in @listProdi";
                            var deleteAgs = conn.Execute(deleteRmsAgs, new
                            {
                                calon1 = simpan.kode_calon_awal,
                                calon2 = simpan.kode_calon_akhir,
                                kd_jalur = simpan.kd_jalur,
                                listProdi = ListProdiAda
                            });

                            foreach (var items in kdCalon)
                            {
                                var insertPtgSPU = conn.Execute(procedurePtgSPU, new { kdCalon = items.kd_calon, is_konsesi = items.konsesi });
                            }

                            //insert Rumus Potongan
                            procedureInsertRmsPtg = procedureInsertRmsPtg + @" and mhs.masuk = @listProdi";
                            var insertRmsPtg = conn.Execute(procedureInsertRmsPtg, new
                            {
                                kode_calon_awal = simpan.kode_calon_awal,
                                kode_calon_akhir = simpan.kode_calon_akhir,
                                kd_jalur = simpan.kd_jalur,
                                listProdi = simpan.id_prodi
                            });

                            //insert Angusuran
                            procedureInsertAngsuran = procedureInsertAngsuran + @" and mhs.masuk = @listProdi";
                            var insertAgs = conn.Execute(procedureInsertAngsuran, new
                            {
                                calon1 = simpan.kode_calon_awal,
                                calon2 = simpan.kode_calon_akhir,
                                kd_jalur = simpan.kd_jalur,
                                listProdi = simpan.id_prodi
                            });

                            //insert SPU
                            foreach (var items in kdCalon)
                            {
                                var insertSPU = conn.Execute(procedureInsertSPU, new
                                {
                                    kdCalon = items.kd_calon,
                                    username = simpan.username,
                                    tgl_cetak = simpan.tgl_cetak,
                                    kd_jalur = simpan.kd_jalur
                                });
                            }

                            return true;
                        }
                    }
                    return false;
                }
                catch(ArgumentException ex)
                {
                    // Penanganan exception
                    return false;
                    //Console.WriteLine($"ArgumentException: {ex.Message}");
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public SPUMhs DetailSPU(string kd_calon)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP (1) S.[KD_CALON]
                                        ,P.[NM_CALON] AS nama_calon
                                        ,P.[THNAKADEMIK]
                                        ,P.[MASUK]
                                        ,P.[periode]
                                        ,S.[SPU]
                                        ,S.[USERNAME]
                                        ,CONVERT(VARCHAR,S.[TANGGAL],23) AS TANGGAL
                                        ,CONVERT(VARCHAR,S.[TGL_CETAK],23) AS TGL_CETAK
                                        ,S.[IS_MATRIKULASI]
                                        FROM [dbo].[SPU] S
                                        JOIN [dbo].[MHS_PENDAFTAR] P ON P.[KD_CALON] = S.[KD_CALON]
                                     WHERE S.[KD_CALON] = @kd_calon";

                    var param = new { kd_calon = kd_calon };
                    var data = conn.QueryFirstOrDefault<SPUMhs>(query, param);

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

        public bool UbahSPU(SPUMhsView spuMhs)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"UPDATE [Mission].[dbo].[SPU] SET
                                        [SPU] = @spu
	                                    ,[USERNAME] = @username
	                                    ,[IS_MATRIKULASI] = @is_matrikulasi
                                     WHERE [KD_CALON] = @kd_calon";

                    var data = conn.Execute(query, spuMhs.SPUCalonMhs);

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public bool HapusSPU(string kd_calon)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"DELETE FROM [dbo].[SPU] WHERE KD_CALON = @kd_calon";

                    var data = conn.Execute(query, new { kd_calon = kd_calon });

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        // Tabel Pendaftar
        public List<Pendaftar> GetPendaftar()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP (5000) MHS.[KD_CALON], MHS.[NM_CALON]
                                         FROM [dbo].[MHS_PENDAFTAR] MHS
                                         WHERE MHS.[KD_CALON] NOT IN
                                            (SELECT SPU.[KD_CALON] 
                                             FROM [dbo].[SPU] SPU)
                                         ORDER BY CONVERT(INT,MHS.[KD_CALON]) DESC";

                    //WHERE [KD_CALON] = '22334455'";
                    var data = conn.Query<Pendaftar>(query).AsList();

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

        public SPUMhs GetSPU(string kd_calon)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string query = @"DECLARE @id_prodi VARCHAR(10), @thnakademik VARCHAR(25)
                                     SET @id_prodi = (SELECT [MASUK] FROM [dbo].[MHS_PENDAFTAR] WHERE [KD_CALON] = @kd_calon)
                                     SET @thnakademik = (SELECT [THNAKADEMIK] FROM [dbo].[MHS_PENDAFTAR] WHERE [KD_CALON] = @kd_calon)

                                     SELECT S.[NOMINAL] AS SPU FROM [dbo].[TBL_SKPU] S
                                     LEFT JOIN [dbo].[MHS_PENDAFTAR] M
                                     ON S.[THNAKADEMIK] = M.[THNAKADEMIK]
                                     WHERE S.[ID_TAGIHAN] = 1 AND M.[MASUK] = @id_prodi AND S.[THNAKADEMIK] = @thnakademik";

                    var data = conn.QueryFirstOrDefault<SPUMhs>(query, new { kd_calon = kd_calon });

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
