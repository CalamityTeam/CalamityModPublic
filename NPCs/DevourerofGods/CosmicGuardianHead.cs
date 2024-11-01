using System;
using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DevourerofGods
{
    [LongDistanceNetSync]
    public class CosmicGuardianHead : ModNPC
    {
        private bool tail = false;
        private const int minLength = 10;
        private const int maxLength = 11;
        private int invinceTime = 180;

        public static Asset<Texture2D> GlowTexture;
        public static Asset<Texture2D> GlowTexture2;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.75f,
                PortraitScale = 0.75f,
                CustomTexturePath = "CalamityMod/ExtraTextures/Bestiary/CosmicGuardian_Bestiary",
                PortraitPositionXOverride = 40,
                PortraitPositionYOverride = 40
            };
            value.Position.X += 62f;
            value.Position.Y += 35f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
                GlowTexture2 = ModContent.Request<Texture2D>(Texture + "Glow2", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.GetNPCDamage();
            NPC.width = 64;
            NPC.height = 76;
            NPC.defense = 40;
            NPC.lifeMax = 50000;
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.Opacity = 0f;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.netAlways = true;

            // Scale stats in Expert and Master
            CalamityGlobalNPC.AdjustExpertModeStatScaling(NPC);
            CalamityGlobalNPC.AdjustMasterModeStatScaling(NPC);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = ModContent.NPCType<DevourerofGodsHead>();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.CosmicGuardian")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(invinceTime);
            writer.Write(NPC.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            invinceTime = reader.ReadInt32();
            NPC.dontTakeDamage = reader.ReadBoolean();
        }

        public override void AI()
        {
            if (invinceTime > 0)
            {
                invinceTime--;
                NPC.damage = 0;
                NPC.dontTakeDamage = true;
            }
            else
            {
                NPC.damage = NPC.defDamage;
                NPC.dontTakeDamage = false;
            }

            Vector2 vector = NPC.Center;

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!tail && NPC.ai[0] == 0f)
                {
                    int Previous = NPC.whoAmI;
                    for (int segmentSpawn = 0; segmentSpawn < maxLength; segmentSpawn++)
                    {
                        int segment;
                        if (segmentSpawn >= 0 && segmentSpawn < minLength)
                        {
                            segment = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<CosmicGuardianBody>(), NPC.whoAmI);
                        }
                        else
                        {
                            segment = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<CosmicGuardianTail>(), NPC.whoAmI);
                        }
                        Main.npc[segment].realLife = NPC.whoAmI;
                        Main.npc[segment].ai[2] = NPC.whoAmI;
                        Main.npc[segment].ai[1] = Previous;
                        Main.npc[Previous].ai[0] = segment;
                        NPC.netUpdate = true;
                        Previous = segment;
                    }
                    tail = true;
                }
                if (!NPC.active && Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                }
            }

            if (CalamityGlobalNPC.DoGHead < 0 || !Main.npc[CalamityGlobalNPC.DoGHead].active)
            {
                NPC.velocity.Y -= 4f;

                if ((double)NPC.position.Y < Main.topWorld + 16f)
                {
                    for (int a = 0; a < Main.maxNPCs; a++)
                    {
                        if (Main.npc[a].type == NPC.type || Main.npc[a].type == ModContent.NPCType<CosmicGuardianBody>() || Main.npc[a].type == ModContent.NPCType<CosmicGuardianTail>())
                            Main.npc[a].active = false;
                    }
                }
                return;
            }

            Player player = Main.player[Main.npc[CalamityGlobalNPC.DoGHead].target];

            bool increaseSpeed = Vector2.Distance(player.Center, vector) > CalamityGlobalNPC.CatchUpDistance200Tiles;
            bool increaseSpeedMore = Vector2.Distance(player.Center, vector) > CalamityGlobalNPC.CatchUpDistance350Tiles;

            NPC.Opacity = Main.npc[CalamityGlobalNPC.DoGHead].Opacity;

            // Fly up and despawn if DoG enters phase 2 and isn't in the final Cosmic Guardian spawn phase.
            bool flyAwayAndDespawn = (Main.npc[CalamityGlobalNPC.DoGHead].life / (float)Main.npc[CalamityGlobalNPC.DoGHead].lifeMax < 0.6f && Main.npc[CalamityGlobalNPC.DoGHead].life / (float)Main.npc[CalamityGlobalNPC.DoGHead].lifeMax > 0.2f) || Main.npc[CalamityGlobalNPC.DoGHead].life <= 1;
            if (flyAwayAndDespawn)
            {
                // Prevents them from doing damage while despawning.
                NPC.Opacity = 0.99f;

                NPC.velocity.Y -= 4f;

                if ((double)NPC.position.Y < Main.topWorld + 16f)
                {
                    for (int a = 0; a < Main.maxNPCs; a++)
                    {
                        if (Main.npc[a].type == NPC.type || Main.npc[a].type == ModContent.NPCType<CosmicGuardianBody>() || Main.npc[a].type == ModContent.NPCType<CosmicGuardianTail>())
                            Main.npc[a].active = false;
                    }
                }
                return;
            }

            Vector2 segmentDirection = NPC.Center;
            float dogHeadX = Main.npc[CalamityGlobalNPC.DoGHead].position.X + (Main.npc[CalamityGlobalNPC.DoGHead].width / 2);
            float dogHeadY = Main.npc[CalamityGlobalNPC.DoGHead].position.Y + (Main.npc[CalamityGlobalNPC.DoGHead].height / 2);
            float segmentVelocity = BossRushEvent.BossRushActive ? 30f : CalamityWorld.revenge ? 25f : 23f;
            float velocityMult = BossRushEvent.BossRushActive ? 0.9f : CalamityWorld.revenge ? 0.75f : 0.23f;
            float maxDistanceFromDoGHead = 160f;

            if (increaseSpeedMore)
                velocityMult *= 4f;
            else if (increaseSpeed)
                velocityMult *= 2f;

            foreach (NPC n in Main.ActiveNPCs)
            {
                if ((n.type == NPC.type || n.type == ModContent.NPCType<DevourerofGodsHead>()) && n.whoAmI != NPC.whoAmI)
                {
                    Vector2 distFromHead = n.Center - NPC.Center;
                    if (distFromHead.Length() < maxDistanceFromDoGHead)
                    {
                        distFromHead.Normalize();
                        distFromHead *= maxDistanceFromDoGHead;
                        dogHeadX -= distFromHead.X;
                        dogHeadY -= distFromHead.Y;
                    }
                }
            }

            float higherMaxDistance = segmentVelocity * 1.3f;
            float lowerMaxDistance = segmentVelocity * 0.7f;
            float npcSpeed = NPC.velocity.Length();
            if (npcSpeed > 0f)
            {
                if (npcSpeed > higherMaxDistance)
                {
                    NPC.velocity.Normalize();
                    NPC.velocity *= higherMaxDistance;
                }
                else if (npcSpeed < lowerMaxDistance)
                {
                    NPC.velocity.Normalize();
                    NPC.velocity *= lowerMaxDistance;
                }
            }

            dogHeadX = (int)(dogHeadX / 16f) * 16;
            dogHeadY = (int)(dogHeadY / 16f) * 16;
            segmentDirection.X = (int)(segmentDirection.X / 16f) * 16;
            segmentDirection.Y = (int)(segmentDirection.Y / 16f) * 16;
            dogHeadX -= segmentDirection.X;
            dogHeadY -= segmentDirection.Y;
            float targetDistance = (float)Math.Sqrt(dogHeadX * dogHeadX + dogHeadY * dogHeadY);
            float absoluteDoGX = Math.Abs(dogHeadX);
            float absoluteDoGY = Math.Abs(dogHeadY);
            float timeToReachTarget = segmentVelocity / targetDistance;
            dogHeadX *= timeToReachTarget;
            dogHeadY *= timeToReachTarget;
            if ((NPC.velocity.X > 0f && dogHeadX > 0f) || (NPC.velocity.X < 0f && dogHeadX < 0f) || (NPC.velocity.Y > 0f && dogHeadY > 0f) || (NPC.velocity.Y < 0f && dogHeadY < 0f))
            {
                if (NPC.velocity.X < dogHeadX)
                {
                    NPC.velocity.X = NPC.velocity.X + velocityMult;
                }
                else
                {
                    if (NPC.velocity.X > dogHeadX)
                    {
                        NPC.velocity.X = NPC.velocity.X - velocityMult;
                    }
                }
                if (NPC.velocity.Y < dogHeadY)
                {
                    NPC.velocity.Y = NPC.velocity.Y + velocityMult;
                }
                else
                {
                    if (NPC.velocity.Y > dogHeadY)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - velocityMult;
                    }
                }
                if (Math.Abs(dogHeadY) < segmentVelocity * 0.2 && ((NPC.velocity.X > 0f && dogHeadX < 0f) || (NPC.velocity.X < 0f && dogHeadX > 0f)))
                {
                    if (NPC.velocity.Y > 0f)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + velocityMult * 2f;
                    }
                    else
                    {
                        NPC.velocity.Y = NPC.velocity.Y - velocityMult * 2f;
                    }
                }
                if (Math.Abs(dogHeadX) < segmentVelocity * 0.2 && ((NPC.velocity.Y > 0f && dogHeadY < 0f) || (NPC.velocity.Y < 0f && dogHeadY > 0f)))
                {
                    if (NPC.velocity.X > 0f)
                    {
                        NPC.velocity.X = NPC.velocity.X + velocityMult * 2f; //changed from 2
                    }
                    else
                    {
                        NPC.velocity.X = NPC.velocity.X - velocityMult * 2f; //changed from 2
                    }
                }
            }
            else
            {
                if (absoluteDoGX > absoluteDoGY)
                {
                    if (NPC.velocity.X < dogHeadX)
                    {
                        NPC.velocity.X = NPC.velocity.X + velocityMult * 1.1f; //changed from 1.1
                    }
                    else if (NPC.velocity.X > dogHeadX)
                    {
                        NPC.velocity.X = NPC.velocity.X - velocityMult * 1.1f; //changed from 1.1
                    }
                    if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < segmentVelocity * 0.5)
                    {
                        if (NPC.velocity.Y > 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y + velocityMult;
                        }
                        else
                        {
                            NPC.velocity.Y = NPC.velocity.Y - velocityMult;
                        }
                    }
                }
                else
                {
                    if (NPC.velocity.Y < dogHeadY)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + velocityMult * 1.1f;
                    }
                    else if (NPC.velocity.Y > dogHeadY)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - velocityMult * 1.1f;
                    }
                    if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < segmentVelocity * 0.5)
                    {
                        if (NPC.velocity.X > 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X + velocityMult;
                        }
                        else
                        {
                            NPC.velocity.X = NPC.velocity.X - velocityMult;
                        }
                    }
                }
            }

            // Calculate contact damage based on velocity
            float minimalContactDamageVelocity = segmentVelocity * 0.25f;
            float minimalDamageVelocity = segmentVelocity * 0.5f;
            if (NPC.velocity.Length() <= minimalContactDamageVelocity)
            {
                NPC.damage = (int)Math.Round(NPC.defDamage * 0.5);
            }
            else
            {
                float velocityDamageScalar = MathHelper.Clamp((NPC.velocity.Length() - minimalContactDamageVelocity) / minimalDamageVelocity, 0f, 1f);
                NPC.damage = (int)MathHelper.Lerp((float)Math.Round(NPC.defDamage * 0.5), NPC.defDamage, velocityDamageScalar);
            }

            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                NPC.Opacity = 1f;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 halfSizeTexture = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / 2));

            Vector2 distFromHead3 = NPC.Center - screenPos;
            distFromHead3 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height)) * NPC.scale / 2f;
            distFromHead3 += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, distFromHead3, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            texture2D15 = GlowTexture.Value;
            Color glowmaskColor = Color.Lerp(Color.White, Color.Fuchsia, 0.5f);

            spriteBatch.Draw(texture2D15, distFromHead3, NPC.frame, glowmaskColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            texture2D15 = GlowTexture2.Value;
            glowmaskColor = Color.Lerp(Color.White, Color.Cyan, 0.5f);

            spriteBatch.Draw(texture2D15, distFromHead3, NPC.frame, glowmaskColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void OnKill()
        {
            int heartAmt = Main.rand.Next(3) + 3;
            for (int i = 0; i < heartAmt; i++)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses;
            return NPC.Opacity >= 1f && invinceTime <= 0;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            return null;
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
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("DoT").Type, 1f);
                }
                NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (float)(NPC.height / 2);
                NPC.width = 50;
                NPC.height = 50;
                NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (float)(NPC.height / 2);
                for (int i = 0; i < 15; i++)
                {
                    int cosmiliteDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[cosmiliteDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[cosmiliteDust].scale = 0.5f;
                        Main.dust[cosmiliteDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 30; j++)
                {
                    int cosmiliteDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
                    Main.dust[cosmiliteDust2].noGravity = true;
                    Main.dust[cosmiliteDust2].velocity *= 5f;
                    cosmiliteDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[cosmiliteDust2].velocity *= 2f;
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120, true);
        }
    }
}
