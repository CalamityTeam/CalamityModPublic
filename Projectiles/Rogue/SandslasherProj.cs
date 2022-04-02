using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SandslasherProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Sandslasher";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sandslasher");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 3;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 16;
            projectile.Calamity().rogue = true;
            projectile.timeLeft = 600;
        }

        public override void AI()
        {
            projectile.ai[0] += 1f;
            projectile.ai[1] += 1f;
            if (projectile.ai[0] == 3f)
                projectile.tileCollide = true;
            if(projectile.velocity.X < 0f)
            {
                projectile.velocity.X -= 0.07f;
                if ((projectile.ai[0] %= 30f) == 0f)
                    projectile.damage -= (int)(projectile.velocity.X * 2f);
            }
            else if(projectile.velocity.X > 0f)
            {
                projectile.velocity.X += 0.07f;
                if ((projectile.ai[0] %= 30f) == 0f)
                    projectile.damage += (int)(projectile.velocity.X * 2f);
            }
            projectile.rotation += 0.1f * projectile.direction + (projectile.velocity.X /85);
            if(projectile.Calamity().stealthStrike && projectile.ai[1] >= 5f)
            {
                Vector2 speed = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
                Projectile.NewProjectile(projectile.position, speed, ModContent.ProjectileType<DuststormCloud>(), (int)(projectile.damage * 0.4), 0f, projectile.owner);
                projectile.ai[1] = 0;
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 85, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 0, default, 1f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
