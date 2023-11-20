using Dapper;
using PMB.Models;
using System.Data.SqlClient;
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

        public bool SimpanDataSPU(StoreSPUMhs simpan)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.koneksi))
            {
                try
                {
                    string queryTambah = @"INSERT INTO [dbo].[SPU]
	                                    ([KD_CALON]
	                                    ,[SPU]
	                                    ,[USERNAME]
	                                    ,[TANGGAL]
                                        ,[TGL_CETAK]
	                                    ,[IS_MATRIKULASI])
                                    VALUES
                                        (@kd_calon, @spu, @username, GETDATE(), @tgl_cetak, NULL)";

                    string queryUbah = @"UPDATE [dbo].[SPU] SET 
                                            SPU = @spu,
                                            USERNAME = @username,
                                            TANGGAL = GETDATE(),
                                            TGL_CETAK = @tgl_cetak
                                        WHERE KD_CALON = @kd_calon";

                    string queryTambah2 = @"INSERT INTO [dbo].[angsuran_mhs]
	                                    ([kd_calon]
	                                    ,[angsuranke]
	                                    ,[jmluang]
                                        ,[tgl_buka]
                                        ,[batas_waktu]
                                        ,[ket_angsuran]
                                        ,[status]
                                        ,[is_jaminan]
                                        ,[jumlah]
                                        ,[potongan]
                                        ,[sks])
                                    VALUES
                                        (@kode_calon, @angsuran_ke, @jmlUang, @tgl_buka, @batas_waktu, @ket_ags, 0, @is_jaminan, @jumlah, @ptg, @sks)";

                    string queryUbah2 = @"UPDATE [dbo].[angsuran_mhs] SET
                                            angsuranke = @angsuran_ke,
                                            jmluang = @jmlUang,
                                            tgl_buka = @tgl_buka,
                                            batas_waktu = @batas_waktu,
                                            ket_angsuran = @ket_ags,
                                            status = 1,
                                            is_jaminan = @is_jaminan,
                                            jumlah = @jumlah,
                                            potongan = @ptg,
                                            sks = @sks,
                                        WHERE kd_calon = @kode_calon";

                    string queryInsertRmsPtg = @"INSERT INTO [dbo].[potongan]
                                                    ([kd_calon]
                                                    ,[jns_potongan]
                                                    ,[jlh_total]
                                                    ,[id_tagihan])
                                                VALUES
                                                    (@kode_calon, @jns_ptg, @jmlUang, @tagihan)";

                    string queryAgsSPU = @"SELECT kd_calon, angsuranke, ket_angsuran, potongan, jumlah
                                        FROM angsuran_mhs WHERE ket_angsuran like '%SPU%' and kd_calon = @kd_calon_mhs and status = '0'";

                    string procedurePtgSPU = @"--Declare @id_tagihan int
                                            --@is_konsesi varchar(10)
                                            --set @kdCalon = '22100097'
                                            --select @is_konsesi = is_konsesi, @pot_spu=0 from dbo.mhs_pendaftar a where kd_calon = @kdcalon
                                                PRINT 'potongan unggulan'
		                                            INSERT INTO POTONGAN(KD_CALON,JNS_POTONGAN,JLH_TOTAL, JENIS)
		                                            SELECT distinct kd_calon, KETERANGAN ,JUMLAHPOTONGAN ,'SPU'
		                                            FROM  MHS_PENDAFTAR INNER JOIN
                                                    REF_POTONGAN ON MHS_PENDAFTAR.KD_JALUR = REF_POTONGAN.id_jalur
		                                            where  th_masuk between tahun_masuk_awal and tahun_masuk_akhir
		                                             and kd_calon = @kdCalon  
		                                            and iscurrent = 1 and JNS_POTONGAN not in ('PI','PN','PL')

		                                        PRINT 'potongan prestasi'
	                                                IF exists (SELECT KD_CALON FROM DT_PRESTASI where DT_PRESTASI.KD_CALON = @kdCalon and 
	                                                DT_PRESTASI.TINGKAT = 'Internasional')
		                                                INSERT INTO POTONGAN (KD_CALON,JNS_POTONGAN,JLH_TOTAL, jenis)
		                                                select distinct kd_Calon,JNS_POTONGAN,JUMLAHPOTONGAN, 'SPU'
		                                                FROM  MHS_PENDAFTAR INNER JOIN
                                                     REF_POTONGAN ON MHS_PENDAFTAR.KD_JALUR = REF_POTONGAN.id_jalur_new
		                                                where th_masuk between tahun_masuk_awal and tahun_masuk_akhir
		                                                and kd_calon = @kdCalon  and  JNS_POTONGAN='PI' and iscurrent = 1

	                                                else if exists (SELECT KD_CALON FROM DT_PRESTASI 
		                                                where DT_PRESTASI.KD_CALON = @kdCalon and 
		                                                DT_PRESTASI.TINGKAT = 'Nasional')
		                                                INSERT INTO POTONGAN (KD_CALON,JNS_POTONGAN,JLH_TOTAL, jenis)
		                                                select distinct kd_Calon,JNS_POTONGAN,JUMLAHPOTONGAN, 'SPU'
		                                                FROM  MHS_PENDAFTAR INNER JOIN
                                                     REF_POTONGAN ON MHS_PENDAFTAR.KD_JALUR = REF_POTONGAN.id_jalur_new
		                                                where th_masuk between tahun_masuk_awal and tahun_masuk_akhir
		                                                and kd_calon = @kdCalon  and JNS_POTONGAN='PN' and iscurrent = 1

	                                                else if exists (SELECT KD_CALON FROM DT_PRESTASI 
	                                                where DT_PRESTASI.KD_CALON = @kdCalon and 
	                                                DT_PRESTASI.TINGKAT = 'Lokal')
		                                                INSERT INTO POTONGAN (KD_CALON,JNS_POTONGAN,JLH_TOTAL, jenis)
		                                                select distinct kd_Calon,JNS_POTONGAN,JUMLAHPOTONGAN , 'SPU'
		                                                FROM  MHS_PENDAFTAR INNER JOIN
                                                     REF_POTONGAN ON MHS_PENDAFTAR.KD_JALUR = REF_POTONGAN.id_jalur_new
		                                                where th_masuk between tahun_masuk_awal and tahun_masuk_akhir
		                                                and kd_calon = @kdCalon  and JNS_POTONGAN='PL' and iscurrent = 1

                                                PRINT 'potongan keluarga'
                                                    IF exists (SELECT KD_CALON FROM DT_POTONGAN_KELUARGA 
	                                                    where DT_POTONGAN_KELUARGA.KD_CALON = @kdCalon)
	                                                    begin
		                                                    INSERT INTO POTONGAN (KD_CALON,JNS_POTONGAN,JLH_TOTAL, jenis)
		                                                    select distinct @kdCalon,JNS_POTONGAN,JUMLAHPOTONGAN, 'SPU' 
		                                                    from dbo.REF_POTONGAN where JNS_POTONGAN='SORTU' and iscurrent = 1

		                                                    update DT_POTONGAN_KELUARGA
		                                                    set status = 'true'
		                                                    where kd_calon = @kdCalon
	                                                    end		

	                                            PRINT 'cek konsesi/anak karyawan uajy!!'
	                                                if(@is_konsesi = 'true')
	                                                begin
	 
		                                                --INSERT INTO POTONGAN (KD_CALON,JNS_POTONGAN,JLH_TOTAL, jenis)
		                                                --select @kdCalon,'SPU',@pot_spu, 'SPU'
		
		                                                INSERT INTO POTONGAN (KD_CALON,JNS_POTONGAN,JLH_TOTAL, jenis)
		                                                SELECT TOP (1) MHS_PENDAFTAR.KD_CALON, 'Konsesi' AS Expr1, SKPU_detail.JUMLAH, 'Spp Tetap'
		                                                FROM SKPU_detail INNER JOIN
		                                                    MHS_PENDAFTAR ON SKPU_detail.ID_PRODI = MHS_PENDAFTAR.MASUK AND SKPU_detail.THNAKADEMIK = MHS_PENDAFTAR.THNAKADEMIK
		                                                WHERE (SKPU_detail.Jenis = 'Spp Tetap') AND (MHS_PENDAFTAR.KD_CALON = @kdCalon)
		
		                                                update dbo.MHS_PENDAFTAR 
		                                                set IS_KONSESI  = 'true'
		                                                where kd_calon = @kdCalon

		                                                delete from potongan where KD_CALON= @kdcalon and JNS_POTONGAN in ('RAPORT','RANGKING')

	                                                end	

	
                                                IF exists (select MHS_PENDAFTAR.MASUK from MHS_PENDAFTAR 
	                                                where masuk < 90 and KD_CALON = @kdCalon)
	                                                begin
                                                print 'INSERT POTONGAN PASCASARJANA '

                                                    if not  exists (select id_jalur from MHS_PENDAFTAR INNER JOIN REF_POTONGAN_BY_IPK ON coalesce(MHS_PENDAFTAR.IS_ALUMNI,0) = REF_POTONGAN_BY_IPK.isAlumni  
                                                       and MHS_PENDAFTAR.KD_JALUR = REF_POTONGAN_BY_IPK.id_jalur 
                                                     where  MHS_PENDAFTAR.KD_CALON = @kdCalon)
                                                     begin
                                                    INSERT INTO POTONGAN (KD_CALON,JNS_POTONGAN,JLH_TOTAL)
                                                    SELECT MHS_PENDAFTAR.KD_CALON,'IPK' JNS_POTONGAN, coalesce(SPU,0)* potongan /100 potongan
                                                    FROM MHS_PENDAFTAR INNER JOIN REF_POTONGAN_BY_IPK ON coalesce(MHS_PENDAFTAR.IS_ALUMNI,0) = REF_POTONGAN_BY_IPK.isAlumni 
                                                    left outer JOIN SPU ON MHS_PENDAFTAR.KD_CALON = SPU.KD_CALON
                                                    WHERE  (MHS_PENDAFTAR.KD_CALON = @kdCalon) AND IPK BETWEEN BATAS_MIN AND BATAS_MAX   and iscurrent = 1
                                                    and  coalesce(SPU,0) >0 and id_jalur is null
                                                    union all
                                                    --begin  ( potongan heregistrasi + Spp T pasca) rachel 31 Mei 2022
                                                    SELECT MHS_PENDAFTAR.KD_CALON,'SPP Tetap' JNS_POTONGAN, 
                                                    coalesce(JUMLAH,0)* REF_POTONGAN_BY_IPK.SPP_TETAP /100 potongan
                                                    FROM MHS_PENDAFTAR INNER JOIN REF_POTONGAN_BY_IPK ON coalesce(MHS_PENDAFTAR.IS_ALUMNI,0) = REF_POTONGAN_BY_IPK.isAlumni  
                                                    left outer JOIN MST_ANGSURAN ON MHS_PENDAFTAR.THNAKADEMIK = MST_ANGSURAN.THNAKADEMIK 
                                                    AND MHS_PENDAFTAR.periode = MST_ANGSURAN.PERIODE  and MHS_PENDAFTAR.KD_JALUR = MST_ANGSURAN.KD_JALUR  
                                                    --left outer JOIN RUMUS_ANGSURAN_detail ON MST_ANGSURAN.ID_RUMUS = RUMUS_ANGSURAN_detail.ID_RUMUS
                                                    left outer JOIN SKPU_detail ON MHS_PENDAFTAR.MASUK = SKPU_detail.ID_PRODI 
                                                    AND MHS_PENDAFTAR.THNAKADEMIK = SKPU_detail.THNAKADEMIK 
                                                    --AND RUMUS_ANGSURAN_detail.jenis = SKPU_detail.Jenis
                                                    WHERE  (MHS_PENDAFTAR.KD_CALON = @kdCalon) and SKPU_detail.Jenis ='SPP Tetap' AND 
                                                    IPK BETWEEN BATAS_MIN AND BATAS_MAX   and iscurrent = 1  and id_jalur is null
                                                    union all
                                                    --begin  ( potongan heregistrasi + Spp T pasca) rachel 31 Mei 2022
                                                    SELECT MHS_PENDAFTAR.KD_CALON,'Biaya Herregistrasi' JNS_POTONGAN, 
                                                    coalesce(JUMLAH*pct/100,0)* REF_POTONGAN_BY_IPK.BIAYA_REGIS /100 potongan
                                                    FROM MHS_PENDAFTAR INNER JOIN REF_POTONGAN_BY_IPK ON coalesce(MHS_PENDAFTAR.IS_ALUMNI,0) = REF_POTONGAN_BY_IPK.isAlumni  
                                                    left outer JOIN MST_ANGSURAN ON MHS_PENDAFTAR.THNAKADEMIK = MST_ANGSURAN.THNAKADEMIK 
                                                    AND MHS_PENDAFTAR.periode = MST_ANGSURAN.PERIODE  and MHS_PENDAFTAR.KD_JALUR = MST_ANGSURAN.KD_JALUR  
                                                     left outer JOIN RUMUS_ANGSURAN_detail ON MST_ANGSURAN.ID_RUMUS = RUMUS_ANGSURAN_detail.ID_RUMUS and jenis ='Biaya Herregistrasi'
                                                    left outer JOIN SKPU_detail ON MHS_PENDAFTAR.MASUK = SKPU_detail.ID_PRODI 
                                                    AND MHS_PENDAFTAR.THNAKADEMIK = SKPU_detail.THNAKADEMIK 
                                                    --AND RUMUS_ANGSURAN_detail.jenis = SKPU_detail.Jenis
                                                    WHERE  (MHS_PENDAFTAR.KD_CALON = @kdCalon) and SKPU_detail.Jenis ='Biaya Herregistrasi' AND 
                                                    IPK BETWEEN BATAS_MIN AND BATAS_MAX   and iscurrent = 1 and id_jalur is null
                                                    union 
                                                     SELECT distinct kd_calon,JNS_POTONGAN ,JUMLAHPOTONGAN 
                                                     FROM REF_POTONGAN INNER JOIN
                                                     MHS_PENDAFTAR ON REF_POTONGAN.id_prodi = MHS_PENDAFTAR.MASUK
                                                     and th_masuk between tahun_masuk_awal and tahun_masuk_akhir
                                                     where kd_calon = @kdCalon  and iscurrent = 1 and id_jalur is null 
 
                                                     end
                                                     else
                                                     begin
                                                    INSERT INTO POTONGAN (KD_CALON,JNS_POTONGAN,JLH_TOTAL)
                                                    --begin  ( potongan heregistrasi + Spp T pasca) rachel 31 Mei 2022
                                                    SELECT MHS_PENDAFTAR.KD_CALON,'SPP Tetap' JNS_POTONGAN, 
                                                    coalesce(JUMLAH,0)* REF_POTONGAN_BY_IPK.SPP_TETAP /100 potongan
                                                    FROM MHS_PENDAFTAR INNER JOIN REF_POTONGAN_BY_IPK ON coalesce(MHS_PENDAFTAR.IS_ALUMNI,0) = REF_POTONGAN_BY_IPK.isAlumni     and MHS_PENDAFTAR.KD_JALUR = REF_POTONGAN_BY_IPK.id_jalur 
                                                    left outer JOIN MST_ANGSURAN ON MHS_PENDAFTAR.THNAKADEMIK = MST_ANGSURAN.THNAKADEMIK 
                                                    AND MHS_PENDAFTAR.periode = MST_ANGSURAN.PERIODE  and MHS_PENDAFTAR.KD_JALUR = MST_ANGSURAN.KD_JALUR  
                                                    --left outer JOIN RUMUS_ANGSURAN_detail ON MST_ANGSURAN.ID_RUMUS = RUMUS_ANGSURAN_detail.ID_RUMUS
                                                    left outer JOIN SKPU_detail ON MHS_PENDAFTAR.MASUK = SKPU_detail.ID_PRODI 
                                                    AND MHS_PENDAFTAR.THNAKADEMIK = SKPU_detail.THNAKADEMIK 
                                                    --AND RUMUS_ANGSURAN_detail.jenis = SKPU_detail.Jenis
                                                    WHERE  (MHS_PENDAFTAR.KD_CALON = @kdCalon) and SKPU_detail.Jenis ='SPP Tetap' AND 
                                                    IPK BETWEEN BATAS_MIN AND BATAS_MAX   and iscurrent = 1  and id_jalur is not null
                                                    union all
                                                    --begin  ( potongan heregistrasi + Spp T pasca) rachel 31 Mei 2022
                                                    SELECT MHS_PENDAFTAR.KD_CALON,'Biaya Herregistrasi' JNS_POTONGAN, 
                                                    coalesce(JUMLAH*pct/100,0)* REF_POTONGAN_BY_IPK.BIAYA_REGIS /100 potongan
                                                    FROM MHS_PENDAFTAR INNER JOIN REF_POTONGAN_BY_IPK ON coalesce(MHS_PENDAFTAR.IS_ALUMNI,0) = REF_POTONGAN_BY_IPK.isAlumni  and MHS_PENDAFTAR.KD_JALUR = REF_POTONGAN_BY_IPK.id_jalur 
                                                    left outer JOIN MST_ANGSURAN ON MHS_PENDAFTAR.THNAKADEMIK = MST_ANGSURAN.THNAKADEMIK 
                                                    AND MHS_PENDAFTAR.periode = MST_ANGSURAN.PERIODE  and MHS_PENDAFTAR.KD_JALUR = MST_ANGSURAN.KD_JALUR  
                                                     left outer JOIN RUMUS_ANGSURAN_detail ON MST_ANGSURAN.ID_RUMUS = RUMUS_ANGSURAN_detail.ID_RUMUS and jenis ='Biaya Herregistrasi'
                                                    left outer JOIN SKPU_detail ON MHS_PENDAFTAR.MASUK = SKPU_detail.ID_PRODI 
                                                    AND MHS_PENDAFTAR.THNAKADEMIK = SKPU_detail.THNAKADEMIK  
                                                    WHERE  (MHS_PENDAFTAR.KD_CALON = @kdCalon) and SKPU_detail.Jenis ='Biaya Herregistrasi' AND 
                                                    IPK BETWEEN BATAS_MIN AND BATAS_MAX   and iscurrent = 1 and id_jalur is not null 
                                                    union 
                                                     SELECT distinct kd_calon,JNS_POTONGAN ,JUMLAHPOTONGAN 
                                                     FROM REF_POTONGAN INNER JOIN
                                                     MHS_PENDAFTAR ON REF_POTONGAN.id_prodi = MHS_PENDAFTAR.MASUK
                                                     and th_masuk between tahun_masuk_awal and tahun_masuk_akhir
                                                     where kd_calon = @kdCalon  and iscurrent = 1 and id_jalur is null 
                                                     end
                                                 end";


                    string queryTagihan = @"SELECT TOP(1) biaya FROM tr_tarif WHERE id_prodi = @id_prodi and id_tagihan = @id_tagihan and thmasuk = @thn_masuk and iscurrent = '1'";

                    string querySKS = @"SELECT TOP(1) jumlah_pengali FROM tr_tarif WHERE id_prodi = @id_prodi and id_tagihan = @id_tagihan and thmasuk = @thn_masuk and iscurrent = '1'";

                    if (simpan.kode_calon_akhir.Equals("ubah"))
                    {
                        string query = queryUbah;
                        var param = new
                        {
                            kd_calon = simpan.kode_calon_awal,
                            spu = simpan.spu,
                            username = simpan.username,
                            tgl_cetak = simpan.tgl_cetak
                        };

                        var data = conn.Execute(query, param);

                        string query2 = queryAgsSPU;

                        var dataAgsMhs2 = conn.Query<AngsuranMhs2>(query2, new { kd_calon_mhs = simpan.kode_calon_awal }).AsList();
                        int total = simpan.spu;
                        if (dataAgsMhs2.Count() > 0)
                        {
                            for (int i = 0; i < dataAgsMhs2.Count(); i++)
                            {
                                total = total - dataAgsMhs2[i].potongan;
                                string persenHilang = new string(dataAgsMhs2[i].ket_angsuran.Where(char.IsDigit).ToArray());
                                int nilaiPersen;
                                if (int.TryParse(persenHilang, out nilaiPersen))
                                {
                                    total = (total * nilaiPersen) / 100;
                                }
                                string ubahAngsuranTidakAktif = @"UPDATE angsuran_mhs SET
                                                                    jmluang = @total_uang,
                                                                    jumlah = @jumlah
                                                                WHERE kd_calon = @kd_calon_mhs and angsuranke = @angsuranke and ket_angsuran = @ket_angsuran";
                                var paramTidakAktif = new
                                {
                                    total_uang = total,
                                    jumlah = simpan.spu,
                                    kd_calon_mhs = simpan.kode_calon_awal,
                                    angsuranke = dataAgsMhs2[i].angsuranke,
                                    ket_angsuran = dataAgsMhs2[i].ket_angsuran,
                                };
                                var ubahAngsuranMhs = conn.Execute(ubahAngsuranTidakAktif, paramTidakAktif);
                                total = simpan.spu;
                                continue;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        int SPU = 0;
                        string kd_calon_mhs = "";
                        string thnmasuk = "";
                        int id_rumus = 0;
                        int id_tagihan_mhs = 0;

                        string GetKdCalon = @"SELECT kd_calon FROM mhs_pendaftar WHERE masuk = @id_prodi and kd_jalur = @jalur and kd_calon BETWEEN @kd_awal AND @kd_akhir ORDER BY kd_calon ASC";

                        var KdCalon = conn.Query<string>(GetKdCalon, new { id_prodi = simpan.id_prodi, jalur = simpan.kd_jalur, kd_awal = simpan.kode_calon_awal, kd_akhir = simpan.kode_calon_akhir,  }).AsList();

                        if (KdCalon.Count() > 0)
                        {
                            for (int i = 0; i < KdCalon.Count(); i++)
                            {
                                kd_calon_mhs = KdCalon[i];

                                string GetTA = @"SELECT TOP(1) thnakademik FROM mhs_pendaftar WHERE kd_calon = @kd_calon_mhs"; 

                                var TA = conn.QueryFirstOrDefault<string>(GetTA, new { kd_calon_mhs = kd_calon_mhs });

                                if (!String.IsNullOrEmpty(TA))
                                {
                                    string GetRmsAgs = @"SELECT TOP(1) id_rumus FROM mst_angsuran WHERE kd_jalur = @kd_jalur and thnakademik = @ta";

                                    var IdRumus = conn.QueryFirstOrDefault<int>(GetRmsAgs, new { kd_jalur = simpan.kd_jalur, ta = TA });

                                    if (IdRumus > 0)
                                    {
                                        id_rumus = IdRumus;
                                        string GetIdTagihan = @"SELECT TOP(1) a.id_tagihan 
                                                                FROM detail_rumus_angsuran a 
                                                                JOIN ref_tagihan b ON a.id_tagihan = b.id_tagihan 
                                                                WHERE id_rumus = @id_rumus and b.nama_tagihan like '%SPU%'";

                                        var IdTagihan = conn.QueryFirstOrDefault<int>(GetIdTagihan, new { id_rumus = IdRumus });

                                        if (IdTagihan > 0)
                                        {
                                            id_tagihan_mhs = IdTagihan;
                                            string GetSPU = queryTagihan;

                                            string[] parts = TA.Split(new char[] { '/' });
                                            thnmasuk = parts[0];
                                            var data_SPU = conn.QueryFirstOrDefault<int>(GetSPU, new { id_prodi = simpan.id_prodi, id_tagihan = IdTagihan, thn_masuk = int.Parse(thnmasuk) });

                                            SPU = data_SPU;
                                        }
                                    }
                                }



                                // Delete Potongan
                                string deletePotongan = @"DELETE FROM potongan where kd_calon = @kdCalon;";
                                var deleteAllPtg = conn.Execute(deletePotongan, new { kdCalon = kd_calon_mhs });

                                // Insert Potongan SPU
                                string getKonsesiCalon = @"select 
                                                            CASE
                                                                WHEN is_konsesi = 1 THEN 'true'
                                                                ELSE 'false'
                                                            END as is_konsesi 
                                                        from mhs_pendaftar where kd_calon = @kdCalon;";
                                var is_konsesi = conn.QueryFirstOrDefault<string>(getKonsesiCalon, new { kdCalon = kd_calon_mhs });
                                var dataPotongan = conn.Execute(procedurePtgSPU, new { kdCalon = kd_calon_mhs, is_konsesi = is_konsesi });

                                // Insert Detail Rumus Potongan 
                                string queryPtgMhs = @"SELECT nominal, keterangan, id_tagihan
                                                            FROM detail_rumus_potongan 
                                                            WHERE id_rumus = @id_rumus and id_prodi = @prodi and kd_jalur = @jalur";

                                var dataRmsPtg = conn.Query<DetailRmsPtg>(queryPtgMhs, new { id_rumus = id_rumus, prodi = simpan.id_prodi, jalur = simpan.kd_jalur }).AsList();

                                if (dataRmsPtg.Count() > 0)
                                {

                                    string insertRmsPtg = queryInsertRmsPtg;
                                    for (int m = 0; m < dataRmsPtg.Count(); m++)
                                    {
                                        var paramPtg = new
                                        {
                                            kode_calon = kd_calon_mhs,
                                            jns_ptg = dataRmsPtg[m].keterangan,
                                            jmlUang = dataRmsPtg[m].nominal,
                                            tagihan = dataRmsPtg[m].id_tagihan
                                        };

                                        var data2 = conn.Execute(insertRmsPtg, paramPtg);
                                        continue;
                                    }
                                }


                                //Insert SPU
                                string queryCekCalon = @"SELECT * FROM SPU WHERE kd_calon = @kd_calon_mhs";

                                var CalonMhs = conn.Query<dynamic>(queryCekCalon, new { kd_calon_mhs = kd_calon_mhs }).AsList();

                                string query = @"";
                                if (CalonMhs.Count > 0)
                                {
                                    query = query + queryUbah;
                                }
                                else
                                {
                                    query = query + queryTambah;
                                }

                                var param = new
                                {
                                    kd_calon = kd_calon_mhs,
                                    spu = SPU,
                                    username = simpan.username,
                                    tgl_cetak = simpan.tgl_cetak
                                };

                                var data = conn.Execute(query, param);

                                //Insert Angsuran Mhs
                                string GetAllAgs = @"SELECT 
                                                        a.id_tagihan,
                                                        b.nama_tagihan,
                                                        angsuran_ke, 
                                                        CAST(prosentase as int) presentase, 
                                                        tgl_buka, 
                                                        tgl_tutup, 
                                                        is_jaminan 
                                                    FROM detail_rumus_angsuran a
                                                    JOIN ref_tagihan b ON a.id_tagihan = b.id_tagihan
                                                    WHERE id_rumus = @id_rumus
                                                    ORDER BY id_tagihan asc";

                                var dataAgs = conn.Query<DetailRmsAgs>(GetAllAgs, new { id_rumus = id_rumus }).AsList();
                               

                                string queryAngsuranMhs2 = @"SELECT kd_calon, angsuranke, ket_angsuran, potongan FROM angsuran_mhs WHERE kd_calon = @kd_calon_mhs and status = '0'";

                                var dataAgsMhs2 = conn.Query<AngsuranMhs2>(queryAngsuranMhs2, new { kd_calon_mhs = kd_calon_mhs }).AsList();

                                if(dataAgsMhs2.Count() > 0)
                                {
                                    for(int l = 0; l < dataAgsMhs2.Count(); l++)
                                    {
                                        string ubahAngsuranTidakAktif = @"DELETE FROM angsuran_mhs
                                                                        WHERE kd_calon = @kd_calon_mhs and angsuranke = @angsuranke and ket_angsuran = @ket_angsuran";
                                        var paramTidakAktif = new
                                        {
                                            kd_calon_mhs = kd_calon_mhs,
                                            angsuranke = dataAgsMhs2[l].angsuranke,
                                            ket_angsuran = dataAgsMhs2[l].ket_angsuran,
                                        };
                                        var ubahAngsuranMhs = conn.Execute(ubahAngsuranTidakAktif, paramTidakAktif);
                                        continue;
                                    }
                                }

                                string queryAngsuranMhs = @"SELECT * FROM angsuran_mhs WHERE kd_calon = @kd_calon_mhs and status = '1'";

                                var dataAgsMhs = conn.Query<dynamic>(queryAngsuranMhs, new { kd_calon_mhs = kd_calon_mhs }).AsList();

                                // Foreacch Insert Angsuran Mhs
                                if (dataAgsMhs.Count() < 1)
                                {
                                    if (dataAgs.Count > 0)
                                    {
                                        for (int j = 0; j < dataAgs.Count(); j++)
                                        {
                                            int total = 0;
                                            int sks = 0;
                                            string GetBiaya = queryTagihan;

                                            string[] parts = TA.Split(new char[] { '/' });
                                            thnmasuk = parts[0];
                                            if (dataAgs[j].id_tagihan == 10|| dataAgs[j].id_tagihan == 3)
                                            {
                                                string GetSks = querySKS;
                                                var data_Sks = conn.QueryFirstOrDefault<int>(GetSks, new { id_prodi = simpan.id_prodi, id_tagihan = dataAgs[j].id_tagihan, thn_masuk = int.Parse(thnmasuk) });
                                                sks = data_Sks;

                                            }
                                            var data_Biaya = conn.QueryFirstOrDefault<int>(GetBiaya, new { id_prodi = simpan.id_prodi, id_tagihan = dataAgs[j].id_tagihan, thn_masuk = int.Parse(thnmasuk) });

                                            total = data_Biaya;
                                            if (dataAgs[j].id_tagihan == 3 || dataAgs[j].id_tagihan == 10)
                                            {                                          
                                                total = total * sks;
                                            }
                                            string GetAllPtg = @"SELECT TOP(1) nominal 
                                                                FROM detail_rumus_potongan 
                                                                WHERE id_rumus = @id_rumus and id_prodi = @id_prodi and id_tagihan = @id_tagihan";
                                            var data_Ptg = conn.QueryFirstOrDefault<int>(GetAllPtg, new { id_rumus = id_rumus, id_prodi = simpan.id_prodi, id_tagihan = dataAgs[j].id_tagihan });
                                            total = total - data_Ptg;
                                            if (dataAgs[j].presentase == 100)
                                            {
                                                total = total * 1;
                                            }
                                            else
                                            {
                                                total = (total * dataAgs[j].presentase)/ 100;
                                            }
                                            var param2 = new
                                            {
                                                kode_calon = kd_calon_mhs,
                                                angsuran_ke = dataAgs[j].angsuran_ke,
                                                jmlUang = total,
                                                tgl_buka = dataAgs[j].tgl_buka,
                                                batas_waktu = dataAgs[j].tgl_tutup,
                                                ket_ags = dataAgs[j].presentase + "% " + dataAgs[j].nama_tagihan,
                                                is_jaminan = dataAgs[j].is_jaminan,
                                                jumlah = data_Biaya,
                                                ptg = data_Ptg,
                                                sks = sks,
                                            };

                                            var data2 = conn.Execute(queryTambah2, param2);
                                            continue;
                                        }
                                    }
                                }

                                // ubah angsuran_mhs sesuai tabel potongan
                                string queryCekPotongan = @"SELECT jenis, jlh_total FROM potongan WHERE kd_calon = @kd_calon_mhs and id_tagihan is null";

                                var dataSPUPtg = conn.Query<DetailPtg>(queryCekPotongan, new { kd_calon_mhs = kd_calon_mhs }).AsList();

                                if(dataSPUPtg.Count > 0)
                                {
                                    for(int n = 0; n < dataSPUPtg.Count(); n++)
                                    {

                                        string jenisPotongan = dataSPUPtg[n].jenis;

                                        string queryPtgSPU = @"SELECT kd_calon, angsuranke, ket_angsuran, potongan, jumlah
                                                              FROM angsuran_mhs 
                                                              WHERE ket_angsuran LIKE '%' + @jenisPotongan + '%'
                                                              AND kd_calon = @kd_calon_mhs 
                                                              AND status = '0'";

                                        int ptgSPU = dataSPUPtg[n].jlh_total;

                                        var dataAgsMhsSPU = conn.Query<AngsuranMhs2>(queryPtgSPU, new { kd_calon_mhs = kd_calon_mhs, jenisPotongan }).AsList();

                                        for (int p = 0; p < dataAgsMhsSPU.Count; p++)
                                        {
                                            int biayaPotong = (dataAgsMhsSPU[p].potongan > 0) ? (dataAgsMhsSPU[p].potongan + ptgSPU) : ptgSPU;
                                            int totalUang = dataAgsMhsSPU[p].jumlah - ptgSPU;
                                            int jumlah = totalUang;
                                            string persenHilang = new string(dataAgsMhsSPU[p].ket_angsuran.Where(char.IsDigit).ToArray());
                                            int nilaiPersen;
                                            if (int.TryParse(persenHilang, out nilaiPersen))
                                            {
                                                totalUang = (totalUang * nilaiPersen) / 100;
                                            }
                                            string ubahAngsuranSPU = @"UPDATE angsuran_mhs SET
                                                                    jmluang = @total_uang,
                                                                    potongan = @pot_uang
                                                                WHERE kd_calon = @kd_calon_mhs and angsuranke = @angsuranke and ket_angsuran = @ket_angsuran";
                                            var paramSPU = new
                                            {
                                                total_uang = totalUang,
                                                jumlah = jumlah,
                                                kd_calon_mhs = kd_calon_mhs,
                                                angsuranke = dataAgsMhsSPU[p].angsuranke,
                                                ket_angsuran = dataAgsMhsSPU[p].ket_angsuran,
                                                pot_uang = biayaPotong
                                            };
                                            var ubahAngsuranMhs = conn.Execute(ubahAngsuranSPU, paramSPU);
                                            continue;
                                        }
                                    }
                                }

                                // ubah SPU sesuai nilai angsuran
                                //string queryJlhAgsTerbaru = @"SELECT jumlah FROM angsuran_mhs WHERE kd_calon = @kd_calon_mhs and ket_angsuran LIKE '%SPU%'";

                                //var jlhAgsTerbaru = conn.QueryFirstOrDefault<int>(queryJlhAgsTerbaru, new { kd_calon_mhs = kd_calon_mhs });


                                //string ubahJlhSPUTerbaru = @"UPDATE spu SET
                                //                        spu = @jlh
                                //                        WHERE kd_calon = @kd_calon_mhs";
                                //var ubahjlhAgsTerbaru = conn.Execute(ubahJlhSPUTerbaru, new { jlh = jlhAgsTerbaru, kd_calon_mhs = kd_calon_mhs });

                            }

                            return true;
                        }
                    }
                   
                    return false;
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
