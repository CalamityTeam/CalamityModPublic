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
		public static bool DisableExpertEnemySpawnsNearHouse = true;
		public static bool ExpertPillarEnemyKillCountReduction = true;

		public static int RageMeterXPos = 500;
		public static int RageMeterYPos = 30;
		public static int AdrenalineMeterXPos = 650;
		public static int AdrenalineMeterYPos = 30;

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
			DisableExpertEnemySpawnsNearHouse = true;
			ExpertPillarEnemyKillCountReduction = true;

			RageMeterXPos = 500;
			RageMeterYPos = 30;
			AdrenalineMeterXPos = 650;
			AdrenalineMeterYPos = 30;
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

				Configuration.Get("RageMeterXPos", ref RageMeterXPos);
				Configuration.Get("RageMeterYPos", ref RageMeterYPos);
				Configuration.Get("AdrenalineMeterXPos", ref AdrenalineMeterXPos);
				Configuration.Get("AdrenalineMeterYPos", ref AdrenalineMeterYPos);
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
			Configuration.Save();
        }

        class MultiplayerSyncWorld : ModWorld
        {
            public override void NetSend(BinaryWriter writer)
            {
                var data = new BitsByte(AdrenalineAndRage, ExpertDebuffDurationReduction, ExpertChilledWaterRemoval,
					ProficiencyEnabled, MiningSpeedBoost, DisableExpertEnemySpawnsNearHouse, ExpertPillarEnemyKillCountReduction);

				writer.Write(data);

				writer.Write(RageMeterXPos);
				writer.Write(RageMeterYPos);
				writer.Write(AdrenalineMeterXPos);
				writer.Write(AdrenalineMeterYPos);
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

				RageMeterXPos = reader.ReadInt32();
				RageMeterYPos = reader.ReadInt32();
				AdrenalineMeterXPos = reader.ReadInt32();
				AdrenalineMeterYPos = reader.ReadInt32();
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
