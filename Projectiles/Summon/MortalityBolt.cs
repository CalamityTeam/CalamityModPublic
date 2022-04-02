using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class MortalityBolt : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public Color ProjectileColor => Main.hslToRgb(projectile.localAI[0], 1f, 0.5f);
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 20;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.minionSlots = 0f;
            projectile.minion = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, Color.White.ToVector3());
            if (!Main.dedServ)
            {
                for (int i = 0; i < 4; i++)
                {
                    Dust rainbowDust = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(5f, 5f), 261);
                    rainbowDust.color = ProjectileColor;
                    rainbowDust.velocity += projectile.velocity;
                    rainbowDust.noGravity = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 90);
            target.AddBuff(BuffID.Frostburn, 90);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 90);
        }

        public override void Kill(int timeLeft)
        {
            if (!Main.dedServ)
            {
                for (int i = 0; i < 15; i++)
                {
                    Dust rainbowDust = Dust.NewDustDirect(projectile.Center + Main.rand.NextVector2Circular(5f, 5f), 40, 40, 261);
                    rainbowDust.color = ProjectileColor;
                    rainbowDust.noGravity = true;
                }
            }
        }
    }
}
