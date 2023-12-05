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
        public int time = 0;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 14;
        }

        public bool Slashing = false;
        public bool Slash1 => Projectile.frame == 10;
        public bool Slash2 => Projectile.frame == 0;
        public bool Slash3 => Projectile.frame == 6;

        public override void SetDefaults()
        {
            Projectile.width = 216;
            Projectile.height = 216;
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
            Rectangle frame = texture.Frame(verticalFrames: Main.projFrames[Type], frameY:Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            SpriteEffects spriteEffects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + (Projectile.velocity * 0.3f) + new Vector2(0, -32).RotatedBy(Projectile.rotation), frame, Color.White, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }

        public override void AI()
        {
            if (time == 0)
            {
                if (Main.zenithWorld)
                {
                    Projectile.scale = 2;
                    Projectile.damage = (int)(Projectile.damage * 2);
                }
                Projectile.frame = Main.zenithWorld ? 6 : 10;
                Projectile.alpha = 0;
                time++;
            }
            Player player = Main.player[Projectile.owner];
            if (Slash2)
            {
                SoundEngine.PlaySound(Murasama.Swing with { Pitch = -0.1f }, Projectile.Center);
                if (hitCooldown == 0)
                    Slashing = true;
                Projectile.numHits = 0;
            }
            else if (Slash3)
            {
                SoundEngine.PlaySound(Murasama.BigSwing with { Pitch = 0f }, Projectile.Center);
                if (hitCooldown == 0)
                    Slashing = true;
                Projectile.numHits = 0;
            }
            else if (Slash1)
            {
                SoundEngine.PlaySound(Murasama.Swing with { Pitch = -0.05f }, Projectile.Center);
                if (hitCooldown == 0)
                    Slashing = true;
                Projectile.numHits = 0;
            }
            else
                Slashing = false;

            if (Projectile.frame == 5 && Projectile.frameCounter % 3 == 0)
            {
                Projectile.damage = (int)(Projectile.damage * 2);
            }
            if (Projectile.frame == 7 && Projectile.frameCounter % 3 == 0)
            {
                Projectile.damage = (int)(Projectile.damage * 0.5f);
            }

            //Frames and crap
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 3 == 0)
            {
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
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
            if (Slashing || Slash1)
            {
                float velocityAngle = Projectile.velocity.ToRotation();
                Projectile.rotation = velocityAngle + (Projectile.direction == -1).ToInt() * MathHelper.Pi;
            }
            float velocityAngle2 = Projectile.velocity.ToRotation();
            Projectile.direction = (Math.Cos(velocityAngle2) > 0).ToDirectionInt();

            // Positioning close to the end of the player's arm.
            float offset = 80f * Projectile.scale;
            Projectile.Center = playerRotatedPoint + velocityAngle2.ToRotationVector2() * offset;

            // Sprite and player directioning.
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
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int size = 60;
            if (Slash3)
                hitbox.Inflate(size, size);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.Organic())
                SoundEngine.PlaySound(Murasama.OrganicHit with { Pitch = (Slash2 ? -0.1f : Slash3 ? 0.1f : Slash1 ? -0.15f : 0) }, Projectile.Center);
            else
                SoundEngine.PlaySound(Murasama.InorganicHit with { Pitch = (Slash2 ? -0.1f : Slash3 ? 0.1f : Slash1 ? -0.15f : 0) }, Projectile.Center);

            for (int i = 0; i < 3; i++)
            {
                Color impactColor = Slash3 ? Main.rand.NextBool(3) ? Color.LightCoral : Color.White : Main.rand.NextBool(4) ? Color.LightCoral : Color.Crimson;
                float impactParticleScale = Main.rand.NextFloat(1f, 1.75f);

                if (Slash3)
                {
                    SparkleParticle impactParticle2 = new SparkleParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.75f, target.height * 0.75f), Vector2.Zero, Color.White, Color.Red, impactParticleScale * 1.2f, 8, 0, 4.5f);
                    GeneralParticleHandler.SpawnParticle(impactParticle2);
                }
                SparkleParticle impactParticle = new SparkleParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.75f, target.height * 0.75f), Vector2.Zero, impactColor, Color.Red, impactParticleScale, 8, 0, 2.5f);
                GeneralParticleHandler.SpawnParticle(impactParticle);
            }

            float sparkCount = MathHelper.Clamp(Slash3 ? 18 - Projectile.numHits * 3 : 5 - Projectile.numHits * 2, 0, 18);
            for (int i = 0; i < sparkCount; i++)
            {
                Vector2 sparkVelocity2 = Projectile.velocity.RotatedBy(Slash2 ? -0.45f * Owner.direction : Slash3 ? 0 : Slash1 ? 0.45f * Owner.direction : 0).RotatedByRandom(0.35f) * Main.rand.NextFloat(0.5f, 1.8f);
                int sparkLifetime2 = Main.rand.Next(23, 35);
                float sparkScale2 = Main.rand.NextFloat(0.95f, 1.8f);
                Color sparkColor2 = Slash3 ? Main.rand.NextBool(3) ? Color.Red : Color.IndianRed : Main.rand.NextBool() ? Color.Red : Color.Firebrick;
                if (Main.rand.NextBool())
                {
                    AltSparkParticle spark = new AltSparkParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f) + Projectile.velocity * 1.2f, sparkVelocity2 * (Slash3 ? 1f : 0.65f), false, (int)(sparkLifetime2 * (Slash3 ? 1.2f : 1f)), sparkScale2 * (Slash3 ? 1.4f : 1f), sparkColor2);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                else
                {
                    LineParticle spark = new LineParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f) + Projectile.velocity * 1.2f, sparkVelocity2 * (Projectile.frame == 7 ? 1f : 0.65f), false, (int)(sparkLifetime2 * (Projectile.frame == 7 ? 1.2f : 1f)), sparkScale2 * (Projectile.frame == 7 ? 1.4f : 1f), Main.rand.NextBool() ? Color.Red : Color.Firebrick);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
            float dustCount = MathHelper.Clamp(Slash3 ? 25 - Projectile.numHits * 3 : 12 - Projectile.numHits * 2, 0, 25);
            for (int i = 0; i <= dustCount; i++)
            {
                int dustID = Main.rand.NextBool(3) ? 182 : Main.rand.NextBool() ? Slash3 ? 309 : 296 : 90;
                Dust dust2 = Dust.NewDustPerfect(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f), dustID, Projectile.velocity.RotatedBy(Slash2 ? -0.45f * Owner.direction : Slash3 ? 0 : Slash1 ? 0.45f * Owner.direction : 0).RotatedByRandom(0.55f) * Main.rand.NextFloat(0.3f, 1.1f));
                dust2.scale = Main.rand.NextFloat(0.9f, 2.4f);
                dust2.noGravity = true;
            }
        }
        public override Color? GetAlpha(Color lightColor) => new Color(100, 0, 0, 0);

        public override bool? CanDamage() => Slashing == false ? false : null;
    }
}
