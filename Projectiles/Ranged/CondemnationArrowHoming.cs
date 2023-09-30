using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class CondemnationArrowHoming : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/Ranged/CondemnationArrow";
        public ref float Time => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 45;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.arrow = true;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.Violet.ToVector3());
            Projectile.Opacity = Utils.GetLerpValue(0f, 20f, Time, true) * Utils.GetLerpValue(0f, 20f, Projectile.timeLeft, true);

            NPC potentialTarget = Projectile.Center.ClosestNPCAt(1500f, false);
            if (potentialTarget != null)
                Projectile.velocity = (Projectile.velocity * 29f + Projectile.SafeDirectionTo(potentialTarget.Center) * 21f) / 30f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Time++;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color c1 = new Color(226, 40, 40, 0);
            Color c2 = new Color(205, 0, 194, 0);
            return Color.Lerp(c1, c2, (float)Math.Cos(Projectile.identity * 1.41f + Main.GlobalTimeWrappedHourly * 8f) * 0.5f + 0.5f) * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            // Release a burst of magic dust on death.
            if (Main.dedServ)
                return;

            for (int i = 0; i < 10; i++)
            {
                Dust fire = Dust.NewDustPerfect(Projectile.Center, 267);
                fire.velocity = Main.rand.NextVector2Circular(2f, 2f);
                fire.color = Color.Lerp(Color.Red, Color.Purple, Main.rand.NextFloat());
                fire.scale = Main.rand.NextFloat(1f, 1.1f);
                fire.noGravity = true;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => Projectile.RotatingHitboxCollision(targetHitbox.TopLeft(), targetHitbox.Size());
    }
}
