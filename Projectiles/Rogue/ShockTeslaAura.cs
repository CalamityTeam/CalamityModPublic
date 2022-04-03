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
            Projectile.width = 218;
            Projectile.height = 218;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = lifetime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 3)
            {
                Projectile.localAI[0]++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.localAI[0] >= framesY)
            {
                Projectile.localAI[0] = 0;
                Projectile.localAI[1]++;
            }
            if (Projectile.localAI[1] >= framesX)
            {
                Projectile.localAI[1] = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D sprite = ModContent.Request<Texture2D>(Texture).Value;

            Color drawColour = Color.White;
            Rectangle sourceRect = new Rectangle(Projectile.width * (int)Projectile.localAI[1], Projectile.height * (int)Projectile.localAI[0], Projectile.width, Projectile.height);
            Vector2 origin = new Vector2(Projectile.width / 2, Projectile.height / 2);

            float opacity = 1f;
            int sparkCount = 0;
            int fadeTime = 20;

            if (Projectile.timeLeft < fadeTime)
            {
                opacity = Projectile.timeLeft * (1f / fadeTime);
                sparkCount = fadeTime - Projectile.timeLeft;
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

                int dust = Dust.NewDust(Projectile.Center + dustPos, 1, 1, dustType, 0, 0, 0, default, 0.75f);
                Main.dust[dust].noGravity = true;
            }

            Main.EntitySpriteDraw(sprite, Projectile.Center - Main.screenPosition, sourceRect, drawColour * opacity, Projectile.rotation, origin, 1f, SpriteEffects.None, 0);
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
                Vector2 trueKnockback = target.Center - Projectile.Center;
                trueKnockback.Normalize();
                target.velocity = trueKnockback * knockbackMultiplier;
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 180);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, radius, targetHitbox);
    }
}
