using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class DrataliornusExoArrow : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/LaserProj";

        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 4;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.arrow = true;
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 25;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            float inc = 3f;
            if (Projectile.ai[1] == 0f)
            {
                Projectile.localAI[0] += inc;
                if (Projectile.localAI[0] > 100f)
                {
                    Projectile.localAI[0] = 100f;
                }
            }
            else
            {
                Projectile.localAI[0] -= inc;
                if (Projectile.localAI[0] <= 0f)
                {
                    Projectile.Kill();
                    return;
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<Dragonfire>(), 180);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 0;

            target.AddBuff(ModContent.BuffType<Dragonfire>(), 180);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(250, 25, 0, Projectile.alpha);

        public override bool PreDraw(ref Color lightColor) => Projectile.DrawBeam(100f, 3f, lightColor);
    }
}
