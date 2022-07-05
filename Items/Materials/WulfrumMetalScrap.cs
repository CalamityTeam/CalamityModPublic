using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Wulfrum;
using Terraria.Audio;
using CalamityMod.Cooldowns;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Items.Materials
{
    [LegacyName("WulfrumShard")]
    public class WulfrumMetalScrap : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
            DisplayName.SetDefault("Wulfrum Metal Scrap");
        }

        public override void SetDefaults()
        {
            Item.width = 13;
            Item.height = 10;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(copper: 80);
            Item.rare = ItemRarityID.Blue;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_Loot)
            {
                if (Main.rand.NextBool())
                    return;

                bool closePlayer = false;

                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if ((Main.player[i].Center - Item.Center).Length() < 1200 && Main.player[i].GetModPlayer<WulfrumBatteryPlayer>().battery)
                    {
                        closePlayer = true;
                        break;
                    }
                }

                if (closePlayer)
                {
                    Item.stack++;
                    SoundEngine.PlaySound(WulfrumBattery.ExtraDropSound, Item.Center);

                    int numDust = Main.rand.Next(3, 7);
                    for (int i = 0; i < numDust; i++)
                    {
                        Dust.NewDustDirect(Item.position, Item.width, Item.height, Main.rand.NextBool() ? 246 : 247, 0, -3f, Scale: Main.rand.NextFloat(0.9f, 1f));
                    }
                }
            }
        }
    }
}
