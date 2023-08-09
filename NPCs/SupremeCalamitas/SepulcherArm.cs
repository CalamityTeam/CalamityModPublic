using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SupremeCalamitas
{
    public class SepulcherArm : ModNPC
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

        public NPC SegmentToAttachTo => Main.npc[(int)NPC.ai[0]];
        public bool CurrentlyMoving
        {
            get => NPC.ai[1] == 1f;
            set => NPC.ai[1] = value.ToInt();
        }
        public Vector2 MoveDestination
        {
            get => new(NPC.ai[2], NPC.ai[3]);
            set
            {
                NPC.ai[2] = value.X;
                NPC.ai[3] = value.Y;
            }
        }
        public ref float Time => ref NPC.localAI[0];
        public bool ReelingBack
        {
            get => NPC.localAI[1] == 0f;
            set => NPC.localAI[1] = 1f - value.ToInt();
        }
        public Player Target => Main.player[NPC.target];
        public SepulcherArmLimb[] Limbs = new SepulcherArmLimb[4];
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.SepulcherHead.DisplayName");
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.damage = 0;
            NPC.npcSlots = 5f;
            NPC.width = 34;
            NPC.height = 48;
            NPC.defense = 0;
            CalamityGlobalNPC global = NPC.Calamity();
            global.DR = 0.999999f;
            global.unbreakableDR = true;
            NPC.lifeMax = CalamityWorld.revenge ? 345000 : 300000;
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.scale *= Main.expertMode ? 1.35f : 1.2f;
            NPC.dontTakeDamage = true;
            NPC.alpha = 255;
            NPC.chaseable = false;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.netAlways = true;
            NPC.dontCountMe = true;

            for (int i = 0; i < Limbs.Length; i++)
                Limbs[i] = new SepulcherArmLimb(NPC.Center, 0f);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            for (int i = 0; i < Limbs.Length; i++)
                Limbs[i].SendData(writer);
            writer.Write(NPC.rotation);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            for (int i = 0; i < Limbs.Length; i++)
                Limbs[i].ReceiveData(reader);
            NPC.rotation = reader.ReadSingle();
        }

        public override void AI()
        {
            // Die if the segment is not present.
            if (!Main.npc.IndexInRange((int)NPC.ai[0]) || !Main.npc[(int)NPC.ai[0]].active)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.checkDead();
                return;
            }

            NPC.Opacity = SegmentToAttachTo.Opacity;
            NPC.TargetClosest(false);

            Vector2 idealMovePosition = SegmentToAttachTo.Center;
            float sideFactor = MathHelper.Lerp(200f, 18f, Utils.GetLerpValue(-0.51f, -0.06f, NPC.rotation, true));
            float aheadFactor = MathHelper.Lerp(284f, 680f, Utils.GetLerpValue(-0.51f, -0.06f, NPC.rotation, true));
            idealMovePosition += (SegmentToAttachTo.rotation + NPC.rotation * NPC.direction - MathHelper.PiOver2).ToRotationVector2() * NPC.scale * aheadFactor;
            idealMovePosition += (SegmentToAttachTo.rotation + NPC.rotation * NPC.direction - MathHelper.PiOver2 + MathHelper.PiOver2 * NPC.direction).ToRotationVector2() * NPC.scale * sideFactor;
            NPC.Center = idealMovePosition;
            UpdateLimbs();

            if (ReelingBack)
            {
                NPC.rotation -= 0.066f;
                if (NPC.rotation < -0.55f)
                    ReelingBack = false;
            }
            else
            {
                NPC.rotation += 0.029f;
                if (NPC.rotation > 0.77f)
                    ReelingBack = true;
            }

            Time++;
        }

        public void MoveToDestination()
        {
            NPC.Center = Vector2.Lerp(NPC.Center, MoveDestination, 0.05f);
            NPC.Center = NPC.Center.MoveTowards(MoveDestination, 10f);

            // Stop moving if sufficiently close to the desired destination.
            if (NPC.WithinRange(MoveDestination, 15f))
            {
                NPC.Center = MoveDestination;
                MoveDestination = Vector2.Zero;
                CurrentlyMoving = false;
                NPC.netUpdate = true;
            }
        }

        public void UpdateLimbs()
        {
            Vector2 offsetFromSegment = Vector2.Zero;

            offsetFromSegment += new Vector2(NPC.direction * 60f, 55f).RotatedBy(SegmentToAttachTo.rotation - (MathHelper.PiOver2 - NPC.rotation * 1.7f - 0.77f) * NPC.direction).SafeNormalize(Vector2.UnitY) * 92f;
            Limbs[0].Center = SegmentToAttachTo.Center + offsetFromSegment;
            Limbs[0].Rotation = offsetFromSegment.ToRotation();

            Limbs[1].Rotation = NPC.AngleFrom(Limbs[0].Center);
            Limbs[1].Center = Limbs[0].Center + offsetFromSegment * 0.5f + (NPC.Center - Limbs[0].Center).SafeNormalize(Vector2.UnitY) * 84f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor_Unused)
        {
            Texture2D armTexture = TextureAssets.Npc[NPC.type].Value;
            Texture2D foreArmTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/SupremeCalamitas/SepulcherForearm").Value;
            Texture2D handTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/SupremeCalamitas/SepulcherHand").Value;

            Vector2 forearmDrawPosition = Limbs[0].Center - screenPos;
            Color drawColor = Lighting.GetColor((int)(Limbs[0].Center.X / 16), (int)(Limbs[0].Center.Y / 16));
            spriteBatch.Draw(foreArmTexture, forearmDrawPosition, null, drawColor, Limbs[0].Rotation + MathHelper.PiOver2, foreArmTexture.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0f);

            Vector2 armDrawPosition = Limbs[1].Center - screenPos;
            drawColor = Lighting.GetColor((int)(Limbs[1].Center.X / 16), (int)(Limbs[1].Center.Y / 16));
            spriteBatch.Draw(armTexture, armDrawPosition, null, drawColor, Limbs[1].Rotation + MathHelper.PiOver2, armTexture.Size() * new Vector2(0.5f, 0f), NPC.scale, SpriteEffects.FlipVertically, 0f);

            Vector2 handDrawPosition = armDrawPosition;
            SpriteEffects handDirection = NPC.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(handTexture, handDrawPosition, null, drawColor, Limbs[1].Rotation - MathHelper.PiOver2, handTexture.Size() * new Vector2(0.5f, 0f), NPC.scale, handDirection, 0f);

            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Vector2 forearmGoreSpawnPosition = Limbs[0].Center + Main.rand.NextVector2Circular(6f, 6f);
                    Vector2 armGoreSpawnPosition = Limbs[1].Center + Main.rand.NextVector2Circular(6f, 6f);
                    Vector2 handGoreSpawnPosition = armGoreSpawnPosition + Main.rand.NextVector2Circular(6f, 6f);

                    Gore.NewGorePerfect(NPC.GetSource_Death(), armGoreSpawnPosition, Main.rand.NextVector2Circular(3f, 3f), Mod.Find<ModGore>($"SepulcherArm_Gore").Type, NPC.scale);
                    for (int i = 1; i <= 2; i++)
                        Gore.NewGorePerfect(NPC.GetSource_Death(), forearmGoreSpawnPosition, Main.rand.NextVector2Circular(3f, 3f), Mod.Find<ModGore>($"SepulcherForearm_Gore{i}").Type, NPC.scale);
                    for (int i = 1; i <= 2; i++)
                        Gore.NewGorePerfect(NPC.GetSource_Death(), handGoreSpawnPosition, Main.rand.NextVector2Circular(3f, 3f), Mod.Find<ModGore>($"SepulcherHand_Gore{i}").Type, NPC.scale);
                }
            }
        }

        public override bool CheckActive() => false;

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;
    }
}
