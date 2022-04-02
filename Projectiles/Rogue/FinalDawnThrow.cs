using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class FinalDawnThrow : ModProjectile
    {
        public const float DesiredSpeed = 38;
        public const float InterpolationTime = 15;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Final Dawn");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 80;
            projectile.friendly = true;
            projectile.Calamity().rogue = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.light = 0.0f;
            projectile.extraUpdates = 2;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            if (player is null || player.dead)
                projectile.Kill();

            if (projectile.localAI[0] == 0)
            {
                Main.PlaySound(SoundID.Item71, projectile.Center);
                projectile.localAI[0] = 1;
            }

            projectile.spriteDirection = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation += 0.25f * projectile.direction;

            projectile.ai[0]++;
            if (projectile.ai[0] >= 30)
            {
                Vector2 desiredVelocity = projectile.SafeDirectionTo(player.Center) * DesiredSpeed;
                projectile.velocity = Vector2.Lerp(projectile.velocity, desiredVelocity, 1f / InterpolationTime);

                float distance = projectile.Distance(player.Center);
                if (distance < 64)
                    projectile.Kill();
            }

            int idx = Dust.NewDust(projectile.position, projectile.width , projectile.height, ModContent.DustType<FinalFlame>(), 0f, 0f, 0, default, 0.5f);
            Main.dust[idx].velocity *= 0.5f;
            Main.dust[idx].velocity += projectile.velocity * 0.5f;
            Main.dust[idx].noGravity = true;
            Main.dust[idx].noLight = false;
            Main.dust[idx].scale = 1.0f;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D scytheTexture = Main.projectileTexture[projectile.type];
            Texture2D scytheGlowTexture = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/FinalDawnThrow_Glow");
            int height = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int yStart = height * projectile.frame;
            Main.spriteBatch.Draw(scytheTexture,
                                  projectile.Center - Main.screenPosition + Vector2.UnitY * projectile.gfxOffY,
                                  new Rectangle?(new Rectangle(0, yStart, scytheTexture.Width, height)),
                                  projectile.GetAlpha(lightColor),
                                  projectile.rotation,
                                  new Vector2(scytheTexture.Width / 2f, height / 2f),
                                  projectile.scale,
                                  projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                                  0f);
            Main.spriteBatch.Draw(scytheGlowTexture,
                                  projectile.Center - Main.screenPosition + Vector2.UnitY * projectile.gfxOffY,
                                  new Rectangle?(new Rectangle(0, yStart, scytheTexture.Width, height)),
                                  projectile.GetAlpha(Color.White),
                                  projectile.rotation,
                                  new Vector2(scytheTexture.Width / 2f, height / 2f),
                                  projectile.scale,
                                  projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                                  0f);
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 240);
        }
    }
}
