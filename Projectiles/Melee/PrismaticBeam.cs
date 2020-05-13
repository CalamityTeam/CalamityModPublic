using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class PrismaticBeam : ModProjectile
    {
        private int alpha = 50;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Doom");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.friendly = true;
            projectile.ranged = true;
			projectile.melee = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.extraUpdates = 100;
            projectile.timeLeft = 100;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, Main.DiscoR * 0.25f / 255f, Main.DiscoG * 0.25f / 255f, Main.DiscoB * 0.25f / 255f);
			projectile.localAI[0] += 1f;
			if (projectile.localAI[0] > 9f)
			{
				Color color = Utils.SelectRandom(Main.rand, new Color[]
				{
					new Color(255, 0, 0, alpha), //Red
					new Color(255, 128, 0, alpha), //Orange
					new Color(255, 255, 0, alpha), //Yellow
					new Color(128, 255, 0, alpha), //Lime
					new Color(0, 255, 0, alpha), //Green
					new Color(0, 255, 128, alpha), //Turquoise
					new Color(0, 255, 255, alpha), //Cyan
					new Color(0, 128, 255, alpha), //Light Blue
					new Color(0, 0, 255, alpha), //Blue
					new Color(128, 0, 255, alpha), //Purple
					new Color(255, 0, 255, alpha), //Fuschia
					new Color(255, 0, 128, alpha) //Hot Pink
				});
				Vector2 vector33 = projectile.position;
				vector33 -= projectile.velocity * 0.25f;
				int num448 = Dust.NewDust(vector33, 1, 1, 267, 0f, 0f, alpha, color, 2.5f);
				Main.dust[num448].position = vector33;
				Main.dust[num448].velocity *= 0.1f;
			}
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 120);
            target.AddBuff(ModContent.BuffType<Plague>(), 120);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
            target.AddBuff(BuffID.CursedInferno, 120);
            target.AddBuff(BuffID.Frostburn, 120);
            target.AddBuff(BuffID.OnFire, 120);
            target.AddBuff(BuffID.Ichor, 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 120);
            target.AddBuff(ModContent.BuffType<Plague>(), 120);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
            target.AddBuff(BuffID.CursedInferno, 120);
            target.AddBuff(BuffID.Frostburn, 120);
            target.AddBuff(BuffID.OnFire, 120);
            target.AddBuff(BuffID.Ichor, 120);
        }
    }
}
