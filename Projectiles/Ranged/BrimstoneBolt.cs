using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class BrimstoneBolt : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 300;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
            }
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 15;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            Lighting.AddLight(Projectile.Center, 0.7f, 0f, 0f);
            if (Projectile.localAI[0] > 2f)
            {
                for (int num121 = 0; num121 < 5; num121++)
                {
                    Dust dust4 = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.Brimstone, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 1f)];
                    dust4.velocity = Vector2.Zero;
                    dust4.position -= Projectile.velocity / 5f * (float)num121;
                    dust4.noGravity = true;
                    dust4.scale = 0.8f;
                    dust4.noLight = true;
                }
            }
            else
                Projectile.localAI[0] += 1f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
        }
    }
}
