using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
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
        public bool ReelingBack
        {
            get => npc.localAI[1] == 0f;
            set => npc.localAI[1] = 1f - value.ToInt();
        }
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
            npc.netAlways = true;
            npc.dontCountMe = true;

            for (int i = 0; i < Limbs.Length; i++)
                Limbs[i] = new SepulcherArmLimb(npc.Center, 0f);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            for (int i = 0; i < Limbs.Length; i++)
                Limbs[i].SendData(writer);
            writer.Write(npc.rotation);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            for (int i = 0; i < Limbs.Length; i++)
                Limbs[i].ReceiveData(reader);
            npc.rotation = reader.ReadSingle();
        }

        public override void AI()
        {
            // Die if the segment is not present.
            if (!Main.npc.IndexInRange((int)npc.ai[0]) || !Main.npc[(int)npc.ai[0]].active)
            {
                npc.life = 0;
                npc.HitEffect(0, 10.0);
                npc.checkDead();
                return;
            }

            npc.Opacity = SegmentToAttachTo.Opacity;
            npc.TargetClosest(false);

            Vector2 idealMovePosition = SegmentToAttachTo.Center;
            float sideFactor = MathHelper.Lerp(200f, 18f, Utils.InverseLerp(-0.51f, -0.06f, npc.rotation, true));
            float aheadFactor = MathHelper.Lerp(284f, 680f, Utils.InverseLerp(-0.51f, -0.06f, npc.rotation, true));
            idealMovePosition += (SegmentToAttachTo.rotation + npc.rotation * npc.direction - MathHelper.PiOver2).ToRotationVector2() * npc.scale * aheadFactor;
            idealMovePosition += (SegmentToAttachTo.rotation + npc.rotation * npc.direction - MathHelper.PiOver2 + MathHelper.PiOver2 * npc.direction).ToRotationVector2() * npc.scale * sideFactor;
            npc.Center = idealMovePosition;
            UpdateLimbs();

            if (ReelingBack)
            {
                npc.rotation -= 0.066f;
                if (npc.rotation < -0.55f)
                    ReelingBack = false;
            }
            else
            {
                npc.rotation += 0.029f;
                if (npc.rotation > 0.77f)
                    ReelingBack = true;
            }

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

            offsetFromSegment += new Vector2(npc.direction * 60f, 55f).RotatedBy(SegmentToAttachTo.rotation - (MathHelper.PiOver2 - npc.rotation * 1.7f - 0.77f) * npc.direction).SafeNormalize(Vector2.UnitY) * 92f;
            Limbs[0].Center = SegmentToAttachTo.Center + offsetFromSegment;
            Limbs[0].Rotation = offsetFromSegment.ToRotation();

            Limbs[1].Rotation = npc.AngleFrom(Limbs[0].Center);
            Limbs[1].Center = Limbs[0].Center + offsetFromSegment * 0.5f + (npc.Center - Limbs[0].Center).SafeNormalize(Vector2.UnitY) * 84f;
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
            spriteBatch.Draw(armTexture, armDrawPosition, null, drawColor, Limbs[1].Rotation + MathHelper.PiOver2, armTexture.Size() * new Vector2(0.5f, 0f), npc.scale, SpriteEffects.FlipVertically, 0f);

            Vector2 handDrawPosition = armDrawPosition;
            SpriteEffects handDirection = npc.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(handTexture, handDrawPosition, null, drawColor, Limbs[1].Rotation - MathHelper.PiOver2, handTexture.Size() * new Vector2(0.5f, 0f), npc.scale, handDirection, 0f);

            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                Vector2 forearmGoreSpawnPosition = Limbs[0].Center + Main.rand.NextVector2Circular(6f, 6f);
                Vector2 armGoreSpawnPosition = Limbs[1].Center + Main.rand.NextVector2Circular(6f, 6f);
                Vector2 handGoreSpawnPosition = armGoreSpawnPosition + Main.rand.NextVector2Circular(6f, 6f);

                Gore.NewGorePerfect(armGoreSpawnPosition, Main.rand.NextVector2Circular(3f, 3f), mod.GetGoreSlot($"Gores/SupremeCalamitas/SepulcherArm_Gore"), npc.scale);
                for (int i = 1; i <= 2; i++)
                    Gore.NewGorePerfect(forearmGoreSpawnPosition, Main.rand.NextVector2Circular(3f, 3f), mod.GetGoreSlot($"Gores/SupremeCalamitas/SepulcherForearm_Gore{i}"), npc.scale);
                for (int i = 1; i <= 2; i++)
                    Gore.NewGorePerfect(handGoreSpawnPosition, Main.rand.NextVector2Circular(3f, 3f), mod.GetGoreSlot($"Gores/SupremeCalamitas/SepulcherHand_Gore{i}"), npc.scale);
            }
        }

        public override bool CheckActive() => false;

        public override bool PreNPCLoot() => false;

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;
    }
}
