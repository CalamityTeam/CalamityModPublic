using System;
using System.IO;
using CalamityMod.BiomeManagers;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.OldDuke
{
    [HeavyKnockbackWhitelisted]
    public class SulphurousSharkron : ModNPC
    {
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            NPCID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 120; // 240
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.width = 44;
            NPC.height = 44;
            NPC.defense = 100;
            NPC.lifeMax = 6000;
            if (BossRushEvent.BossRushActive)
            {
                NPC.lifeMax = 10000;
            }
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.Opacity = 0f;
            NPC.chaseable = false;
            NPC.noGravity = true;
            NPC.dontTakeDamage = true;
            NPC.noTileCollide = true;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<SulphurousSeaBiome>().Type };
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.dontTakeDamage);
            writer.Write(NPC.noGravity);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.dontTakeDamage = reader.ReadBoolean();
            NPC.noGravity = reader.ReadBoolean();
        }

        public override void AI()
        {
            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0.7f * NPC.Opacity, 0.9f * NPC.Opacity, 0f);

            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

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

            NPC.Opacity += 0.025f;
            if (NPC.Opacity > 1f)
                NPC.Opacity = 1f;

            // Fly towards Old Duke
            bool normalAI = NPC.ai[3] == 0f;

            // Fly up
            bool upwardAI = NPC.ai[3] < 0f;

            float flyTowardTargetGateValue = death ? 70f : revenge ? 75f : expertMode ? 80f : 90f;
            float extraTime = death ? 70f : revenge ? 75f : expertMode ? 80f : 90f;
            float aiGateValue = flyTowardTargetGateValue + extraTime;
            float dieGateValue = aiGateValue + extraTime * 4f;
            float fallDownGateValue = aiGateValue + extraTime;
            float maxVelocity = death ? 20f : revenge ? 19f : expertMode ? 18f : 16f;

            if (NPC.ai[0] == 0f)
            {
                // Set velocity to fly towards a specified location on the first frame
                if (NPC.ai[1] == 0f)
                {
                    if (normalAI)
                        NPC.velocity = Vector2.Normalize(Main.npc[(int)NPC.ai[2]].Center - NPC.Center) * (maxVelocity * 0.67f);
                    else
                        NPC.velocity = new Vector2(NPC.ai[2], NPC.ai[3]);

                    SoundEngine.PlaySound(SoundID.NPCDeath19, NPC.Center);
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
                    Vector2 targetDistance = Main.player[NPC.target].Center - NPC.Center;
                    targetDistance.Normalize();
                    targetDistance *= scaleFactor2;
                    float inertia = death ? 23f : revenge ? 25f : expertMode ? 27f : 30f;
                    NPC.velocity = (NPC.velocity * (inertia - 1f) + targetDistance) / inertia;
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
                if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height) || NPC.ai[1] >= dieGateValue)
                {
                    if (NPC.DeathSound.HasValue)
                        SoundEngine.PlaySound(NPC.DeathSound.GetValueOrDefault(), NPC.Center);

                    NPC.life = 0;
                    NPC.HitEffect();
                    NPC.checkDead();
                    return;
                }

                // Fall down after a certain time has passed
                if (NPC.ai[1] >= fallDownGateValue)
                {
                    NPC.noGravity = false;
                    NPC.velocity.Y += 0.3f;
                }
            }

            float pushVelocity = 0.5f;
            foreach (var n in Main.ActiveNPCs)
            {
                if (n.whoAmI != NPC.whoAmI && n.type == NPC.type)
                {
                    if (Vector2.Distance(NPC.Center, n.Center) < 160f)
                    {
                        if (NPC.position.X < n.position.X)
                            NPC.velocity.X -= pushVelocity;
                        else
                            NPC.velocity.X += pushVelocity;

                        if (NPC.position.Y < n.position.Y)
                            NPC.velocity.Y -= pushVelocity;
                        else
                            NPC.velocity.Y += pushVelocity;
                    }
                }
            }
        }

        public override void OnKill()
        {
            int closestPlayer = Player.FindClosest(NPC.Center, 1, 1);
            if (Main.rand.NextBool(8) && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);

            if (Main.netMode != NetmodeID.MultiplayerClient && Main.getGoodWorld)
            {
                int spawnX = NPC.width / 2;
                int type = ModContent.ProjectileType<OldDukeGore>();
                for (int i = 0; i < 10; i++)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + Main.rand.Next(-spawnX, spawnX), NPC.Center.Y,
                        Main.rand.Next(-3, 4), Main.rand.Next(-12, -6), type, OldDuke.GoreDamage, 0f, Main.myPlayer);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                NPC.Opacity = 1f;

            SpriteEffects spriteEffects = SpriteEffects.FlipHorizontally;
            if (NPC.spriteDirection == -1)
                spriteEffects = SpriteEffects.None;

            Texture2D texture2D15 = TextureAssets.Npc[Type].Value;
            Vector2 halfSizeTexture = new Vector2(TextureAssets.Npc[Type].Value.Width / 2, TextureAssets.Npc[Type].Value.Height / Main.npcFrameCount[Type] / 2);
            int afterimageAmt = 10;

            if (CalamityClientConfig.Instance.Afterimages)
            {
                for (int i = 1; i < afterimageAmt; i += 2)
                {
                    Color afterimageColor = drawColor;
                    afterimageColor = Color.Lerp(afterimageColor, Color.Lime, 0.5f);
                    afterimageColor = NPC.GetAlpha(afterimageColor);
                    afterimageColor *= (afterimageAmt - i) / 15f;
                    afterimageColor.A = 0;
                    Vector2 afterimagePos = NPC.oldPos[i] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    afterimagePos -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[Type]) * NPC.scale / 2f;
                    afterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, afterimagePos, NPC.frame, afterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[Type]) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses;
            return NPC.Opacity == 1f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulphurousSeaAcid, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulphurousSeaAcid, hit.HitDirection, -1f, 0, default, 1f);

                SoundEngine.PlaySound(SoundID.NPCDeath12, NPC.Center);

                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = NPC.height = 96;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);

                for (int i = 0; i < 15; i++)
                {
                    int toxicDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulphurousSeaAcid, 0f, 0f, 100, default, 2f);
                    Main.dust[toxicDust].velocity.Y *= 6f;
                    Main.dust[toxicDust].velocity.X *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[toxicDust].scale = 0.5f;
                        Main.dust[toxicDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }

                for (int j = 0; j < 30; j++)
                {
                    int bloody = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, 3f);
                    Main.dust[bloody].noGravity = true;
                    Main.dust[bloody].velocity.Y *= 10f;
                    bloody = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, 2f);
                    Main.dust[bloody].velocity.X *= 2f;
                }

                if (!Main.dedServ)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SulphurousSharkronGore").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SulphurousSharkronGore2").Type, NPC.scale);
                }
            }
        }
    }
}
