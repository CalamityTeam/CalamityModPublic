using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class MechwormLaser : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/LaserProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 4;
            Projectile.MaxUpdates = 3;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10 * Projectile.MaxUpdates;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            // Very quickly fade in.
            Projectile.alpha = Utils.Clamp(Projectile.alpha - 25, 0, 255);

            Lighting.AddLight(Projectile.Center, Color.Cyan.ToVector3());
            if (Projectile.ai[1] == 0f)
            {
                Projectile.localAI[0] += 6f;
                if (Projectile.localAI[0] > 30)
                    Projectile.localAI[0] = 30;
            }
            else
            {
                Projectile.localAI[0] -= 6f;
                if (Projectile.localAI[0] <= 0f)
                {
                    Projectile.Kill();
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(Color.Fuchsia, Color.Cyan, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1.9f + Projectile.identity * 2.4f) * 0.5f + 0.5f);

        public override bool PreDraw(ref Color lightColor) => Projectile.DrawBeam(100f, 3f, lightColor);

        public override void OnKill(int timeLeft)
        {
            int dustAmt = Main.rand.Next(3, 7);
            for (int d = 0; d < dustAmt; d++)
            {
                int rainbow = Dust.NewDust(Projectile.Center - Projectile.velocity / 2f, 0, 0, 66, 0f, 0f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 2.1f);
                Main.dust[rainbow].velocity *= 2f;
                Main.dust[rainbow].noGravity = true;
            }
        }
    }
}
