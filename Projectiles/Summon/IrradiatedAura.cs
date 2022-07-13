using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;

namespace CalamityMod.Projectiles.Summon
{
    public class IrradiatedAura : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Irradiated Aura");
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0f / 255f, (255 - Projectile.alpha) * 0.15f / 255f, (255 - Projectile.alpha) * 0.4f / 255f);
            int randomDust = Main.rand.Next(4);
            if (randomDust == 3)
            {
                randomDust = 89;
            }
            else if (randomDust == 2)
            {
                randomDust = (int)CalamityDusts.SulfurousSeaAcid;
            }
            else
            {
                randomDust = 33;
            }
            for (int num468 = 0; num468 < (Main.rand.NextBool(2) ? 1 : 2); num468++)
            {
                int num469 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDust, 0f, 0f, 100, default, 1f);
                if (randomDust == 89)
                {
                    Main.dust[num469].scale *= 0.35f;
                }
                Main.dust[num469].velocity *= 0f;
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
