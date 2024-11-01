using System.IO;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SupremeCalamitas
{
    [LongDistanceNetSync(SyncWith = typeof(SepulcherHead))]
    public class SepulcherTail : ModNPC
    {
        private bool setAlpha = false;
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.SepulcherHead.DisplayName");
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.damage = 0;
            NPC.npcSlots = 5f;
            NPC.width = 20;
            NPC.height = 20;
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
            writer.Write(setAlpha);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
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
            {
                shouldDie = true;
            }
            else if (Main.npc[(int)NPC.ai[1]].life <= 0 || NPC.life <= 0)
            {
                shouldDie = true;
            }
            if (shouldDie)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.checkDead();
            }

            if (Main.npc[(int)NPC.ai[1]].alpha < 128 && !setAlpha)
            {
                if (NPC.alpha != 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        int redDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TheDestroyer, 0f, 0f, 100, default, 2f);
                        Main.dust[redDust].noGravity = true;
                        Main.dust[redDust].noLight = true;
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
            {
                NPC.alpha = Main.npc[(int)NPC.ai[2]].alpha;
            }

            Vector2 segmentLocation = NPC.Center;
            float targetX = Main.player[NPC.target].position.X + (Main.player[NPC.target].width / 2);
            float targetY = Main.player[NPC.target].position.Y + (Main.player[NPC.target].height / 2);
            targetX = (int)(targetX / 16f) * 16;
            targetY = (int)(targetY / 16f) * 16;
            segmentLocation.X = (int)(segmentLocation.X / 16f) * 16;
            segmentLocation.Y = (int)(segmentLocation.Y / 16f) * 16;
            targetX -= segmentLocation.X;
            targetY -= segmentLocation.Y;
            float targetDistance = (float)System.Math.Sqrt(targetX * targetX + targetY * targetY);
            if (NPC.ai[1] > 0f && NPC.ai[1] < Main.npc.Length)
            {
                try
                {
                    segmentLocation = NPC.Center;
                    targetX = Main.npc[(int)NPC.ai[1]].position.X + (Main.npc[(int)NPC.ai[1]].width / 2) - segmentLocation.X;
                    targetY = Main.npc[(int)NPC.ai[1]].position.Y + (Main.npc[(int)NPC.ai[1]].height / 2) - segmentLocation.Y;
                }
                catch
                {
                }
                NPC.rotation = (float)System.Math.Atan2(targetY, targetX) + 1.57f;
                targetDistance = (float)System.Math.Sqrt(targetX * targetX + targetY * targetY);
                int npcWidth = NPC.width;
                targetDistance = (targetDistance - npcWidth) / targetDistance;
                targetX *= targetDistance;
                targetY *= targetDistance;
                NPC.velocity = Vector2.Zero;
                NPC.position.X = NPC.position.X + targetX;
                NPC.position.Y = NPC.position.Y + targetY;
                if (targetX < 0f)
                {
                    NPC.spriteDirection = -1;
                }
                else if (targetX > 0f)
                {
                    NPC.spriteDirection = 1;
                }
            }

            if (Main.zenithWorld && !NPC.AnyNPCs(ModContent.NPCType<BrimstoneHeart>()))
            {
                CalamityGlobalNPC global = NPC.Calamity();
                global.DR = 0.5f;
                global.unbreakableDR = false;
                NPC.chaseable = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 halfSizeTexture = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / 2));

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height)) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            // hit sound in gfb
            if (NPC.soundDelay == 0 && NPC.Calamity().unbreakableDR == false)
            {
                NPC.soundDelay = Main.rand.Next(5, 8);
                SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt, NPC.Center);
            }
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 1; i <= 2; i++)
                    {
                        Vector2 goreSpawnPosition = NPC.Center;
                        if (i == 2)
                            goreSpawnPosition -= (NPC.rotation - MathHelper.PiOver2).ToRotationVector2() * 20f;
                        Gore.NewGorePerfect(NPC.GetSource_Death(), goreSpawnPosition, Main.rand.NextVector2Circular(3f, 3f), Mod.Find<ModGore>($"SepulcherTail_Gore{i}").Type, NPC.scale);
                    }
                }
            }
        }

        public override bool CheckActive()
        {
            return false;
        }
    }
}
