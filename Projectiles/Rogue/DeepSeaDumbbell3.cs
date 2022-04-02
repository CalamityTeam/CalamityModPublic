using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Rogue
{
    public class DeepSeaDumbbell3 : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/DeepSeaDumbbell";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deep Sea Dumbbell");
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.Calamity().rogue = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
        }

        public override void AI()
        {
            if (projectile.ai[0] < 60f)
                projectile.ai[0] += 1f;
            else
            {
                CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 300f, 12f, 20f);
            }

            projectile.rotation += Math.Abs(projectile.velocity.X) * 0.01f * (float)projectile.direction;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Main.PlaySound(SoundID.NPCKilled, (int)projectile.position.X, (int)projectile.position.Y, 43, 0.35f, 0f);

            if (projectile.owner == Main.myPlayer)
            {
                float num628 = (float)Main.rand.Next(-35, 36) * 0.01f;
                float num629 = (float)Main.rand.Next(-35, 36) * 0.01f;
                int num3;
                for (int num627 = 0; num627 < 2; num627 = num3 + 1)
                {
                    if (num627 == 1)
                    {
                        num628 *= 10f;
                        num629 *= 10f;
                    }
                    else
                    {
                        num628 *= -10f;
                        num629 *= -10f;
                    }

                    Projectile.NewProjectile(projectile.position.X, projectile.position.Y, num628, num629, ModContent.ProjectileType<DeepSeaDumbbellWeight>(),
                        (int)((double)projectile.damage * 0.25), projectile.knockBack * 0.25f, Main.myPlayer, 0f, 0f);

                    num3 = num627;
                }
            }

            projectile.Kill();

            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 120);

            Main.PlaySound(SoundID.NPCKilled, (int)projectile.position.X, (int)projectile.position.Y, 43, 0.35f, 0f);

            if (projectile.owner == Main.myPlayer)
            {
                float num628 = (float)Main.rand.Next(-35, 36) * 0.01f;
                float num629 = (float)Main.rand.Next(-35, 36) * 0.01f;
                int num3;
                for (int num627 = 0; num627 < 2; num627 = num3 + 1)
                {
                    if (num627 == 1)
                    {
                        num628 *= 10f;
                        num629 *= 10f;
                    }
                    else
                    {
                        num628 *= -10f;
                        num629 *= -10f;
                    }

                    Projectile.NewProjectile(projectile.position.X, projectile.position.Y, num628, num629, ModContent.ProjectileType<DeepSeaDumbbellWeight>(),
                        (int)((double)projectile.damage * 0.25), projectile.knockBack * 0.25f, Main.myPlayer, 0f, 0f);

                    num3 = num627;
                }
            }

            projectile.Kill();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 120);

            Main.PlaySound(SoundID.NPCKilled, (int)projectile.position.X, (int)projectile.position.Y, 43, 0.35f, 0f);

            if (projectile.owner == Main.myPlayer)
            {
                float num628 = (float)Main.rand.Next(-35, 36) * 0.01f;
                float num629 = (float)Main.rand.Next(-35, 36) * 0.01f;
                int num3;
                for (int num627 = 0; num627 < 2; num627 = num3 + 1)
                {
                    if (num627 == 1)
                    {
                        num628 *= 10f;
                        num629 *= 10f;
                    }
                    else
                    {
                        num628 *= -10f;
                        num629 *= -10f;
                    }

                    Projectile.NewProjectile(projectile.position.X, projectile.position.Y, num628, num629, ModContent.ProjectileType<DeepSeaDumbbellWeight>(),
                        (int)((double)projectile.damage * 0.25), projectile.knockBack * 0.25f, Main.myPlayer, 0f, 0f);

                    num3 = num627;
                }
            }

            projectile.Kill();
        }
    }
}
