using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Earth : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Earth");
            Tooltip.SetDefault("Lowers enemy defense by 1 with every strike\n" +
                "Your attacks will heal you a lot\n" +
                "Rains RGB meteors that explode into more meteors after a short time on enemy hits\n" +
                "Ice meteors freeze enemies\n" +
                "Flame meteors explode\n" +
                "Green meteors spawn healing orbs");
        }

        public override void SetDefaults()
        {
            item.width = 92;
            item.height = 104;
            item.scale = 1.5f;
            item.damage = 170;
            item.melee = true;
            item.useAnimation = 16;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 16;
            item.useTurn = true;
            item.knockBack = 9.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;

            item.value = CalamityGlobalItem.Rarity16BuyPrice;
            item.Calamity().customRarity = CalamityRarity.HotPink;
            item.Calamity().devItem = true;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            float num72 = 25f;
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num78 = (float)Main.mouseX - Main.screenPosition.X - vector2.X;
            float num79 = (float)Main.mouseY - Main.screenPosition.Y - vector2.Y;
            if (player.gravDir == -1f)
            {
                num79 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
            }
            float num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
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
                float speedX4 = num78;
                float speedY5 = num79 + (float)Main.rand.Next(-180, 181) * 0.02f;
                Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5, ModContent.ProjectileType<EarthProj>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f)), knockback, player.whoAmI, 0f, (float)Main.rand.Next(10));
            }

            if (target.Calamity().miscDefenseLoss < target.defense)
                target.Calamity().miscDefenseLoss += 1;

            if (!target.canGhostHeal || player.moonLeech)
                return;

            int heal = Main.rand.Next(1, 69);
            player.statLife += heal;
            player.HealEffect(heal);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            float num72 = 25f;
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num78 = (float)Main.mouseX - Main.screenPosition.X - vector2.X;
            float num79 = (float)Main.mouseY - Main.screenPosition.Y - vector2.Y;
            if (player.gravDir == -1f)
            {
                num79 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
            }
            float num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
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
                float speedX4 = num78;
                float speedY5 = num79 + (float)Main.rand.Next(-180, 181) * 0.02f;
                Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5, ModContent.ProjectileType<EarthProj>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f)), item.knockBack, player.whoAmI, 0f, (float)Main.rand.Next(10));
            }

            if (player.moonLeech)
                return;

            int heal = Main.rand.Next(1, 69);
            player.statLife += heal;
            player.HealEffect(heal);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<GrandGuardian>());
            recipe.AddIngredient(ModContent.ItemType<GalacticaBlade>());
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
