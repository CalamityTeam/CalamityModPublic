using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class SevensStrikerPlatinumCoin : ModProjectile, ILocalizedModType
    {
        public string LocalizationCategory => "Projectiles.Ranged";
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.PlatinumCoin);
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.PlatinumCoin;
            Projectile.width = 10; // Cool gaming facts: the vanilla coin projectiles are literally 4 pixels in hitbox size
            Projectile.height = 10;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Midas, 25200); // 7 Minutes of Midas
        }
    }
}
