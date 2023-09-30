using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class NovaShot : ModProjectile, ILocalizedModType
    {
        public bool FirstFrameNoDraw = true;
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 20;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
            Projectile.extraUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            //Dust dust = Dust.NewDustPerfect(Projectile.Center, 107); // + Main.rand.NextVector2Circular(-3, 3)
            //dust.noGravity = true;
            //dust.scale = 0.5f;
            if (Main.rand.NextBool(4))
            {
                Vector2 SparkVelocity1 = Projectile.velocity.RotatedBy(-3, default) * 0.1f - Projectile.velocity / 2f;
                Vector2 SparkPosition1 = Projectile.velocity.RotatedBy(-0.8, default);
                SparkParticle spark = new SparkParticle(Projectile.Center + SparkPosition1, SparkVelocity1, false, Main.rand.Next(4, 5), Main.rand.NextFloat(0.4f, 0.6f), Color.Lime);
                GeneralParticleHandler.SpawnParticle(spark);

            }
            if (Main.rand.NextBool(4))
            {
                Vector2 SparkVelocity2 = Projectile.velocity.RotatedBy(3, default) * 0.1f - Projectile.velocity / 2f;
                Vector2 SparkPosition2 = Projectile.velocity.RotatedBy(0.8, default);
                SparkParticle spark2 = new SparkParticle(Projectile.Center + SparkPosition2, SparkVelocity2, false, Main.rand.Next(4, 5), Main.rand.NextFloat(0.4f, 0.6f), Main.rand.NextBool(4) ? Color.Chartreuse : Color.Lime);
                GeneralParticleHandler.SpawnParticle(spark2);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(50, 255, 50, Projectile.alpha);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, 90);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Projectile.damage = (int)(Projectile.damage * 0.88f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i <= 4; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.position, 107, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(0.1f, 0.8f), 0, default, Main.rand.NextFloat(1.2f, 1.6f));
                dust.noGravity = false;
            }
        }
        public override bool? CanDamage() => base.CanDamage();
    }
}
