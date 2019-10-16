using CalamityMod.CalPlayer;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class AbyssalMirrorProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lumenyl Fluid");
            Main.projFrames[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.alpha = 0;
            projectile.penetrate = -1;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 50;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            Mod calamity = ModLoader.GetMod("CalamityMod");
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.timeLeft < 25)
            { projectile.alpha += 10; }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (!target.friendly && target.rarity != 2 && !CalamityPlayer.areThereAnyDamnBosses)
            {
                target.AddBuff(ModContent.BuffType<SilvaStun>(), 300);
            }
        }
    }
}
