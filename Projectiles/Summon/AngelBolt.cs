using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class AngelBolt : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.extraUpdates = 200;
            Projectile.friendly = true;
            Projectile.timeLeft = 45;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            if (Projectile.timeLeft % 2f == 0f)
            {
                Vector2 vector33 = Projectile.position;
                vector33 -= Projectile.velocity * 0.25f;
                int num448 = Dust.NewDust(vector33, 1, 1, (int)CalamityDusts.ProfanedFire, 0f, 0f, 0, default, 1.25f);
                Main.dust[num448].position = vector33;
                Main.dust[num448].noGravity = true;
                Main.dust[num448].noLight = true;
                Main.dust[num448].scale = Main.rand.Next(70, 110) * 0.013f;
                Main.dust[num448].velocity *= 0.1f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(ModContent.BuffType<BanishingFire>(), 300);

        public override void OnHitPvp(Player target, int damage, bool crit) => target.AddBuff(ModContent.BuffType<BanishingFire>(), 300);
    }
}
