using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class InfernalKrisCinder : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Infernal Cinder");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.alpha = 0;
            projectile.penetrate = 3;
            projectile.tileCollide = true;
            projectile.timeLeft = 120;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.velocity.Y += 0.1f;
            projectile.rotation += 0.4f * projectile.direction;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);

            float minScale = 1.9f;
            float maxScale = 2.5f;
            int numDust = 2;
            for (int i = 0; i < numDust; i++)
            {
                Dust.NewDust(projectile.position, 4, 4, 6, projectile.velocity.X, projectile.velocity.Y, 0, default, Main.rand.NextFloat(minScale, maxScale));
            }

            projectile.Kill();
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 90);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 90);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            // If this is a stealth strike, make the blade glow orange
            Color glowColour = new Color(255, 215, 100, 100);
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, glowColour, projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);

            float minScale = 1.9f;
            float maxScale = 2.5f;
            int numDust = 2;
            for (int i = 0; i < numDust; i++)
            {
                int dust = Dust.NewDust(projectile.position, 4, 4, 6, 0f, -2f, 0, default, Main.rand.NextFloat(minScale, maxScale));
                Main.dust[dust].noGravity = true;
            }
            return false;
        }
    }
}
