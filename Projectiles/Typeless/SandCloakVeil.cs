using CalamityMod.DataStructures;
using CalamityMod.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Typeless
{
    public class SandCloakVeil : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        private const float radius = 272f;
        private const int duration = 900;

        public override void SetDefaults()
        {
            Projectile.width = 450;
            Projectile.height = 450;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = duration;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
			Projectile.scale = 1.2f;
        }

        public override void AI()
        {
            Projectile.rotation += 0.01f;

            Player player = Main.player[Main.myPlayer];
            Vector2 posDiff = player.Center - Projectile.Center;
            if (posDiff.Length() <= radius)
            {
                player.statDefense += 6;
                player.lifeRegen += 2;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Sprite Circle
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            float scaleStep = 0.03f;
            float rotationOffset = 0.03f;
            Color drawCol = Projectile.GetAlpha(lightColor);
            float drawTransparency = 0.1f;

            if (Projectile.timeLeft > duration - 10)
            {
                drawTransparency = (duration - Projectile.timeLeft) * 0.01f;
            }
            else if (Projectile.timeLeft < 25)
            {
                drawTransparency = Projectile.timeLeft * 0.004f;
            }

            // Dust effects
            Circle dustCircle = new Circle(Projectile.Center, radius);

            for (int i = 0; i < 20; i++)
            {
                // Sprite
                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, drawCol * drawTransparency, Projectile.rotation + (rotationOffset * i * i), tex.Size() / 2f, Projectile.scale - (i * scaleStep), SpriteEffects.None, 0);

                // Dust
                Vector2 dustPos = dustCircle.RandomPointInCircle();
                if ((dustPos - Projectile.Center).Length() > 48)
                {
                    int dustIndex = Dust.NewDust(dustPos, 1, 1, 32);
                    Main.dust[dustIndex].noGravity = true;
                    Main.dust[dustIndex].fadeIn = 1f;
                    Vector2 dustVelocity = Projectile.Center - Main.dust[dustIndex].position;
                    float distToCenter = dustVelocity.Length();
                    dustVelocity.Normalize();
                    dustVelocity = dustVelocity.RotatedBy(MathHelper.ToRadians(-90f));
                    dustVelocity *= distToCenter * 0.04f;
                    Main.dust[dustIndex].velocity = dustVelocity;
                }
            }

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Knockback has to be done manually to ensure the enemies are repelled from the aura as opposed to thrown to one side of it

            if (target.knockBackResist <= 0f)
                return;

            // 12AUG2023: Ozzatron: TML was giving NaN knockback, probably due to 0 base knockback. Do not use hit.Knockback
            if (CalamityGlobalNPC.ShouldAffectNPC(target))
            {
                float knockbackMultiplier = MathHelper.Clamp(1f - target.knockBackResist, 0f, 1f);
                Vector2 trueKnockback = target.Center - Projectile.Center;
                trueKnockback.Normalize();
                target.velocity = trueKnockback * knockbackMultiplier;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, radius, targetHitbox);
    }
}
