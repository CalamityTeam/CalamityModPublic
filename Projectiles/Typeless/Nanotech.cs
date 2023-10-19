using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class Nanotech : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.ai[1] >= 30f && target.CanBeChasedBy(Projectile);

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.075f, 0.4f, 0.15f));

            Projectile.rotation += Projectile.velocity.X * 0.2f;
            if (Projectile.velocity.X > 0f)
                Projectile.rotation += 0.08f;
            else
                Projectile.rotation -= 0.08f;

            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] > 60f)
            {
                Projectile.alpha += 5;
                if (Projectile.alpha >= 255)
                {
                    Projectile.alpha = 255;
                    Projectile.Kill();
                    return;
                }
            }

            if (Projectile.ai[1] >= 30f)
                CalamityUtils.HomeInOnNPC(Projectile, true, 200f, 12f, 20f);
        }

        // Reduce damage of projectiles if more than the cap are active
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            int cap = 5;
            float capDamageFactor = 0.05f;
            int excessCount = Main.player[Projectile.owner].ownedProjectileCounts[Projectile.type] - cap;
            modifiers.SourceDamage *= MathHelper.Clamp(1f - (capDamageFactor * excessCount), 0f, 1f);
        }

        public override void OnKill(int timeLeft)
        {
            int inc;
            for (int i = 0; i < 2; i = inc + 1)
            {
                int dustScale = (int)(10f * Projectile.scale);
                int greenDust = Dust.NewDust(Projectile.Center - Vector2.One * (float)dustScale, dustScale * 2, dustScale * 2, 107, 0f, 0f, 0, default, 1f);
                Dust nanoDust = Main.dust[greenDust];
                Vector2 dustDirection = Vector2.Normalize(nanoDust.position - Projectile.Center);
                nanoDust.position = Projectile.Center + dustDirection * (float)dustScale * Projectile.scale;
                if (i < 30)
                {
                    nanoDust.velocity = dustDirection * nanoDust.velocity.Length();
                }
                else
                {
                    nanoDust.velocity = dustDirection * (float)Main.rand.Next(45, 91) / 10f;
                }
                nanoDust.color = Main.hslToRgb((float)(0.40000000596046448 + Main.rand.NextDouble() * 0.20000000298023224), 0.9f, 0.5f);
                nanoDust.color = Color.Lerp(nanoDust.color, Color.White, 0.3f);
                nanoDust.noGravity = true;
                nanoDust.scale = 0.7f;
                inc = i;
            }
        }
    }
}
