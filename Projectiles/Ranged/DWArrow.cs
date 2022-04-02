using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class DWArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Death's Arrow");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 5;
            projectile.timeLeft = 600;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.arrow = true;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            //Rotation
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = (projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi)) + MathHelper.ToRadians(90) *projectile.direction;

            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                Main.PlaySound(SoundID.Item12, (int)projectile.position.X, (int)projectile.position.Y);
            }

            Lighting.AddLight(projectile.Center, 0.4f, 0.2f, 0.4f);
            for (int num121 = 0; num121 < 2; num121++)
            {
                Dust dust4 = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.NextBool(3) ? 56 : 242, projectile.velocity.X, projectile.velocity.Y, 100, default, 1f)];
                dust4.velocity = Vector2.Zero;
                dust4.position -= projectile.velocity / 5f * (float)num121;
                dust4.noGravity = true;
                dust4.scale = 0.8f;
                dust4.noLight = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 180);
            target.AddBuff(BuffID.Frostburn, 90);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture;
            if (projectile.localAI[1] == 0f)
                projectile.localAI[1] = Main.rand.Next(1, 3);
            switch (projectile.localAI[1])
            {

                case 2f:
                    texture = ModContent.GetTexture("CalamityMod/Projectiles/Ranged/DWArrow2");
                    break;
                default:
                    texture = ModContent.GetTexture("CalamityMod/Projectiles/Ranged/DWArrow");
                    break;
            }
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture.Width / 2f, texture.Height / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
