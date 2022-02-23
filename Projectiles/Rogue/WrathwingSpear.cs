using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class WrathwingSpear : ModProjectile
    {
        private const float FireballAngleVariance = 0.07f;
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wrathwing");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 62;
            projectile.height = 62;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            // Spit fireballs constantly, but slightly randomly. Always spits one fireball immediately upon being thrown.
            if (projectile.owner == Main.myPlayer && projectile.ai[0] <= 0f)
            {
                projectile.ai[0] = Main.rand.NextFloat(15f, 19f);
                
                int fireballID = ModContent.ProjectileType<WrathwingFireball>();
                int damage = (int)(projectile.damage * 0.8f);
                float angleDiff = Main.rand.NextFloat(-FireballAngleVariance, FireballAngleVariance);
                Vector2 velocity = projectile.velocity.RotatedBy(angleDiff) * 1.06f;
                float kb = projectile.knockBack * 0.6f;
                Projectile.NewProjectile(projectile.Center, velocity, fireballID, damage, kb, projectile.owner);
            }

            projectile.ai[0] -= 1f;

            // Homing
            // The item's default velocity is 28. Homing speed is intentionally a bit lower.
            CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 450f, 23f, 30f);

            // Animation
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
                projectile.frame = 0;

            // Rotation
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }

        public override void Kill(int timeLeft)
        {
            // Stealth strikes create an eruption on hit.
            if (projectile.owner == Main.myPlayer && projectile.Calamity().stealthStrike)
            {
                int eruptionID = ModContent.ProjectileType<WrathwingCinder>();
                int damage = (int)(projectile.damage * 0.375f);
                float kb = 0f;

                // Spawns 13 erupting fireballs in total.
                for (int x = -6; x <= 6; x++)
                {
                    Vector2 pos = projectile.Center + Vector2.UnitY * Main.rand.NextFloat(44f, 60f);
                    pos.X += Main.rand.NextFloat(-14f, 14f);
                    float ySpeed = x % 2 == 0 ? -13f : -19f;
                    ySpeed *= Main.rand.NextFloat(0.85f, 1.05f);
                    Vector2 velocity = new Vector2(x, ySpeed);
                    Projectile.NewProjectile(pos, velocity, eruptionID, damage, kb, projectile.owner);
                }
            }

            // Spawn shrapnel dust. Code adapted from Holy Fire Bullets.
            for (int k = 0; k < 36; k++)
            {
                float scale = Main.rand.NextFloat(1.4f, 1.8f);
                Vector2 corner = projectile.Center - Vector2.One * 2f;
                int dustID = Dust.NewDust(corner, 4, 4, DustID.CopperCoin);
                Main.dust[dustID].noGravity = false;
                Main.dust[dustID].scale = scale;
                float angleDeviation = 0.25f;
                float angle = Main.rand.NextFloat(-angleDeviation, angleDeviation);
                float velMult = Main.rand.NextFloat(0.08f, 0.14f);
                Vector2 shrapnelVelocity = projectile.velocity.RotatedBy(angle) * velMult;
                Main.dust[dustID].velocity = shrapnelVelocity;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(BuffID.Daybreak, 300);
        public override void OnHitPvp(Player target, int damage, bool crit) => target.AddBuff(BuffID.Daybreak, 300);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
