using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class AngelBolt : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.extraUpdates = 200;
            Projectile.friendly = true;
            Projectile.timeLeft = 45;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            if (Projectile.timeLeft % 2f == 0f)
            {
                Vector2 dustPos = Projectile.position;
                dustPos -= Projectile.velocity * 0.25f;
                int dust = Dust.NewDust(dustPos, 1, 1, (int)CalamityDusts.ProfanedFire, 0f, 0f, 0, default, 1.25f);
                Main.dust[dust].position = dustPos;
                Main.dust[dust].noGravity = true;
                Main.dust[dust].noLight = true;
                Main.dust[dust].scale = Main.rand.Next(70, 110) * 0.013f;
                Main.dust[dust].velocity *= 0.1f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<BanishingFire>(), 300);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<BanishingFire>(), 300);
    }
}
