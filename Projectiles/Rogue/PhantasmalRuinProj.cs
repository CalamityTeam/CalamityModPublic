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
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = Lifetime;
            Projectile.extraUpdates = 1;
            Projectile.Calamity().rogue = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 3);
            return false;
        }

        public override void AI()
        {
            // Set the projectile's direction correctly
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            // Dust and light
            Dust d = Dust.NewDustDirect(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 175, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 0, default, 0.85f);
            d.noLight = true;
            Lighting.AddLight(Projectile.Center + Projectile.velocity * 0.1f, 0.4f, 0.7f, 0.9f);

            // Fire sub projectiles occasionally
            bool shouldFireSubProjectile = (Lifetime - Projectile.timeLeft) % (Projectile.MaxUpdates * FramesPerSubProjectile) == 8;
            if (Projectile.owner == Main.myPlayer && shouldFireSubProjectile)
            {
                bool ss = Projectile.Calamity().stealthStrike;
                int projID = ss ? ModContent.ProjectileType<PhantasmalRuinGhost>() : ModContent.ProjectileType<LostSoulFriendly>();
                int damage = (int)(Projectile.damage * 0.25f);
                float kb = Projectile.knockBack * (ss ? 1f : 0.25f);
                Vector2 velocity = ss
                    ? (Projectile.velocity * 0.4f).RotatedBy(Main.rand.NextFloat(-0.04f, 0.04f))
                    : (Projectile.velocity * 0.08f) + Main.rand.NextVector2Circular(0.4f, 0.4f);
                Projectile.NewProjectile(Projectile.Center, velocity, projID, damage, kb, Projectile.owner);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => OnHitEffects();
        public override void OnHitPvp(Player target, int damage, bool crit) => OnHitEffects();

        private void OnHitEffects()
        {
            if (Projectile.owner != Main.myPlayer)
                return;

            int numSouls = 4;
            int projID = ModContent.ProjectileType<PhantasmalSoul>();
            int soulDamage = (int)(Projectile.damage * 0.1f);
            float soulKB = 0f;
            float speed = 5f;
            float startAngle = Main.rand.NextFloat(-0.07f, 0.07f) + MathHelper.PiOver4;
            Vector2 velocity = (Vector2.UnitX * speed).RotatedBy(startAngle);
            for (int i = 0; i < numSouls; i += 2)
            {
                // Each pair of souls has randomized player homing strength
                float ai1 = Main.rand.NextFloat() + 0.5f;
                if (Main.rand.NextBool(2))
                    Projectile.NewProjectile(Projectile.Center, velocity, projID, soulDamage, soulKB, Projectile.owner, 0f, ai1);
                if (Main.rand.NextBool(2))
                    Projectile.NewProjectile(Projectile.Center, -velocity, projID, soulDamage, soulKB, Projectile.owner, 0f, ai1);

                // Rotate direction for the next pair of souls.
                velocity = velocity.RotatedBy(MathHelper.TwoPi / numSouls);
            }
        }
    }
}
