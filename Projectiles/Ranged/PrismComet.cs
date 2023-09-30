using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class PrismComet : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public ref float Time => ref Projectile.ai[0];
        public override string Texture => "CalamityMod/Projectiles/Melee/Exocomet";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.scale = 0.86f;
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.alpha = 50;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.LightSeaGreen.ToVector3());
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 6 == 5)
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];

            Projectile.alpha = Utils.Clamp(Projectile.alpha - 25, 30, 255);

            if (Projectile.alpha < 40)
                ReleaseIdleDust();

            // Home onto any targets after a short amount of time.
            if (Time >= 25f)
            {
                NPC potentialTarget = Projectile.Center.ClosestNPCAt(1050f);
                if (potentialTarget != null)
                    Projectile.velocity = (Projectile.velocity * 12f + Projectile.SafeDirectionTo(potentialTarget.Center) * 19f) / 13f;
            }
            else
                Projectile.velocity *= 1.05f;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Time++;
        }
        public void ReleaseIdleDust()
        {
            if (Main.dedServ)
                return;

            Dust prismEnergy = Dust.NewDustDirect(Projectile.position - Projectile.velocity * 4f, 8, 8, 107, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 100, new Color(0, 255, 255), 0.5f);
            prismEnergy.velocity *= -0.25f;
            prismEnergy.velocity -= Projectile.velocity * 0.3f;

            prismEnergy = Dust.NewDustDirect(Projectile.position - Projectile.velocity * 4f, 8, 8, 107, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 100, new Color(0, 255, 255), 0.5f);
            prismEnergy.velocity *= -0.25f;
            prismEnergy.position -= Projectile.velocity * 0.5f;
            prismEnergy.velocity -= Projectile.velocity * 0.3f;
        }

        public override bool? CanDamage() => Time >= 20f ? null : false;

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color color = Color.Honeydew * Projectile.Opacity;
            color.A = 0;
            return color;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Zombie103, Projectile.position);
            Projectile.ExpandHitboxBy(80);
            Projectile.Damage();

            if (Main.myPlayer != Projectile.owner)
                return;

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PrismExplosionSmall>(), Projectile.damage, 0f, Projectile.owner);
        }
    }
}
