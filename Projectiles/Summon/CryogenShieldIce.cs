using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class CryogenShieldIce : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Icicle");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.width = 28;
            projectile.height = 60;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.ignoreWater = false;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            projectile.ai[1] += MathHelper.ToRadians(2f);
            projectile.rotation = projectile.ai[1] + MathHelper.PiOver2;

            projectile.Center = player.Center + projectile.ai[1].ToRotationVector2() * 230f;
        }
    }
}
