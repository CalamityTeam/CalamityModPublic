using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class MoonSigil : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Moon Sigil");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = 2;
            projectile.ignoreWater = true;
            projectile.timeLeft = 250;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.timeLeft < 52)
            {
                projectile.alpha += 5;
                projectile.scale -= 0.013f;
            }
            if (projectile.alpha >= 255)
            {
                projectile.alpha = 255;
                projectile.Kill();
                return;
            }

            CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 300f, 8f, 20f);
        }

        // Reduce damage of projectiles if more than the cap are active
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            int projectileCount = Main.player[projectile.owner].ownedProjectileCounts[projectile.type];
            int cap = 5;
            int oldDamage = damage;
            if (projectileCount > cap)
            {
                damage -= (int)(oldDamage * ((projectileCount - cap) * 0.05));
                if (damage < 1)
                    damage = 1;
            }
        }

        public override void Kill(int timeLeft)
        {
            float dustSp = 0.2f;
            int dustD = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Vector2 dustspeed = new Vector2(dustSp, dustSp).RotatedBy(MathHelper.ToRadians(dustD));
                    int d = Dust.NewDust(projectile.Center, projectile.width, projectile.height, 31, dustspeed.X, dustspeed.Y, 200, new Color(213, 242, 232, 200), 1f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].position = projectile.Center;
                    Main.dust[d].velocity = dustspeed;
                    dustSp += 0.2f;
                }
                dustD += 90;
                dustSp = 0.2f;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/MoonSigil");
            spriteBatch.Draw
            (
                texture,
                new Vector2
                (
                    projectile.position.X - Main.screenPosition.X + projectile.width * 0.5f,
                    projectile.position.Y - Main.screenPosition.Y + projectile.height - 20 * 0.5f
                ),
                new Rectangle(0, 0, 20, 20),
                Color.White,
                projectile.rotation,
                new Vector2(10, 10),
                projectile.scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}
