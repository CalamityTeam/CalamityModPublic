using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class Shaderain : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/AuraRain";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shady Rain");
        }

        public override void SetDefaults()
        {
            Projectile.timeLeft = 600;
            
            Projectile.width = 20;
            Projectile.height = 20;

            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            // The projectile will fall.
            Projectile.velocity.Y += ShaderainStaff.GravityStrenght;
            
            // The projectile will look towards where it's going.
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            Projectile.netUpdate = true;
        }
    }
}
