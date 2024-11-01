using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Perforator
{
    [LongDistanceNetSync(SyncWith = typeof(PerforatorHeadMedium))]
    public class PerforatorBodyMedium : ModNPC
    {
        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/NPCHit/PerfMediumHit", 3);
        public static readonly SoundStyle DeathSound = new("CalamityMod/Sounds/NPCKilled/PerfMediumDeath");

        public static Asset<Texture2D> GlowTexture;

        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.PerforatorHeadMedium.DisplayName");
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetDefaults()
        {
            NPC.GetNPCDamage();
            NPC.npcSlots = 5f;
            NPC.width = 40;
            NPC.height = 40;
            NPC.defense = 6;

            NPC.LifeMaxNERB(180, 216, 7000);
            if (Main.zenithWorld)
                NPC.lifeMax *= 4;

            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.alpha = 255;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = HitSound;
            NPC.DeathSound = DeathSound;
            NPC.netAlways = true;
            NPC.dontCountMe = true;

            if (BossRushEvent.BossRushActive)
                NPC.scale *= 1.25f;
            else if (CalamityWorld.death)
                NPC.scale *= 1.2f;
            else if (CalamityWorld.revenge)
                NPC.scale *= 1.15f;
            else if (Main.expertMode)
                NPC.scale *= 1.1f;

            NPC.Calamity().SplittingWorm = true;

            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;

            // Scale stats in Expert and Master
            CalamityGlobalNPC.AdjustExpertModeStatScaling(NPC);
            CalamityGlobalNPC.AdjustMasterModeStatScaling(NPC);
        }

        public override void AI()
        {
            NPC.realLife = -1;

            // Target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest(true);

            if (Main.player[NPC.target].dead)
                NPC.TargetClosest(false);

            if (Main.npc[(int)NPC.ai[1]].alpha < 128)
            {
                NPC.alpha -= 42;
                if (NPC.alpha < 0)
                    NPC.alpha = 0;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.ai[0] == 0f)
                {
                    if (NPC.ai[2] > 0f)
                        NPC.ai[0] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)(NPC.position.Y + NPC.height), NPC.type, NPC.whoAmI);
                    else
                        NPC.ai[0] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)(NPC.position.Y + NPC.height), ModContent.NPCType<PerforatorTailMedium>(), NPC.whoAmI);

                    Main.npc[(int)NPC.ai[0]].ai[1] = NPC.whoAmI;
                    Main.npc[(int)NPC.ai[0]].ai[2] = NPC.ai[2] - 1f;
                    NPC.netUpdate = true;
                }

                // Splitting effect
                if (!Main.npc[(int)NPC.ai[1]].active && !Main.npc[(int)NPC.ai[0]].active)
                {
                    NPC.life = 0;
                    NPC.HitEffect(0, 10.0);
                    NPC.checkDead();
                    NPC.active = false;
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                }
                if (!Main.npc[(int)NPC.ai[1]].active || Main.npc[(int)NPC.ai[1]].aiStyle != NPC.aiStyle)
                {
                    NPC.type = ModContent.NPCType<PerforatorHeadMedium>();
                    int whoAmI = NPC.whoAmI;
                    float lifeRatio = NPC.life / (float)NPC.lifeMax;
                    float ai0 = NPC.ai[0];
                    NPC.SetDefaultsKeepPlayerInteraction(NPC.type);
                    NPC.life = (int)(NPC.lifeMax * lifeRatio);
                    NPC.ai[0] = ai0;
                    NPC.TargetClosest(true);
                    NPC.netUpdate = true;
                    NPC.whoAmI = whoAmI;
                }
                if (!Main.npc[(int)NPC.ai[0]].active || Main.npc[(int)NPC.ai[0]].aiStyle != NPC.aiStyle)
                {
                    int whoAmI2 = NPC.whoAmI;
                    float otherLifeRatio = NPC.life / (float)NPC.lifeMax;
                    float ai1 = NPC.ai[1];
                    NPC.SetDefaultsKeepPlayerInteraction(NPC.type);
                    NPC.life = (int)(NPC.lifeMax * otherLifeRatio);
                    NPC.ai[1] = ai1;
                    NPC.TargetClosest(true);
                    NPC.netUpdate = true;
                    NPC.whoAmI = whoAmI2;
                }

                if (!NPC.active && Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f, 0f, 0f, 0, 0, 0);
            }

            Vector2 segmentDirection = NPC.Center;
            float targetX = Main.player[NPC.target].Center.X;
            float targetY = Main.player[NPC.target].Center.Y;

            targetX = (int)(targetX / 16f) * 16;
            targetY = (int)(targetY / 16f) * 16;
            segmentDirection.X = (int)(segmentDirection.X / 16f) * 16;
            segmentDirection.Y = (int)(segmentDirection.Y / 16f) * 16;
            targetX -= segmentDirection.X;
            targetY -= segmentDirection.Y;
            float targetDistance = (float)Math.Sqrt(targetX * targetX + targetY * targetY);

            if (NPC.ai[1] > 0f && NPC.ai[1] < Main.npc.Length)
            {
                try
                {
                    segmentDirection = NPC.Center;
                    targetX = Main.npc[(int)NPC.ai[1]].Center.X - segmentDirection.X;
                    targetY = Main.npc[(int)NPC.ai[1]].Center.Y - segmentDirection.Y;
                }
                catch
                {
                }

                NPC.rotation = (float)Math.Atan2(targetY, targetX) + MathHelper.PiOver2;
                targetDistance = (float)Math.Sqrt(targetX * targetX + targetY * targetY);
                int npcWidth = NPC.width;
                npcWidth = (int)(npcWidth * NPC.scale);
                targetDistance = (targetDistance - npcWidth) / targetDistance;
                targetX *= targetDistance;
                targetY *= targetDistance;
                NPC.velocity = Vector2.Zero;
                NPC.position.X += targetX;
                NPC.position.Y += targetY;
            }

            // Calculate contact damage based on velocity
            // This worm requires more velocity to deal damage with the body because it doesn't have spikes or metal bits or etc.
            float maxChaseSpeed = 16f;
            float minimalContactDamageVelocity = maxChaseSpeed * 0.4f;
            float minimalDamageVelocity = maxChaseSpeed * 0.8f;
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

            texture2D15 = GlowTexture.Value;
            Color glowmaskColor = Color.Lerp(Color.White, Color.Yellow, 0.5f);

            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, glowmaskColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void OnKill()
        {
            int closestPlayer = Player.FindClosest(NPC.Center, 1, 1);
            if (Main.rand.NextBool(4) && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);

            if (Main.netMode != NetmodeID.MultiplayerClient && Main.zenithWorld)
            {
                int type = ModContent.ProjectileType<IchorBlob>();
                int damage = NPC.GetProjectileDamage(type);
                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.UnitY, type, damage, 0f, Main.myPlayer);

                for (int i = -1; i < 2; i++) //releases 3 Ichor Shots
                {
                    int type2 = ModContent.ProjectileType<IchorShot>();
                    Vector2 baseVelocity = Vector2.UnitY * Main.rand.NextFloat(-12.5f, -5f);
                    int spread = Main.rand.Next(16, 36);
                    Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, baseVelocity.RotatedBy(MathHelper.ToRadians(spread * i)), type2, damage, 0f, Main.myPlayer);
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("MediumPerf2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("MediumPerf3").Type, NPC.scale);
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<BurningBlood>(), 120, true);
        }
    }
}
