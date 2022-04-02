using CalamityMod.DataStructures;
using CalamityMod.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Typeless
{
    public class SandCloakVeil : ModProjectile
    {
        private const float radius = 225f;
        private const int duration = 900;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dust Veil");
        }

        public override void SetDefaults()
        {
            projectile.width = 450;
            projectile.height = 450;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = duration;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            projectile.rotation += 0.01f;

            Player player = Main.player[Main.myPlayer];
            Vector2 posDiff = player.Center - projectile.Center;
            if (posDiff.Length() <= radius)
            {
                player.statDefense += 6;
                player.lifeRegen += 2;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            // Sprite Circle
            Texture2D tex = Main.projectileTexture[projectile.type];
            float scaleStep = 0.03f;
            float rotationOffset = 0.03f;
            Color drawCol = projectile.GetAlpha(lightColor);
            float drawTransparency = 0.1f;

            if (projectile.timeLeft > duration - 10)
            {
                drawTransparency = (duration - projectile.timeLeft) * 0.01f;
            }
            else if (projectile.timeLeft < 25)
            {
                drawTransparency = projectile.timeLeft * 0.004f;
            }

            // Dust effects
            Circle dustCircle = new Circle(projectile.Center, radius);

            for (int i = 0; i < 20; i++)
            {
                // Sprite
                spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, drawCol * drawTransparency, projectile.rotation + (rotationOffset * i * i), tex.Size() / 2f, projectile.scale - (i * scaleStep), SpriteEffects.None, 0f);

                // Dust
                Vector2 dustPos = dustCircle.RandomPointInCircle();
                if ((dustPos - projectile.Center).Length() > 48)
                {
                    int dustIndex = Dust.NewDust(dustPos, 1, 1, 32);
                    Main.dust[dustIndex].noGravity = true;
                    Main.dust[dustIndex].fadeIn = 1f;
                    Vector2 dustVelocity = projectile.Center - Main.dust[dustIndex].position;
                    float distToCenter = dustVelocity.Length();
                    dustVelocity.Normalize();
                    dustVelocity = dustVelocity.RotatedBy(MathHelper.ToRadians(-90f));
                    dustVelocity *= distToCenter * 0.04f;
                    Main.dust[dustIndex].velocity = dustVelocity;
                }
            }

            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Knockback has to be done manually to ensure the enemies are repelled from the aura as opposed to thrown to one side of it

            if (target.knockBackResist <= 0f)
                return;

            if (CalamityGlobalNPC.ShouldAffectNPC(target))
            {
                float knockbackMultiplier = knockback - (1f - target.knockBackResist);
                if (knockbackMultiplier < 0)
                {
                    knockbackMultiplier = 0;
                }
                Vector2 trueKnockback = target.Center - projectile.Center;
                trueKnockback.Normalize();
                target.velocity = trueKnockback * knockbackMultiplier;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, radius, targetHitbox);
    }
}
