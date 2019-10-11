using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class InkCloud3 : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ink Cloud");
		}

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.alpha = 0;
            projectile.penetrate = -1;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 100;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            Mod calamity = ModLoader.GetMod("CalamityMod");
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.timeLeft < 50)
                projectile.alpha += 5;
            if (projectile.timeLeft < 75)
                projectile.velocity *= 0.95f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (!target.friendly)
            {
                target.AddBuff(BuffID.Confused, 300);
            }
        }
    }
}
