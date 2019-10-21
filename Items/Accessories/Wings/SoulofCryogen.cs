using CalamityMod.CalPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class SoulofCryogen : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul of Cryogen");
            Tooltip.SetDefault("The magic of the ancient ice castle is yours\n" +
                "Counts as wings\n" +
                "Horizontal speed: 6.25\n" +
                "Acceleration multiplier: 1\n" +
                "Average vertical speed\n" +
                "Flight time: 100\n" +
                "5% increase to all damage and pick speed\n" +
                "All melee attacks and projectiles inflict frostburn");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(8, 4));
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = Item.buyPrice(0, 39, 99, 99);
            item.expert = true;
            item.rare = 9;
            item.accessory = true;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            maxFallSpeed *= 0f;
            float num = (float)Main.rand.Next(90, 111) * 0.01f;
            num *= Main.essScale;
            Lighting.AddLight((int)((item.position.X + (float)(item.width / 2)) / 16f), (int)((item.position.Y + (float)(item.height / 2)) / 16f), 0f * num, 0.3f * num, 0.3f * num);
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.5f;
            ascentWhenRising = 0.1f;
            maxCanAscendMultiplier = 0.5f;
            maxAscentMultiplier = 1.5f;
            constantAscend = 0.1f;
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 6.25f;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.cryogenSoul = true;
            player.pickSpeed -= 0.05f;
            player.allDamage += 0.05f;
            player.wingTimeMax = 100;
        }
    }
}
