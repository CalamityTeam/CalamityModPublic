using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class TemporalFloeNumberTwo : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/TemporalFloeSwordProjectile";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Floe");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.aiStyle = 27;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 60;
            projectile.coldDamage = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, (255 - projectile.alpha) * 0.05f / 255f, (255 - projectile.alpha) * 0.35f / 255f);
            if (Main.rand.NextBool(2))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 56, 0f, 0f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(50, 50, 255, projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);
            for (int num795 = 0; num795 < 27; num795++)
            {
                float num796 = projectile.oldVelocity.X * (30f / (float)num795);
                float num797 = projectile.oldVelocity.Y * (30f / (float)num795);
                int num798 = Dust.NewDust(new Vector2(projectile.oldPosition.X - num796, projectile.oldPosition.Y - num797), 8, 8, 56, projectile.oldVelocity.X, projectile.oldVelocity.Y, 100, default, 1.8f);
                Dust dust = Main.dust[num798];
                dust.noGravity = true;
                dust.velocity *= 0.5f;
                num798 = Dust.NewDust(new Vector2(projectile.oldPosition.X - num796, projectile.oldPosition.Y - num797), 8, 8, 56, projectile.oldVelocity.X, projectile.oldVelocity.Y, 100, default, 1.4f);
                dust.velocity *= 0.05f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
            target.AddBuff(BuffID.Frostburn, 120);
        }
    }
}
