using CalamityMod.Events;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DesertScourge
{
    [LongDistanceNetSync(SyncWith = typeof(DesertScourgeHead))]
    public class DesertScourgeBody : ModNPC
    {
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.DesertScourgeHead.DisplayName");

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
        }

        public override void SetDefaults()
        {
            NPC.GetNPCDamage();
            NPC.width = 104;
            NPC.height = 104;
            NPC.defense = 6;
            NPC.DR_NERD(0.05f);

            NPC.LifeMaxNERB(4200, 5000, 1650000);
            if (Main.getGoodWorld)
                NPC.lifeMax *= 4;

            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.alpha = 255;
            NPC.boss = true;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.netAlways = true;
            NPC.dontCountMe = true;

            if (Main.getGoodWorld)
                NPC.scale *= 0.4f;

            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.alpha);
            writer.Write(NPC.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.alpha = reader.ReadInt32();
            NPC.dontTakeDamage = reader.ReadBoolean();
        }

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

            NPC.dontTakeDamage = Main.npc[(int)NPC.ai[1]].dontTakeDamage;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phases
            bool phase2 = lifeRatio < 0.5f;

            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            bool shouldDespawn = !NPC.AnyNPCs(ModContent.NPCType<DesertScourgeHead>());
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
            else
            {
                NPC.alpha = Main.npc[(int)NPC.ai[1]].alpha;
                if (NPC.alpha != 255 && NPC.dontTakeDamage)
                {
                    for (int dustIndex = 0; dustIndex < 2; dustIndex++)
                    {
                        int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.UnusedBrown, 0f, 0f, 100, default, 2f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].noLight = true;
                    }
                }
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
            int segmentOffset = 70;
            NPC.rotation = directionToNextSegment.ToRotation() + MathHelper.PiOver2;
            NPC.Center = aheadSegment.Center - directionToNextSegment.SafeNormalize(Vector2.Zero) * NPC.scale * segmentOffset;
            NPC.spriteDirection = (directionToNextSegment.X > 0).ToDirectionInt();

            NPC head = Main.npc[(int)NPC.ai[2]];
            float burrowTimeGateValue = DesertScourgeHead.BurrowTimeGateValue;
            bool burrow = head.Calamity().newAI[0] >= burrowTimeGateValue;
            bool lungeUpward = burrow && head.Calamity().newAI[1] == 1f;
            bool quickFall = head.Calamity().newAI[1] == 2f;

            // Calculate contact damage based on velocity
            float maxChaseSpeed = Main.zenithWorld ? DesertScourgeHead.SegmentVelocity_ZenithSeed :
                Main.getGoodWorld ? DesertScourgeHead.SegmentVelocity_GoodWorld :
                masterMode ? DesertScourgeHead.SegmentVelocity_Master :
                expertMode ? DesertScourgeHead.SegmentVelocity_Expert :
                DesertScourgeHead.SegmentVelocity_Normal;
            if (burrow || lungeUpward)
                maxChaseSpeed *= 1.5f;
            if (expertMode)
                maxChaseSpeed += maxChaseSpeed * 0.2f * (1f - lifeRatio);

            float minimalContactDamageVelocity = maxChaseSpeed * 0.25f;
            float minimalDamageVelocity = maxChaseSpeed * 0.5f;
            float bodyAndTailVelocity = (NPC.position - NPC.oldPosition).Length();
            if (bodyAndTailVelocity <= minimalContactDamageVelocity || NPC.dontTakeDamage)
            {
                NPC.damage = 0;
            }
            else
            {
                float velocityDamageScalar = MathHelper.Clamp((bodyAndTailVelocity - minimalContactDamageVelocity) / minimalDamageVelocity, 0f, 1f);
                NPC.damage = (int)MathHelper.Lerp(0f, NPC.defDamage, velocityDamageScalar);
            }
        }

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
                    hitDistance = 45f;
                    break;

                case 2:
                case 20:
                    hitDistance = 45f;
                    break;

                case 3:
                case 30:
                    hitDistance = 30f;
                    break;
            }

            return minDist <= hitDistance * NPC.scale && NPC.alpha <= 0;
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

        public override bool CheckActive() => false;

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
                            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScourgeBody").Type, NPC.scale);
                            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScourgeBody2").Type, NPC.scale);
                            break;

                        case 1:
                        case 10:
                            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScourgeBody2").Type, NPC.scale);
                            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScourgeBody3").Type, NPC.scale);
                            break;

                        case 2:
                        case 20:
                            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScourgeBody2").Type, NPC.scale);
                            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScourgeBody4").Type, NPC.scale);
                            break;

                        case 3:
                        case 30:
                            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScourgeBody2").Type, NPC.scale);
                            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScourgeBody5").Type, NPC.scale);
                            break;
                    }
                }

                for (int k = 0; k < 10; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            // Sometimes "Deflect" projectiles in gfb into water blasts.
            if (Main.rand.NextBool(20) && Main.zenithWorld)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Vector2 velocity = new Vector2(-projectile.velocity.X, -projectile.velocity.Y);
                    velocity.Normalize();
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity * 4, ModContent.ProjectileType<HorsWaterBlast>(), projectile.damage, 1f, Main.myPlayer);
                }
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(BuffID.Bleeding, 240);
        }

        public override Color? GetAlpha(Color drawColor)
        {
            if (Main.zenithWorld)
            {
                Color lightColor = Color.MediumBlue * drawColor.A;
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
