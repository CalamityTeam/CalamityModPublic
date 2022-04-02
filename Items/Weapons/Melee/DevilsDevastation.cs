using CalamityMod.Items.Materials;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class DevilsDevastation : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Devil's Devastation");
            Tooltip.SetDefault("Fires a spread of demonic scythes\n" + "Pitchforks rise from the underworld to skewer your foes\n" +
                "Critical hits cause shadowflame explosions");
        }

        public override void SetDefaults()
        {
            item.width = 118;
            item.height = 118;
            item.damage = 166;
            item.melee = true;
            item.useAnimation = 20;
            item.useTime = 20;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 6.75f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = ItemRarityID.Red;
            item.shoot = ModContent.ProjectileType<Oathblade>();
            item.shootSpeed = 28f;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            //Shoot 3 oathblades in a spread
            int index = 8;
            for (int j = -index; j <= index; j += index)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(j));
                Projectile.NewProjectile(position, perturbedSpeed, type, damage, knockBack, player.whoAmI);
            }

            //Not actually sure what this middle code does
            float speed = item.shootSpeed;
            Vector2 source = player.RotatedRelativePoint(player.MountedCenter, true);
            float directionX = (float)Main.mouseX + Main.screenPosition.X - source.X;
            float directionY = (float)Main.mouseY + Main.screenPosition.Y - source.Y;
            if (player.gravDir == -1f)
            {
                directionY = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - source.Y;
            }
            Vector2 direction = new Vector2(directionX, directionY);
            float aimDist = direction.Length();
            if ((float.IsNaN(direction.X) && float.IsNaN(direction.Y)) || (direction.X == 0f && direction.Y == 0f))
            {
                direction.X = (float)player.direction;
                direction.Y = 0f;
                aimDist = speed;
            }
            else
            {
                aimDist = speed / aimDist;
            }

            //Shoot 9 tridents from below
            int projAmt = 3; //Since the method spawns 3 projectiles, it spawns 9 total
            for (int projIndex = 0; projIndex < projAmt; projIndex++)
            {
                source = new Vector2(player.Center.X + (float)(Main.rand.Next(201) * -(float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y + 600f);
                source.X = (source.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
                source.Y += (float)(100 * projIndex);
                direction.X = (float)Main.mouseX + Main.screenPosition.X - source.X;
                direction.Y = (float)Main.mouseY + Main.screenPosition.Y - source.Y;
                if (direction.Y < 0f)
                {
                    direction.Y *= -1f;
                }
                if (direction.Y < 20f)
                {
                    direction.Y = 20f;
                }
                aimDist = direction.Length();
                aimDist = speed / aimDist;
                direction.X *= aimDist;
                direction.Y *= aimDist;
                direction.X += Main.rand.NextFloat(-40f, 40f) * 0.02f;
                direction.Y += Main.rand.NextFloat(-40f, 40f) * 0.02f;
                direction.Y *= -1;
                Projectile.NewProjectile(source, direction, ModContent.ProjectileType<DemonBlast>(), damage, knockBack, player.whoAmI, 0f, Main.rand.Next(5));
                Projectile.NewProjectile(source, direction, ModContent.ProjectileType<DemonBlast>(), damage, knockBack, player.whoAmI, 0f, Main.rand.Next(3));
                Projectile.NewProjectile(source, direction, ModContent.ProjectileType<DemonBlast>(), damage, knockBack, player.whoAmI, 0f, 1f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Devastation>());
            recipe.AddIngredient(ModContent.ItemType<TrueForbiddenOathblade>());
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 8);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 20);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 173);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 150);
            target.AddBuff(BuffID.OnFire, 300);
            if (crit)
            {
                damage /= 2;
                target.AddBuff(BuffID.ShadowFlame, 450);
                target.AddBuff(BuffID.OnFire, 900);
                player.ApplyDamageToNPC(target, damage * 4, 0f, 0, false);
                float scalar1 = 1.7f;
                float scalar2 = 0.8f;
                float scalar3 = 2f;
                Vector2 value3 = (target.rotation - MathHelper.PiOver2).ToRotationVector2();
                Vector2 value4 = value3 * target.velocity.Length();
                Main.PlaySound(SoundID.Item14, target.position);
                for (int i = 0; i < 40; i++)
                {
                    int dustInt = Dust.NewDust(target.position, target.width, target.height, 173, 0f, 0f, 200, default, scalar1);
                    Dust dust = Main.dust[dustInt];
                    dust.position = target.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * Main.rand.NextFloat() * target.width / 2f;
                    dust.noGravity = true;
                    dust.velocity.Y -= 4.5f;
                    dust.velocity *= 3f;
                    dust.velocity += value4 * Main.rand.NextFloat();
                    dustInt = Dust.NewDust(target.position, target.width, target.height, 173, 0f, 0f, 100, default, scalar2);
                    dust.position = target.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * Main.rand.NextFloat() * target.width / 2f;
                    dust.velocity.Y -= 3f;
                    dust.velocity *= 2f;
                    dust.noGravity = true;
                    dust.fadeIn = 1f;
                    dust.color = Color.Crimson * 0.5f;
                    dust.velocity += value4 * Main.rand.NextFloat();
                }
                for (int j = 0; j < 20; j++)
                {
                    int dustInt = Dust.NewDust(target.position, target.width, target.height, 173, 0f, 0f, 0, default, scalar3);
                    Dust dust = Main.dust[dustInt];
                    dust.position = target.Center + Vector2.UnitX.RotatedByRandom(Math.PI).RotatedBy((double)target.velocity.ToRotation(), default) * target.width / 3f;
                    dust.noGravity = true;
                    dust.velocity.Y -= 1.5f;
                    dust.velocity *= 0.5f;
                    dust.velocity += value4 * (0.6f + 0.6f * Main.rand.NextFloat());
                }
            }
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Shadowflame>(), 150);
            target.AddBuff(BuffID.OnFire, 300);
            if (crit)
            {
                target.AddBuff(ModContent.BuffType<Shadowflame>(), 450);
                target.AddBuff(BuffID.OnFire, 900);
                Main.PlaySound(SoundID.Item14, target.position);
            }
        }
    }
}
