using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class StealthRain : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/ShaderainHostile";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rain");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 40;
            projectile.friendly = true;
            projectile.extraUpdates = 1;
            projectile.penetrate = 3;
            projectile.ignoreWater = true;
            projectile.timeLeft = 300;
            projectile.Calamity().rogue = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 15;
        }

        public override void AI()
        {
            projectile.alpha = 50;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //Changes the texture of the projectile
            if (projectile.ai[0] == 1f)
            {
                Texture2D texture = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/StealthRain2");
                Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture.Width / 2f, texture.Height / 2f), projectile.scale, SpriteEffects.None, 0f);
                return false;
            }
            return true;
        }

        public override void Kill(int timeLeft)
        {
            int dustType = projectile.ai[0] == 0f ? 14 : 114;

            int num310 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + (float)projectile.height - 2f), 2, 2, dustType, 0f, 0f, 0, default, 1f);
            Dust dust = Main.dust[num310];
            dust.position.X -= 2f;
            dust.alpha = 38;
            dust.velocity *= 0.1f;
            dust.velocity += -projectile.oldVelocity * 0.25f;
            dust.scale = 0.95f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int buffType = projectile.ai[0] == 0f ? BuffID.CursedInferno : ModContent.BuffType<BurningBlood>();
            target.AddBuff(buffType, 90);
        }
    }
}
