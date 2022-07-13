using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.StatDebuffs;
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Events;
using CalamityMod.World;
using Terraria.Audio;

namespace CalamityMod.NPCs.OldDuke
{
    public class SulphurousSharkron : ModNPC
    {
        bool spawnedProjectiles = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphurous Sharkron");
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Scale = 0.65f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.width = 44;
            NPC.height = 44;
            NPC.GetNPCDamage();
            NPC.defense = 100;
            NPC.lifeMax = 6000;
            if (BossRushEvent.BossRushActive)
            {
                NPC.lifeMax = 10000;
            }
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.alpha = 255;
            NPC.noGravity = true;
            NPC.dontTakeDamage = true;
            NPC.noTileCollide = true;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<SulphurousSeaBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = ModContent.NPCType<OldDuke>();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("Rotting, corpselike offspring of the Old Duke. In the Sulphurous Seas, even from birth, their appearances are wizened and decrepit.")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.dontTakeDamage);
            writer.Write(NPC.noGravity);
            writer.Write(spawnedProjectiles);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.dontTakeDamage = reader.ReadBoolean();
            NPC.noGravity = reader.ReadBoolean();
            spawnedProjectiles = reader.ReadBoolean();
        }

        public override void AI()
        {
            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || bossRush;

            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead)
            {
                NPC.TargetClosest(false);
                NPC.netUpdate = true;
            }

            if (NPC.velocity.X < 0f)
            {
                NPC.spriteDirection = -1;
                NPC.rotation = (float)Math.Atan2(-NPC.velocity.Y, -NPC.velocity.X);
            }
            else
            {
                NPC.spriteDirection = 1;
                NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);
            }

            NPC.alpha -= 6;
            if (NPC.alpha < 0)
                NPC.alpha = 0;

            // Fly towards Old Duke
            bool normalAI = NPC.ai[3] == 0f;

            // Fly up
            bool upwardAI = NPC.ai[3] < 0f;

            // Fly down
            bool downwardAI = NPC.ai[3] > 0f;

            float flyTowardTargetGateValue = bossRush ? 60f : death ? 70f : revenge ? 75f : expertMode ? 80f : 90f;
            float extraTime = bossRush ? 90f : death ? 100f : revenge ? 105f : expertMode ? 110f : 120f;
            float aiGateValue = flyTowardTargetGateValue + extraTime;
            if (!normalAI)
                aiGateValue -= extraTime * 0.3f;
            float explodeIntoGoreGateValue = aiGateValue + extraTime;
            float fallDownGateValue = aiGateValue + extraTime * 0.5f;
            float maxVelocity = bossRush ? 24f : death ? 22f : revenge ? 21f : expertMode ? 20f : 18f;

            if (NPC.ai[0] == 0f)
            {
                // Set velocity to fly towards a specified location on the first frame
                if (NPC.ai[1] == 0f)
                {
                    if (normalAI)
                        NPC.velocity = Vector2.Normalize(Main.npc[(int)NPC.ai[2]].Center - NPC.Center) * (maxVelocity - 6f);
                    else
                        NPC.velocity = new Vector2(NPC.ai[2], NPC.ai[3]);

                    SoundEngine.PlaySound(SoundID.NPCDeath19, NPC.position);
                }

                // Fly towards a target after a certain time has passed
                NPC.ai[1] += 1f;
                if (NPC.ai[1] >= flyTowardTargetGateValue)
                {
                    // Start second part of AI if not inside tiles and a certain time has passed
                    if (!Collision.SolidCollision(NPC.position, NPC.width, NPC.height) && NPC.ai[1] >= aiGateValue)
                        NPC.ai[0] = 1f;

                    // If not set to fly towards Old Duke from the start, accelerate
                    if (!normalAI)
                    {
                        if (NPC.velocity.Length() < maxVelocity)
                            NPC.velocity *= 1.01f;
                    }

                    // Fly towards the target
                    float scaleFactor2 = NPC.velocity.Length();
                    Vector2 vector17 = Main.player[NPC.target].Center - NPC.Center;
                    vector17.Normalize();
                    vector17 *= scaleFactor2;
                    float inertia = bossRush ? 16f : death ? 18f : revenge ? 20f : expertMode ? 22f : 25f;
                    NPC.velocity = (NPC.velocity * (inertia - 1f) + vector17) / inertia;
                    NPC.velocity.Normalize();
                    NPC.velocity *= scaleFactor2;
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                // Move slower if set to fly upward from the start
                if (upwardAI)
                    maxVelocity -= 6f;

                // Decrease velocity if moving faster than max
                if (NPC.velocity.Length() > maxVelocity)
                    NPC.velocity *= 0.99f;

                NPC.dontTakeDamage = false;

                // Explode into gores if colliding with tiles or after a certain time has passed
                NPC.ai[1] += 1f;
                if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height) || NPC.ai[1] >= explodeIntoGoreGateValue)
                {
                    if (NPC.DeathSound.HasValue)
                        SoundEngine.PlaySound(NPC.DeathSound.GetValueOrDefault(), NPC.position);

                    NPC.life = 0;
                    NPC.HitEffect(0, 10.0);
                    NPC.checkDead();
                    NPC.active = false;
                    return;
                }

                // Fall down after a certain time has passed
                if (NPC.ai[1] >= fallDownGateValue)
                {
                    NPC.noGravity = false;
                    NPC.velocity.Y += 0.3f;
                }
            }
        }

        public override void OnKill()
        {
            int closestPlayer = Player.FindClosest(NPC.Center, 1, 1);
            if (Main.rand.NextBool(8) && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                NPC.Opacity = 1f;

            SpriteEffects spriteEffects = SpriteEffects.FlipHorizontally;
            if (NPC.spriteDirection == -1)
                spriteEffects = SpriteEffects.None;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 vector11 = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2);
            Color color36 = Color.Lime;
            float amount9 = 0.5f;
            int num153 = 10;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num155 = 1; num155 < num153; num155 += 2)
                {
                    Color color38 = drawColor;
                    color38 = Color.Lerp(color38, color36, amount9);
                    color38 = NPC.GetAlpha(color38);
                    color38 *= (num153 - num155) / 15f;
                    Vector2 vector41 = NPC.oldPos[num155] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    vector41 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    vector41 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector41, NPC.frame, color38, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 vector43 = NPC.Center - screenPos;
            vector43 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return NPC.alpha == 0;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<Irradiated>(), 240);
        }

        public override bool CheckDead()
        {
            SoundEngine.PlaySound(SoundID.NPCDeath12, NPC.position);

            NPC.position.X = NPC.position.X + (NPC.width / 2);
            NPC.position.Y = NPC.position.Y + (NPC.height / 2);
            NPC.width = NPC.height = 96;
            NPC.position.X = NPC.position.X - (NPC.width / 2);
            NPC.position.Y = NPC.position.Y - (NPC.height / 2);

            for (int num621 = 0; num621 < 15; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity.Y *= 6f;
                Main.dust[num622].velocity.X *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }

            for (int num623 = 0; num623 < 30; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity.Y *= 10f;
                num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity.X *= 2f;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && !spawnedProjectiles)
            {
                spawnedProjectiles = true;
                int spawnX = NPC.width / 2;
                int type = ModContent.ProjectileType<OldDukeGore>();
                int damage = NPC.GetProjectileDamage(type);
                for (int i = 0; i < 2; i++)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + Main.rand.Next(-spawnX, spawnX), NPC.Center.Y,
                        Main.rand.Next(-3, 4), Main.rand.Next(-12, -6), type, damage, 0f, Main.myPlayer);
            }

            return true;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
