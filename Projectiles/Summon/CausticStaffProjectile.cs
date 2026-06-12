using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class CausticStaffProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public ref float MinionCount => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.MaxUpdates = 3;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 45 * Projectile.MaxUpdates;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
        }
        public override void AI()
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, GetDust(MinionCount), 0f, 0f, 0, default, 0.5f);
            dust.velocity *= 0.1f;
            dust.scale = 1.3f;
            dust.noGravity = true;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, GetDust(MinionCount));
                dust.noGravity = true;
                dust.velocity = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2f);
            }
        }

        public static int GetDust(float type)
        {
            if (type >= 4f && Main.rand.NextBool(5))
                return DustID.IchorTorch;
            if (type >= 3f && Main.rand.NextBool(4))
                return DustID.VenomStaff;
            if (type >= 2f && Main.rand.NextBool(3))
                return DustID.CursedTorch;
            return DustID.Torch;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 30);

            if (MinionCount >= 2f)
                target.AddBuff(BuffID.CursedInferno, 30);
            if (MinionCount >= 3f)
                target.AddBuff(BuffID.Venom, 30);
            if (MinionCount >= 4f)
            {
                target.AddBuff(BuffID.Ichor, 30);
                target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 30);
            }
        }
    }
}
