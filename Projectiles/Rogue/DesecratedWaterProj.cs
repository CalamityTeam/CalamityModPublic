using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class DesecratedWaterProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/DesecratedWater";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Desecrated Water");
        }

        public override void SetDefaults()
        {
            projectile.width = 25;
            projectile.height = 25;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.aiStyle = 2;
            projectile.timeLeft = 300;
            aiType = ProjectileID.ThrowingKnife;
            projectile.Calamity().rogue = true;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Shatter, (int) projectile.position.X, (int) projectile.position.Y, 1, 1f, 0.0f);
            Vector2 vector2 = new Vector2(20f, 20f);
            for (int index = 0; index < 10; ++index)
                Dust.NewDust(projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, 191, 0.0f, 0.0f);
            for (int index1 = 0; index1 < 20; ++index1)
            {
                int index2 = Dust.NewDust(projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, 179, 0.0f, 0.0f, 100, new Color(), 2.5f);
                Main.dust[index2].noGravity = true;
                Dust dust1 = Main.dust[index2];
                dust1.velocity = dust1.velocity * 5f;
                int index3 = Dust.NewDust(projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, 179, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Dust dust2 = Main.dust[index3];
                dust2.velocity = dust2.velocity * 3f;
            }
            int num220 = (projectile.Calamity().stealthStrike ? Main.rand.Next(10, 16) : Main.rand.Next(5, 11));
            if (projectile.owner == Main.myPlayer)
            {
                for (int num221 = 0; num221 < num220; num221++)
                {
                    Vector2 value17 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    value17.Normalize();
                    value17 *= (float)Main.rand.Next(10, 201) * 0.01f;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value17.X, value17.Y, ModContent.ProjectileType<DesecratedBubble>(), (int)(projectile.damage * 0.5), 1f, projectile.owner, (projectile.Calamity().stealthStrike ? 1f : 0f), 1f);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
