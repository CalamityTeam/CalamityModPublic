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
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.direction * 0.05f;

            if (!initialized)
            {
                Projectile.scale = Main.rand.NextFloat(0.85f, 1.15f);
                initialized = true;
            }
        }

        //glowmask effect
        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 200);
    }
}
