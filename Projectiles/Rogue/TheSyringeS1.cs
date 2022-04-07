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
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.aiStyle = 24;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.light = 0.5f;
            Projectile.alpha = 50;
            Projectile.scale = 1.2f;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.Calamity().rogue = true;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 150 && target.CanBeChasedBy(Projectile);

        public override void PostAI()
        {
            if (Projectile.timeLeft < 150)
                CalamityGlobalProjectile.HomeInOnNPC(Projectile, true, 600f, 10f, 20f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Changes the texture of the projectile
            if (Projectile.ai[0] == 1f)
            {
                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/TheSyringeS2").Value;
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, texture.Width, 6)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, 20 / 2f), Projectile.scale, SpriteEffects.None, 0);
                return false;
            }
            if (Projectile.ai[0] == 2f)
            {
                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/TheSyringeS3").Value;
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, texture.Width, 6)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, 20 / 2f), Projectile.scale, SpriteEffects.None, 0);
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
