using CalamityMod.NPCs;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class GoldBurdenBreaker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Burden Breaker");
            Tooltip.SetDefault("The good time\nGo fast\nWARNING: May have disastrous results");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.value = Item.buyPrice(0, 21, 0, 0);
            item.rare = 10;
            item.accessory = true;
            item.Calamity().postMoonLordRarity = 12;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (CalamityGlobalNPC.AnyBossNPCS())
            { return; }
            if (player.velocity.X > 5f)
            {
                player.velocity.X *= 1.025f;
                if (player.velocity.X >= 500f)
                {
                    player.velocity.X = 0f;
                }
            }
            else if (player.velocity.X < -5f)
            {
                player.velocity.X *= 1.025f;
                if (player.velocity.X <= -500f)
                {
                    player.velocity.X = 0f;
                }
            }
        }
    }
}
