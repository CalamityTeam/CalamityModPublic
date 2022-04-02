using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class DracoBeam2 : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/DracoBeam";

        private int start = 60;
        private int speedTimer = 120;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 14;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 50;
            projectile.aiStyle = 27;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 240;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            start--;
            if (start <= 0)
            {
                speedTimer--;
                if (speedTimer > 60)
                {
                    projectile.velocity.X = 0f;
                    projectile.velocity.Y = 10f;
                }
                else if (speedTimer <= 60)
                {
                    projectile.velocity.X = 0f;
                    projectile.velocity.Y = -10f;
                }
                if (speedTimer <= 0)
                {
                    speedTimer = 120;
                }
            }
            Lighting.AddLight(projectile.Center, 0.5f, 0.25f, 0f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft > 235)
                return false;

            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 244, 0f, 0f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Daybreak, 180);
        }
    }
}
