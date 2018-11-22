using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class SporeKnife : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spore Knife");
            Tooltip.SetDefault("Enemies release spore clouds on death");
        }

        public override void SetDefaults()
        {
            item.useStyle = 3;
            item.useTurn = false;
            item.useAnimation = 12;
            item.useTime = 12;  //Ranges from 1 to 55.
            item.width = 28;  //The width of the .png file in pixels divided by 2.
            item.height = 28;  //The height of the .png file in pixels divided by 2.
            item.damage = 33;  //Keep this reasonable please.
            item.melee = true;  //Dictates whether this is a melee-class weapon.
            item.knockBack = 5.75f;  //Ranges from 1 to 9.
            item.UseSound = SoundID.Item1;
            item.useTurn = true;
            item.autoReuse = true;  //Dictates whether the weapon can be "auto-fired".
            item.maxStack = 1;
            item.value = 45000;  //Value is calculated in copper coins.
            item.rare = 3;  //Ranges from 1 to 11.
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.JungleSpores, 10);
            recipe.AddIngredient(ItemID.Stinger, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.Next(5) == 0)
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 2);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (target.life <= 0)
            {
                Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, Main.rand.Next(569, 572), (int)((float)item.damage * player.meleeDamage), knockback, Main.myPlayer);
            }
        }
    }
}
