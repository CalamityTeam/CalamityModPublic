using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class SevensStrikerPlatinumCoin : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Platinum Coin");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.PlatinumCoin);
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.PlatinumCoin;
            Projectile.width = 10; // Cool gaming facts: the vanilla coin projectiles are literally 4 pixels in hitbox size
            Projectile.height = 10;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Midas, 25200); // 7 Minutes of Midas
        }
    }
}
