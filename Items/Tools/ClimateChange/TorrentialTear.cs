using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools.ClimateChange
{
    public class TorrentialTear : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Tools";
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Pink;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item66;
            Item.consumable = false;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.EventItem;
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.slimeRain;
        }

        public override bool? UseItem(Player player)
        {
            // Only SinglePlayer and Server need to sync those parameters
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return true;

            if (!Main.raining)
            {
                CalamityUtils.StartRain(torrentialTear: true, worldSync: true);
            }
            else
            {
                CalamityUtils.StopRain(clearWeather: false, worldSync: true);
            }

            return true;
        }

        public static void AdjustRainSeverity(bool maxSeverity)
        {
            if (maxSeverity)
            {
                Main.cloudBGActive = 1f;
                Main.numCloudsTemp = Main.maxClouds;
                Main.numClouds = Main.numCloudsTemp;
                Main.windSpeedCurrent = Main.rand.Next(50, 75) * 0.01f;
                Main.windSpeedTarget = Main.windSpeedCurrent;
                Main.weatherCounter = Main.rand.Next(3600, 18000);
                Main.maxRaining = 0.89f;
            }
            else
            {
                if (Main.cloudBGActive >= 1f || (double)Main.numClouds > 150.0)
                {
                    if (Main.rand.NextBool(3))
                        Main.maxRaining = (float)Main.rand.Next(20, 90) * 0.01f;
                    else
                        Main.maxRaining = (float)Main.rand.Next(40, 90) * 0.01f;
                }
                else if ((double)Main.numClouds > 100.0)
                {
                    if (Main.rand.NextBool(3))
                        Main.maxRaining = (float)Main.rand.Next(10, 70) * 0.01f;
                    else
                        Main.maxRaining = (float)Main.rand.Next(20, 60) * 0.01f;
                }
                else
                {
                    if (Main.rand.NextBool(3))
                        Main.maxRaining = (float)Main.rand.Next(5, 40) * 0.01f;
                    else
                        Main.maxRaining = (float)Main.rand.Next(5, 30) * 0.01f;
                }
            }
        }
    }
}
