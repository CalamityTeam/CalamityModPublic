using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class BloodRain : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Rain");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.aiStyle = 45;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
            projectile.alpha = 255;
            projectile.scale = 1.1f;
            projectile.minion = true;
            aiType = ProjectileID.RainFriendly;
        }

        public override void AI()
        {
			int blood = Dust.NewDust(projectile.position, projectile.width, projectile.height, 5, projectile.velocity.X, projectile.velocity.Y, 100, default, 1f);
            Dust dust = Main.dust[blood];
            dust.velocity = Vector2.Zero;
            dust.position -= projectile.velocity / 5f;
            dust.noGravity = true;
            dust.scale = 2f;
            dust.noLight = true;
        }
    }
}
