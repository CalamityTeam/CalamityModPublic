using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class DragonRageFireball : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Rogue/DragonShit";
        public NPC target;
        private int lifeTime = 420;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fire");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 64;
            projectile.height = 66;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.timeLeft = lifeTime;
            projectile.melee = true;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0f)
            {
                projectile.ai[0] = Main.rand.NextFloat(40f, 70f);
                projectile.ai[1] = Main.rand.NextFloat(35f, 55f);
            }
            target = projectile.Center.ClosestNPCAt(1200f);
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }

            projectile.velocity *= 1.01f;

            if (target != null && projectile.timeLeft < lifeTime - 10)
            {
                float inertia = projectile.ai[0];
                float speed = projectile.ai[1];
                Vector2 moveDirection = projectile.SafeDirectionTo(target.Center, Vector2.UnitY);
                projectile.velocity = (projectile.velocity * (inertia - 1f) + moveDirection * speed) / inertia;
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, projectile.alpha);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D projectileTexture = Main.projectileTexture[projectile.type];
            int frameHeight = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int frameY = frameHeight * projectile.frame;
            Main.spriteBatch.Draw(projectileTexture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameY, projectileTexture.Width, frameHeight)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)projectileTexture.Width / 2f, (float)frameHeight / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.Center);
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 80);
            for (int d = 0; d < 5; d++)
            {
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 1f);
                Main.dust[idx].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[idx].scale = 0.5f;
                    Main.dust[idx].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 8; d++)
            {
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 5f;
                idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 1f);
                Main.dust[idx].velocity *= 2f;
            }
            CalamityUtils.ExplosionGores(projectile.Center, 3);
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Daybreak, 120);
        }
    }
}
