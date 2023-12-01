using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class MeteorStar : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 361;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            bool flippedGravity = player.gravDir == -1f;

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }

            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0.15f / 255f);

            bool explodingSoon = Projectile.timeLeft <= 120;
            if (Projectile.soundDelay <= 0)
            {
                Projectile.soundDelay = 30 + Main.rand.Next(40);
                if (explodingSoon)
                    Projectile.soundDelay -= 30;
                if (Main.rand.NextBool(10) || explodingSoon)
                {
                    SoundEngine.PlaySound(SoundID.Item9, Projectile.Center);
                }
            }
            if ((Main.rand.NextBool(20) || explodingSoon) && Main.rand.NextBool(3) && Main.netMode != NetmodeID.Server)
            {
                int idx = Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * 0.2f, Main.rand.Next(16, 18), 1f);
                Main.gore[idx].velocity *= 0.66f;
                Main.gore[idx].velocity += Projectile.velocity * 0.3f;
            }
            if (explodingSoon)
            {
                for (int i = 0; i < 3; i++)
                {
                    int smoke = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                    Dust dust = Main.dust[smoke];
                    dust.velocity *= 0.3f;
                    dust.position.X = Projectile.Center.X + 4f + Main.rand.NextFloat(-6f, 6f);
                    dust.position.Y = Projectile.Center.Y + Main.rand.NextFloat(-6f, 6f);
                    dust.noGravity = true;
                }
            }

            if (Main.myPlayer == Projectile.owner)
            {
                if (player.channel)
                {
                    float speed = 14f;
                    float mouseDistX = (float)Main.mouseX + Main.screenPosition.X - Projectile.Center.X;
                    float mouseDistY = (float)Main.mouseY + Main.screenPosition.Y - Projectile.Center.Y;
                    if (player.gravDir == -1f)
                    {
                        mouseDistY = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - Projectile.Center.Y;
                    }
                    Vector2 mouseVec = new Vector2(mouseDistX, mouseDistY);
                    float mouseDist = mouseVec.Length();
                    if (mouseDist > speed)
                    {
                        mouseDist = speed / mouseDist;
                        mouseVec.X *= mouseDist;
                        mouseVec.Y *= mouseDist;
                        int mouseSpeedX = (int)(mouseVec.X * 1000f);
                        int projSpeedX = (int)(Projectile.velocity.X * 1000f);
                        int mouseSpeedY = (int)(mouseVec.Y * 1000f);
                        int projSpeedY = (int)(Projectile.velocity.Y * 1000f);
                        if (mouseSpeedX != projSpeedX || mouseSpeedY != projSpeedY)
                        {
                            Projectile.netUpdate = true;
                        }
                        Projectile.velocity.X = mouseVec.X;
                        Projectile.velocity.Y = mouseVec.Y;
                    }
                    else
                    {
                        int mouseSpeedX = (int)(mouseVec.X * 1000f);
                        int projSpeedX = (int)(Projectile.velocity.X * 1000f);
                        int mouseSpeedY = (int)(mouseVec.Y * 1000f);
                        int projSpeedY = (int)(Projectile.velocity.Y * 1000f);
                        if (mouseSpeedX != projSpeedX || mouseSpeedY != projSpeedY)
                        {
                            Projectile.netUpdate = true;
                        }
                        Projectile.velocity.X = mouseVec.X;
                        Projectile.velocity.Y = mouseVec.Y;
                    }
                    player.ChangeDir((int)Projectile.velocity.X);
                    player.velocity = Projectile.velocity;
                    if (Projectile.velocity.Y > 16f)
                    {
                        Projectile.velocity.Y = 16f;
                        player.velocity.Y = 16f;
                    }
                    if (player.mount != null)
                    {
                        player.mount.Dismount(player);
                    }
                    if (!flippedGravity)
                        player.Bottom = Projectile.Center;
                    else
                        player.Top = Projectile.Center;
                }
                else
                {
                    Explode(true);
                }
            }

            Projectile.tileCollide = Projectile.velocity != Vector2.Zero && ++Projectile.ai[0] >= 5f;

            // Die immediately if the owner of this projectile is clipping into tiles because of its movement.
            if (Collision.SolidCollision(player.position, player.width, player.height) && Projectile.velocity != Vector2.Zero)
            {
                player.velocity.Y = 0f;
                Explode();
            }

            if (Projectile.timeLeft <= 1)
                Explode();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Explode();

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => Explode();

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Explode();
            return base.OnTileCollide(oldVelocity);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);

            // Draw the main texture in fullbright
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;
            float scale = Projectile.scale;
            float rotation = Projectile.rotation;

            Rectangle rectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle, Color.White, rotation, origin, scale, SpriteEffects.None, 0);
            return false;
        }

        private void Explode(bool reducedDmg = false)
        {
            Projectile.ExpandHitboxBy(64);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            Vector2 spawnPos = Projectile.Center;
            spawnPos.Y -= 70f;
            if (reducedDmg)
                Projectile.damage /= 6;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, Vector2.Zero, ModContent.ProjectileType<MeteorStarExplosion>(), Projectile.damage * 3, Projectile.knockBack * 3f, Projectile.owner, reducedDmg.ToInt());

            for (int i = 0; i < 10; i++)
            {
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.2f);
                Main.dust[idx].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[idx].scale = 0.5f;
                    Main.dust[idx].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            if (Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < 5; i++)
                {
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, Projectile.velocity * 0.05f, Main.rand.Next(16, 18), 1f);
                }
            }
            Projectile.Kill();
        }
    }
}
