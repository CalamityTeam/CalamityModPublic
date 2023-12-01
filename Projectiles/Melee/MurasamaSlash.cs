using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class MurasamaSlash : ModProjectile, ILocalizedModType
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Murasama>();
        private Player Owner => Main.player[Projectile.owner];
        public ref int hitCooldown => ref Main.player[Projectile.owner].Calamity().murasamaHitCooldown;
        public int frameX = 0;
        public int frameY = 0;
        public int time = 0;

        public int CurrentFrame
        {
            get => frameX * 7 + frameY;
            set
            {
                frameX = value / 7;
                frameY = value % 7;
            }
        }

        public bool Slashing = false;

        public override void SetDefaults()
        {
            Projectile.width = 236;
            Projectile.height = 236;
            Projectile.scale = 1f;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = TrueMeleeNoSpeedDamageClass.Instance;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 6;
            Projectile.frameCounter = 0;
            Projectile.alpha = 255;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.frameCounter <= 1)
                return false;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = texture.Size() / new Vector2(2f, 7f) * 0.5f;
            Rectangle frame = texture.Frame(2, 7, frameX, frameY);
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }

        public override void AI()
        {
            if (time == 0)
            {
                if (Main.zenithWorld)
                    Projectile.scale = 2;
                CurrentFrame = Main.zenithWorld ? 6 : 10;
                Projectile.alpha = 0;
                time++;
            }
            Player player = Main.player[Projectile.owner];
            if (CurrentFrame == 0)
            {
                SoundEngine.PlaySound(Murasama.Swing with { Pitch = -0.1f }, Projectile.Center);
                if (hitCooldown == 0)
                    Slashing = true;
                Projectile.numHits = 0;
            }
            else if (CurrentFrame == 6)
            {
                SoundEngine.PlaySound(Murasama.BigSwing with { Pitch = 0f }, Projectile.Center);
                if (hitCooldown == 0)
                    Slashing = true;
                Projectile.numHits = 0;
            }
            else if (CurrentFrame == 10)
            {
                SoundEngine.PlaySound(Murasama.Swing with { Pitch = -0.05f }, Projectile.Center);
                if (hitCooldown == 0)
                    Slashing = true;
                Projectile.numHits = 0;
            }
            else
                Slashing = false;

            if (CurrentFrame == 5 && Projectile.frameCounter % 3 == 0)
            {
                Projectile.damage = (int)(Projectile.damage * 2);
            }
            if (CurrentFrame == 7 && Projectile.frameCounter % 3 == 0)
            {
                Projectile.damage = (int)(Projectile.damage * 0.5f);
            }

            //Frames and crap
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 3 == 0)
            {
                CurrentFrame++;
                if (frameX >= 2)
                    CurrentFrame = 0;
            }

            Vector2 origin = Projectile.Center + Projectile.velocity * 3f;
            Lighting.AddLight(origin, Color.Red.ToVector3() * (Slashing == true ? 3.5f : 2f));

            Vector2 playerRotatedPoint = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Main.myPlayer == Projectile.owner)
            {
                if (player.channel && !player.noItems && !player.CCed)
                    HandleChannelMovement(player, playerRotatedPoint);
                else
                {
                    hitCooldown = Main.zenithWorld ? 0 : 8;
                    Projectile.Kill();
                }
            }

            // Rotation and directioning.
            if (Slashing || CurrentFrame == 10)
            {
                float velocityAngle = Projectile.velocity.ToRotation();
                Projectile.rotation = velocityAngle + (Projectile.spriteDirection == -1).ToInt() * MathHelper.Pi;
            }
            float velocityAngle2 = Projectile.velocity.ToRotation();
            Projectile.direction = (Math.Cos(velocityAngle2) > 0).ToDirectionInt();

            // Positioning close to the end of the player's arm.
            float offset = 80f * Projectile.scale;
            Projectile.position = playerRotatedPoint - Projectile.Size * 0.5f + velocityAngle2.ToRotationVector2() * offset;

            // Sprite and player directioning.
            Projectile.spriteDirection = Projectile.direction;
            player.ChangeDir(Projectile.direction);

            // Prevents the projectile from dying
            Projectile.timeLeft = 2;

            // Player item-based field manipulation.
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
        }

        public void HandleChannelMovement(Player player, Vector2 playerRotatedPoint)
        {
            float speed = 1f;
            if (player.ActiveItem().shoot == Projectile.type)
            {
                speed = player.ActiveItem().shootSpeed * Projectile.scale;
            }
            Vector2 newVelocity = (Main.MouseWorld - playerRotatedPoint).SafeNormalize(Vector2.UnitX * player.direction) * speed;

            // Sync if a velocity component changes.
            if (Slashing)
            {
                if (Projectile.velocity.X != newVelocity.X || Projectile.velocity.Y != newVelocity.Y)
                {
                    Projectile.netUpdate = true;
                }
                Projectile.velocity = newVelocity;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.Organic())
                SoundEngine.PlaySound(Murasama.OrganicHit with { Pitch = (CurrentFrame == 0 ? -0.1f : CurrentFrame == 6 ? 0.1f : CurrentFrame == 10 ? -0.15f : 0) }, Projectile.Center);
            else
                SoundEngine.PlaySound(Murasama.InorganicHit with { Pitch = (CurrentFrame == 0 ? -0.1f : CurrentFrame == 6 ? 0.1f : CurrentFrame == 10 ? -0.15f : 0) }, Projectile.Center);

            for (int i = 0; i < 3; i++)
            {
                Color impactColor = CurrentFrame == 6 ? Main.rand.NextBool(3) ? Color.LightCoral : Color.White : Main.rand.NextBool(4) ? Color.LightCoral : Color.Crimson;
                float impactParticleScale = Main.rand.NextFloat(1f, 1.75f);

                if (CurrentFrame == 6)
                {
                    SparkleParticle impactParticle2 = new SparkleParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.75f, target.height * 0.75f), Vector2.Zero, Color.White, Color.Red, impactParticleScale * 1.2f, 8, 0, 4.5f);
                    GeneralParticleHandler.SpawnParticle(impactParticle2);
                }
                SparkleParticle impactParticle = new SparkleParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.75f, target.height * 0.75f), Vector2.Zero, impactColor, Color.Red, impactParticleScale, 8, 0, 2.5f);
                GeneralParticleHandler.SpawnParticle(impactParticle);
            }

            float sparkCount = MathHelper.Clamp(CurrentFrame == 6 ? 18 - Projectile.numHits * 3 : 5 - Projectile.numHits * 2, 0, 18);
            for (int i = 0; i < sparkCount; i++)
            {
                Vector2 sparkVelocity2 = Projectile.velocity.RotatedBy(CurrentFrame == 0 ? -0.45f * Owner.direction : CurrentFrame == 6 ? 0 : CurrentFrame == 10 ? 0.45f * Owner.direction : 0).RotatedByRandom(0.35f) * Main.rand.NextFloat(0.5f, 1.8f);
                int sparkLifetime2 = Main.rand.Next(23, 35);
                float sparkScale2 = Main.rand.NextFloat(0.95f, 1.8f);
                Color sparkColor2 = CurrentFrame == 6 ? Main.rand.NextBool(3) ? Color.Red : Color.IndianRed : Main.rand.NextBool() ? Color.Red : Color.Firebrick;
                if (Main.rand.NextBool())
                {
                    AltSparkParticle spark = new AltSparkParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f) + Projectile.velocity * 1.2f, sparkVelocity2 * (CurrentFrame == 6 ? 1f : 0.65f), false, (int)(sparkLifetime2 * (CurrentFrame == 6 ? 1.2f : 1f)), sparkScale2 * (CurrentFrame == 6 ? 1.4f : 1f), sparkColor2);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                else
                {
                    LineParticle spark = new LineParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f) + Projectile.velocity * 1.2f, sparkVelocity2 * (CurrentFrame == 7 ? 1f : 0.65f), false, (int)(sparkLifetime2 * (CurrentFrame == 7 ? 1.2f : 1f)), sparkScale2 * (CurrentFrame == 7 ? 1.4f : 1f), Main.rand.NextBool() ? Color.Red : Color.Firebrick);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
            float dustCount = MathHelper.Clamp(CurrentFrame == 6 ? 25 - Projectile.numHits * 3 : 12 - Projectile.numHits * 2, 0, 25);
            for (int i = 0; i <= dustCount; i++)
            {
                int dustID = Main.rand.NextBool(3) ? 182 : Main.rand.NextBool() ? CurrentFrame == 6 ? 309 : 296 : 90;
                Dust dust2 = Dust.NewDustPerfect(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f), dustID, Projectile.velocity.RotatedBy(CurrentFrame == 0 ? -0.45f * Owner.direction : CurrentFrame == 6 ? 0 : CurrentFrame == 10 ? 0.45f * Owner.direction : 0).RotatedByRandom(0.55f) * Main.rand.NextFloat(0.3f, 1.1f));
                dust2.scale = Main.rand.NextFloat(0.9f, 2.4f);
                dust2.noGravity = true;
            }
        }
        public override Color? GetAlpha(Color lightColor) => new Color(200, 0, 0, 0);

        public override bool? CanDamage() => Slashing == false ? false : null;
    }
}
