using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

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
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.Calamity().rogue = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
            if (Projectile.ai[0] < 60f)
                Projectile.ai[0] += 1f;
            else
            {
                CalamityGlobalProjectile.HomeInOnNPC(Projectile, !Projectile.tileCollide, 300f, 12f, 20f);
            }

            Projectile.rotation += Math.Abs(Projectile.velocity.X) * 0.01f * (float)Projectile.direction;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.NPCKilled, (int)Projectile.position.X, (int)Projectile.position.Y, 43, 0.35f, 0f);

            if (Projectile.owner == Main.myPlayer)
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

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, num628, num629, ModContent.ProjectileType<DeepSeaDumbbellWeight>(),
                        (int)((double)Projectile.damage * 0.25), Projectile.knockBack * 0.25f, Main.myPlayer, 0f, 0f);

                    num3 = num627;
                }
            }

            Projectile.Kill();

            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 120);

            SoundEngine.PlaySound(SoundID.NPCKilled, (int)Projectile.position.X, (int)Projectile.position.Y, 43, 0.35f, 0f);

            if (Projectile.owner == Main.myPlayer)
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

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, num628, num629, ModContent.ProjectileType<DeepSeaDumbbellWeight>(),
                        (int)((double)Projectile.damage * 0.25), Projectile.knockBack * 0.25f, Main.myPlayer, 0f, 0f);

                    num3 = num627;
                }
            }

            Projectile.Kill();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 120);

            SoundEngine.PlaySound(SoundID.NPCKilled, (int)Projectile.position.X, (int)Projectile.position.Y, 43, 0.35f, 0f);

            if (Projectile.owner == Main.myPlayer)
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

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, num628, num629, ModContent.ProjectileType<DeepSeaDumbbellWeight>(),
                        (int)((double)Projectile.damage * 0.25), Projectile.knockBack * 0.25f, Main.myPlayer, 0f, 0f);

                    num3 = num627;
                }
            }

            Projectile.Kill();
        }
    }
}
