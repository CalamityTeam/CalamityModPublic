using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace CalamityMod.Projectiles.Magic
{
    public class RancorArm : ModProjectile
    {
        public Vector2 IdealPosition;
        public Player Owner => Main.player[projectile.owner];
        public float RotationDirection => projectile.rotation + projectile.ai[0];
        public ref float Time => ref projectile.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vengeful Arm");
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 4;
            projectile.friendly = true;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.timeLeft = 240;
            projectile.hide = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 12;
            projectile.ignoreWater = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.frame);
            writer.Write(projectile.rotation);
            writer.WriteVector2(IdealPosition);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.frame = reader.ReadInt32();
            projectile.rotation = reader.ReadSingle();
            IdealPosition = reader.ReadVector2();
        }

        public override void AI()
        {
            // Decide a frame to use and adjust the hitbox based on it.
            if (projectile.localAI[0] == 0f)
            {
                // Play a wraith death sound.
                Main.PlaySound(SoundID.NPCDeath52, projectile.Center);

                projectile.frame = Main.rand.Next(Main.projFrames[projectile.type]);

                Vector2 newSize;
                switch (projectile.frame)
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
                Vector2 idealCenter = projectile.Center;
                if (WorldUtils.Find(idealCenter.ToTileCoordinates(), Searches.Chain(new Searches.Down(25), new CustomConditions.SolidOrPlatform()), out Point result))
                {
                    idealCenter = result.ToWorldCoordinates();
                }
                idealCenter.Y += 36f;

                CalamityGlobalProjectile.ExpandHitboxBy(projectile, (int)newSize.X, (int)newSize.Y);

                // Sometimes use a slanted rotation.
                if (Main.rand.NextBool(3))
                    projectile.rotation = Main.rand.NextFloatDirection() * 0.3f;

                IdealPosition = idealCenter;
                projectile.localAI[0] = 1f;
                projectile.netUpdate = true;
            }

            // Rise from underneath the ideal position and fade in.
            // At the end of the projectile's lifetime the arm sinks back into the ground and withers away.
            float descendCompletion = Utils.InverseLerp(0f, 24f, projectile.timeLeft, true);
            float riseCompletion = Utils.InverseLerp(0f, 30f, Time, true);
            float heightOffset = -58f;
            if (projectile.height > 90f)
                heightOffset += 22f;

            projectile.Bottom = IdealPosition + Vector2.UnitY.RotatedBy(RotationDirection) * ((1f - riseCompletion * descendCompletion) * 64f + heightOffset);
            projectile.Opacity = (float)Math.Pow(riseCompletion, 2.4) * descendCompletion;
            projectile.scale = descendCompletion;

            Time++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            Vector2 bottom = projectile.Bottom;
            Vector2 top = projectile.Bottom - Vector2.UnitY.RotatedBy(RotationDirection) * projectile.height;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), bottom, top, projectile.width * projectile.scale, ref _);
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindNPCsAndTiles.Add(index);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            damage = Main.rand.Next(80, 90);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Rectangle frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 drawPosition = projectile.Center - Main.screenPosition;
            Color light = Lighting.GetColor((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16);
            Color drawColor = light * projectile.Opacity;
            spriteBatch.Draw(texture, drawPosition, frame, drawColor, RotationDirection, origin, new Vector2(projectile.scale, 1f), SpriteEffects.None, 0f);
            return false;
        }
    }
}
