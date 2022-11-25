using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

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
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 5;
            Projectile.timeLeft = 600;
            Projectile.arrow = true;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void AI()
        {
            //Rotation
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = (Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi)) + MathHelper.ToRadians(90) *Projectile.direction;

            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item12, Projectile.position);
            }

            Lighting.AddLight(Projectile.Center, 0.4f, 0.2f, 0.4f);
            for (int num121 = 0; num121 < 2; num121++)
            {
                Dust dust4 = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool(3) ? 56 : 242, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 1f)];
                dust4.velocity = Vector2.Zero;
                dust4.position -= Projectile.velocity / 5f * (float)num121;
                dust4.noGravity = true;
                dust4.scale = 0.8f;
                dust4.noLight = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture;
            if (Projectile.localAI[1] == 0f)
                Projectile.localAI[1] = Main.rand.Next(1, 3);
            switch (Projectile.localAI[1])
            {

                case 2f:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/DWArrow2").Value;
                    break;
                default:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/DWArrow").Value;
                    break;
            }
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, texture.Height / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
