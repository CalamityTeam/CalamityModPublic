using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons
{
    public class PlagueKeeper : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plague Keeper");
        }

        public override void SetDefaults()
        {
            item.width = 74;
            item.damage = 110;
            item.melee = true;
            item.useAnimation = 18;
            item.useStyle = 1;
            item.useTime = 18;
            item.useTurn = true;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 92;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.shoot = mod.ProjectileType("PlagueBeeDust");
            item.shootSpeed = 9f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 12;
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "VirulentKatana");
            recipe.AddIngredient(ItemID.BeeKeeper);
            recipe.AddIngredient(ItemID.FragmentSolar, 10);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(mod.BuffType("Plague"), 300);
            for (int i = 0; i < 3; i++)
            {
                int bee = Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, player.beeType(), 
                    player.beeDamage(item.damage / 3), player.beeKB(0f), player.whoAmI, 0f, 0f);
                Main.projectile[bee].penetrate = 1;
            }
        }
    }
}
