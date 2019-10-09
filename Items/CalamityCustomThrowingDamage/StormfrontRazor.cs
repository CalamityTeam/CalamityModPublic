using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
    public class StormfrontRazor : CalamityDamageItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stormfront Razor");
            Tooltip.SetDefault("Throws a throwing knife that leaves sparks as it travels.\n" +
                               "Stealth Strike: The knife is faster and leaves a huge shower of sparks as it travels");
        }

        public override void SafeSetDefaults()
        {
            item.width = 40;
            item.height = 42;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.useStyle = 1;
            item.value = Item.buyPrice(0, 30, 0, 0);
            item.rare = 5;

            item.useAnimation = 15;
            item.useTime = 15;
            item.damage = 50;
            item.crit += 8;
            item.knockBack = 7f;
            item.shoot = mod.ProjectileType("Stormfrontproj");
            item.shootSpeed = 7f;
            item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "Cinquedea");
            recipe.AddIngredient(null, "AerialiteBar", 8);
            recipe.AddIngredient(null, "EssenceofCinder", 4);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.GetCalamityPlayer().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(position, new Vector2(speedX * 1.6f, speedY * 1.6f), mod.ProjectileType("Stormfrontproj"), damage, knockBack, player.whoAmI, 0, 40f);
                Main.projectile[p].GetCalamityProj().stealthStrike = true;
                return false;
            }
            else
            {
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), mod.ProjectileType("Stormfrontproj"), damage, knockBack, player.whoAmI, 0, 1);
                return false;
            }
        }
    }
}
