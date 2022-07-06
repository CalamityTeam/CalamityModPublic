using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Summon
{
    public class SarosSunfire : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Saros Sunfire");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.light = 1f;
            Projectile.timeLeft = 300;
            Projectile.minion = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Dust dust = Dust.NewDustPerfect(Projectile.Center, (int)CalamityDusts.ProfanedFire);
            dust.velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi);
            dust.noGravity = true;
            NPC potentialTarget = Projectile.Center.MinionHoming(1750f, player);
            if (potentialTarget != null)
                Projectile.velocity = (Projectile.velocity * 19f + Projectile.SafeDirectionTo(potentialTarget.Center) * 20f) / 20f;
        }

        public override void Kill(int timeLeft)
        {
            Projectile.ExpandHitboxBy(60);
            Projectile.maxPenetrate = Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.damage /= 3;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, (int)CalamityDusts.ProfanedFire);
                dust.velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2f, 6f);
                dust.noGravity = true;
            }
        }
    }
}
