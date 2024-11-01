using System;
using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Abyss
{
    [LongDistanceNetSync]
    public class BobbitWormHead : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                //Preferably would have its head animated, but this will do for now
                CustomTexturePath = "CalamityMod/ExtraTextures/Bestiary/BobbitWorm_Bestiary"
            };
            value.Position.Y += 40;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.lavaImmune = true;
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.aiStyle = -1;
            NPC.damage = 150;
            NPC.width = 80;
            NPC.height = 40;
            NPC.defense = 50;
            NPC.DR_NERD(0.25f);
            NPC.lifeMax = 6000;
            NPC.knockBackResist = 0f;
            AIType = -1;
            NPC.noGravity = true;
            NPC.value = Item.buyPrice(0, 0, 50, 0);
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<BobbitWormBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<AbyssLayer4Biome>().Type };

            // Scale stats in Expert and Master
            CalamityGlobalNPC.AdjustExpertModeStatScaling(NPC);
            CalamityGlobalNPC.AdjustMasterModeStatScaling(NPC);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.BobbitWorm")
            });
        }

        public override void AI()
        {
            if (!Main.npc[(int)NPC.ai[2]].active || (int)NPC.ai[2] < 0)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            if (NPC.ai[0] == 0f)
            {
                NPC.noTileCollide = true;
                float launchSpeed = 14f;
                Vector2 bobbitCenter = new Vector2(NPC.Center.X, NPC.Center.Y);
                float segmentXDist = Main.npc[(int)NPC.ai[2]].Center.X - bobbitCenter.X;
                float segmentYDist = Main.npc[(int)NPC.ai[2]].Center.Y - bobbitCenter.Y;
                float segmentDistance = (float)Math.Sqrt((double)(segmentXDist * segmentXDist + segmentYDist * segmentYDist));
                if (segmentDistance < 11f + launchSpeed)
                {
                    NPC.rotation = 0f;
                    NPC.velocity.X = segmentXDist;
                    NPC.velocity.Y = segmentYDist;
                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] >= 60f)
                    {
                        NPC.TargetClosest(true);
                        if (NPC.Center.Y > Main.player[NPC.target].Center.Y && (Main.player[NPC.target].Center - NPC.Center).Length() < Main.player[NPC.target].Calamity().GetAbyssAggro(480f) &&
                            Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                        {
                            NPC.ai[1] = 0f;
                            NPC.ai[0] = 1f;
                            return;
                        }
                        NPC.ai[1] = 0f;
                        return;
                    }
                }
                else
                {
                    segmentDistance = launchSpeed / segmentDistance;
                    NPC.velocity.X = segmentXDist * segmentDistance;
                    NPC.velocity.Y = segmentYDist * segmentDistance;
                    NPC.rotation = (float)Math.Atan2((double)NPC.velocity.Y, (double)NPC.velocity.X) - 1.57f;
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                NPC.noTileCollide = true;
                NPC.collideX = false;
                NPC.collideY = false;
                float returnSpeed = 16f;
                Vector2 bobbitCenterReturn = new Vector2(NPC.Center.X, NPC.Center.Y);
                float segmentReturnXDist = Main.player[NPC.target].Center.X - bobbitCenterReturn.X;
                float segmentReturnYDist = Main.player[NPC.target].Center.Y - bobbitCenterReturn.Y;
                float segmentReturnDistance = (float)Math.Sqrt((double)(segmentReturnXDist * segmentReturnXDist + segmentReturnYDist * segmentReturnYDist));
                segmentReturnDistance = returnSpeed / segmentReturnDistance;
                NPC.velocity.X = segmentReturnXDist * segmentReturnDistance;
                NPC.velocity.Y = segmentReturnYDist * segmentReturnDistance;
                NPC.ai[0] = 2f;
                NPC.rotation = (float)Math.Atan2((double)-(double)NPC.velocity.Y, (double)-(double)NPC.velocity.X) - 1.57f;
            }
            else if (NPC.ai[0] == 2f)
            {
                if (Math.Abs(NPC.velocity.X) > Math.Abs(NPC.velocity.Y))
                {
                    if (NPC.velocity.X > 0f && NPC.Center.X > Main.player[NPC.target].Center.X)
                    {
                        NPC.noTileCollide = false;
                    }
                    if (NPC.velocity.X < 0f && NPC.Center.X < Main.player[NPC.target].Center.X)
                    {
                        NPC.noTileCollide = false;
                    }
                }
                else
                {
                    if (NPC.velocity.Y > 0f && NPC.Center.Y > Main.player[NPC.target].Center.Y)
                    {
                        NPC.noTileCollide = false;
                    }
                    if (NPC.velocity.Y < 0f && NPC.Center.Y < Main.player[NPC.target].Center.Y)
                    {
                        NPC.noTileCollide = false;
                    }
                }
                Vector2 bobbitCenterReturning = new Vector2(NPC.Center.X, NPC.Center.Y);
                float segmentReturningXDist = Main.npc[(int)NPC.ai[2]].Center.X - bobbitCenterReturning.X;
                float segmentReturningYDist = Main.npc[(int)NPC.ai[2]].Center.Y - bobbitCenterReturning.Y;
                float segmentReturningDistance = (float)Math.Sqrt((double)(segmentReturningXDist * segmentReturningXDist + segmentReturningYDist * segmentReturningYDist));
                if (segmentReturningDistance > 700f || NPC.collideX || NPC.collideY)
                {
                    NPC.noTileCollide = true;
                    NPC.ai[0] = 0f;
                    return;
                }
            }
            else if (NPC.ai[0] == 3f)
            {
                NPC.noTileCollide = true;
                float unusedSpeed = 16f;
                float unusedAcceleration = 0.25f;
                Vector2 unusedBobbitCenter = new Vector2(NPC.Center.X, NPC.Center.Y);
                float unusedTargetXDist = Main.player[NPC.target].Center.X - unusedBobbitCenter.X;
                float unusedTargetYDist = Main.player[NPC.target].Center.Y - unusedBobbitCenter.Y;
                float unusedTargetDistance = (float)Math.Sqrt((double)(unusedTargetXDist * unusedTargetXDist + unusedTargetYDist * unusedTargetYDist));
                unusedTargetDistance = unusedSpeed / unusedTargetDistance;
                unusedTargetXDist *= unusedTargetDistance;
                unusedTargetYDist *= unusedTargetDistance;
                if (NPC.velocity.X < unusedTargetXDist)
                {
                    NPC.velocity.X = NPC.velocity.X + unusedAcceleration;
                    if (NPC.velocity.X < 0f && unusedTargetXDist > 0f)
                    {
                        NPC.velocity.X = NPC.velocity.X + unusedAcceleration * 2f;
                    }
                }
                else if (NPC.velocity.X > unusedTargetXDist)
                {
                    NPC.velocity.X = NPC.velocity.X - unusedAcceleration;
                    if (NPC.velocity.X > 0f && unusedTargetXDist < 0f)
                    {
                        NPC.velocity.X = NPC.velocity.X - unusedAcceleration * 2f;
                    }
                }
                if (NPC.velocity.Y < unusedTargetYDist)
                {
                    NPC.velocity.Y = NPC.velocity.Y + unusedAcceleration;
                    if (NPC.velocity.Y < 0f && unusedTargetYDist > 0f)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + unusedAcceleration * 2f;
                    }
                }
                else if (NPC.velocity.Y > unusedTargetYDist)
                {
                    NPC.velocity.Y = NPC.velocity.Y - unusedAcceleration;
                    if (NPC.velocity.Y > 0f && unusedTargetYDist < 0f)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - unusedAcceleration * 2f;
                    }
                }
                NPC.rotation = (float)Math.Atan2((double)-(double)NPC.velocity.Y, (double)-(double)NPC.velocity.X);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.1f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 center = new Vector2(NPC.Center.X, NPC.Center.Y);
            float drawPositionX = Main.npc[(int)NPC.ai[2]].Center.X - center.X;
            float drawPositionY = Main.npc[(int)NPC.ai[2]].Center.Y - center.Y;
            float rotation = (float)Math.Atan2((double)drawPositionY, (double)drawPositionX) - 1.57f;
            bool draw = !NPC.IsABestiaryIconDummy;
            while (draw)
            {
                float totalDrawDistance = (float)Math.Sqrt((double)(drawPositionX * drawPositionX + drawPositionY * drawPositionY));
                if (totalDrawDistance < 16f)
                {
                    draw = false;
                }
                else
                {
                    totalDrawDistance = 16f / totalDrawDistance;
                    drawPositionX *= totalDrawDistance;
                    drawPositionY *= totalDrawDistance;
                    center.X += drawPositionX;
                    center.Y += drawPositionY;
                    drawPositionX = Main.npc[(int)NPC.ai[2]].Center.X - center.X;
                    drawPositionY = Main.npc[(int)NPC.ai[2]].Center.Y - center.Y;
                    drawPositionY += 4f;
                    Color color = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16f));
                    Main.spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/BobbitWormSegment").Value, new Vector2(center.X - screenPos.X, center.Y - screenPos.Y),
                        new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/BobbitWormSegment").Value.Width, ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/BobbitWormSegment").Value.Height)), color, rotation,
                        new Vector2((float)ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/BobbitWormSegment").Value.Width * 0.5f, (float)ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/BobbitWormSegment").Value.Height * 0.5f), 1f, SpriteEffects.None, 0f);
                }
            }
            return true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<CrushDepth>(), 300, true);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            var postLevi = npcLoot.DefineConditionalDropSet(DropHelper.PostLevi());
            postLevi.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<DepthCells>(), 2, 5, 7, 7, 10));
            npcLoot.AddIf(DropHelper.PostPolter(), ModContent.ItemType<BobbitHook>(), 3);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("BobbitWorm").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("BobbitWorm2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("BobbitWorm3").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("BobbitWorm4").Type, 1f);
                }
            }
        }
    }
}
