using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class OpalChargedStrike : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.alpha = 55;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
            Projectile.extraUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 50;
        }

        public override void AI()
        {
            if (Main.rand.NextBool())
            {
                Vector2 position = Projectile.Center + Vector2.Normalize(Projectile.velocity);
                Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 162, 0f, 0f, 0, default, Main.rand.NextFloat(1.2f, 1.5f))];
                dust.position = position;
                dust.velocity = Projectile.velocity.RotatedBy(0.2, default) * 0.1f + Projectile.velocity / 8f;
                dust.position += Projectile.velocity.RotatedBy(0.2, default);
                dust.fadeIn = 0.5f;
                dust.noGravity = true;
                dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 162, 0f, 0f, 0, default, Main.rand.NextFloat(1.2f, 1.5f))];
                dust.position = position;
                dust.velocity = Projectile.velocity.RotatedBy(-0.2, default) * 0.1f + Projectile.velocity / 8f;
                dust.position += Projectile.velocity.RotatedBy(-0.2, default);
                dust.fadeIn = 0.5f;
                dust.noGravity = true;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 300);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Projectile.damage = (int)(Projectile.damage * 0.80f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i <= 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.position, 162, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30f)) / 2, 0, default, Main.rand.NextFloat(1.6f, 2.3f));
                dust.noGravity = false;
            }
        }
        public override bool? CanDamage() => base.CanDamage();
    }
}
