using System.IO;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SupremeCalamitas
{
    [LongDistanceNetSync(SyncWith = typeof(SepulcherHead))]
    public class SepulcherBody : ModNPC
    {
        private bool setAlpha = false;
        public NPC AheadSegment => Main.npc[(int)NPC.ai[1]];
        public NPC HeadSegment => Main.npc[(int)NPC.ai[2]];
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.SepulcherHead.DisplayName");

        public static Asset<Texture2D> AltTexture;

        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            if (!Main.dedServ)
            {
                AltTexture = ModContent.Request<Texture2D>(Texture + "Alt", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetDefaults()
        {
            NPC.damage = 0;
            NPC.npcSlots = 5f;
            NPC.width = NPC.height = 48;
            NPC.defense = 0;
            CalamityGlobalNPC global = NPC.Calamity();
            global.DR = 0.999999f;
            global.unbreakableDR = true;
            NPC.lifeMax = CalamityWorld.revenge ? 345000 : 300000;
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.scale *= Main.expertMode ? 1.35f : 1.2f;
            NPC.alpha = 255;
            NPC.chaseable = false;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.netAlways = true;
            NPC.dontCountMe = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[3]);
            writer.Write(setAlpha);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[3] = reader.ReadSingle();
            setAlpha = reader.ReadBoolean();
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            if (NPC.ai[2] > 0f)
            {
                NPC.realLife = (int)NPC.ai[2];
            }

            bool shouldDie = false;
            if (NPC.ai[1] <= 0f)
                shouldDie = true;
            else if (AheadSegment.life <= 0 || !AheadSegment.active || NPC.life <= 0)
                shouldDie = true;

            if (shouldDie)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.checkDead();
            }

            if (AheadSegment.alpha < 128 && !setAlpha)
            {
                if (NPC.alpha != 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Dust fire = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.TheDestroyer, 0f, 0f, 100, default, 2f);
                        fire.noGravity = true;
                        fire.noLight = true;
                    }
                }
                NPC.alpha -= 42;
                if (NPC.alpha <= 0)
                {
                    setAlpha = true;
                    NPC.alpha = 0;
                }
            }
            else
                NPC.alpha = HeadSegment.alpha;

            if (Main.npc.IndexInRange((int)NPC.ai[1]))
            {
                Vector2 offsetToAheadSegment = AheadSegment.Center - NPC.Center;
                NPC.rotation = offsetToAheadSegment.ToRotation() + MathHelper.PiOver2;
                NPC.velocity = Vector2.Zero;
                NPC.Center = AheadSegment.Center - offsetToAheadSegment.SafeNormalize(Vector2.UnitY) * 52f;
                NPC.spriteDirection = (offsetToAheadSegment.X > 0f).ToDirectionInt();
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = NPC.localAI[3] / 2f % 2f == 0f ? AltTexture.Value : TextureAssets.Npc[NPC.type].Value;
            Vector2 halfSizeTexture = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / 2));

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height)) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    int variant = (int)(NPC.localAI[3] / 2f % 2f);
                    if (variant == 0)
                    {
                        for (int i = 1; i <= 9; i++)
                        {
                            if (!Main.rand.NextBool(3))
                                continue;

                            Vector2 goreSpawnPosition = NPC.Center;
                            Gore.NewGorePerfect(NPC.GetSource_Death(), goreSpawnPosition, Main.rand.NextVector2Circular(2f, 2f), Mod.Find<ModGore>($"SepulcherBody1_Gore{i}").Type, NPC.scale);
                        }
                    }
                    else
                    {
                        for (int i = 1; i <= 7; i++)
                        {
                            if (!Main.rand.NextBool(3))
                                continue;

                            Vector2 goreSpawnPosition = NPC.Center;
                            Gore.NewGorePerfect(NPC.GetSource_Death(), goreSpawnPosition, Main.rand.NextVector2Circular(2f, 2f), Mod.Find<ModGore>($"SepulcherBody2_Gore{i}").Type, NPC.scale);
                        }
                    }
                }
            }
        }
    }
}
