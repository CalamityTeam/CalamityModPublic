using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Projectiles.Rogue
{
    public class AbyssalMirrorProjectile3 : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lumenyl Fluid");
		}

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.alpha = 0;
            projectile.penetrate = -1;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 50;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 2;
            Mod calamity = ModLoader.GetMod("CalamityMod");
            projectile.GetGlobalProjectile<CalamityGlobalProjectile>(calamity).rogue = true;
        }

        public override void AI()
        {
            if (projectile.timeLeft < 25) { projectile.alpha += 10; }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (!target.friendly && target.rarity != 2 && !CalamityPlayer.areThereAnyDamnBosses)
            {
                target.AddBuff(mod.BuffType("SilvaStun"), 300);
            }
        }
    }
}
