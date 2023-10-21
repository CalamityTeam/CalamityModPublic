using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class VenusianBolt : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + MathHelper.ToRadians(45);
            Lighting.AddLight(Projectile.Center, 0.25f, 0.2f, 0f);
            if (Projectile.wet && !Projectile.lavaWet)
            {
                Projectile.Kill();
            }
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item73, Projectile.position);
                Projectile.localAI[0] += 1f;
            }
            for (int i = 0; i < 3; i++)
            {
                int venusDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 55, 0f, 0f, 100, default, 1.2f);
                Main.dust[venusDust].noGravity = true;
                Main.dust[venusDust].velocity *= 0.5f;
                Main.dust[venusDust].velocity += Projectile.velocity * 0.1f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Daybreak, 300);
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                int explosionDamage = Projectile.damage;
                float explosionKB = 6f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<VenusianExplosion>(), explosionDamage, explosionKB, Projectile.owner);

                int cinderDamage = (int)(Projectile.damage * 0.75);
                float cinderKB = 0f;
                Vector2 cinderPos = Projectile.oldPosition + 0.5f * Projectile.Size;
                int numCinders = Main.rand.Next(7, 10);
                for (int i = 0; i < numCinders; i++)
                {
                    Vector2 cinderVel = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                    while (cinderVel.X == 0f && cinderVel.Y == 0f)
                    {
                        cinderVel = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                    }
                    cinderVel.Normalize();
                    cinderVel *= Main.rand.Next(70, 101) * 0.1f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), cinderPos, cinderVel, ModContent.ProjectileType<VenusianFlame>(), cinderDamage, cinderKB, Projectile.owner);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
