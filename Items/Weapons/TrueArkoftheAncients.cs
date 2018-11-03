using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class TrueArkoftheAncients : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("True Ark of the Ancients");
            Tooltip.SetDefault("A heavenly blade forged to vanquish all evil");
        }

        public override void SetDefaults()
        {
            item.width = 60;
            item.damage = 82;
            item.melee = true;
            item.useAnimation = 25;
            item.useTime = 25;
            item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 6.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 60;
            item.value = 1000000;
            item.rare = 8;
            item.shoot = mod.ProjectileType("EonBeam");
            item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            switch (Main.rand.Next(2))
            {
                case 0: type = mod.ProjectileType("EonBeam"); break;
                case 1: type = mod.ProjectileType("EonBeamV2"); break;
            }
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, Main.myPlayer);
            int i = Main.myPlayer;
            float num72 = Main.rand.Next(18, 27);
            int num73 = Main.rand.Next(60, 91);
            float num74 = knockBack;
            num74 = player.GetWeaponKnockback(item, num74);
            player.itemTime = item.useTime;
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            Vector2 value = Vector2.UnitX.RotatedBy((double)player.fullRotation, default(Vector2));
            Vector2 vector3 = Main.MouseWorld - vector2;
            float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
            float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
            if (player.gravDir == -1f)
            {
                num79 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
            }
            float num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
            float num81 = num80;
            if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
            {
                num78 = (float)player.direction;
                num79 = 0f;
                num80 = num72;
            }
            else
            {
                num80 = num72 / num80;
            }
            num78 *= num80;
            num79 *= num80;
            int num107 = 3;
            for (int num108 = 0; num108 < num107; num108++)
            {
                vector2 = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -(float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
                vector2.X = (vector2.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
                vector2.Y -= (float)(100 * num108);
                num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
                num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
                if (num79 < 0f)
                {
                    num79 *= -1f;
                }
                if (num79 < 20f)
                {
                    num79 = 20f;
                }
                num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
                num80 = num72 / num80;
                num78 *= num80;
                num79 *= num80;
                float speedX4 = num78 + (float)Main.rand.Next(-160, 161) * 0.02f;
                float speedY5 = num79 + (float)Main.rand.Next(-160, 161) * 0.02f;
                int star = Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5, 92, num73, num74, i, 0f, (float)Main.rand.Next(10));
                Main.projectile[star].ranged = false;
                Main.projectile[star].melee = true;
                Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5, mod.ProjectileType("TerraBall"), num73, num74, i, 0f, (float)Main.rand.Next(5));
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "ArkoftheAncients");
            recipe.AddIngredient(null, "CoreofCalamity");
            recipe.AddIngredient(ItemID.BrokenHeroSword);
            recipe.AddIngredient(null, "LivingShard", 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.Next(5) == 0)
            {
                int num249 = Main.rand.Next(3);
                if (num249 == 0)
                {
                    num249 = 15;
                }
                else if (num249 == 1)
                {
                    num249 = 57;
                }
                else
                {
                    num249 = 58;
                }
                int num250 = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, num249, (float)(player.direction * 2), 0f, 150, default(Color), 1.3f);
                Main.dust[num250].velocity *= 0.2f;
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.Next(2) == 0)
            {
                target.AddBuff(mod.BuffType("HolyLight"), 500);
            }
        }
    }
}
