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
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.aiStyle = ProjAIStyleID.RainCloud;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
            Projectile.scale = 1.1f;
            Projectile.minion = true;
            AIType = ProjectileID.RainFriendly;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            int blood = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 5, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 1f);
            Dust dust = Main.dust[blood];
            dust.velocity = Vector2.Zero;
            dust.position -= Projectile.velocity / 5f;
            dust.noGravity = true;
            dust.scale = 2f;
            dust.noLight = true;
        }
    }
}
