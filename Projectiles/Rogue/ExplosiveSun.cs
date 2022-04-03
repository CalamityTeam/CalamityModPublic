using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class ExplosiveSun : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosive Sun");
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            //projectile.aiStyle = 115;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.light = 0.5f;
            Projectile.alpha = 50;
            Projectile.scale = 1.2f;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Projectile.velocity.X *= 0.985f;
            Projectile.velocity.Y *= 0.985f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Changes the texture of the projectile
            if (Projectile.ai[0] == 1f)
            {
                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/ExplosiveSun2");
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height)), Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
                return false;
            }
            if (Projectile.ai[0] == 2f)
            {
                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/ExplosiveSun3");
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height)), Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
                return false;
            }
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
        }

        public override void Kill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 192;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.damage /= 2;
            Projectile.Damage();
            if (Main.rand.NextBool(3))
            {
                SoundEngine.PlaySound(SoundID.Item, (int)Projectile.Center.X, (int)Projectile.Center.Y, 14, 0.5f);
            }
            for (int num621 = 0; num621 < 5; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 10; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
        }
    }
}
