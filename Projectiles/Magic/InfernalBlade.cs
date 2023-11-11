using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Magic
{
    public class InfernalBlade : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 180;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            DelegateMethods.v3_1 = new Vector3(0.6f, 1f, 1f) * 0.2f;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * 10f, 8f, DelegateMethods.CastLightOpen);
            if (Projectile.alpha > 0)
            {
                SoundEngine.PlaySound(SoundID.Item9, Projectile.Center);
                Projectile.alpha = 0;
                Projectile.scale = 1.1f;
                Projectile.frame = Main.rand.Next(14);
                float dustLoopcheck = 16f;
                int dustIncr = 0;
                while ((float)dustIncr < dustLoopcheck)
                {
                    Vector2 dustRotate = Vector2.UnitX * 0f;
                    dustRotate += -Vector2.UnitY.RotatedBy((double)((float)dustIncr * (6.28318548f / dustLoopcheck)), default) * new Vector2(1f, 4f);
                    dustRotate = dustRotate.RotatedBy((double)Projectile.velocity.ToRotation(), default);
                    int deepRed = Dust.NewDust(Projectile.Center, 0, 0, 182, 0f, 0f, 0, default, 1f);
                    Main.dust[deepRed].scale = 1.5f;
                    Main.dust[deepRed].noGravity = true;
                    Main.dust[deepRed].position = Projectile.Center + dustRotate;
                    Main.dust[deepRed].velocity = Projectile.velocity * 0f + dustRotate.SafeNormalize(Vector2.UnitY) * 1f;
                    dustIncr++;
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + 0.7853982f;
        }

        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            int dustAmt = Main.rand.Next(4, 10);
            for (int d = 0; d < dustAmt; d++)
            {
                int fire = Dust.NewDust(Projectile.Center, 0, 0, 182, 0f, 0f, 100, default, 1f);
                Dust dust = Main.dust[fire];
                dust.velocity *= 1.6f;
                dust.velocity.Y -= 1f;
                dust.velocity += -Projectile.velocity * (Main.rand.NextFloat() * 2f - 1f) * 0.5f;
                dust.scale = 2f;
                dust.fadeIn = 0.5f;
                dust.noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 50, 50, 0);

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Color colorArea = Lighting.GetColor((int)((double)Projectile.position.X + (double)Projectile.width * 0.5) / 16, (int)(((double)Projectile.position.Y + (double)Projectile.height * 0.5) / 16.0));
            Texture2D texture2D3 = ModContent.Request<Texture2D>(Texture).Value;
            int textureArea = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int y3 = textureArea * Projectile.frame;
            Rectangle rectangle = new Rectangle(0, y3, texture2D3.Width, textureArea);
            Vector2 halfRect = rectangle.Size() / 2f;
            rectangle = new Rectangle(38 * Projectile.frame, 0, 38, 38);
            halfRect = rectangle.Size() / 2f;
            for (int i = 1; i < 3; i += 1)
            {
                Color alphaColor = colorArea;
                alphaColor = Projectile.GetAlpha(alphaColor);
                alphaColor *= (float)(3 - i) / ((float)ProjectileID.Sets.TrailCacheLength[Projectile.type] * 1.5f);
                Vector2 oldPosition = Projectile.oldPos[i];
                float projRotation = Projectile.rotation;
                SpriteEffects effects = spriteEffects;
                if (ProjectileID.Sets.TrailingMode[Projectile.type] == 2)
                {
                    projRotation = Projectile.oldRot[i];
                    effects = (Projectile.oldSpriteDirection[i] == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                }
                Main.spriteBatch.Draw(texture2D3, oldPosition + Projectile.Size / 2f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), alphaColor, projRotation + Projectile.rotation * 0f * (float)(i - 1) * (float)-(float)spriteEffects.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), halfRect, MathHelper.Lerp(Projectile.scale, 8f, (float)i / 15f), effects, 0f);
            }
            Color alphaColorAgain = Projectile.GetAlpha(colorArea);
            Main.spriteBatch.Draw(texture2D3, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), alphaColorAgain, Projectile.rotation, halfRect, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnHitEffects(target.Center);
            target.AddBuff(BuffID.OnFire3, 180);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            OnHitEffects(target.Center);
            target.AddBuff(BuffID.OnFire3, 180);
        }

        private void OnHitEffects(Vector2 targetPos)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                var source = Projectile.GetSource_FromThis();
                CalamityUtils.ProjectileBarrage(source, Projectile.Center, targetPos, Main.rand.NextBool(), 800f, 800f, 0f, 800f, 10f, ModContent.ProjectileType<InfernalBlade2>(), (int)(Projectile.damage * 0.75), 1f, Projectile.owner, true);
            }
        }
    }
}
