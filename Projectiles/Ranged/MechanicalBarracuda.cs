using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class MechanicalBarracuda : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Barracuda");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.aiStyle = ProjAIStyleID.MechanicalPiranha;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Ranged;
        }
    }
}
