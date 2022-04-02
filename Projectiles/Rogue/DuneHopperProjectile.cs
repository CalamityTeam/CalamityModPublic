using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class DuneHopperProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/DuneHopper";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scourge");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.penetrate = 6;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 20f)
            {
                if (projectile.velocity.Y < 24f)
                {
                    projectile.velocity.Y += 0.4f;
                }
            }
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.penetrate--;
            if (projectile.penetrate <= 0)
            {
                projectile.Kill();
            }
            else
            {
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, projectile.Calamity().stealthStrike ? 4 : 2);
            return false;
        }
    }
}
