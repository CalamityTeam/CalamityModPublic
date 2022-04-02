using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class SoulPiercerBolt : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Piercer");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.extraUpdates = 100;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 180;
            projectile.penetrate = -1;
            projectile.magic = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Vector2 vector33 = projectile.position;
            vector33 -= projectile.velocity;
            int num448 = Dust.NewDust(vector33, 1, 1, 173, 0f, 0f, 0, default, 0.5f);
            Main.dust[num448].position = vector33;
            Main.dust[num448].scale = Main.rand.Next(70, 110) * 0.014f;
            Main.dust[num448].velocity *= 0.2f;
        }
    }
}
