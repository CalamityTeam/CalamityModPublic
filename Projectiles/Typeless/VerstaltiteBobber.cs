using CalamityMod.Items.Fishing.FishingRods;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Typeless
{
    public class VerstaltiteBobber : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Verstaltite Bobber");
        }

        public override void SetDefaults()
        {
            //projectile.CloneDefaults(360); //Wooden Bobber
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = ProjAIStyleID.Bobber;
            Projectile.bobber = true;
            Projectile.penetrate = -1;
        }

        public override bool PreDrawExtras()
        {
            Lighting.AddLight(Projectile.Center, 0.4f, 0f, 0.4f);
            CalamityUtils.DrawFishingLine(Projectile, ModContent.ItemType<VerstaltiteFishingRod>(), new Color(95, 158, 160, 100));
            return false;
        }
    }
}
