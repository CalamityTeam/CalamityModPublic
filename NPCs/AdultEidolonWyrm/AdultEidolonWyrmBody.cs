using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AdultEidolonWyrm
{
    public class AdultEidolonWyrmBody : ModNPC
    {
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            DisplayName.SetDefault("Adult Eidolon Wyrm");
        }

        public override void SetDefaults()
        {
            NPC.damage = 100;
            NPC.width = 60;
            NPC.height = 88;
            NPC.defense = 0;
            NPC.LifeMaxNERB(2415000, 2898000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.Opacity = 0f;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.netAlways = true;
            NPC.dontCountMe = true;
            NPC.dontTakeDamage = true;
            NPC.chaseable = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

        public override void AI()
        {
            NPC.damage = 0;

            // Difficulty modes
            bool malice = CalamityWorld.malice;
            bool death = CalamityWorld.death;
            bool revenge = CalamityWorld.revenge;
            bool expertMode = Main.expertMode;

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            // Check if other segments are still alive, if not, die
            bool shouldDespawn = true;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<AdultEidolonWyrmHead>())
                {
                    shouldDespawn = false;
                    break;
                }
            }
            if (!shouldDespawn)
            {
                if (NPC.ai[1] <= 0f)
                    shouldDespawn = true;
                else if (Main.npc[(int)NPC.ai[1]].life <= 0)
                    shouldDespawn = true;
            }
            if (shouldDespawn)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.checkDead();
                NPC.active = false;
            }

            CalamityGlobalNPC calamityGlobalNPC_Head = Main.npc[(int)NPC.ai[2]].Calamity();

            float chargePhaseGateValue = malice ? 120f : death ? 180f : revenge ? 210f : expertMode ? 240f : 300f;
            float lightningChargePhaseGateValue = malice ? 90f : death ? 120f : revenge ? 135f : expertMode ? 150f : 180f;

            bool invisiblePartOfChargePhase = calamityGlobalNPC_Head.newAI[2] >= chargePhaseGateValue && calamityGlobalNPC_Head.newAI[2] <= chargePhaseGateValue + 1f && (calamityGlobalNPC_Head.newAI[0] == (float)AdultEidolonWyrmHead.Phase.ChargeOne || calamityGlobalNPC_Head.newAI[0] == (float)AdultEidolonWyrmHead.Phase.ChargeTwo || calamityGlobalNPC_Head.newAI[0] == (float)AdultEidolonWyrmHead.Phase.FastCharge);
            bool invisiblePartOfLightningChargePhase = calamityGlobalNPC_Head.newAI[2] >= lightningChargePhaseGateValue && calamityGlobalNPC_Head.newAI[2] <= lightningChargePhaseGateValue + 1f && calamityGlobalNPC_Head.newAI[0] == (float)AdultEidolonWyrmHead.Phase.LightningCharge;
            bool invisiblePhase = calamityGlobalNPC_Head.newAI[0] == 1f || calamityGlobalNPC_Head.newAI[0] == 5f || calamityGlobalNPC_Head.newAI[0] == 7f;
            if (!invisiblePartOfChargePhase && !invisiblePartOfLightningChargePhase && !invisiblePhase)
            {
                if (Main.npc[(int)NPC.ai[1]].Opacity > 0.5f)
                {
                    NPC.Opacity += 0.2f;
                    if (NPC.Opacity > 1f)
                        NPC.Opacity = 1f;
                }
            }
            else
            {
                NPC.Opacity -= 0.05f;
                if (NPC.Opacity < 0f)
                    NPC.Opacity = 0f;
            }

            bool spawnAncientLights = (calamityGlobalNPC_Head.newAI[0] == (float)AdultEidolonWyrmHead.Phase.ShadowFireballSpin && calamityGlobalNPC_Head.newAI[2] > 0f) ||
                (calamityGlobalNPC_Head.newAI[0] == (float)AdultEidolonWyrmHead.Phase.FinalPhase && calamityGlobalNPC_Head.newAI[1] > 0f);
            if (spawnAncientLights && Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (Vector2.Distance(NPC.Center, Main.player[Main.npc[(int)NPC.ai[2]].target].Center) > 160f)
                {
                    NPC.localAI[0] += 1f;
                    float spawnAncientLightGateValue = malice ? 90f : death ? 100f : revenge ? 105f : expertMode ? 110f : 120f;
                    float divisor = 4f;
                    if (NPC.ai[3] % divisor == 0f && NPC.localAI[0] >= spawnAncientLightGateValue)
                    {
                        NPC.localAI[0] = 0f;
                        float distanceVelocityBoost = MathHelper.Clamp((Vector2.Distance(Main.npc[(int)NPC.ai[2]].Center, Main.player[Main.npc[(int)NPC.ai[2]].target].Center) - 1600f) * 0.025f, 0f, 16f);
                        float lightVelocity = (Main.player[Main.npc[(int)NPC.ai[2]].target].Calamity().ZoneAbyssLayer4 ? 6f : 8f) + distanceVelocityBoost;
                        Vector2 destination = Main.player[Main.npc[(int)NPC.ai[2]].target].Center - NPC.Center;
                        Vector2 velocity = Vector2.Normalize(destination) * lightVelocity;
                        int type = NPCID.AncientLight;
                        float ai = (Main.rand.NextFloat() - 0.5f) * 0.3f * MathHelper.TwoPi / 60f;
                        int light = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, type, 0, 0f, ai, velocity.X, velocity.Y, 255);
                        Main.npc[light].velocity = velocity;
                    }
                }
            }

            Vector2 vector18 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
            float num191 = Main.player[NPC.target].position.X + (Main.player[NPC.target].width / 2);
            float num192 = Main.player[NPC.target].position.Y + (Main.player[NPC.target].height / 2);
            num191 = (int)(num191 / 16f) * 16;
            num192 = (int)(num192 / 16f) * 16;
            vector18.X = (int)(vector18.X / 16f) * 16;
            vector18.Y = (int)(vector18.Y / 16f) * 16;
            num191 -= vector18.X;
            num192 -= vector18.Y;

            float num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);
            if (NPC.ai[1] > 0f && NPC.ai[1] < Main.npc.Length)
            {
                try
                {
                    vector18 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
                    num191 = Main.npc[(int)NPC.ai[1]].position.X + (Main.npc[(int)NPC.ai[1]].width / 2) - vector18.X;
                    num192 = Main.npc[(int)NPC.ai[1]].position.Y + (Main.npc[(int)NPC.ai[1]].height / 2) - vector18.Y;
                } catch
                {
                }

                NPC.rotation = (float)Math.Atan2(num192, num191) + MathHelper.PiOver2;
                num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);
                int num194 = NPC.width;
                num193 = (num193 - num194) / num193;
                num191 *= num193;
                num192 *= num193;
                NPC.velocity = Vector2.Zero;
                NPC.position.X = NPC.position.X + num191;
                NPC.position.Y = NPC.position.Y + num192;

                if (num191 < 0f)
                    NPC.spriteDirection = -1;
                else if (num191 > 0f)
                    NPC.spriteDirection = 1;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 center = new Vector2(NPC.Center.X, NPC.Center.Y);
            Vector2 vector11 = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / 2);
            Vector2 vector = center - screenPos;
            vector -= new Vector2(ModContent.Request<Texture2D>("CalamityMod/NPCs/AdultEidolonWyrm/EidolonWyrmBodyGlowHuge").Value.Width, ModContent.Request<Texture2D>("CalamityMod/NPCs/AdultEidolonWyrm/EidolonWyrmBodyGlowHuge").Value.Height) * 0.5f;
            vector += vector11 * 1f + new Vector2(0f, 4f + NPC.gfxOffY);
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityMod/NPCs/AdultEidolonWyrm/EidolonWyrmBodyGlowHuge").Value, vector,
                new Microsoft.Xna.Framework.Rectangle?(NPC.frame), Color.White, NPC.rotation, vector11, 1f, spriteEffects, 0f);
        }

        public override bool CheckActive() => false;

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, hitDirection, -1f, 0, default, 1f);

                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, Mod.Find<ModGore>("WyrmAdult2").Type, 1f);
                }
            }
        }
    }
}
