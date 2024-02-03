using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using DpeZak.Entities.DPE;

namespace DpeZak.Connection.DPE
{
    public partial class DpeContext : DbContext
    {
        public virtual DbSet<ASWS> ASWS_Tbl { get; set; }
        public virtual DbSet<DKAT> DKAT_Tbl { get; set; }
        public virtual DbSet<FLTR> FLTR_Tbl { get; set; }
        public virtual DbSet<R_Grup> R_Grup_Tbl { get; set; }
        public virtual DbSet<R_INIT> R_INIT_Tbl { get; set; }
        public virtual DbSet<R_Usgr> R_Usgr_Tbl { get; set; }
        public virtual DbSet<R_Usrs> R_Usrs_Tbl { get; set; }
        public virtual DbSet<VFUE> VFUE_Tbl { get; set; }
        public virtual DbSet<VORF> VORF_Tbl { get; set; }
        public virtual DbSet<V_LADEZETTEL> V_LADEZETTEL_Tbl { get; set; }

        public DpeContext(DbContextOptions<DpeContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ASWS>(entity =>
            {
                entity.HasKey(e => new { e.ASW_NAME, e.ITEM_POS });

                entity.Property(e => e.ASW_NAME)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.BEMERKUNG)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ERFASST_AM).HasColumnType("datetime");

                entity.Property(e => e.ERFASST_VON)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.GEAENDERT_AM).HasColumnType("datetime");

