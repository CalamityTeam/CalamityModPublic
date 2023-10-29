using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class EvergladeSprayProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 6;
            Projectile.MaxUpdates = 3;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6 * Projectile.MaxUpdates; // 6 effective, 18 total
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.01f / 255f, (255 - Projectile.alpha) * 0.15f / 255f, (255 - Projectile.alpha) * 0.05f / 255f);
            Projectile.scale -= 0.002f;
            if (Projectile.scale <= 0f)
            {
                Projectile.Kill();
            }
            if (Projectile.ai[0] <= 3f)
            {
                Projectile.ai[0] += 1f;
                return;
            }
            Projectile.velocity.Y = Projectile.velocity.Y + 0.075f;
            for (int i = 0; i < 3; i++)
            {
                float shortXVel = Projectile.velocity.X / 3f * (float)i;
                float shortYVel = Projectile.velocity.Y / 3f * (float)i;
                int natureDust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 157, 0f, 0f, 100, default, 1f);
                Dust dust = Main.dust[natureDust];
                dust.noGravity = true;
                dust.velocity *= 0.1f;
                dust.velocity += Projectile.velocity * 0.5f;
                dust.position.X -= shortXVel;
                dust.position.Y -= shortYVel;
            }
            if (Main.rand.NextBool(8))
            {
                int natual = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 157, 0f, 0f, 100, default, 0.5f);
                Main.dust[natual].velocity *= 0.25f;
                Main.dust[natual].velocity += Projectile.velocity * 0.5f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Ichor, 600);
            target.AddBuff(BuffID.CursedInferno, 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
