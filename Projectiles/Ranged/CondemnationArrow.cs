using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class CondemnationArrow : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public ref float Time => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 90;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.arrow = true;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 300;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void AI()
        {
            //Sound is played in the first frame instead of in the weapon shoot for proper mp sync
            if (Projectile.timeLeft == 300)
                SoundEngine.PlaySound(SoundID.DD2_BallistaTowerShot, Projectile.Center);


            Lighting.AddLight(Projectile.Center, Color.Violet.ToVector3());
            Projectile.Opacity = Utils.GetLerpValue(0f, 20f, Time, true);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Time++;

            // Release side arrows every so often.
            if (Main.netMode != NetmodeID.MultiplayerClient && Time % 90f == 89f)
            {
                for (int i = -1; i <= 1; i += 2)
                {
                    Vector2 shootVelocity = Projectile.velocity.RotatedBy(i * 0.036f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVelocity, ModContent.ProjectileType<CondemnationArrowHoming>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color c1 = new Color(226, 40, 40, 0);
            Color c2 = new Color(205, 0, 194, 0);
            Color fadeColor = Color.Lerp(c1, c2, (float)Math.Cos(Projectile.identity * 1.41f + Main.GlobalTimeWrappedHourly * 8f) * 0.5f + 0.5f);
            return Color.Lerp(lightColor, fadeColor, 0.5f) * Projectile.Opacity;
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

            for (int i = 0; i < 30; i++)
            {
                Dust fire = Dust.NewDustPerfect(Projectile.Center, 130);
                fire.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(0.8f) * new Vector2(4f, 1.25f) * Main.rand.NextFloat(0.9f, 1f);
                fire.velocity = fire.velocity.RotatedBy(Projectile.rotation - MathHelper.PiOver2);
                fire.velocity += Projectile.velocity * 0.7f;

                fire.noGravity = true;
                fire.color = Color.Lerp(Color.White, Color.Purple, Main.rand.NextFloat());
                fire.scale = Main.rand.NextFloat(1f, 1.1f);

                fire = Dust.CloneDust(fire);
                fire.velocity = Main.rand.NextVector2Circular(3f, 3f);
                fire.velocity += Projectile.velocity * 0.6f;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => Projectile.RotatingHitboxCollision(targetHitbox.TopLeft(), targetHitbox.Size());
    }
}