                entity.Property(e => e.GEAENDERT_VON)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ITEM_DISPLAY)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ITEM_VALUE)
                    .HasMaxLength(80)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DKAT>(entity =>
            {
                entity.HasKey(e => e.DKAT_NR);

                entity.Property(e => e.DKAT_NR)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.BEMERKUNG)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.DKAT_BEZ)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.DKAT_DTMBIS).HasColumnType("datetime");

                entity.Property(e => e.DKAT_DTMVON).HasColumnType("datetime");

                entity.Property(e => e.ERFASST_AM).HasColumnType("datetime");

                entity.Property(e => e.ERFASST_VON)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.GEAENDERT_AM).HasColumnType("datetime");

                entity.Property(e => e.GEAENDERT_VON)
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<FLTR>(entity =>
            {
                entity.HasKey(e => e.FLTR_ID);

                entity.HasIndex(e => new { e.FORM, e.NAME }, "UK_FLTR")
                    .IsUnique();

                entity.Property(e => e.BEMERKUNG)
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.COLUMNLIST).HasColumnType("text");

                entity.Property(e => e.ERFASST_AM).HasColumnType("datetime");

                entity.Property(e => e.ERFASST_DATENBANK)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ERFASST_VON)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.FLTRLIST)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.FORM)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.GEAENDERT_AM).HasColumnType("datetime");

                entity.Property(e => e.GEAENDERT_DATENBANK)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.GEAENDERT_VON)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ISPUBLIC)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.KEYFIELDS)
                    .HasMaxLength(80)
                    .IsUnicode(false);

                entity.Property(e => e.NAME)
                    .IsRequired()
                    .HasMaxLength(80)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<R_Grup>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => e.GRUP_ID, "R_Grup0")
                    .IsUnique()
                    .HasFillFactor(90);

                entity.Property(e => e.BEMERKUNG).HasColumnType("text");

                entity.Property(e => e.ERFASST_AM).HasColumnType("datetime");

                entity.Property(e => e.ERFASST_VON)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.GEAENDERT_AM).HasColumnType("datetime");

                entity.Property(e => e.GEAENDERT_VON)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.GRUP_INFORMATION)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.GRUP_NAME)
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<R_INIT>(entity =>
            {
                entity.HasKey(e => e.INIT_ID)
                    .HasName("PK_INIT");

                entity.HasIndex(e => new { e.ANWENDUNG, e.TYP, e.NAME, e.SECTION, e.PARAM }, "UK_INIT")
                    .IsUnique();

                entity.Property(e => e.INIT_ID).HasDefaultValueSql("([dbo].[NEW_ID_READONLY]('INIT_ID'))");

                entity.Property(e => e.ANWENDUNG)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.BEMERKUNG)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ERFASST_AM).HasColumnType("datetime");

                entity.Property(e => e.ERFASST_DATENBANK)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ERFASST_VON)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.GEAENDERT_AM).HasColumnType("datetime");

                entity.Property(e => e.GEAENDERT_DATENBANK)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.GEAENDERT_VON)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.NAME)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.PARAM)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.SECTION)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.TYP)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.WERT)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<R_Usgr>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => e.USGR_GRUP_ID, "I_USGR_GRUP_ID")
                    .HasFillFactor(90);

                entity.HasIndex(e => e.USGR_USER_ID, "I_USGR_USER_ID")
                    .HasFillFactor(90);

                entity.HasIndex(e => e.USGR_ID, "R_Usgr0")
                    .IsUnique()
                    .HasFillFactor(90);

                entity.HasIndex(e => new { e.USGR_USER_ID, e.USGR_GRUP_ID }, "UK_USGR")
                    .IsUnique();

                entity.Property(e => e.BEMERKUNG).HasColumnType("text");

                entity.Property(e => e.ERFASST_AM).HasColumnType("datetime");

                entity.Property(e => e.ERFASST_VON)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.GEAENDERT_AM).HasColumnType("datetime");

                entity.Property(e => e.GEAENDERT_VON)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.SO_GRUP_NAME)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.SO_USER_KENNUNG)
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<R_Usrs>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => e.USER_ID, "R_Usrs0")
                    .IsUnique()
                    .HasFillFactor(90);

                entity.Property(e => e.BEMERKUNG).HasColumnType("text");

                entity.Property(e => e.ERFASST_AM).HasColumnType("datetime");

                entity.Property(e => e.ERFASST_VON)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.FLAG_PASSWORT)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.GEAENDERT_AM).HasColumnType("datetime");

                entity.Property(e => e.GEAENDERT_VON)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.USER_INFORMATION)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.USER_KENNUNG)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.USER_LANGNAME)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.USER_LEVEL)
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.USER_PASSWORT)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.USER_TELEFON_NR)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VFUE>(entity =>
            {
                entity.HasKey(e => e.VFUE_NR);

                entity.HasIndex(e => e.VFUE_BEZ, "UK_VFUE")
                    .IsUnique();

                entity.Property(e => e.VFUE_NR).ValueGeneratedNever();

                entity.Property(e => e.BEMERKUNG).HasColumnType("text");

                entity.Property(e => e.ERFASST_AM).HasColumnType("datetime");

                entity.Property(e => e.ERFASST_VON)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.GEAENDERT_AM).HasColumnType("datetime");

                entity.Property(e => e.GEAENDERT_VON)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.GUELTIG_AB).HasColumnType("datetime");

                entity.Property(e => e.GUELTIG_BIS).HasColumnType("datetime");

                entity.Property(e => e.VFUE_BEZ)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VORF>(entity =>
            {
                entity.HasKey(e => e.vorf_nr)
                    .HasName("PK_NEUVORF")
                    .IsClustered(false);

                entity.HasIndex(e => e.FREMD_NR, "GMLVORF_FREMD_I");

                entity.HasIndex(e => new { e.ents_nr, e.edt }, "I_VORF_VEEN");

                entity.HasIndex(e => new { e.srte_nr, e.bezi_nr, e.lort_nr, e.KOS_SATZ, e.edt }, "UK_ZSOD");

                entity.HasIndex(e => e.AANL_NR, "VORF_AANL_I")
                    .HasFillFactor(90);

                entity.HasIndex(e => new { e.ABHI_STUFE, e.edt }, "VORF_ABHI_I");

                entity.HasIndex(e => e.ER_ANKAUF, "VORF_ANKAUF_I");

                entity.HasIndex(e => e.brwanr, "VORF_BRWANR");

                entity.HasIndex(e => new { e.CHME_NR, e.edt }, "VORF_CHME_I");

                entity.HasIndex(e => new { e.EANV_ID, e.EANV_STA }, "VORF_EANV");

                entity.HasIndex(e => new { e.EANV_REGISTER, e.EANV_SIGNIERT, e.sta, e.EANV_STA }, "VORF_EANVREG_I");

                entity.HasIndex(e => e.EANV_STA, "VORF_EANV_STA");

                entity.HasIndex(e => e.ER_ENTSORGUNG, "VORF_ENTSORGUNG_I");

                entity.HasIndex(e => new { e.expsta, e.edt }, "VORF_EXPSTA_EDT")
                    .HasFillFactor(90);

                entity.HasIndex(e => new { e.zah_nr, e.srte_nr, e.edt, e.lityp, e.vorf_nr }, "VORF_EXP_SUN_I")
                    .HasFillFactor(90);

                entity.HasIndex(e => e.FREMD_NR, "VORF_FREMD_I");

                entity.HasIndex(e => e.ER_GEBSONST, "VORF_GEBSONST_I");

                entity.HasIndex(e => e.ER_GEBUEHR, "VORF_GEBUEHR_I");

                entity.HasIndex(e => e.ER_GUTSCHRIFT, "VORF_GUTSCHRIFT_I");

                entity.HasIndex(e => e.HAUPT_VORF_NR, "VORF_HAUPT_I");

                entity.HasIndex(e => new { e.KASS_NR, e.zatyp, e.sta, e.erfasst_von }, "VORF_KASS_I");

                entity.HasIndex(e => new { e.LCHA_NR, e.sta }, "VORF_LCHA_I");

                entity.HasIndex(e => new { e.sta, e.erz_nr, e.MEKO_NR, e.edt }, "VORF_MEKO_I");

                entity.HasIndex(e => e.ER_MIETE, "VORF_MIETE_I");

                entity.HasIndex(e => e.PRVE_NR, "VORF_PRVE_I");

                entity.HasIndex(e => e.RECH_NR2, "VORF_RECH2_I");

                entity.HasIndex(e => e.rech_nr, "VORF_RECH_I")
                    .HasFillFactor(90);

                entity.HasIndex(e => new { e.sta, e.ents_nr, e.ents_lfnr }, "VORF_RESTMEN_I")
                    .HasFillFactor(90);

                entity.HasIndex(e => e.bar_btr, "VORF_REST_I");

                entity.HasIndex(e => e.AANL_NR, "VORF_SOA_AANL_I")
                    .HasFillFactor(90);

                entity.HasIndex(e => new { e.depo_nr, e.SUM_FLAG }, "VORF_SOA_SUM_FLAG_I")
                    .HasFillFactor(90);

                entity.HasIndex(e => new { e.depo_nr, e.srte_nr, e.SUM_FLAG, e.lityp, e.sta }, "VORF_SOA_SUM_I")
                    .HasFillFactor(90);

                entity.HasIndex(e => e.srte_nr, "VORF_SRTE_I");

                entity.HasIndex(e => new { e.sta, e.adt, e.ATm }, "VORF_STA_ADT_ATM_I");

                entity.HasIndex(e => new { e.sta, e.edt, e.ETm }, "VORF_STA_EDT_ETM_I")
                    .HasFillFactor(90);

                entity.HasIndex(e => new { e.sta, e.fahr_knz }, "VORF_STA_FAHR_KNZ_I")
                    .HasFillFactor(90);

                entity.HasIndex(e => new { e.sta, e.zatyp }, "VORF_STA_ZATYP_I")
                    .HasFillFactor(90);

                entity.HasIndex(e => e.ER_TRANSPORT, "VORF_TRANSPORT_I");

                entity.HasIndex(e => e.UEB_NR, "VORF_UEB_NR");

                entity.HasIndex(e => e.ER_UMLAGE, "VORF_UMLAGE_I");

                entity.HasIndex(e => e.VERT_NR, "VORF_VERT_I");

                entity.HasIndex(e => e.VJOB_ID, "VORF_VJOB_I");

                entity.HasIndex(e => new { e.zah_nr, e.srte_nr }, "VORF_ZAH_SRTE_I")
                    .HasFillFactor(90);

                entity.Property(e => e.vorf_nr).ValueGeneratedNever();

                entity.Property(e => e.ALT_MA_SRTE_NR)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ALT_SRTE_BEZ)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ALT_SRTE_NR)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ANL_BEFNR)
                    .HasMaxLength(9)
                    .IsUnicode(false);

                entity.Property(e => e.ANNE_NR)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.ATm)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.AUVO_ID)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.BUNDESLAND)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.CHARGENUMMER)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.DKAT_NR)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.EANV_ID)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.EANV_KNZ)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('N')");

                entity.Property(e => e.EANV_REGISTER)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.EANV_SIGNIERT)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('N')");

                entity.Property(e => e.EANV_STA)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.EFF_BRPRS).HasComputedColumnSql("([dbo].[GET_VORF_EFF_BRPRS]((isnull([neprs],(0))+isnull([NEPRS2],(0)))-isnull([VG_BETRAG],(0)),[MW_KNZ],[MW_PRZ]))", false);

                entity.Property(e => e.EFF_NEPRS).HasComputedColumnSql("((isnull([neprs],(0))+isnull([NEPRS2],(0)))-isnull([VG_BETRAG],(0)))", false);

                entity.Property(e => e.EINWAEGER)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ENTS_LITYP)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.ENTS_TO).HasComputedColumnSql("([dbo].[GET_VORF_ENTS_TO]([MEN], [ME], [LITYP], [ENTS_LITYP]))", false);

                entity.Property(e => e.ENT_ENTNR)
                    .HasMaxLength(9)
                    .IsUnicode(false);

                entity.Property(e => e.ERZD_HNR)
                    .HasMaxLength(7)
                    .IsUnicode(false);

                entity.Property(e => e.ERZD_LND)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.ERZD_NA1)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ERZD_NA2)
                    .HasMaxLength(35)
                    .IsUnicode(false);

                entity.Property(e => e.ERZD_ORT)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.ERZD_PLZ)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.ERZD_STR)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ERZ_ERZNR)
                    .HasMaxLength(9)
                    .IsUnicode(false);

                entity.Property(e => e.ER_ANKAUF)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ER_BEMERKUNG)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ER_ENTSORGUNG)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ER_GEBSONST)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ER_GEBUEHR)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ER_GUTSCHRIFT)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ER_MIETE)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ER_TRANSPORT)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ER_UMLAGE)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ETm)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.EWC_GEFAHR)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.EXPSTA2)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.FKSTA2)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.FREMD_NR)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.FTXT_KUERZEL)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.HANDEINGABE)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.HKBE_NR)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.HOFL_KTRL)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.HOFL_OK)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.IKAR_NR)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.KOLO_NR)
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.KUGR_NR)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.LABORANALYSE)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.LABOREMAIL)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.LAGER_NR)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasComputedColumnSql("([dbo].[GET_VORF_LAGER_NR]([LITYP], [LORT_NR], [BEZI_NR]))", false);

                entity.Property(e => e.LCHA_NR)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.LZET)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.MATKENN)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.ME2)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.MEN_KG).HasComputedColumnSql("([dbo].[GET_VORF_MEN_KG]([men],[me]))", false);

                entity.Property(e => e.MEN_M3).HasComputedColumnSql("([dbo].[GET_VORF_ME]('3',[men],[me]))", false);

                entity.Property(e => e.MEN_STK).HasComputedColumnSql("([dbo].[GET_VORF_ME]('S',[men],[me]))", false);

                entity.Property(e => e.MEN_TAG).HasComputedColumnSql("([dbo].[GET_VORF_ME]('d',[men],[me]))", false);

                entity.Property(e => e.MEN_TO).HasComputedColumnSql("([dbo].[GET_VORF_MEN_TO]([men],[me]))", false);

                entity.Property(e => e.NACHWEISTYP)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PAUBER)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PE2)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PRCO_NR2)
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.PROBENAHME_OK)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PROBE_NR)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.PROJEKTNR)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.REST_ADR_FAHRER)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.REST_ADR_FIRMA)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.REST_AUSWEIS)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SATZART)
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.SENT_BEF)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.SENT_BEH)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.SENT_ENT)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.SENT_ERZ)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.SGD_ABD_EIG)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.SGD_JBERICHT)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('N')");

                entity.Property(e => e.SGD_JBERICHT_K)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.SRTE_BEZ2)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.SRTE_NR2)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.SUM_FLAG)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.TOURENART)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.UEB_NR)
                    .HasMaxLength(14)
                    .IsUnicode(false);

                entity.Property(e => e.VERT_NR)
                    .HasMaxLength(14)
                    .IsUnicode(false);

                entity.Property(e => e.WE)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.ZAMITTEL)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.ZEITKAT_BEZ)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasComputedColumnSql("([dbo].[GET_ZEITKAT_BEZ]([dbo].[GET_ZEITKAT_NR]([EDT],[ETM],[ADT],[ATM])))", false);

                entity.Property(e => e.ZEITKAT_NR).HasComputedColumnSql("([dbo].[GET_ZEITKAT_NR]([EDT],[ETM],[ADT],[ATM]))", false);

                entity.Property(e => e.ZUSATZKENNUNG)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.abgsta)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.adt).HasColumnType("datetime");

                entity.Property(e => e.anh_knz)
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.anh_vanr)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.anl_hnr)
                    .HasMaxLength(7)
                    .IsUnicode(false);

                entity.Property(e => e.anl_lnd)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.anl_na1)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.anl_na2)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.anl_ort)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.anl_plz)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.anl_str)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.arcsta)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.bemerkung)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.bezi_nr)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.bgl_nr)
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.brspnr)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.depo_nr)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.edt).HasColumnType("datetime");

                entity.Property(e => e.ents_nr)
                    .HasMaxLength(13)
                    .IsUnicode(false);

                entity.Property(e => e.ents_typ)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.erfasst_am).HasColumnType("datetime");

                entity.Property(e => e.erfasst_von)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.erz_hnr)
                    .HasMaxLength(7)
                    .IsUnicode(false);

                entity.Property(e => e.erz_lnd)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.erz_na1)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.erz_na2)
                    .HasMaxLength(35)
                    .IsUnicode(false);

                entity.Property(e => e.erz_ort)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.erz_plz)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.erz_str)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ewc_code)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.expsta)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.fahr_knz)
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.fahr_vanr)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.fkgru)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.fksor)
                    .HasMaxLength(22)
                    .IsUnicode(false);

                entity.Property(e => e.fksta)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.ge)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.geaendert_am).HasColumnType("datetime");

                entity.Property(e => e.geaendert_von)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.idknz)
                    .HasMaxLength(9)
                    .IsUnicode(false);

                entity.Property(e => e.kata_nr)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.laga_code)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.lityp)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.lort_nr)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.ma_srte_nr)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ma_txt)
                    .HasMaxLength(120)
                    .IsUnicode(false);

                entity.Property(e => e.mand_nr)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.masta)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.me)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.mod)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.mw_knz)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.mw_nr)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.pe)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.prco_nr)
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.sam_knz)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.sogr_nr)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.srte_bez)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.srte_nr)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.sta)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.taspnr)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.vbez_nr)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.zahler)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.zatyp)
                    .HasMaxLength(1)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<V_LADEZETTEL>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_LADEZETTEL");

                entity.Property(e => e.ANL_ADRE)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ANL_BEFNR)
                    .HasMaxLength(9)
                    .IsUnicode(false);

                entity.Property(e => e.ANNE_NR)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.ATm)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.AUVO_ID)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.BUNDESLAND)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.CHARGENUMMER)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.DKAT_NR)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.EANV_ID)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.EANV_KNZ)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.EANV_REGISTER)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.EANV_SIGNIERT)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.EANV_STA)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ENCH_NAME)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.ENTS_ENTS_TYP)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.ENTS_LITYP)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.ENT_ENTNR)
                    .HasMaxLength(9)
                    .IsUnicode(false);

                entity.Property(e => e.ERZD_HNR)
                    .HasMaxLength(7)
                    .IsUnicode(false);

                entity.Property(e => e.ERZD_LND)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.ERZD_NA1)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ERZD_NA2)
                    .HasMaxLength(35)
                    .IsUnicode(false);

                entity.Property(e => e.ERZD_ORT)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.ERZD_PLZ)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.ERZD_STR)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ERZ_ADRE)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ERZ_ERZNR)
                    .HasMaxLength(9)
                    .IsUnicode(false);

                entity.Property(e => e.ER_ANKAUF)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ER_BEMERKUNG)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ER_ENTSORGUNG)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ER_GEBSONST)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ER_GEBUEHR)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ER_GUTSCHRIFT)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ER_MIETE)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ER_TRANSPORT)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ER_UMLAGE)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ETm)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.EWC_BEZ)
                    .HasMaxLength(254)
                    .IsUnicode(false);

                entity.Property(e => e.EWC_GEFAHR)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.FREMD_NR)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.FTXT_KUERZEL)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.HANDEINGABE)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.HOFL_KTRL)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.HOFL_OK)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.KATASTER_BEZ)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.KATASTER_NR)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.KUGR_NR)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.LABORANALYSE)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.LABOREMAIL)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.LAGER_BEMERKUNG).HasColumnType("text");

                entity.Property(e => e.LAGER_BEZ)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.LAGER_NR)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.LCHA_NR)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.LZET)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.MATKENN)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.MKEN_BEZ)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PAUBER)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PROBENAHME_OK)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PROBE_NR)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.PROJEKTNR)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.REST_ADR_FAHRER)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.REST_ADR_FIRMA)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.REST_AUSWEIS)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SATZART)
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.SENT_BEF)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.SENT_BEH)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.SENT_ENT)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.SENT_ERZ)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.SGD_ABD_EIG)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.SGD_JBERICHT)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.SGD_JBERICHT_K)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.SUM_FLAG)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.UEB_NR)
                    .HasMaxLength(14)
                    .IsUnicode(false);

                entity.Property(e => e.VERFUELLABSCHNITT)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.VERT_NR)
                    .HasMaxLength(14)
                    .IsUnicode(false);

                entity.Property(e => e.WE)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.ZAMITTEL)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.ZEITKAT_BEZ)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.abgsta)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.adt).HasColumnType("datetime");

                entity.Property(e => e.anh_knz)
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.anh_vanr)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.anl_hnr)
                    .HasMaxLength(7)
                    .IsUnicode(false);

                entity.Property(e => e.anl_lnd)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.anl_na1)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.anl_na2)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.anl_ort)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.anl_plz)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.anl_str)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.arcsta)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.bemerkung)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.bezi_nr)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.bgl_nr)
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.brspnr)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.depo_nr)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.edt).HasColumnType("datetime");

                entity.Property(e => e.ents_nr)
                    .HasMaxLength(13)
                    .IsUnicode(false);

                entity.Property(e => e.ents_typ)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.erfasst_am).HasColumnType("datetime");

                entity.Property(e => e.erfasst_von)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.erz_hnr)
                    .HasMaxLength(7)
                    .IsUnicode(false);

                entity.Property(e => e.erz_lnd)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.erz_na1)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.erz_na2)
                    .HasMaxLength(35)
                    .IsUnicode(false);

                entity.Property(e => e.erz_ort)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.erz_plz)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.erz_str)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ewc_code)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.expsta)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.fahr_knz)
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.fahr_vanr)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.fkgru)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.fksor)
                    .HasMaxLength(22)
                    .IsUnicode(false);

                entity.Property(e => e.fksta)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.ge)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.geaendert_am).HasColumnType("datetime");

                entity.Property(e => e.geaendert_von)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.idknz)
                    .HasMaxLength(9)
                    .IsUnicode(false);

                entity.Property(e => e.kata_nr)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.laga_code)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.lityp)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.lort_nr)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.ma_srte_nr)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ma_txt)
                    .HasMaxLength(120)
                    .IsUnicode(false);

                entity.Property(e => e.mand_nr)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.masta)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.me)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.mod)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.mw_knz)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.mw_nr)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.pe)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.prco_nr)
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.sam_knz)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.sogr_nr)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.srte_bez)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.srte_nr)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.sta)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.taspnr)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.vbez_nr)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.zahler)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.zatyp)
                    .HasMaxLength(1)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
