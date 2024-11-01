using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DesertScourge
{
    [LongDistanceNetSync(SyncWith = typeof(DesertScourgeHead))]
    public class DesertScourgeTail : ModNPC
    {
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.DesertScourgeHead.DisplayName");

        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.GetNPCDamage();
            NPC.width = 104;
            NPC.height = 104;
            NPC.defense = 9;
            NPC.DR_NERD(0.1f);

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

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

        public override void AI()
        {
            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool masterMode = Main.masterMode || bossRush;

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

            Vector2 segmentTilePos = NPC.Center;
            float playerXPos = Main.player[NPC.target].Center.X;
            float playerYPos = Main.player[NPC.target].Center.Y;
            playerXPos = (float)((int)(playerXPos / 16f) * 16);
            playerYPos = (float)((int)(playerYPos / 16f) * 16);
            segmentTilePos.X = (float)((int)(segmentTilePos.X / 16f) * 16);
            segmentTilePos.Y = (float)((int)(segmentTilePos.Y / 16f) * 16);
            playerXPos -= segmentTilePos.X;
            playerYPos -= segmentTilePos.Y;
            float playerDistance = (float)Math.Sqrt((double)(playerXPos * playerXPos + playerYPos * playerYPos));
            if (NPC.ai[1] > 0f && NPC.ai[1] < (float)Main.npc.Length)
            {
                try
                {
                    segmentTilePos = NPC.Center;
                    playerXPos = Main.npc[(int)NPC.ai[1]].Center.X - segmentTilePos.X;
                    playerYPos = Main.npc[(int)NPC.ai[1]].Center.Y - segmentTilePos.Y;
                }
                catch
                {
                }
                NPC.rotation = (float)Math.Atan2((double)playerYPos, (double)playerXPos) + MathHelper.PiOver2;
                playerDistance = (float)Math.Sqrt((double)(playerXPos * playerXPos + playerYPos * playerYPos));

                int segmentOffset = 70;
                playerDistance = (playerDistance - segmentOffset) / playerDistance;
                playerXPos *= playerDistance;
                playerYPos *= playerDistance;
                NPC.velocity = Vector2.Zero;
                NPC.position.X = NPC.position.X + playerXPos;
                NPC.position.Y = NPC.position.Y + playerYPos;

                if (playerXPos < 0f)
                    NPC.spriteDirection = 1;
                else if (playerXPos > 0f)
                    NPC.spriteDirection = -1;
            }

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

            return minDist <= 30f * NPC.scale && NPC.alpha <= 0;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScourgeTail").Type, NPC.scale);

                for (int k = 0; k < 10; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
        }

        public override bool CheckActive() => false;

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(BuffID.Bleeding, 180);
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
    }
}
