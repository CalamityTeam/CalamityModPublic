using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class PhantomBlast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom Blast");
        }

        public override void SetDefaults()
        {
            projectile.Calamity().canBreakPlayerDefense = true;
            projectile.width = 18;
            projectile.height = 18;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            cooldownSlot = 1;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                Main.PlaySound(SoundID.Item20, projectile.position);
            }

            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

            if (projectile.velocity.Length() < 10f)
                projectile.velocity *= 1.01f;

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 9f)
            {
                projectile.alpha -= 5;
                if (projectile.alpha < 30)
                    projectile.alpha = 30;
            }

            projectile.localAI[1] += 1f;
            if (projectile.localAI[1] == 24f)
            {
                projectile.localAI[1] = 0f;
                for (int l = 0; l < 12; l++)
                {
                    Vector2 vector3 = Vector2.UnitX * -projectile.width / 2f;
                    vector3 += -Vector2.UnitY.RotatedBy(l * MathHelper.Pi / 6f) * new Vector2(8f, 16f);
                    vector3 = vector3.RotatedBy(projectile.rotation - MathHelper.PiOver2);
                    int num9 = Dust.NewDust(projectile.Center, 0, 0, 180, 0f, 0f, 160, default, 1f);
                    Main.dust[num9].scale = 1.1f;
                    Main.dust[num9].noGravity = true;
                    Main.dust[num9].position = projectile.Center + vector3;
                    Main.dust[num9].velocity = projectile.velocity * 0.1f;
                    Main.dust[num9].velocity = Vector2.Normalize(projectile.Center - projectile.velocity * 3f - Main.dust[num9].position) * 1.25f;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(100, 250, 250, projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item125, projectile.position);
            projectile.position.X = projectile.position.X + (projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (projectile.height / 2);
            projectile.width = 50;
            projectile.height = 50;
            projectile.position.X = projectile.position.X - (projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (projectile.height / 2);
            for (int num621 = 0; num621 < 3; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 180, 0f, 0f, 100, default, 1.2f);
                Main.dust[num622].velocity *= 3f;
                Main.dust[num622].noGravity = true;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 5; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 180, 0f, 0f, 100, default, 1.7f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 180, 0f, 0f, 100, default, 1f);
                Main.dust[num624].velocity *= 2f;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 180);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
