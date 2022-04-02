using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class CrystalPiercerShard : ModProjectile
    {
        private bool initialized = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Shard");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 120;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.rotation += projectile.direction * 0.05f;

            if (!initialized)
            {
                projectile.scale = Main.rand.NextFloat(0.85f, 1.15f);
                initialized = true;
            }
        }

        //glowmask effect
        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 200);
    }
}
