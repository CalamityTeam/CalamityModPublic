using System.IO;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;

namespace CalamityMod
{
    public static class Config
    {
        public static bool AdrenalineAndRage = true;
		public static bool ExpertDebuffDurationReduction = true;
		public static bool ExpertChilledWaterRemoval = true;
		public static bool ProficiencyEnabled = true;
		public static bool MiningSpeedBoost = false;
		public static bool DisableExpertEnemySpawnsNearHouse = false;
		public static bool ExpertPillarEnemyKillCountReduction = true;
		public static bool LethalLava = true;

		public static bool BossRushAccessoryCurse = false;
		public static bool BossRushHealthCurse = false;
		public static bool BossRushDashCurse = false;
		public static bool BossRushXerocCurse = false;
		public static bool BossRushImmunityFrameCurse = false;

		public static int RageMeterXPos = 500;
		public static int RageMeterYPos = 30;
		public static int AdrenalineMeterXPos = 650;
		public static int AdrenalineMeterYPos = 30;
		public static int BossHealthPercentageBoost = 0;

		static readonly string ConfigPath = Path.Combine(Main.SavePath, "Mod Configs", "CalamityConfig.json");
        static Preferences Configuration = new Preferences(ConfigPath);

        public static void Load()
        {
            if (!ReadConfig())
            {
                SetDefaults();
                ErrorLogger.Log("The Calamity Mod's config file could not be found. Creating config...");
            }
            SaveConfig();
        }

		public static void SetDefaults()
        {
			AdrenalineAndRage = true;
			ExpertDebuffDurationReduction = true;
			ExpertChilledWaterRemoval = true;
			ProficiencyEnabled = true;
			MiningSpeedBoost = false;
			DisableExpertEnemySpawnsNearHouse = false;
			ExpertPillarEnemyKillCountReduction = true;
			LethalLava = true;

			BossRushAccessoryCurse = false;
			BossRushHealthCurse = false;
			BossRushDashCurse = false;
			BossRushXerocCurse = false;
			BossRushImmunityFrameCurse = false;

			RageMeterXPos = 500;
			RageMeterYPos = 30;
			AdrenalineMeterXPos = 650;
			AdrenalineMeterYPos = 30;
			BossHealthPercentageBoost = 0;
		}

        public static bool ReadConfig()
        {
            if (Configuration.Load())
            {
                Configuration.Get("AdrenalineAndRage", ref AdrenalineAndRage);
				Configuration.Get("ExpertDebuffDurationReduction", ref ExpertDebuffDurationReduction);
				Configuration.Get("ExpertChilledWaterRemoval", ref ExpertChilledWaterRemoval);
				Configuration.Get("ProficiencyEnabled", ref ProficiencyEnabled);
				Configuration.Get("MiningSpeedBoost", ref MiningSpeedBoost);
				Configuration.Get("DisableExpertEnemySpawnsNearHouse", ref DisableExpertEnemySpawnsNearHouse);
				Configuration.Get("ExpertPillarEnemyKillCountReduction", ref ExpertPillarEnemyKillCountReduction);
				Configuration.Get("LethalLava", ref LethalLava);

				Configuration.Get("BossRushAccessoryCurse", ref BossRushAccessoryCurse);
				Configuration.Get("BossRushHealthCurse", ref BossRushHealthCurse);
				Configuration.Get("BossRushDashCurse", ref BossRushDashCurse);
				Configuration.Get("BossRushXerocCurse", ref BossRushXerocCurse);
				Configuration.Get("BossRushImmunityFrameCurse", ref BossRushImmunityFrameCurse);

				Configuration.Get("RageMeterXPos", ref RageMeterXPos);
				Configuration.Get("RageMeterYPos", ref RageMeterYPos);
				Configuration.Get("AdrenalineMeterXPos", ref AdrenalineMeterXPos);
				Configuration.Get("AdrenalineMeterYPos", ref AdrenalineMeterYPos);
				Configuration.Get("BossHealthPercentageBoost", ref BossHealthPercentageBoost);
				return true;
            }
            return false;
        }

