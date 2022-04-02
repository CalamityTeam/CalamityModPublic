using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class PenumbraSoul : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Penumbra Soul");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.height = 18;
            projectile.width = 18;
            projectile.timeLeft = 150;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.Calamity().rogue = true;
            projectile.alpha = 80;

            projectile.penetrate = 2;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            drawOffsetX = 1;
            drawOriginOffsetY = 4;

            // Continuously trail dust
            int trailDust = 1;
            for (int i = 0; i < trailDust; ++i)
            {
                int dustID = DustID.Shadowflame;

                int idx = Dust.NewDust(projectile.position - projectile.velocity, projectile.width, projectile.height, dustID,0f,0f, 0, new Color(38, 30, 43));
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity += projectile.velocity * 0.8f;
            }

            // If tentacle is currently on cooldown, reduce the cooldown.
            if (projectile.ai[0] > 0f)
                projectile.ai[0] -= 1f;

            // Home in on nearby enemies if homing is enabled
            if (projectile.ai[1] == 0f)
                HomingAI();
        }

        private void HomingAI()
        {
            CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 200f, Penumbra.ShootSpeed * 1.5f, 35f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Rapidly screech to a halt upon touching an enemy and disable homing.
            projectile.velocity *= 0.4f;
            projectile.ai[1] = 1f;

            // Fade out a bit with every hit
            projectile.alpha += 20;
            if (projectile.alpha > 255)
                projectile.alpha = 255;

            // Explode into dust (as if being shredded apart on contact)
            int onHitDust = Main.rand.Next(6, 11);
            for (int i = 0; i < onHitDust; ++i)
            {
                int dustID = DustID.Shadowflame;
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustID, 0f, 0f,0, new Color(38, 30, 43));

                Main.dust[idx].noGravity = true;
                float speed = Main.rand.NextFloat(1.4f, 2.6f);
                Main.dust[idx].velocity *= speed;
                float scale = Main.rand.NextFloat(1.0f, 1.8f);
                Main.dust[idx].scale = scale;
            }
        }

        public override void Kill(int timeLeft)
        {
            // Create a burst of dust
            int killDust = Main.rand.Next(30, 41);
            for (int i = 0; i < killDust; ++i)
            {
                int dustID = DustID.Shadowflame;
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustID, 0f, 0f, 0, new Color(38, 30, 43));

                Main.dust[idx].noGravity = true;
                float speed = Main.rand.NextFloat(2.0f, 3.1f);
                Main.dust[idx].velocity *= speed;
                float scale = Main.rand.NextFloat(1.0f, 1.8f);
                Main.dust[idx].scale = scale;
            }
        }
    }
}
