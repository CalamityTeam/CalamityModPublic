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

        private const int FramesPerSubProjectile = 11;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantasmal Ruin");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 1;
            projectile.Calamity().rogue = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void AI()
        {
            // Set the projectile's direction correctly
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;

            // Spawn dust constantly
            Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 175, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 0, default, 0.85f);

            // Fire sub projectiles occasionally
            if (projectile.owner == Main.myPlayer && projectile.timeLeft % FramesPerSubProjectile * projectile.MaxUpdates == 0)
            {
                bool ss = projectile.Calamity().stealthStrike;
                int projID = ss ? ModContent.ProjectileType<PhantasmalRuinGhost>() : ModContent.ProjectileType<LostSoulFriendly>();
                int damage = (int)(projectile.damage * (ss ? 0.25f : 0.5f));
                float kb = projectile.knockBack * (ss ? 1f : 0.25f);
                Vector2 velocity = ss ? projectile.velocity * 0.25f : Main.rand.NextVector2Circular(2f, 2f);
                Projectile.NewProjectile(projectile.Center, velocity, projID, damage, kb, projectile.owner);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            OnHitEffects();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            OnHitEffects();
        }

        private void OnHitEffects()
        {
            if (projectile.owner != Main.myPlayer)
                return;

            int numSouls = 4;
            int projID = ModContent.ProjectileType<PhantasmalSoul>();
            int soulDamage = (int)(projectile.damage * 0.1f);
            float soulKB = 0f;
            float speed = 5f;
            Vector2 velocity = (Vector2.UnitX * speed).RotatedBy(MathHelper.PiOver4);
            for (int i = 0; i < numSouls; i += 2)
            {
                float ai1 = Main.rand.NextFloat() + 0.5f;
                Projectile.NewProjectile(projectile.Center, velocity, projID, soulDamage, soulKB, projectile.owner, 1f, ai1);
                Projectile.NewProjectile(projectile.Center, -velocity, projID, soulDamage, soulKB, projectile.owner, 1f, ai1);

                // Rotate direction for the next pair of souls.
                velocity = velocity.RotatedBy(MathHelper.TwoPi / numSouls);
            }
        }
    }
}
