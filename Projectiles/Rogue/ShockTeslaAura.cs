using CalamityMod.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class ShockTeslaAura : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Typeless/TeslaAura";

        private const float radius = 98f;
        private const int lifetime = 240;
        private const int framesX = 3;
        private const int framesY = 6;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electrifying Aura");
        }

        public override void SetDefaults()
        {
            projectile.width = 218;
            projectile.height = 218;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = lifetime;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 20;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 3)
            {
                projectile.localAI[0]++;
                projectile.frameCounter = 0;
            }
            if (projectile.localAI[0] >= framesY)
            {
                projectile.localAI[0] = 0;
                projectile.localAI[1]++;
            }
            if (projectile.localAI[1] >= framesX)
            {
                projectile.localAI[1] = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D sprite = Main.projectileTexture[projectile.type];

            Color drawColour = Color.White;
            Rectangle sourceRect = new Rectangle(projectile.width * (int)projectile.localAI[1], projectile.height * (int)projectile.localAI[0], projectile.width, projectile.height);
            Vector2 origin = new Vector2(projectile.width / 2, projectile.height / 2);

            float opacity = 1f;
            int sparkCount = 0;
            int fadeTime = 20;

            if (projectile.timeLeft < fadeTime)
            {
                opacity = projectile.timeLeft * (1f / fadeTime);
                sparkCount = fadeTime - projectile.timeLeft;
            }

            for (int i = 0; i < sparkCount * 2; i++)
            {
                int dustType = 132;
                if (Main.rand.NextBool())
                {
                    dustType = 264;
                }
                float rangeDiff = 2f;

                Vector2 dustPos = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                dustPos.Normalize();
                dustPos *= radius + Main.rand.NextFloat(-rangeDiff, rangeDiff);

                int dust = Dust.NewDust(projectile.Center + dustPos, 1, 1, dustType, 0, 0, 0, default, 0.75f);
                Main.dust[dust].noGravity = true;
            }

            spriteBatch.Draw(sprite, projectile.Center - Main.screenPosition, sourceRect, drawColour * opacity, projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 180);

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

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 180);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, radius, targetHitbox);
    }
}
