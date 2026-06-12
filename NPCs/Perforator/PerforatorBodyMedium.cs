using System;
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
    [HasPierceResist]
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
            NPC.damage = 14; // 28
            NPC.npcSlots = 5f;
            NPC.width = 40;
            NPC.height = 40;
            NPC.defense = 6;

            NPC.LifeMaxNERB(120, 150, 7000);
            if (Main.zenithWorld)
                NPC.lifeMax *= 4;

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

            if (CalamityWorld.death || BossRushEvent.BossRushActive)
                NPC.scale *= 1.2f;
            else if (CalamityWorld.revenge)
                NPC.scale *= 1.15f;
            else if (Main.expertMode)
                NPC.scale *= 1.1f;

            NPC.Calamity().SplittingWorm = true;

            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override void AI()
        {
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            NPC.realLife = -1;

            // Target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest(true);

            if (Main.player[NPC.target].dead)
                NPC.TargetClosest(false);

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
                bool spawnedBlob = false;
                if (!Main.npc[(int)NPC.ai[1]].active && !Main.npc[(int)NPC.ai[0]].active)
                {
                    if (death)
                    {
                        spawnedBlob = true;
                        int type = ModContent.ProjectileType<IchorBlob>();
                        Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2CircularEdge(3f, 3f), type, PerforatorHive.IchorBlobDamage, 0f, Main.myPlayer, 0f, NPC.Center.Y);
                    }

                    NPC.life = 0;
                    NPC.HitEffect(0, 10.0);
                    NPC.checkDead();
                    NPC.active = false;
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                }
                if (!Main.npc[(int)NPC.ai[1]].active || Main.npc[(int)NPC.ai[1]].aiStyle != NPC.aiStyle)
                {
                    if (death && !spawnedBlob)
                    {
                        spawnedBlob = true;
                        int type = ModContent.ProjectileType<IchorBlob>();
                        Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2CircularEdge(3f, 3f), type, PerforatorHive.IchorBlobDamage, 0f, Main.myPlayer, 0f, NPC.Center.Y);
                    }

                    NPC.type = ModContent.NPCType<PerforatorHeadMedium>();
                    int whoAmI = NPC.whoAmI;
                    float lifeRatio = NPC.life / (float)NPC.lifeMax;
                    float ai0 = NPC.ai[0];
                    NPC.SetDefaultsKeepPlayerInteraction(NPC.type);
                    NPC.life = (int)(NPC.lifeMax * lifeRatio);
                    NPC.ai[0] = ai0;
                    NPC.TargetClosest(true);
                    NPC.ForceNetUpdate();
                    NPC.whoAmI = whoAmI;
                    NPC.alpha = 0;
                }
                if (!Main.npc[(int)NPC.ai[0]].active || Main.npc[(int)NPC.ai[0]].aiStyle != NPC.aiStyle)
                {
                    if (death && !spawnedBlob)
                    {
                        int type = ModContent.ProjectileType<IchorBlob>();
                        Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2CircularEdge(3f, 3f), type, PerforatorHive.IchorBlobDamage, 0f, Main.myPlayer, 0f, NPC.Center.Y);
                    }

                    int whoAmI2 = NPC.whoAmI;
                    float otherLifeRatio = NPC.life / (float)NPC.lifeMax;
                    float ai1 = NPC.ai[1];
                    NPC.SetDefaultsKeepPlayerInteraction(NPC.type);
                    NPC.life = (int)(NPC.lifeMax * otherLifeRatio);
                    NPC.ai[1] = ai1;
                    NPC.TargetClosest(true);
                    NPC.ForceNetUpdate();
                    NPC.whoAmI = whoAmI2;
                    NPC.alpha = 0;
                }

                if (!NPC.active && Main.dedServ)
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

            if (Main.npc[(int)NPC.ai[1]].alpha >= 85)
            {
                if (NPC.alpha > 0 && NPC.life > 0)
                {
                    for (int dustIndex = 0; dustIndex < 2; dustIndex++)
                    {
                        int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, Main.rand.NextBool() ? DustID.Ichor : DustID.Blood, 0f, 0f, 100, default, 2f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].noLight = true;
                    }
                }

                if ((NPC.position - NPC.oldPosition).Length() > 2f)
                {
                    NPC.alpha -= 42;
                    if (NPC.alpha < 0)
                        NPC.alpha = 0;
                }
            }
            else if (NPC.alpha > 0)
            {
                NPC.alpha -= 42;
                if (NPC.alpha < 0)
                    NPC.alpha = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[Type].Value;
            Vector2 halfSizeTexture = new Vector2((float)(TextureAssets.Npc[Type].Value.Width / 2), (float)(TextureAssets.Npc[Type].Value.Height / 2));

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
                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.UnitY, type, PerforatorHive.IchorBlobDamage, 0f, Main.myPlayer);

                for (int i = -1; i < 2; i++) //releases 3 Ichor Shots
                {
                    int type2 = ModContent.ProjectileType<IchorShot>();
                    Vector2 baseVelocity = Vector2.UnitY * Main.rand.NextFloat(-12.5f, -5f);
                    int spread = Main.rand.Next(16, 36);
                    Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, baseVelocity.RotatedBy(MathHelper.ToRadians(spread * i)), type2, PerforatorHive.IchorShotDamage, 0f, Main.myPlayer);
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
                if (!Main.dedServ)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("MediumPerf2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("MediumPerf3").Type, NPC.scale);
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
            {
                target.AddBuff(BuffID.Ichor, 240);
            }
        }
    }
}
