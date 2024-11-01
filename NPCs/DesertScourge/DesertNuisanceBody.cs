using System.IO;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DesertScourge
{
    [LongDistanceNetSync(SyncWith = typeof(DesertNuisanceHead))]
    public class DesertNuisanceBody : ModNPC
    {
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.DesertNuisanceHead.DisplayName");

        public static Asset<Texture2D> BodyTexture2;
        public static Asset<Texture2D> BodyTexture3;
        public static Asset<Texture2D> BodyTexture4;

        private const int ClosedFinFrame = 5;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 7;

            this.HideFromBestiary();

            if (!Main.dedServ)
            {
                BodyTexture2 = ModContent.Request<Texture2D>(Texture + "2", AssetRequestMode.AsyncLoad);
                BodyTexture3 = ModContent.Request<Texture2D>(Texture + "3", AssetRequestMode.AsyncLoad);
                BodyTexture4 = ModContent.Request<Texture2D>(Texture + "4", AssetRequestMode.AsyncLoad);
            }
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.GetNPCDamage();
            NPC.width = 88;
            NPC.height = 88;

            NPC.defense = 5;
            if (Main.getGoodWorld)
                NPC.defense += 27;

            NPC.LifeMaxNERB(1500, 1800, 40000);
            if (CalamityWorld.LegendaryMode && CalamityWorld.revenge)
                NPC.lifeMax = 4800;

            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.alpha = 255;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.netAlways = true;
            NPC.dontCountMe = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

        public override void AI()
        {
            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool masterMode = Main.masterMode || bossRush;

            if (NPC.ai[3] > 0f)
            {
                switch ((int)NPC.ai[3])
                {
                    default:
                        break;

                    case 10:

                        NPC.ai[3] = 1f;

                        NPC.position = NPC.Center;
                        NPC.position -= NPC.Size * 0.5f;
                        NPC.frame = new Rectangle(0, 0, BodyTexture2 is null ? 0 : BodyTexture2.Width(), BodyTexture2 is null ? 0 : BodyTexture2.Height());

                        NPC.netUpdate = true;

                        // Prevent netUpdate from being blocked by the spam counter.
                        NPC.netSpam = 0;

                        break;

                    case 20:

                        NPC.ai[3] = 2f;

                        NPC.position = NPC.Center;
                        NPC.position -= NPC.Size * 0.5f;
                        NPC.frame = new Rectangle(0, 0, BodyTexture3 is null ? 0 : BodyTexture3.Width(), BodyTexture3 is null ? 0 : BodyTexture3.Height());

                        NPC.netUpdate = true;

                        // Prevent netUpdate from being blocked by the spam counter.
                        NPC.netSpam = 0;

                        break;

                    case 30:

                        NPC.ai[3] = 3f;

                        NPC.position = NPC.Center;
                        NPC.position -= NPC.Size * 0.5f;
                        NPC.frame = new Rectangle(0, 0, BodyTexture4 is null ? 0 : BodyTexture4.Width(), BodyTexture4 is null ? 0 : BodyTexture4.Height());

                        NPC.netUpdate = true;

                        // Prevent netUpdate from being blocked by the spam counter.
                        NPC.netSpam = 0;

                        break;
                }
            }

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            if (NPC.life > Main.npc[(int)NPC.ai[1]].life)
                NPC.life = Main.npc[(int)NPC.ai[1]].life;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            bool shouldDespawn = !NPC.AnyNPCs(ModContent.NPCType<DesertNuisanceHead>());
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

            if (Main.npc[(int)NPC.ai[1]].alpha < 128)
            {
                NPC.alpha -= 42;
                if (NPC.alpha < 0)
                    NPC.alpha = 0;
            }

            if (Main.player[NPC.target].dead)
                NPC.TargetClosest(false);

            NPC aheadSegment = Main.npc[(int)NPC.ai[1]];
            Vector2 directionToNextSegment = aheadSegment.Center - NPC.Center;
            if (aheadSegment.rotation != NPC.rotation)
            {
                directionToNextSegment = directionToNextSegment.RotatedBy(MathHelper.WrapAngle(aheadSegment.rotation - NPC.rotation) * 0.08f);
                directionToNextSegment = directionToNextSegment.MoveTowards((aheadSegment.rotation - NPC.rotation).ToRotationVector2(), 1f);
            }

            // Decide segment offset stuff.
            int segmentOffset = 62;
            NPC.rotation = directionToNextSegment.ToRotation() + MathHelper.PiOver2;
            NPC.Center = aheadSegment.Center - directionToNextSegment.SafeNormalize(Vector2.Zero) * NPC.scale * segmentOffset;
            NPC.spriteDirection = (directionToNextSegment.X > 0).ToDirectionInt();

            // Calculate contact damage based on velocity
            float maxChaseSpeed = Main.zenithWorld ? DesertNuisanceHead.SegmentVelocity_ZenithSeed :
                Main.getGoodWorld ? DesertNuisanceHead.SegmentVelocity_GoodWorld :
                masterMode ? DesertNuisanceHead.SegmentVelocity_Master :
                DesertNuisanceHead.SegmentVelocity_Expert;
            maxChaseSpeed += maxChaseSpeed * 0.2f * (1f - lifeRatio);
            if (masterMode)
                maxChaseSpeed += maxChaseSpeed * 0.2f * (1f - lifeRatio);

            float minimalContactDamageVelocity = maxChaseSpeed * 0.25f;
            float minimalDamageVelocity = maxChaseSpeed * 0.5f;
            float bodyAndTailVelocity = (NPC.position - NPC.oldPosition).Length();
            if (bodyAndTailVelocity <= minimalContactDamageVelocity)
            {
                NPC.damage = 0;
            }
            else
            {
                float velocityDamageScalar = MathHelper.Clamp((bodyAndTailVelocity - minimalContactDamageVelocity) / minimalDamageVelocity, 0f, 1f);
                NPC.damage = (int)MathHelper.Lerp(0f, NPC.defDamage, velocityDamageScalar);
            }
        }

        public override bool CheckActive() => false;

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            Rectangle targetHitbox = target.Hitbox;

            float hitboxTopLeft = Vector2.Distance(NPC.Center, targetHitbox.TopLeft());
            float hitboxTopRight = Vector2.Distance(NPC.Center, targetHitbox.TopRight());
            float hitboxBotLeft = Vector2.Distance(NPC.Center, targetHitbox.BottomLeft());
            float hitboxBotRight = Vector2.Distance(NPC.Center, targetHitbox.BottomRight());

            float minDist = hitboxTopLeft;
            if (hitboxTopRight < minDist)
                minDist = hitboxTopRight;
            if (hitboxBotLeft < minDist)
                minDist = hitboxBotLeft;
            if (hitboxBotRight < minDist)
                minDist = hitboxBotRight;

            float hitDistance = 30f;
            switch ((int)NPC.ai[3])
            {
                default:
                case 1:
                case 10:
                    hitDistance = 40f;
                    break;

                case 2:
                case 20:
                    hitDistance = 40f;
                    break;

                case 3:
                case 30:
                    hitDistance = 30f;
                    break;
            }

            return minDist <= hitDistance * NPC.scale;
        }

        public override void FindFrame(int frameHeight)
        {
            // Fin animation segment.
            if (NPC.ai[3] == 0f)
            {
                // Close fins while head is in tiles.
                NPC head = Main.npc[(int)NPC.ai[2]];
                Point headTileCenter = head.Center.ToTileCoordinates();
                Tile tileSafely = Framing.GetTileSafely(headTileCenter);
                bool headInSolidTile = tileSafely.HasUnactuatedTile || tileSafely.LiquidAmount > 0;
                if (headInSolidTile)
                {
                    NPC.frameCounter += 1D;
                    if (NPC.frameCounter > 10D)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0D;
                    }
                    if (NPC.frame.Y >= frameHeight * ClosedFinFrame)
                        NPC.frame.Y = frameHeight * ClosedFinFrame;
                }

                // Open fins while head is outside tiles.
                else
                {
                    if (NPC.frame.Y > 0)
                    {
                        NPC.frameCounter += 1D;
                        if (NPC.frameCounter > 10D)
                        {
                            NPC.frame.Y += frameHeight;
                            NPC.frameCounter = 0D;
                        }
                        if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
                            NPC.frame.Y = 0;
                    }
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    switch ((int)NPC.ai[3])
                    {
                        default:
                        case 0:
                            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScourgeNuisanceBody").Type, NPC.scale);
                            break;

                        case 1:
                        case 10:
                            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScourgeNuisanceBody2").Type, NPC.scale);
                            break;

                        case 2:
                        case 20:
                            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScourgeNuisanceBody3").Type, NPC.scale);
                            break;

                        case 3:
                        case 30:
                            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScourgeNuisanceBody4").Type, NPC.scale);
                            break;
                    }
                }

                for (int k = 0; k < 10; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.7f * balance);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(BuffID.Bleeding, 60, true);
        }

        public override Color? GetAlpha(Color drawColor)
        {
            if (Main.zenithWorld)
            {
                Color lightColor = Color.Orange * drawColor.A;
                return lightColor * NPC.Opacity;
            }
            else return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.ai[3] > 0f)
            {
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (NPC.spriteDirection == 1)
                    spriteEffects = SpriteEffects.FlipHorizontally;

                Texture2D texture = default;
                switch ((int)NPC.ai[3])
                {
                    default:
                    case 1:
                    case 10:
                        texture = BodyTexture2.Value;
                        break;

                    case 2:
                    case 20:
                        texture = BodyTexture3.Value;
                        break;

                    case 3:
                    case 30:
                        texture = BodyTexture4.Value;
                        break;
                }

                Vector2 halfSizeTexture = new Vector2((float)(texture.Width / 2), (float)(texture.Height / 2));
                Vector2 drawLocation = NPC.Center - screenPos;
                drawLocation -= new Vector2((float)texture.Width, (float)(texture.Height)) * NPC.scale / 2f;
                drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                spriteBatch.Draw(texture, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

                return false;
            }

            return true;
        }
    }
}
