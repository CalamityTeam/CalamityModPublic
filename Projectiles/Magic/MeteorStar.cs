using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class MeteorStar : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 0;
            Main.projFrames[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 361;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 6 % Main.projFrames[Type];
            Color LightYellow = new Color(255, 255, 76);
            Lighting.AddLight(Projectile.Center, LightYellow.ToVector3() * Projectile.Opacity * 0.5f);

            bool explodingSoon = Projectile.timeLeft <= 120;
            if (Projectile.soundDelay <= 0)
            {
                Projectile.soundDelay = 30 + Main.rand.Next(explodingSoon ? 10 : 40);
                if (Main.rand.NextBool(4) || explodingSoon)
                    SoundEngine.PlaySound(SoundID.Item9, Projectile.Center);
            }
            if ((Main.rand.NextBool(12) || (explodingSoon && Main.rand.NextBool(3))) && !Main.dedServ)
            {
                Gore star = Gore.NewGoreDirect(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * 0.2f, Main.rand.Next(16, 18), 1f);
                star.velocity *= 0.66f;
                star.velocity += Projectile.velocity * 0.3f;
            }
            if (explodingSoon)
            {
                for (int i = 0; i < 3; i++)
                {
                    Dust smoke = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                    smoke.velocity *= 0.3f;
                    smoke.position.X = Projectile.Center.X + 4f + Main.rand.NextFloat(-6f, 6f);
                    smoke.position.Y = Projectile.Center.Y + Main.rand.NextFloat(-6f, 6f);
                    smoke.noGravity = true;
                }
            }

            if (Main.myPlayer == Projectile.owner)
            {
                // Initialize position
                if (Projectile.ai[2] != 1f)
                {
                    if (Owner.gravDir == -1f)
                        Projectile.Center = Owner.Top;
                    else
                        Projectile.Center = Owner.Bottom;

                    Projectile.ai[2] = 1f;
                    Projectile.ForceNetUpdate();
                }
                if (Owner.channel)
                {
                    Owner.mount?.Dismount(Owner);
                    Owner.RemoveAllGrapplingHooks();

                    // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                    Projectile.velocity = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld) * 12f;
                    Owner.velocity = Projectile.velocity;
                    Owner.ChangeDir(Math.Sign(Projectile.velocity.X) <= 0 ? -1 : 1);

                    // Move the player to the projectile, allowing them to bypass platforms
                    if (Owner.gravDir == -1f)
                        Owner.Top = Projectile.Center;
                    else
                        Owner.Bottom = Projectile.Center;
                }
                else
                    Explode(true);
            }

            // Die immediately if the owner of this projectile is clipping into tiles because of its movement.
            if (Collision.SolidCollision(Owner.position + Projectile.velocity, Owner.width, Owner.height) && Projectile.velocity != Vector2.Zero)
            {
                Owner.velocity.Y = 0f;
                Explode();
            }

            if (Projectile.timeLeft <= 1)
                Explode();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Explode();

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => Explode();

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);

            // Draw the main texture in fullbright
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
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
                Dust smoke = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.2f);
                smoke.velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    smoke.scale = 0.5f;
                    smoke.fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            if (!Main.dedServ)
            {
                for (int i = 0; i < 5; i++)
                {
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, Projectile.velocity * 0.05f, Main.rand.Next(16, 18), 1f);
                }
            }
            Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
            Owner.velocity *= 0.8f;
            Owner.fullRotation = 0f;
        }
    }
}
