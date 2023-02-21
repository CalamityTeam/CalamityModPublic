using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class SoulPiercerBolt : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Piercer");
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.extraUpdates = 100;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Vector2 vector33 = Projectile.position;
            vector33 -= Projectile.velocity;
            int num448 = Dust.NewDust(vector33, 1, 1, 173, 0f, 0f, 0, default, 0.5f);
            Main.dust[num448].position = vector33;
            Main.dust[num448].scale = Main.rand.Next(70, 110) * 0.014f;
            Main.dust[num448].velocity *= 0.2f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180);

        public override void OnHitPvp(Player target, int damage, bool crit) => target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180);
    }
}
