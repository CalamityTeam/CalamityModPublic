using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class FinalDawnFireball : ModProjectile
    {
        public const float DesiredSpeed = 30;
        public const float InterpolationTime = 10;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Final Dawn");
            Main.projFrames[projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            projectile.width = 36;
            projectile.height = 80;
            projectile.Calamity().rogue = true;
            projectile.penetrate = 1;
            projectile.hostile = false;
            projectile.friendly = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.alpha = 50;
            projectile.timeLeft = 180;
        }
        public override void AI()
        {
            NPC chargeAt = Main.npc[(int)projectile.ai[1]];
            if(!chargeAt.active)
                projectile.Kill();

            int idx = Dust.NewDust(projectile.position, projectile.width , projectile.height, ModContent.DustType<FinalFlame>(), 0f, 0f, 0, default, 1.0f);
            Main.dust[idx].velocity = projectile.velocity * 0.5f;
            Main.dust[idx].noGravity = true;
            Main.dust[idx].noLight = true;

            projectile.ai[0]++;
            if (projectile.ai[0] >= 20)
            {
                projectile.friendly = true;
                NPC npc = Main.npc[(int)projectile.ai[1]];
                Vector2 desiredVelocity = projectile.SafeDirectionTo(npc.Center) * DesiredSpeed;
                projectile.velocity = Vector2.Lerp(projectile.velocity, desiredVelocity, 1f / InterpolationTime);
                if(!npc.active)
                    projectile.Kill();
            }

            projectile.frameCounter++;
            if (projectile.frameCounter >= 5)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
                if (projectile.frame >= Main.projFrames[projectile.type])
                    projectile.frame = 0;
            }
        }
        public override void Kill(int timeLeft)
        {
            if (Main.myPlayer == projectile.owner)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<FinalDawnReticle>(), projectile.damage, projectile.knockBack, projectile.owner);
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D glowmask = Main.projectileTexture[projectile.type];
            int height = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int yStart = height * projectile.frame;
            Main.spriteBatch.Draw(glowmask,
                                  projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY),
                                  new Rectangle?(new Rectangle(0, yStart, glowmask.Width, height)),
                                  projectile.GetAlpha(Color.White), projectile.rotation,
                                  new Vector2(glowmask.Width / 2f, height / 2f), projectile.scale,
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
