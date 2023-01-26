using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace CalamityMod.Projectiles.Turret
{
    public class WaterShotBuffer : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Water Shot");
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 1;
        }


        public override void AI()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                var source = Main.player[Main.myPlayer].GetSource_FromThis();
                Projectile.NewProjectile(source, Projectile.Center + new Vector2(3f, 0f), Projectile.velocity, ModContent.ProjectileType<WaterShot>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                Projectile.Kill();
            }
        }

        public override bool? CanDamage() => false;
    }
}
