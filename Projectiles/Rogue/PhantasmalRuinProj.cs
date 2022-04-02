using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class PhantasmalRuinProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/PhantasmalRuin";

        private const int Lifetime = 600;
        private const int FramesPerSubProjectile = 13;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantasmal Ruin");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = Lifetime;
            projectile.extraUpdates = 1;
            projectile.Calamity().rogue = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 3);
            return false;
        }

        public override void AI()
        {
            // Set the projectile's direction correctly
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;

            // Dust and light
            Dust d = Dust.NewDustDirect(projectile.position + projectile.velocity, projectile.width, projectile.height, 175, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 0, default, 0.85f);
            d.noLight = true;
            Lighting.AddLight(projectile.Center + projectile.velocity * 0.1f, 0.4f, 0.7f, 0.9f);

            // Fire sub projectiles occasionally
            bool shouldFireSubProjectile = (Lifetime - projectile.timeLeft) % (projectile.MaxUpdates * FramesPerSubProjectile) == 8;
            if (projectile.owner == Main.myPlayer && shouldFireSubProjectile)
            {
                bool ss = projectile.Calamity().stealthStrike;
                int projID = ss ? ModContent.ProjectileType<PhantasmalRuinGhost>() : ModContent.ProjectileType<LostSoulFriendly>();
                int damage = (int)(projectile.damage * 0.25f);
                float kb = projectile.knockBack * (ss ? 1f : 0.25f);
                Vector2 velocity = ss
                    ? (projectile.velocity * 0.4f).RotatedBy(Main.rand.NextFloat(-0.04f, 0.04f))
                    : (projectile.velocity * 0.08f) + Main.rand.NextVector2Circular(0.4f, 0.4f);
                Projectile.NewProjectile(projectile.Center, velocity, projID, damage, kb, projectile.owner);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => OnHitEffects();
        public override void OnHitPvp(Player target, int damage, bool crit) => OnHitEffects();

        private void OnHitEffects()
        {
            if (projectile.owner != Main.myPlayer)
                return;

            int numSouls = 4;
            int projID = ModContent.ProjectileType<PhantasmalSoul>();
            int soulDamage = (int)(projectile.damage * 0.1f);
            float soulKB = 0f;
            float speed = 5f;
            float startAngle = Main.rand.NextFloat(-0.07f, 0.07f) + MathHelper.PiOver4;
            Vector2 velocity = (Vector2.UnitX * speed).RotatedBy(startAngle);
            for (int i = 0; i < numSouls; i += 2)
            {
                // Each pair of souls has randomized player homing strength
                float ai1 = Main.rand.NextFloat() + 0.5f;
                if (Main.rand.NextBool(2))
                    Projectile.NewProjectile(projectile.Center, velocity, projID, soulDamage, soulKB, projectile.owner, 0f, ai1);
                if (Main.rand.NextBool(2))
                    Projectile.NewProjectile(projectile.Center, -velocity, projID, soulDamage, soulKB, projectile.owner, 0f, ai1);

                // Rotate direction for the next pair of souls.
                velocity = velocity.RotatedBy(MathHelper.TwoPi / numSouls);
            }
        }
    }
}
