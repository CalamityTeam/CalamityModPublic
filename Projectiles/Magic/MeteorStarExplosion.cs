using CalamityMod.Items.Weapons.Magic;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class MeteorStarExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosion");
            Main.projFrames[projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            projectile.width = 84;
            projectile.height = 152;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = -1;
            projectile.timeLeft = Main.projFrames[projectile.type] * 5;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 35;
            projectile.tileCollide = false;
            projectile.hostile = true;
        }

        public override void AI()
        {
            if (projectile.timeLeft % 5f == 4f)
                projectile.frame++;
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            damage = Main.rand.Next(GloriousEnd.PlayerExplosionDmgMin, GloriousEnd.PlayerExplosionDmgMax + 1);
            if (Main.expertMode)
                damage /= 2;
            if (projectile.ai[0] == 1f)
                damage /= 2;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (target.townNPC)
            {
                damage = Main.rand.Next(GloriousEnd.PlayerExplosionDmgMin, GloriousEnd.PlayerExplosionDmgMax + 1);
                if (Main.expertMode)
                    damage /= 2;
                if (projectile.ai[0] == 1f)
                    damage /= 2;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.GiveIFrames(target.longInvince ? 100 : 60, true);
        }
    }
}
