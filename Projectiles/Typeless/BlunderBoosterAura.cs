using CalamityMod.NPCs;
using CalamityMod.NPCs.AcidRain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class BlunderBoosterAura : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        private const float radius = 98f;
        private const int framesX = 3;
        private const int framesY = 6;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 218;
            Projectile.height = 218;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 0f;
            Projectile.timeLeft = 18000;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.timeLeft *= 5;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
        }

        public override void AI()
        {
            //Protect against other mod projectile reflection like emode Granite Golems
            Projectile.friendly = true;
            Projectile.hostile = false;

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
            Player player = Main.player[Projectile.owner];
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.15f / 255f, (255 - Projectile.alpha) * 0.15f / 255f, (255 - Projectile.alpha) * 0.01f / 255f);
            Projectile.Center = player.Center;
            if (player is null || player.dead)
            {
                player.Calamity().blunderBooster = false;
                Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 180);

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

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Electrified, 180);

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

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, radius, targetHitbox);

        public override bool? CanHitNPC(NPC target)
        {
            if (target.catchItem != 0 && target.type != ModContent.NPCType<Radiator>())
            {
                return false;
            }
            return null;
        }
    }
}
