using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class BarrelShrapnel : ModProjectile
    {
        public bool hitTile = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shrapnel");
            Main.projFrames[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 160;
            projectile.tileCollide = true;
            projectile.Calamity().rogue = true;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            projectile.ai[0] += 1f;
            if (hitTile)
            {
                projectile.velocity.X = 0f;
                projectile.rotation = MathHelper.Pi;
            }
            else
            {
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            projectile.velocity.Y += 0.2f;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            hitTile = true;
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 60 * 4);
            projectile.Kill();
        }
        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 60 * 4);
            projectile.Kill();
        }
    }
}
