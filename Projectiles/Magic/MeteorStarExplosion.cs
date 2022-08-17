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
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 84;
            Projectile.height = 152;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Main.projFrames[Projectile.type] * 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 35;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
        }

        public override void AI()
        {
            if (Projectile.timeLeft % 5f == 4f)
                Projectile.frame++;
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            damage = Main.rand.Next(GloriousEnd.PlayerExplosionDmgMin, GloriousEnd.PlayerExplosionDmgMax + 1);
            if (Projectile.ai[0] == 1f)
                damage /= 2;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (target.townNPC)
            {
                damage = Main.rand.Next(GloriousEnd.PlayerExplosionDmgMin, GloriousEnd.PlayerExplosionDmgMax + 1);
                if (Projectile.ai[0] == 1f)
                    damage /= 2;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.GiveIFrames(target.longInvince ? 100 : 60, true);
        }
    }
}
