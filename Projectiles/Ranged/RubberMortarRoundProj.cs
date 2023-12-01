using CalamityMod.Items.Ammo;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class RubberMortarRoundProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Items/Ammo/RubberMortarRound";

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // If moving fast enough, produce dust. Since it's a bullet, it should always be moving fast enough.
            if (Projectile.velocity.Length() >= 8f)
            {
                for (int d = 0; d < 2; d++)
                {
                    float xOffset = 0f;
                    float yOffset = 0f;
                    if (d == 1)
                    {
                        xOffset = Projectile.velocity.X * 0.5f;
                        yOffset = Projectile.velocity.Y * 0.5f;
                    }
                    int fire = Dust.NewDust(new Vector2(Projectile.position.X + 3f + xOffset, Projectile.position.Y + 3f + yOffset) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, 6, 0f, 0f, 100, default, 1f);
                    Main.dust[fire].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
                    Main.dust[fire].velocity *= 0.2f;
                    Main.dust[fire].noGravity = true;
                    int smoke = Dust.NewDust(new Vector2(Projectile.position.X + 3f + xOffset, Projectile.position.Y + 3f + yOffset) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, DustID.Smoke, 0f, 0f, 100, default, 0.5f);
                    Main.dust[smoke].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
                    Main.dust[smoke].velocity *= 0.05f;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        private void Explode()
        {
            // Apply damage a second time on explosion. This explosion also has double knockback.
            Projectile.ExpandHitboxBy(MortarRound.HitboxBlastRadius);
            Projectile.maxPenetrate = Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.knockBack *= 2f;
            Projectile.Damage();

            // Actually destroy tiles. Blast radius is significantly increased in GFB.
            if (Projectile.owner == Main.myPlayer)
                Projectile.ExplodeTiles(MortarRound.TileBlastRadius, true);

            // Play standard explosion sound.
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            // Spawn standard explosion dust and gores.
            MortarRoundProj.SpawnDust(Projectile);
            MortarRoundProj.SpawnExplosionGores(Projectile);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // If there are no bounces left, just explode with no other effects.
            if (Projectile.penetrate <= 1)
            {
                Projectile.Kill();
                Projectile.active = false;
                return false;
            }

            // Otherwise, the Rubber Mortar Round will explode AND bounce, in that order.

            // Explosions change both the penetrate value and hitbox, so the previous values have to be stored.
            Rectangle origHitbox = Projectile.Hitbox;
            int origPen = Projectile.penetrate;
            Explode();
            Projectile.Hitbox = origHitbox;
            Projectile.penetrate = origPen;

            // Each bounce consumes a penetrate value.
            --Projectile.penetrate;

            // If bounces are still left, bounce.
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            Projectile.velocity *= 1.25f;

            // Do not use vanilla tile collide logic.
            return false;
        }

        public override void OnKill(int timeLeft) => Explode();
    }
}
