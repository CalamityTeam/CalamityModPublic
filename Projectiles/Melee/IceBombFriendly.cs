using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class IceBombFriendly : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/IceBomb";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Bomb");
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.scale = 0.5f;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
        }

        public override bool? CanDamage() => Projectile.ai[0] >= 120f;

        public override void AI()
        {
            Projectile.velocity *= 0.98f;

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.scale += 0.01f;
                Projectile.alpha -= 50;
                if (Projectile.alpha <= 0)
                {
                    Projectile.localAI[0] = 1f;
                    Projectile.alpha = 0;
                }
            }
            else
            {
                Projectile.scale -= 0.01f;
                Projectile.alpha += 50;
                if (Projectile.alpha >= 255)
                {
                    Projectile.localAI[0] = 0f;
                    Projectile.alpha = 255;
                }
            }

            if (Projectile.ai[0] < 120f)
            {
                Projectile.ai[0] += 1f;
                if (Projectile.ai[0] == 120f)
                {
                    for (int num621 = 0; num621 < 8; num621++)
                    {
                        int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 67, 0f, 0f, 100, default, 2f);
                        Main.dust[num622].velocity *= 3f;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[num622].scale = 0.5f;
                            Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                        }
                    }
                    for (int num623 = 0; num623 < 14; num623++)
                    {
                        int num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 67, 0f, 0f, 100, default, 3f);
                        Main.dust[num624].noGravity = true;
                        Main.dust[num624].velocity *= 5f;
                        num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 67, 0f, 0f, 100, default, 2f);
                        Main.dust[num624].velocity *= 2f;
                    }

                    Projectile.scale = 1f;
                    CalamityGlobalProjectile.ExpandHitboxBy(Projectile, (int)(30f * Projectile.scale));
                    SoundEngine.PlaySound(SoundID.Item30, Projectile.Center);
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Main.dayTime ? new Color(50, 50, 255, Projectile.alpha) : new Color(255, 255, 255, Projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            float spread = 90f * 0.0174f;
            double startAngle = Math.Atan2(Projectile.velocity.X, Projectile.velocity.Y) - spread / 2;
            double deltaAngle = spread / 8f;
            double offsetAngle;
            int i;
            if (Projectile.owner == Main.myPlayer)
            {
                for (i = 0; i < 2; i++)
                {
                    offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                    int projectile1 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ProjectileID.FrostShard, (int)(Projectile.damage * 0.5), 0f, Projectile.owner, 0f, 0f);
                    if (projectile1.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[projectile1].DamageType = DamageClass.Melee;
                        Main.projectile[projectile1].penetrate = 2;
                    }
                    int projectile2 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ProjectileID.FrostShard, (int)(Projectile.damage * 0.5), 0f, Projectile.owner, 0f, 0f);
                    if (projectile2.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[projectile2].DamageType = DamageClass.Melee;
                        Main.projectile[projectile2].penetrate = 2;
                    }
                }
            }

            for (int k = 0; k < 3; k++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 67, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
        }
    }
}
