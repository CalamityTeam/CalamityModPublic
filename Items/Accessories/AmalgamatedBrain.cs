using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class AmalgamatedBrain : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Amalgamated Brain");
            Tooltip.SetDefault("12% increased damage\n" +
                               "Shade rains down when you are hit\n" +
                               "You will confuse nearby enemies when you are struck");
        }

        public override void SetDefaults()
        {
            item.width = 34;
            item.height = 34;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.expert = true;
            item.rare = 4;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.aBrain = true;
            if (player.immune)
            {
                if (Main.rand.NextBool(8))
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
						Projectile rain = CalamityUtils.ProjectileRain(player.Center, 400f, 100f, 500f, 800f, 22f, ModContent.ProjectileType<AuraRain>(), (int)(60 * player.AverageDamage()), 2f, player.whoAmI, 6, 1);
						rain.tileCollide = false;
                    }
                }
            }
            player.allDamage += 0.12f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<RottenBrain>());
            recipe.AddIngredient(ItemID.BrainOfConfusion);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
