using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SludgeSplotchProj2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sludge Splotch");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.friendly = true;
            projectile.penetrate = 2;
            projectile.timeLeft = 150;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.velocity.Y += 0.1f;
            if (projectile.velocity.Y >= 16f)
            {
                projectile.velocity.Y = 16f;
            }

            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

            int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 191, 0f, 0f, 225, new Color(255, 255, 255), 2);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].noLight = true;
            Main.dust[dust].velocity = Main.dust[dust].velocity * 0.25f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Dig, projectile.position);
            Main.PlaySound(SoundID.NPCKilled, (int)projectile.position.X, (int)projectile.position.Y, 28, 0.3f, 0);
            projectile.Kill();
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Slow, 60);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Slow, 60);
        }

        public override void Kill(int timeLeft)
        {
            int numDust = 10;
            int dustType = 191;
            float spread = 2f;
            for (int i = 0; i < numDust; i++)
            {
                Vector2 velocity = projectile.velocity + new Vector2(Main.rand.NextFloat(-spread, spread), Main.rand.NextFloat(-spread, spread));

                int dust = Dust.NewDust(projectile.Center, 1, 1, dustType, velocity.X, velocity.Y, 175, default, 2f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
