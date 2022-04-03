using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Rogue
{
    public class PartisanExplosion : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Profaned Explosion");
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.Calamity().rogue = true;
            Projectile.friendly = true;
            Projectile.timeLeft = 10;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
        }

        public override void AI()
        {
            for (int i = 0; i < 7; i++)
            {
                float speedx = Main.rand.NextFloat(-0.9f, 0.9f);
                float speedy = Main.rand.NextFloat(-0.9f, 0.9f);
                int d = Dust.NewDust(Projectile.position, 33, 33, 244, speedx, speedy, 120, default(Color), 2.6f);
                Main.dust[d].position = Projectile.Center;
            }
        }
    }
}
