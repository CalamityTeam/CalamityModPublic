using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class SolsticeClaymore : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Solstice Claymore");
            Tooltip.SetDefault("Changes projectile color based on the time of year\n" +
                               "Inflicts daybroken during the day and nightwither during the night");
        }

        public override void SetDefaults()
        {
            item.width = 86;
            item.damage = 470;
            item.melee = true;
            item.useAnimation = 16;
            item.useStyle = 1;
            item.useTime = 16;
            item.useTurn = true;
            item.knockBack = 6.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 86;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.shoot = mod.ProjectileType("SolsticeBeam");
            item.shootSpeed = 16f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 12;
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BeamSword);
            recipe.AddIngredient(null, "AstralBar", 20);
            recipe.AddIngredient(ItemID.FragmentSolar, 5);
            recipe.AddIngredient(ItemID.FragmentVortex, 5);
            recipe.AddIngredient(ItemID.FragmentStardust, 5);
            recipe.AddIngredient(ItemID.FragmentNebula, 5);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            int dustType = Main.dayTime ?
            Utils.SelectRandom<int>(Main.rand, new int[]
            {
            6,
            259,
            158
            }) :
            Utils.SelectRandom<int>(Main.rand, new int[]
            {
            173,
            27,
            234
            });
            if (Main.rand.Next(4) == 0)
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, dustType);
                Main.dust[dust].noGravity = true;
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (Main.dayTime)
            {
                target.AddBuff(BuffID.Daybreak, 300);
            }
            else
            {
                target.AddBuff(mod.BuffType("Nightwither"), 300);
            }
        }
    }
}
