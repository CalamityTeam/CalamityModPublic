using CalamityMod.Items.Weapons.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class MeteorStarExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetStaticDefaults()
        {
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
            int intendedDamage = Main.rand.Next(GloriousEnd.PlayerExplosionDmgMin, GloriousEnd.PlayerExplosionDmgMax + 1);

            modifiers.SourceDamage *= 0f;
            modifiers.SourceDamage.Flat += intendedDamage * (Main.masterMode ? 2.4f : Main.expertMode ? 1.6f : 1f);
            if (Projectile.ai[0] == 1f)
                modifiers.SourceDamage.Flat /= 2;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.townNPC)
            {
                int intendedDamage = Main.rand.Next(GloriousEnd.PlayerExplosionDmgMin, GloriousEnd.PlayerExplosionDmgMax + 1);

                modifiers.SourceDamage *= 0f;
                modifiers.SourceDamage.Flat += intendedDamage * (Main.masterMode ? 2.4f : Main.expertMode ? 1.6f : 1f);
                if (Projectile.ai[0] == 1f)
                    modifiers.SourceDamage.Flat /= 2;
            }
        }
    }
}
