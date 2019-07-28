using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class UrchinSpike : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Urchin Spike");
		}

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.aiStyle = 1;
        }

        public override void AI()
        {
        	projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(BuffID.Poisoned, 180);
        }
    }
}
