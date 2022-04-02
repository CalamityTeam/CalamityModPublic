using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Rogue
{
    public class ProfanedPartisanSpear : ModProjectile
    {
        public int timer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Profaned Spear");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.Calamity().rogue = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 3;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.timeLeft = 600;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.ai[1] != 1f)
            {
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
                projectile.ai[1] = 1f;
                projectile.ai[0] = 1f;
                projectile.extraUpdates = 2;
                if (projectile.timeLeft > 280)
                    projectile.timeLeft = 280;
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
            OnHitEffects();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
            OnHitEffects();
        }

        private void OnHitEffects()
        {
            if (projectile.ai[1] != 1f)
            {
                projectile.velocity.X *= -1f;
                projectile.velocity.Y *= -1f;
                projectile.ai[1] = 1f;
                projectile.ai[0] = 1f;
                projectile.extraUpdates = 2;
                if (projectile.timeLeft > 280)
                    projectile.timeLeft = 280;
            }
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (projectile.ai[0] == 1f)
                timer++;
            if (timer >= 5)
                projectile.penetrate = 1;
            if (timer >= 10)
            {
                CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 300f, 7f, 20f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
