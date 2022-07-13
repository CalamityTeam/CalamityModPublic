using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class OrangeExoArrow : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/LaserProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orange Exo Arrow");
        }

        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.arrow = true;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
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
            float num55 = 40f;
            float num56 = 1.5f;
            if (Projectile.ai[1] == 0f)
            {
                Projectile.localAI[0] += num56;
                if (Projectile.localAI[0] > num55)
                {
                    Projectile.localAI[0] = num55;
                }
            }
            else
            {
                Projectile.localAI[0] -= num56;
                if (Projectile.localAI[0] <= 0f)
                {
                    Projectile.Kill();
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(250, 100, 0, Projectile.alpha);

        public override bool PreDraw(ref Color lightColor) => Projectile.DrawBeam(40f, 1.5f, lightColor);

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.ExoDebuffs();

        public override void OnHitPvp(Player target, int damage, bool crit) => target.ExoDebuffs();

        public override void Kill(int timeLeft)
        {
            Projectile.ExpandHitboxBy(188);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int d = 0; d < 4; d++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 127, 0f, 0f, 50, default, 1f);
            }
            for (int d = 0; d < 40; d++)
            {
                int orange = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 127, 0f, 0f, 0, default, 1.5f);
                Main.dust[orange].noGravity = true;
                Main.dust[orange].noLight = true;
                Main.dust[orange].velocity *= 3f;
                orange = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 127, 0f, 0f, 50, default, 1f);
                Main.dust[orange].velocity *= 2f;
                Main.dust[orange].noGravity = true;
                Main.dust[orange].noLight = true;
            }
        }
    }
}
