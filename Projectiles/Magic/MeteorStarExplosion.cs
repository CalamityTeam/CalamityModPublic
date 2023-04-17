using CalamityMod.Items.Weapons.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class MeteorStarExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Explosion");
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
            CooldownSlot = ImmunityCooldownID.TileContactDamage;
        }

        public override void AI()
        {
            if (Projectile.timeLeft % 5f == 4f)
                Projectile.frame++;
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            // Surely there is a more intuitive way to do this, right...?
            int intendedDamage = Main.rand.Next(GloriousEnd.PlayerExplosionDmgMin, GloriousEnd.PlayerExplosionDmgMax + 1);
            modifiers.SourceDamage *= intendedDamage / (float)Projectile.damage;
            if (Projectile.ai[0] == 1f)
                modifiers.SourceDamage /= 2;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.townNPC)
            {
                int intendedDamage = Main.rand.Next(GloriousEnd.PlayerExplosionDmgMin, GloriousEnd.PlayerExplosionDmgMax + 1);
                modifiers.SourceDamage *= intendedDamage / (float)Projectile.damage;
                if (Projectile.ai[0] == 1f)
                    modifiers.SourceDamage /= 2;
            }
        }
    }
}
