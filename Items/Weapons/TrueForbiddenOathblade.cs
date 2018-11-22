using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class TrueForbiddenOathblade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("True Forbidden Oathblade");
            Tooltip.SetDefault("Sword of an ancient demon god");
        }

        public override void SetDefaults()
        {
            item.width = 78;
            item.damage = 80;
            item.melee = true;
            item.useAnimation = 23;
            item.useTime = 23;
            item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 7.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 78;
            item.value = 1000000;
            item.rare = 8;
            item.shoot = mod.ProjectileType("Oathblade");
            item.shootSpeed = 3f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int numProj = 2;
            float rotation = MathHelper.ToRadians(8);
            for (int i = 0; i < numProj + 1; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "ForbiddenOathblade");
            recipe.AddIngredient(null, "CalamityDust", 3);
            recipe.AddIngredient(null, "LivingShard", 3);
            recipe.AddIngredient(ItemID.BrokenHeroSword);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.Next(3) == 0)
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 173);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 300);
            target.AddBuff(BuffID.OnFire, 300);
        }
    }
}
