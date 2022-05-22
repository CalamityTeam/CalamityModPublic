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
            Projectile.width = 4;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 3;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
        }

        public override void AI()
        {
            Projectile.alpha = 50;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Changes the texture of the projectile
            if (Projectile.ai[0] == 1f)
            {
                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/StealthRain2").Value;
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, texture.Height / 2f), Projectile.scale, SpriteEffects.None, 0);
                return false;
            }
            return true;
        }

        public override void Kill(int timeLeft)
        {
            int dustType = Projectile.ai[0] == 0f ? 14 : 114;

            int num310 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + (float)Projectile.height - 2f), 2, 2, dustType, 0f, 0f, 0, default, 1f);
            Dust dust = Main.dust[num310];
            dust.position.X -= 2f;
            dust.alpha = 38;
            dust.velocity *= 0.1f;
            dust.velocity += -Projectile.oldVelocity * 0.25f;
            dust.scale = 0.95f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int buffType = Projectile.ai[0] == 0f ? BuffID.CursedInferno : ModContent.BuffType<BurningBlood>();
            target.AddBuff(buffType, 90);
        }
    }
}
