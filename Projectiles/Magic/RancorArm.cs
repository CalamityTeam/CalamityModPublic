using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class RancorArm : ModProjectile
    {
        public Vector2 IdealPosition;
        public Player Owner => Main.player[Projectile.owner];
        public float RotationDirection => Projectile.rotation + Projectile.ai[0];
        public ref float Time => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vengeful Arm");
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 240;
            Projectile.hide = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 12;
            Projectile.ignoreWater = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.frame);
            writer.Write(Projectile.rotation);
            writer.WriteVector2(IdealPosition);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.frame = reader.ReadInt32();
            Projectile.rotation = reader.ReadSingle();
            IdealPosition = reader.ReadVector2();
        }

        public override void AI()
        {
            // Decide a frame to use and adjust the hitbox based on it.
            if (Projectile.localAI[0] == 0f)
            {
                // Play a wraith death sound.
                SoundEngine.PlaySound(SoundID.NPCDeath52, Projectile.Center);

                Projectile.frame = Main.rand.Next(Main.projFrames[Projectile.type]);

                Vector2 newSize;
                switch (Projectile.frame)
                {
                    case 0:
                        newSize = new Vector2(16f, 48f);
                        break;
                    case 1:
                        newSize = new Vector2(28f, 120f);
                        break;
                    case 2:
                        newSize = new Vector2(30f, 82f);
                        break;
                    case 3:
                        newSize = new Vector2(22f, 90f);
                        break;
                    case 4:
                        newSize = new Vector2(36f, 110f);
                        break;
                    case 5:
                    default:
                        newSize = new Vector2(28f, 50f);
                        break;
                }

                // Find the lowest point.
                Vector2 idealCenter = Projectile.Center;
                if (WorldUtils.Find(idealCenter.ToTileCoordinates(), Searches.Chain(new Searches.Down(25), new CustomConditions.SolidOrPlatform()), out Point result))
                {
                    idealCenter = result.ToWorldCoordinates();
                }
                idealCenter.Y += 36f;

                CalamityGlobalProjectile.ExpandHitboxBy(Projectile, (int)newSize.X, (int)newSize.Y);

                // Sometimes use a slanted rotation.
                if (Main.rand.NextBool(3))
                    Projectile.rotation = Main.rand.NextFloatDirection() * 0.3f;

                IdealPosition = idealCenter;
                Projectile.localAI[0] = 1f;
                Projectile.netUpdate = true;
            }

            // Rise from underneath the ideal position and fade in.
            // At the end of the projectile's lifetime the arm sinks back into the ground and withers away.
            float descendCompletion = Utils.GetLerpValue(0f, 24f, Projectile.timeLeft, true);
            float riseCompletion = Utils.GetLerpValue(0f, 30f, Time, true);
            float heightOffset = -58f;
            if (Projectile.height > 90f)
                heightOffset += 22f;

            Projectile.Bottom = IdealPosition + Vector2.UnitY.RotatedBy(RotationDirection) * ((1f - riseCompletion * descendCompletion) * 64f + heightOffset);
            Projectile.Opacity = (float)Math.Pow(riseCompletion, 2.4) * descendCompletion;
            Projectile.scale = descendCompletion;

            Time++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            Vector2 bottom = Projectile.Bottom;
            Vector2 top = Projectile.Bottom - Vector2.UnitY.RotatedBy(RotationDirection) * Projectile.height;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), bottom, top, Projectile.width * Projectile.scale, ref _);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            damage = Main.rand.Next(80, 90);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Color light = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16);
            Color drawColor = light * Projectile.Opacity;
            Color fadedColor = Color.Red * Projectile.Opacity * Projectile.scale * 0.4f;
            fadedColor.A = 0;

            Vector2 scale = new Vector2(Projectile.scale, 1f);
            float afterimageOffset = Projectile.Opacity * Projectile.scale * 5f;

            for (int i = 0; i < 6; i++)
            {
                Vector2 afterimageOffsetVector = (MathHelper.TwoPi * i / 6f).ToRotationVector2() * afterimageOffset;
                Main.EntitySpriteDraw(texture, drawPosition + afterimageOffsetVector, frame, fadedColor, RotationDirection, origin, scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, drawPosition, frame, drawColor, RotationDirection, origin, scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
