using CalamityMod.CalPlayer;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    [AutoloadEquip(EquipType.Wings)]
    public class ElysianWings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elysian Wings");
            Tooltip.SetDefault("Horizontal speed: 9.75\n" +
                "Acceleration multiplier: 2.5\n" +
                "Great vertical speed\n" +
                "Flight time: 180");
        }

        public override void SetDefaults()
        {
            item.width = 36;
            item.height = 32;
            item.value = Item.buyPrice(0, 45, 0, 0);
            item.rare = 10;
            item.Calamity().postMoonLordRarity = 12;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.moveSpeed += 0.4f;
            player.lavaMax += 240;
            player.wingTimeMax = 180;
            modPlayer.elysianFire = true;
            if (hideVisual)
            {
                modPlayer.elysianFire = false;
            }
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.85f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 3f;
            constantAscend = 0.135f;
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 9.75f;
            acceleration *= 2.5f;
        }
    }
}
