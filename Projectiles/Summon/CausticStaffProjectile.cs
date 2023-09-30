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
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 360;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
        }
        public override void AI()
        {
            int fire = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 0, default, 0.5f);
            Dust dust = Main.dust[fire];
            dust.velocity *= 0.1f;
            dust.scale = 1.3f;
            dust.noGravity = true;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.velocity.X *= 0.99f;
            if (Projectile.velocity.Y < 9f)
                Projectile.velocity.Y += 0.085f;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, 6);
                dust.noGravity = true;
                dust.velocity = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2f);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if ((player.ActiveItem().CountsAsClass<SummonDamageClass>() &&
                !player.ActiveItem().CountsAsClass<MeleeDamageClass>() &&
                !player.ActiveItem().CountsAsClass<RangedDamageClass>() &&
                !player.ActiveItem().CountsAsClass<MagicDamageClass>() &&
                !player.ActiveItem().CountsAsClass<ThrowingDamageClass>()) ||
                player.ActiveItem().hammer > 0 ||
                player.ActiveItem().pick > 0 ||
                player.ActiveItem().axe > 0)
            {
                int duration = Main.rand.Next(60, 181); // Anywhere between 1 and 3 seconds
                switch ((int)Projectile.ai[0])
                {
                    case 0:
                        if (target.Calamity().marked <= 0)
                            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), duration);
                        break;
                    case 1:
                        if (!target.ichor)
                            target.AddBuff(BuffID.Ichor, duration);
                        break;
                    case 2:
                        if (!target.venom)
                            target.AddBuff(BuffID.Venom, duration);
                        break;
                    case 3:
                        if (!target.onFire2)
                            target.AddBuff(BuffID.CursedInferno, duration);
                        break;
                    case 4:
                        if (!target.onFire3)
                            target.AddBuff(BuffID.OnFire3, duration);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
