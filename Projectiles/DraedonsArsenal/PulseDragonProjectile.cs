using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class PulseDragonProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public bool ReelingBack
        {
            get => Projectile.timeLeft <= ReelbackTime;
            set
            {
                if (value)
                    Projectile.timeLeft = ReelbackTime;
            }
        }

        public Player Owner => Main.player[Projectile.owner];
        public float Outwardness => OutwardnessMax * (float)Math.Sin(Projectile.timeLeft / (float)Lifetime * MathHelper.Pi);
        public ref float InitialRotation => ref Projectile.ai[0];
        public ref float OutwardnessMax => ref Projectile.ai[1];
        public ref float SwingDirection => ref Projectile.localAI[0];
        public const int ChargeTime = 25;
        public const int ReelbackTime = 25;
        public const int Lifetime = ChargeTime + ReelbackTime;
        public const float MaximumPossibleOutwardness = 72f;
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.timeLeft = Lifetime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(SwingDirection);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            SwingDirection = reader.ReadSingle();
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.AngleTo(Owner.Center) - MathHelper.PiOver2;
            if (Owner.dead)
            {
                Projectile.Kill();
                return;
            }

            Projectile.direction = (Owner.Center.X - Projectile.Center.X > 0).ToDirectionInt();
            Projectile.spriteDirection = Projectile.direction;

            ManipulateOwnerFields();

            if (Projectile.timeLeft >= Lifetime - ChargeTime)
            {
                float time = Utils.GetLerpValue(Lifetime, Lifetime - ChargeTime, Projectile.timeLeft, true);
                float offsetAngle = MathHelper.Lerp(-1.1f, 1.5f, time); // Set the range of the offset to -1.8 and 1.8 based off of the time.
                offsetAngle *= Projectile.localAI[0]; // Incorporate direction into the offset angle.

                Projectile.velocity = InitialRotation.ToRotationVector2().RotatedBy(offsetAngle) * 29f;

                // Adjust the velocity to go along with the player's if the velocity directions are similar so that the player's movement doesn't hinder the projectile's movement.
                if (Vector2.Dot(Projectile.velocity.SafeNormalize(Vector2.Zero), Owner.velocity.SafeNormalize(Vector2.Zero)) > 0.45f)
                {
                    Projectile.velocity += Owner.velocity;
                }
            }
            else
            {
                Projectile.velocity = Projectile.SafeDirectionTo(Owner.Center, Vector2.UnitX * Owner.direction) * 43f;
                Projectile.timeLeft = ReelbackTime;
                if (Projectile.WithinRange(Owner.Center, 45f))
                    Projectile.Kill();
            }

            GenerateIdleDust();
            if (Projectile.timeLeft % 3 == 2 && Projectile.Distance(Owner.Center) > 40f)
                SpawnElectricFields();
        }

        public void ManipulateOwnerFields()
        {
            Owner.itemAnimation = 4;
            Owner.itemTime = 4;
            Owner.ChangeDir(-Projectile.direction);
            Owner.itemRotation = Projectile.rotation + (Projectile.spriteDirection == -1).ToInt() * MathHelper.TwoPi;
        }

        public void GenerateIdleDust()
        {
            if (Main.dedServ)
                return;
            for (int i = 0; i < 4; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(52f, 52f), 261);
                dust.velocity = Vector2.Zero;
                dust.noGravity = true;
            }
        }

        public void SpawnElectricFields()
        {
            if (Main.myPlayer != Projectile.owner)
                return;
            Projectile field = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileID.Electrosphere, Projectile.damage, Projectile.knockBack, Projectile.owner);
            if (field.whoAmI.WithinBounds(Main.maxProjectiles))
            {
                field.DamageType = DamageClass.MeleeNoSpeed;
                field.usesLocalNPCImmunity = true;
                field.localNPCHitCooldown = 3;
                field.timeLeft = 12;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D chainTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/DraedonsArsenal/PulseDragonChain").Value;
            Texture2D pulseTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/SmallGreyscaleCircle").Value;
            Texture2D dragonHeadTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/DraedonsArsenal/PulseDragonProjectile").Value;
            if (ReelingBack)
                dragonHeadTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/DraedonsArsenal/PulseDragonHeadClosed").Value;
            Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;

            // Chain drawing.
            List<Vector2> bezierPoints = new List<Vector2>()
            {
                mountedCenter
            };
            for (int i = 0; i < 20; i++)
            {
                Vector2 offset = Vector2.UnitX * -SwingDirection;
                offset *= Outwardness * (float)Math.Sin(i / 20f * MathHelper.Pi);
                offset *= Utils.GetLerpValue(0f, 300f, Owner.Distance(Vector2.Lerp(mountedCenter, Projectile.Center, i / 20f) + offset), true);
                bezierPoints.Add(Vector2.Lerp(mountedCenter, Projectile.Center, i / 20f) + offset);
            }
            bezierPoints.Add(Projectile.Center);

            BezierCurve bezierCurve = new BezierCurve(bezierPoints.ToArray());
            int totalChains = (int)(Projectile.Distance(mountedCenter) / chainTexture.Height);
            totalChains = (int)MathHelper.Clamp(totalChains, 40f, 1000f);
            for (int i = 0; i < totalChains - 1; i++)
            {
                Vector2 drawPosition = bezierCurve.Evaluate(i / (float)totalChains);
                float angle = (bezierCurve.Evaluate(i / (float)totalChains + 1f / totalChains) - drawPosition).ToRotation();
                angle -= MathHelper.PiOver2;
                Main.EntitySpriteDraw(chainTexture, drawPosition - Main.screenPosition, null, lightColor, angle, chainTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            }

            // Aura drawing.
            for (int i = 0; i < 5; i++)
            {
                Vector2 offset = (i / 5f * MathHelper.TwoPi).ToRotationVector2() * 24f;
                float time = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1.8f);
                float angle = time * MathHelper.Pi + Main.GlobalTimeWrappedHourly * 2.1f;
                float scale = 1.1f + time * 0.2f;
                Main.EntitySpriteDraw(pulseTexture, Projectile.Center + offset - Main.screenPosition, null, Color.Cyan * 0.3f, angle, pulseTexture.Size() * 0.5f, scale, SpriteEffects.None, 0);
            }

            // Head drawing.
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(dragonHeadTexture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, dragonHeadTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
