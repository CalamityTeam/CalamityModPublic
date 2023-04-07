using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class InkCloud : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ink Cloud");
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.alpha = 0;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 100;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            if (Projectile.timeLeft < 50)
                Projectile.alpha += 5;
            if (Projectile.timeLeft < 75)
                Projectile.velocity *= 0.95f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.friendly)
            {
                target.AddBuff(BuffID.Confused, 300);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Confused, 300);
    }
}
