using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Rogue
{
    public class TheSyringeS1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shrapnel");
            Main.projFrames[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.aiStyle = 24;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.light = 0.5f;
            projectile.alpha = 50;
            projectile.scale = 1.2f;
            projectile.timeLeft = 180;
            projectile.tileCollide = false;
            projectile.Calamity().rogue = true;
        }

        public override bool? CanHitNPC(NPC target) => projectile.timeLeft < 150 && target.CanBeChasedBy(projectile);

        public override void PostAI()
        {
            if (projectile.timeLeft < 150)
                CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 600f, 10f, 20f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //Changes the texture of the projectile
            if (projectile.ai[0] == 1f)
            {
                Texture2D texture = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/TheSyringeS2");
                Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, texture.Width, 6)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture.Width / 2f, 20 / 2f), projectile.scale, SpriteEffects.None, 0f);
                return false;
            }
            if (projectile.ai[0] == 2f)
            {
                Texture2D texture = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/TheSyringeS3");
                Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, texture.Width, 6)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture.Width / 2f, 20 / 2f), projectile.scale, SpriteEffects.None, 0f);
                return false;
            }
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 120);
        }
    }
}
