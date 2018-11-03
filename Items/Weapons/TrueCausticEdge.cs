using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class TrueCausticEdge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("True Caustic Edge");
            Tooltip.SetDefault("Pestilent Defilement");
        }

        public override void SetDefaults()
        {
            item.width = 62;  //The width of the .png file in pixels divided by 2.
            item.damage = 42;  //Keep this reasonable please.
            item.melee = true;  //Dictates whether this is a melee-class weapon.
            item.useAnimation = 28;
            item.useStyle = 1;
            item.useTime = 28;
            item.useTurn = true;
            item.knockBack = 5.75f;  //Ranges from 1 to 9.
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;  //Dictates whether the weapon can be "auto-fired".
            item.height = 68;  //The height of the .png file in pixels divided by 2.
            item.value = 635000;  //Value is calculated in copper coins.
            item.rare = 8;  //Ranges from 1 to 11.
            item.shoot = mod.ProjectileType("TrueCausticEdgeProjectile");
            item.shootSpeed = 16f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CausticEdge");
            recipe.AddIngredient(ItemID.FlaskofCursedFlames, 5);
            recipe.AddIngredient(ItemID.FlaskofPoison, 5);
            recipe.AddIngredient(ItemID.Deathweed, 3);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CausticEdge");
            recipe.AddIngredient(ItemID.FlaskofIchor, 5);
            recipe.AddIngredient(ItemID.FlaskofPoison, 5);
            recipe.AddIngredient(ItemID.Deathweed, 3);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.Next(3) == 0)
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 74);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
            target.AddBuff(BuffID.Venom, 180);
        }
    }
}
