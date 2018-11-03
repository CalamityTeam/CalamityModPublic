using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class TyrantYharimsUltisword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tyrant Yharim's Ultisword");
            Tooltip.SetDefault("Necrotic blade of Jungle King Yharim\n50% chance to give the player the tyrant's fury buff on enemy hits\nThis buff increases melee damage, speed, and crit chance by 30%");
        }

        public override void SetDefaults()
        {
            item.width = 84;  //The width of the .png file in pixels divided by 2.
            item.damage = 64;  //Keep this reasonable please.
            item.melee = true;  //Dictates whether this is a melee-class weapon.
            item.useAnimation = 26;
            item.useStyle = 1;
            item.useTime = 26;
            item.useTurn = true;
            item.knockBack = 5.50f;  //Ranges from 1 to 9.
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;  //Dictates whether the weapon can be "auto-fired".
            item.height = 84;  //The height of the .png file in pixels divided by 2.
            item.value = 1250000;  //Value is calculated in copper coins.
            item.rare = 9;  //Ranges from 1 to 11.
            item.shoot = mod.ProjectileType("BlazingPhantomBlade");
            item.shootSpeed = 12f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "TrueCausticEdge");
            recipe.AddIngredient(ItemID.BrokenHeroSword);
            recipe.AddIngredient(ItemID.FlaskofVenom, 5);
            recipe.AddIngredient(ItemID.FlaskofCursedFlames, 5);
            recipe.AddIngredient(ItemID.ChlorophyteBar, 15);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "TrueCausticEdge");
            recipe.AddIngredient(ItemID.BrokenHeroSword);
            recipe.AddIngredient(ItemID.FlaskofVenom, 5);
            recipe.AddIngredient(ItemID.FlaskofIchor, 5);
            recipe.AddIngredient(ItemID.ChlorophyteBar, 15);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.Next(5) == 0)
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 106);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.Next(2) == 0)
            {
                player.AddBuff(mod.BuffType("TyrantsFury"), 180);
            }
            target.AddBuff(BuffID.OnFire, 300);
            target.AddBuff(BuffID.Venom, 240);
            target.AddBuff(BuffID.CursedInferno, 180);
        }
    }
}