        public static void SaveConfig()
        {
            Configuration.Clear();
            Configuration.Put("AdrenalineAndRage", AdrenalineAndRage);
			Configuration.Put("ExpertDebuffDurationReduction", ExpertDebuffDurationReduction);
			Configuration.Put("ExpertChilledWaterRemoval", ExpertChilledWaterRemoval);
			Configuration.Put("ProficiencyEnabled", ProficiencyEnabled);
			Configuration.Put("MiningSpeedBoost", MiningSpeedBoost);
			Configuration.Put("DisableExpertEnemySpawnsNearHouse", DisableExpertEnemySpawnsNearHouse);
			Configuration.Put("ExpertPillarEnemyKillCountReduction", ExpertPillarEnemyKillCountReduction);
			Configuration.Put("LethalLava", LethalLava);

			Configuration.Put("BossRushAccessoryCurse", BossRushAccessoryCurse);
			Configuration.Put("BossRushHealthCurse", BossRushHealthCurse);
			Configuration.Put("BossRushDashCurse", BossRushDashCurse);
			Configuration.Put("BossRushXerocCurse", BossRushXerocCurse);
			Configuration.Put("BossRushImmunityFrameCurse", BossRushImmunityFrameCurse);

			Configuration.Put("RageMeterXPos", RageMeterXPos);
			if (RageMeterXPos < 0)
				RageMeterXPos = 0;
			Configuration.Put("RageMeterYPos", RageMeterYPos);
			if (RageMeterYPos < 0)
				RageMeterYPos = 0;
			Configuration.Put("AdrenalineMeterXPos", AdrenalineMeterXPos);
			if (AdrenalineMeterXPos < 0)
				AdrenalineMeterXPos = 0;
			Configuration.Put("AdrenalineMeterYPos", AdrenalineMeterYPos);
			if (AdrenalineMeterYPos < 0)
				AdrenalineMeterYPos = 0;
			Configuration.Put("BossHealthPercentageBoost", BossHealthPercentageBoost);
			if (BossHealthPercentageBoost < 0)
				BossHealthPercentageBoost = 0;
			if (BossHealthPercentageBoost > 900)
				BossHealthPercentageBoost = 900;
			Configuration.Save();
        }

        class MultiplayerSyncWorld : ModWorld
        {
            public override void NetSend(BinaryWriter writer)
            {
                var data = new BitsByte(AdrenalineAndRage, ExpertDebuffDurationReduction, ExpertChilledWaterRemoval,
					ProficiencyEnabled, MiningSpeedBoost, DisableExpertEnemySpawnsNearHouse, ExpertPillarEnemyKillCountReduction,
					LethalLava);

				var data2 = new BitsByte(BossRushAccessoryCurse, BossRushHealthCurse, BossRushDashCurse,
					BossRushXerocCurse, BossRushImmunityFrameCurse);

				writer.Write(data);
				writer.Write(data2);

				writer.Write(RageMeterXPos);
				writer.Write(RageMeterYPos);
				writer.Write(AdrenalineMeterXPos);
				writer.Write(AdrenalineMeterYPos);
				writer.Write(BossHealthPercentageBoost);
			}

            public override void NetReceive(BinaryReader reader)
            {
                SaveConfig();

                var data = (BitsByte)reader.ReadByte();
				AdrenalineAndRage = data[0];
				ExpertDebuffDurationReduction = data[1];
				ExpertChilledWaterRemoval = data[2];
				ProficiencyEnabled = data[3];
				MiningSpeedBoost = data[4];
				DisableExpertEnemySpawnsNearHouse = data[5];
				ExpertPillarEnemyKillCountReduction = data[6];
				LethalLava = data[7];

				var data2 = (BitsByte)reader.ReadByte();
				BossRushAccessoryCurse = data2[0];
				BossRushHealthCurse = data2[1];
				BossRushDashCurse = data2[2];
				BossRushXerocCurse = data2[3];
				BossRushImmunityFrameCurse = data2[4];

				RageMeterXPos = reader.ReadInt32();
				RageMeterYPos = reader.ReadInt32();
				AdrenalineMeterXPos = reader.ReadInt32();
				AdrenalineMeterYPos = reader.ReadInt32();
				BossHealthPercentageBoost = reader.ReadInt32();
			}
        }

        class MultiplayerSyncPlayer : ModPlayer
        {
            public override void PlayerDisconnect(Player player)
            {
                ReadConfig();
            }
        }
    }
}
