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
            projectile.width = 100;
            projectile.height = 100;
            projectile.Calamity().rogue = true;
            projectile.friendly = true;
            projectile.timeLeft = 10;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
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
                int d = Dust.NewDust(projectile.position, 33, 33, 244, speedx, speedy, 120, default(Color), 2.6f);
                Main.dust[d].position = projectile.Center;
            }
        }
    }
}
