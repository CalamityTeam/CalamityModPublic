using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class BloodScythe : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.alpha = 100;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.35f / 255f, (255 - Projectile.alpha) * 0.05f / 255f, (255 - Projectile.alpha) * 0.075f / 255f);

            Projectile.velocity *= 0.935f;
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.1f;
            Projectile.Opacity = Utils.GetLerpValue(1f, 6f, Projectile.velocity.Length(), true);
            if (Projectile.Opacity <= 0f)
                Projectile.Kill();

            // Weakly home in on enemies.
            NPC potentialTarget = Projectile.Center.ClosestNPCAt(540f);
            if (potentialTarget != null && Projectile.timeLeft >= 480)
            {
                float flySpeed = Projectile.velocity.Length();
                if (flySpeed < 10f)
                    flySpeed = 10f;

                Projectile.Center = Projectile.Center.MoveTowards(potentialTarget.Center, 1.75f);
                Projectile.velocity = (Projectile.velocity * 14f + Projectile.SafeDirectionTo(potentialTarget.Center) * flySpeed) / 15f;
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * flySpeed;
            }

            if (Main.rand.NextBool(5))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 5, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }
    }
}
