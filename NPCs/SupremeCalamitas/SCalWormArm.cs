using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SupremeCalamitas
{
    public class SCalWormArm : ModNPC
    {
        public class SepulcherArmLimb
        {
            public Vector2 Center;
            public float Rotation;

            public SepulcherArmLimb(Vector2 center, float rotation)
            {
                Center = center;
                Rotation = rotation;
            }
            
            public void SendData(BinaryWriter writer)
            {
                writer.WritePackedVector2(Center);
                writer.Write(Rotation);
            }

            public void ReceiveData(BinaryReader reader)
            {
                Center = reader.ReadPackedVector2();
                Rotation = reader.ReadSingle();
            }
        }

        public NPC SegmentToAttachTo => Main.npc[(int)npc.ai[0]];
        public bool CurrentlyMoving
        {
            get => npc.ai[1] == 1f;
            set => npc.ai[1] = value.ToInt();
        }
        public Vector2 MoveDestination
        {
            get => new Vector2(npc.ai[2], npc.ai[3]);
            set
            {
                npc.ai[2] = value.X;
                npc.ai[3] = value.Y;
            }
        }
        public ref float Time => ref npc.localAI[0];
        public Player Target => Main.player[npc.target];
        public SepulcherArmLimb[] Limbs = new SepulcherArmLimb[4];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sepulcher");
        }

        public override void SetDefaults()
        {
            npc.damage = 0;
            npc.npcSlots = 5f;
            npc.width = 34;
            npc.height = 48;
            npc.defense = 0;
            CalamityGlobalNPC global = npc.Calamity();
            global.DR = 0.999999f;
            global.unbreakableDR = true;
			npc.lifeMax = CalamityWorld.revenge ? 345000 : 300000;
			npc.aiStyle = aiType = -1;
            npc.knockBackResist = 0f;
            npc.scale = Main.expertMode ? 1.35f : 1.2f;
            npc.dontTakeDamage = true;
            npc.alpha = 255;
            npc.chaseable = false;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.netAlways = true;
            npc.dontCountMe = true;

            for (int i = 0; i < Limbs.Length; i++)
                Limbs[i] = new SepulcherArmLimb(npc.Center, 0f);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            for (int i = 0; i < Limbs.Length; i++)
                Limbs[i].SendData(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            for (int i = 0; i < Limbs.Length; i++)
                Limbs[i].ReceiveData(reader);
        }

        public override void AI()
        {
            // Die if the segment is not present.
            if (!Main.npc.IndexInRange((int)npc.ai[0]) || !Main.npc[(int)npc.ai[0]].active)
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }

            npc.Opacity = SegmentToAttachTo.Opacity;
            npc.TargetClosest(false);

            Vector2 idealMovePosition = SegmentToAttachTo.Center;
            float rotationalOffset = (float)Math.Sin(Time / 60f + npc.whoAmI * 0.6f) * 0.69f;
            idealMovePosition += (SegmentToAttachTo.rotation - MathHelper.PiOver2).ToRotationVector2() * npc.scale * 290f;
            idealMovePosition += (SegmentToAttachTo.rotation - MathHelper.PiOver2 + MathHelper.PiOver2 * npc.direction + rotationalOffset).ToRotationVector2() * npc.scale * 120f;
            UpdateLimbs();

            if (CurrentlyMoving)
                MoveToDestination();

            MoveDestination = idealMovePosition;
            if (!CurrentlyMoving && !npc.WithinRange(idealMovePosition, 205f))
            {
                CurrentlyMoving = true;
                npc.netUpdate = true;
            }
            Vector2 center = npc.Center;
            CalamityUtils.DistanceClamp(ref center, ref idealMovePosition, 290f);
            npc.Center = center;
            Time++;
        }

        public void MoveToDestination()
        {
            npc.Center = Vector2.Lerp(npc.Center, MoveDestination, 0.05f);
            npc.Center = npc.Center.MoveTowards(MoveDestination, 10f);

            // Stop moving if sufficiently close to the desired destination.
            if (npc.WithinRange(MoveDestination, 15f))
            {
                npc.Center = MoveDestination;
                MoveDestination = Vector2.Zero;
                CurrentlyMoving = false;
                npc.netUpdate = true;
            }
        }

        public void UpdateLimbs()
        {
            Vector2 offsetFromSegment = Vector2.Zero;

            offsetFromSegment += new Vector2(npc.direction * 80f, 55f).RotatedBy(SegmentToAttachTo.rotation - (MathHelper.PiOver2 - 0.5f) * npc.direction).SafeNormalize(Vector2.UnitY) * 92f;
            Limbs[0].Center = SegmentToAttachTo.Center + offsetFromSegment;
            Limbs[0].Rotation = offsetFromSegment.ToRotation();

            Limbs[1].Center = SegmentToAttachTo.Center + offsetFromSegment * 1.5f + (npc.Center - Limbs[0].Center).SafeNormalize(Vector2.UnitY) * 84f;
            Limbs[1].Rotation = npc.AngleFrom(Limbs[0].Center);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
            Texture2D armTexture = Main.npcTexture[npc.type];
            Texture2D foreArmTexture = ModContent.GetTexture("CalamityMod/NPCs/SupremeCalamitas/SCalWormForeArm");
            Texture2D handTexture = ModContent.GetTexture("CalamityMod/NPCs/SupremeCalamitas/SCalWormHand");

            Vector2 forearmDrawPosition = Limbs[0].Center - Main.screenPosition;
            Color drawColor = Lighting.GetColor((int)(Limbs[0].Center.X / 16), (int)(Limbs[0].Center.Y / 16));
            spriteBatch.Draw(foreArmTexture, forearmDrawPosition, null, drawColor, Limbs[0].Rotation + MathHelper.PiOver2, foreArmTexture.Size() * 0.5f, npc.scale, SpriteEffects.None, 0f);

            Vector2 armDrawPosition = Limbs[1].Center - Main.screenPosition;
            drawColor = Lighting.GetColor((int)(Limbs[1].Center.X / 16), (int)(Limbs[1].Center.Y / 16));
            spriteBatch.Draw(armTexture, armDrawPosition, null, drawColor, Limbs[1].Rotation + MathHelper.PiOver2, armTexture.Size() * new Vector2(0.5f, 0f), npc.scale, SpriteEffects.None, 0f);

            Vector2 handDrawPosition = armDrawPosition;
            SpriteEffects handDirection = npc.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(handTexture, handDrawPosition, null, drawColor, Limbs[1].Rotation - MathHelper.PiOver2, handTexture.Size() * new Vector2(0.5f, 0f), npc.scale, handDirection, 0f);

            return false;
		}

        public override bool CheckActive() => false;

        public override bool PreNPCLoot() => false;

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;
    }
}
