using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class TorrentialTear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Torrential Tear");
            Tooltip.SetDefault("Summons the rain\n" +
                "Rain will start some time after this item is used\n" +
                "If used when raining the rain will stop some time after this item is used");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 5;
            item.useAnimation = 20;
            item.useTime = 20;
            item.useStyle = 4;
            item.UseSound = SoundID.Item66;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.slimeRain;
        }

        public override bool UseItem(Player player)
        {
            if (!Main.raining)
            {
                int num = 86400;
                int num2 = num / 24;
                Main.rainTime = Main.rand.Next(num2 * 8, num);
                if (Main.rand.NextBool(3))
                {
                    Main.rainTime += Main.rand.Next(0, num2);
                }
                if (Main.rand.NextBool(4))
                {
                    Main.rainTime += Main.rand.Next(0, num2 * 2);
                }
                if (Main.rand.NextBool(5))
                {
                    Main.rainTime += Main.rand.Next(0, num2 * 2);
                }
                if (Main.rand.NextBool(6))
                {
                    Main.rainTime += Main.rand.Next(0, num2 * 3);
                }
                if (Main.rand.NextBool(7))
                {
                    Main.rainTime += Main.rand.Next(0, num2 * 4);
                }
                if (Main.rand.NextBool(8))
                {
                    Main.rainTime += Main.rand.Next(0, num2 * 5);
                }
                float num3 = 1f;
                if (Main.rand.NextBool(2))
                {
                    num3 += 0.05f;
                }
                if (Main.rand.NextBool(3))
                {
                    num3 += 0.1f;
                }
                if (Main.rand.NextBool(4))
                {
                    num3 += 0.15f;
                }
                if (Main.rand.NextBool(5))
                {
                    num3 += 0.2f;
                }
                Main.rainTime = (int)((float)Main.rainTime * num3);
                Main.raining = true;
            }
            else
                Main.raining = false;

            CalamityMod.UpdateServerBoolean();
            return true;
        }
    }
}
