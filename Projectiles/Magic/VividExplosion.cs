using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class VividExplosion : ModProjectile
    {
        private const float radius = 204.5f;
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosion");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 1; //Uses custom collision, this field is irrelevant
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.Calamity().rogue = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 6;
            projectile.timeLeft = 5;
        }

        public override void AI()
        {

            Lighting.AddLight(projectile.Center, Main.DiscoR * 0.5f / 255f, Main.DiscoG * 0.5f / 255f, Main.DiscoB * 0.5f / 255f);

            float dustSpeed = Main.rand.NextFloat(12f, 35f);
            Vector2 dustVel = CalamityUtils.RandomVelocity(40f, dustSpeed, dustSpeed, 1f);
            int dustType = Utils.SelectRandom(Main.rand, new int[]
            {
                107,
                234,
                269
            });
            int rainbow = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 2f);
            Dust dust = Main.dust[rainbow];
            dust.noGravity = true;
            dust.position = projectile.Center;
            dust.position.X += (float)Main.rand.Next(-10, 11);
            dust.position.Y += (float)Main.rand.Next(-10, 11);
            dust.velocity = dustVel;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, radius, targetHitbox);

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.ExoDebuffs();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.ExoDebuffs();
        }
    }
}
