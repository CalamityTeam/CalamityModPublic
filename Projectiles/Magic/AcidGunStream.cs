using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class AcidGunStream : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acid Stream");
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 3;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.localAI[1]++;
            if (Projectile.localAI[1] >= 4f)
            {
                Projectile.tileCollide = true;
            }
            Projectile.scale -= 0.002f;
            if (Projectile.scale <= 0f)
            {
                Projectile.Kill();
            }
            if (Projectile.localAI[0] <= 3f)
            {
                Projectile.localAI[0] += 1f;
                return;
            }
            Projectile.velocity.Y += 0.075f;
            for (int i = 0; i < 3; i++)
            {
                Vector2 positionDelta = Projectile.velocity / 3f * i;
                int spawnDelta = 14;
                int dustIdx = Dust.NewDust(new Vector2(Projectile.position.X + spawnDelta, Projectile.position.Y + spawnDelta), Projectile.width - spawnDelta * 2, Projectile.height - spawnDelta * 2, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default, 1f);
                Dust dust = Main.dust[dustIdx];
                dust.noGravity = true;
                dust.velocity *= 0.1f;
                dust.velocity += Projectile.velocity * 0.5f;
                dust.position -= positionDelta;
            }
            if (Main.rand.NextBool(8))
            {
                int spawnDelta = 16;
                int dustIdx = Dust.NewDust(new Vector2(Projectile.position.X + spawnDelta, Projectile.position.Y + spawnDelta), Projectile.width - spawnDelta * 2, Projectile.height - spawnDelta * 2, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default, 0.5f);
                Main.dust[dustIdx].velocity *= 0.25f;
                Main.dust[dustIdx].velocity += Projectile.velocity * 0.5f;
            }
        }

        // If any of the streams are destroyed, kill the accompanying acid streams
        public override void Kill(int timeLeft)
        {
            if (Projectile.penetrate > 1)
                return;

            if (Main.projectile.IndexInRange((int)Projectile.ai[0]))
            {
                Projectile proj = Main.projectile[(int)Projectile.ai[0]];
                proj.ai[0] = -1f;
                proj.ai[1] = -1f;
                proj.Kill();
            }
            if (Main.projectile.IndexInRange((int)Projectile.ai[1]))
            {
                Projectile proj = Main.projectile[(int)Projectile.ai[1]];
                proj.ai[0] = -1f;
                proj.ai[1] = -1f;
                proj.Kill();
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
        }
    }
}
