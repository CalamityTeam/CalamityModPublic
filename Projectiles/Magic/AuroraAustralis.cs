using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class AuroraAustralis : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private static float CosFrequency = 0.05f;
        private static float CosAmplitude = 0.008f;
        public int[] dustTypes = new int[]
        {
            ModContent.DustType<AstralBlue>(),
            ModContent.DustType<AstralOrange>()
        };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aurora Australis");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 300;
            projectile.magic = true;
            projectile.alpha = 255;

            CosFrequency = 0.15f;
            CosAmplitude = 0.06f;
        }

        public override void AI()
        {
            // On-spawn effects
            if (projectile.ai[0] == 0)
            {
                // Store the X and Y of the spawn velocity so it can be used for trig calculations
                projectile.localAI[0] = projectile.velocity.X;
                projectile.localAI[1] = projectile.velocity.Y;
            }

            // Doesn't collide with tiles for the first 2 frames
            projectile.tileCollide = projectile.ai[0] > 2f;

            // Apply fancy cosine movement, then slight gravity, then cap velocity.
            // Original velocity is reconstructed so that it can be used in the calculation
            Vector2 originalVelocity = new Vector2(projectile.localAI[0], projectile.localAI[1]);
            ApplyCosVelocity(originalVelocity);
            float currentSpeed = projectile.velocity.Length();
            float maxSpeed = 1.4f * originalVelocity.Length();
            if (currentSpeed > maxSpeed)
                projectile.velocity *= maxSpeed / currentSpeed;

            // spawn dust
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, Main.rand.Next(dustTypes), projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            int rainbow = Dust.NewDust(projectile.Center, projectile.width, projectile.height, 66, 0f, 0f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
            Dust dust = Main.dust[rainbow];
            dust.velocity *= 0.1f;
            dust.velocity += projectile.velocity * 0.2f;
            dust.position.X = projectile.Center.X + 4f + Main.rand.Next(-2, 3);
            dust.position.Y = projectile.Center.Y + Main.rand.Next(-2, 3);
            dust.noGravity = true;

            if (projectile.timeLeft % 10 == 0 && Main.myPlayer == projectile.owner) //spawn stars every 10 ticks
            {
                if (Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<AstralStarMagic>()] < 30)
                {
                    float dmgKBMult = Main.rand.NextFloat(0.25f, 0.75f);
                    Projectile star = CalamityUtils.ProjectileRain(projectile.Center, projectile.velocity.X, 100f, 500f, 800f, Main.rand.NextFloat(10f, 20f), ModContent.ProjectileType<AstralStarMagic>(), (int)(projectile.damage * dmgKBMult), projectile.knockBack * dmgKBMult, projectile.owner);
                    star.timeLeft = 120;
                }
            }

            projectile.ai[0]++;
        }

        private void ApplyCosVelocity(Vector2 baseVelocity)
        {
            float radians = -(-MathHelper.PiOver2 + CosFrequency * projectile.ai[0]);
            projectile.velocity += CosAmplitude * baseVelocity.RotatedBy(radians);
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i <= 5; i++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, Main.rand.Next(dustTypes), projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);
        }
    }
}
