using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class EclipsesFallMain : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/EclipsesFall";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eclipse's Fall");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.extraUpdates = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 138, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            SpawnSpears(target.Center);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            SpawnSpears(target.Center);
        }

        private void SpawnSpears(Vector2 targetPos)
        {
            int spearAmt = Main.rand.Next(3, 6); //3 to 5 spears
            var source = Projectile.GetProjectileSource_FromThis();
            for (int n = 0; n < spearAmt; n++)
            {
                float dmgMult = 0.08f * Main.rand.NextFloat(4f, 7f);
                float kBMult = 0.1f * Main.rand.NextFloat(7f, 10f);
                CalamityUtils.ProjectileRain(source, targetPos, 400f, 100f, 500f, 800f, 29f, ModContent.ProjectileType<EclipsesSmol>(), (int)(Projectile.damage * dmgMult), Projectile.knockBack * kBMult, Projectile.owner);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
