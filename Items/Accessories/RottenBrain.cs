using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class RottenBrain : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rotten Brain");
            Tooltip.SetDefault("10% increased damage when below 75% life\n5% decreased movement speed when below 50% life\nShade rains down when you are hit");
        }

        public override void SetDefaults()
        {
            item.width = 34;
            item.height = 34;
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.expert = true;
            item.rare = 3;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.immune)
            {
                if (Main.rand.NextBool(8))
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
						Projectile rain = CalamityUtils.ProjectileRain(player.Center, 400f, 100f, 500f, 800f, 22f, ModContent.ProjectileType<AuraRain>(), (int)(18 * player.AverageDamage()), 2f, player.whoAmI, 6, 1);
						rain.tileCollide = false;
                    }
                }
            }
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.rBrain = true;
        }
    }
}
