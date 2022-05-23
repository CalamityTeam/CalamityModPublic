using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class TheAtomSplitterDuplicate : ModProjectile
    {
        public ref float Time => ref Projectile.ai[0];
        public ref float Lifetime => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Atom Splitter");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.scale = 0.66f;
            Projectile.width = Projectile.height = (int)(124f * Projectile.scale);
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
            Projectile.timeLeft = 900;
            Projectile.extraUpdates = 1;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            // Accelerate until at a certain speed.
            if (Projectile.velocity.Length() < 20f)
                Projectile.velocity *= 1.02f;

            Projectile.Opacity = CalamityUtils.Convert01To010(Time / Lifetime) * 0.6f;
            if (Time >= Lifetime)
                Projectile.Kill();

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Time++;
        }

        public override bool? CanDamage() => Projectile.alpha < 180 ? null : false;

        public override bool PreDraw(ref Color lightColor)
        {
            if (Time <= 1f)
                return false;

            Color drawColor = CalamityUtils.MulticolorLerp((Time / 35f + Projectile.identity / 4f) % 1f, CalamityUtils.ExoPalette);
            drawColor.A = 0;
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], drawColor);
            return false;
        }
    }
}
