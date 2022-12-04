using CalamityMod.Dusts;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class BileExplosion : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bile");
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            // projectile.ai[0] == 1f means spawned by Skyfin Bombers SS
            Projectile.position = Projectile.Center;

            if (Projectile.Calamity().stealthStrike || Projectile.ai[0] == 1f)
                Projectile.width = Projectile.height = 120;
            else
                Projectile.width = Projectile.height = 70;

            Projectile.position -= Projectile.Size / 2f;

            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, (int)CalamityDusts.SulfurousSeaAcid);
                dust.velocity = Projectile.width / 33.333f * Vector2.One.RotatedByRandom(MathHelper.TwoPi);
                dust.scale = Projectile.width == 120 ? 3.1f : 2.2f;
                dust.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.ai[0] != 1f)
                target.immune[Projectile.owner] = 9;

            target.AddBuff(ModContent.BuffType<Irradiated>(), 480);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 480);
        }
    }
}
