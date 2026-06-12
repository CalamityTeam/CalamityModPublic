using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class WildBumblebirb : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 5;
            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Position = Vector2.UnitX * 36f
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override string Texture => "CalamityMod/NPCs/Bumblebirb/BumbleFolly";

        public override void SetDefaults()
        {
            NPC.npcSlots = 1f;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 90;
            NPC.width = 120;
            NPC.height = 80;
            NPC.defense = 20;
            NPC.LifeMaxNERB(9375, 11250, 5000); // Old HP - 12000, 15000
            NPC.knockBackResist = 0.15f;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit51;
            NPC.DeathSound = SoundID.NPCDeath46;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<DraconicSwarmerBanner>();
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.WildBumblefuck")
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || !NPC.downedMoonlord || spawnInfo.Player.Calamity().ZoneSunkenSea || !spawnInfo.Player.ZoneJungle)
                return 0f;

            // Keep this as a separate if check, because it's a loop and we don't want to be checking it constantly.
            if (NPC.AnyNPCs(NPC.type))
                return 0f;

            return SpawnCondition.SurfaceJungle.Chance * 0.14f;
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];

            float rotationMult = 4f;
            float rotationAmt = 0.04f;

            NPC.ai[3]++;

            if (Vector2.Distance(player.Center, NPC.Center) > 5600f)
            {
                if (NPC.timeLeft > 5)
                    NPC.timeLeft = 5;
            }

            NPC.rotation = (NPC.rotation * rotationMult + NPC.velocity.X * rotationAmt * 1.25f) / 10f;

            if (NPC.ai[0] == 0f || NPC.ai[0] == 1f)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (i != NPC.whoAmI && Main.npc[i].active && Main.npc[i].type == NPC.type && NPC.ai[3] >= 2)
                    {
                        Vector2 otherSwarmerDirection = Main.npc[i].Center - NPC.Center;
                        if (otherSwarmerDirection.Length() < (NPC.width + NPC.height))
                        {
                            otherSwarmerDirection.Normalize();
                            otherSwarmerDirection *= -0.1f;
                            NPC.velocity += otherSwarmerDirection;
                            NPC nPC6 = Main.npc[i];
                            nPC6.velocity -= otherSwarmerDirection;
                        }
                    }
                }
            }

            if (NPC.target < 0 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest(true);
                Vector2 swarmerTargetDist = Main.player[NPC.target].Center - NPC.Center;
                if (Main.player[NPC.target].dead || swarmerTargetDist.Length() > 2800f)
                    NPC.ai[0] = -1f;
            }
            else
            {
                Vector2 swarmerCatchUpTargetDist = Main.player[NPC.target].Center - NPC.Center;
                if (NPC.ai[0] > 1f && swarmerCatchUpTargetDist.Length() > 3600f)
                    NPC.ai[0] = 1f;
            }

            if (NPC.ai[0] == -1f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                Vector2 swarmerDespawnVelMult = new Vector2(0f, -8f);
                NPC.velocity = (NPC.velocity * 21f + swarmerDespawnVelMult) / 10f;
                return;
            }

            if (NPC.ai[0] == 0f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.TargetClosest(true);
                NPC.spriteDirection = NPC.direction;

                Vector2 swarmerIdleTargetDist = Main.player[NPC.target].Center - NPC.Center;
                if (swarmerIdleTargetDist.Length() > 2800f)
                {
                    NPC.ai[0] = 1f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                }
                else if (swarmerIdleTargetDist.Length() > 400f)
                {
                    float swarmerIdleSpeed = 7f + swarmerIdleTargetDist.Length() / 100f + NPC.ai[1] / 15f;
                    swarmerIdleTargetDist.Normalize();
                    swarmerIdleTargetDist *= swarmerIdleSpeed;
                    NPC.velocity = (NPC.velocity * 29f + swarmerIdleTargetDist) / 30f;
                }
                else if (NPC.velocity.Length() > 2f)
                    NPC.velocity *= 0.95f;
                else if (NPC.velocity.Length() < 1f)
                    NPC.velocity *= 1.05f;

                NPC.ai[1] += 1f;
                if (NPC.ai[1] >= 105f)
                {
                    NPC.ai[1] = 0f;
                    NPC.ai[0] = 2f;
                }
            }
            else
            {
                if (NPC.ai[0] == 1f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    if (NPC.target < 0 || !Main.player[NPC.target].active || Main.player[NPC.target].dead)
                        NPC.TargetClosest(true);

                    if (NPC.velocity.X < 0f)
                        NPC.direction = -1;
                    else if (NPC.velocity.X > 0f)
                        NPC.direction = 1;

                    NPC.spriteDirection = NPC.direction;
                    NPC.rotation = (NPC.rotation * rotationMult + NPC.velocity.X * rotationAmt) / 10f;

                    Vector2 swarmerChargeTargetDist = Main.player[NPC.target].Center - NPC.Center;
                    if (swarmerChargeTargetDist.Length() < 800f && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                    {
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                    }

                    NPC.ai[2] += 0.0166666675f;
                    float swarmerChargeSpeed = 9f + NPC.ai[2] + swarmerChargeTargetDist.Length() / 150f;
                    float swarmerChargeVelMult = 25f;
                    swarmerChargeTargetDist.Normalize();
                    swarmerChargeTargetDist *= swarmerChargeSpeed;
                    NPC.velocity = (NPC.velocity * (swarmerChargeVelMult - 1f) + swarmerChargeTargetDist) / swarmerChargeVelMult;

                    NPC.ForceNetUpdate();

                    return;
                }

                if (NPC.ai[0] == 2f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    if (NPC.velocity.X < 0f)
                        NPC.direction = -1;
                    else if (NPC.velocity.X > 0f)
                        NPC.direction = 1;

                    NPC.spriteDirection = NPC.direction;
                    NPC.rotation = (NPC.rotation * rotationMult * 0.75f + NPC.velocity.X * rotationAmt * 1.25f) / 8f;

                    Vector2 swarmerDecelerateTargetDist = Main.player[NPC.target].Center - NPC.Center;
                    swarmerDecelerateTargetDist.Y -= 8f;
                    float swarmerDecelerateSpeed = 14f;
                    float swarmerDecelerateVelMult = 8f;
                    swarmerDecelerateTargetDist.Normalize();
                    swarmerDecelerateTargetDist *= swarmerDecelerateSpeed;
                    NPC.velocity = (NPC.velocity * (swarmerDecelerateVelMult - 1f) + swarmerDecelerateTargetDist) / swarmerDecelerateVelMult;

                    if (NPC.velocity.X < 0f)
                        NPC.direction = -1;
                    else
                        NPC.direction = 1;

                    NPC.spriteDirection = NPC.direction;

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] > 10f)
                    {
                        // Set damage
                        NPC.damage = NPC.defDamage;

                        NPC.velocity = swarmerDecelerateTargetDist;

                        if (NPC.velocity.X < 0f)
                            NPC.direction = -1;
                        else
                            NPC.direction = 1;

                        NPC.ai[0] = 2.1f;
                        NPC.ai[1] = 0f;
                    }
                }
                else if (NPC.ai[0] == 2.1f)
                {
                    // Set damage
                    NPC.damage = NPC.defDamage;

                    if (NPC.velocity.X < 0f)
                        NPC.direction = -1;
                    else if (NPC.velocity.X > 0f)
                        NPC.direction = 1;

                    NPC.spriteDirection = NPC.direction;

                    NPC.velocity *= 1.01f;

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] > 30f)
                    {
                        if (!Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                        {
                            NPC.ai[0] = 0f;
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                            return;
                        }

                        if (NPC.ai[1] > 60f)
                        {
                            NPC.ai[0] = 1f;
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                        }
                    }
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(ModContent.ItemType<EffulgentFeather>(), 1, 5, 7);
        public override void OnKill()
        {
            if (Main.zenithWorld)
            {
                SoundEngine.PlaySound(CommonCalamitySounds.LightningSound, NPC.Center - Vector2.UnitY * 300f);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 fireFrom = new Vector2(NPC.Center.X + (40 * i) - 120, NPC.Center.Y - 900f);
                        Vector2 ai0 = NPC.Center - fireFrom;
                        float ai = Main.rand.Next(100);
                        Vector2 velocity = Vector2.Normalize(ai0.RotatedByRandom(MathHelper.PiOver4)) * 7f;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), fireFrom.X, fireFrom.Y, velocity.X, velocity.Y, ModContent.ProjectileType<RedLightning>(), 45, 0f, Main.myPlayer, ai0.ToRotation(), ai);
                    }
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += NPC.ai[0] == 2.1f ? 1.5 : 1D;
            if (Main.zenithWorld)
            {
                NPC.frameCounter += 2D;
            }
            if (NPC.frameCounter > 4D) //iban said the time between frames was 5 so using that as a base
            {
                NPC.frameCounter = 0D;
                NPC.frame.Y += frameHeight;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[Type].Value;
            Vector2 halfSizeTexture = new Vector2(TextureAssets.Npc[Type].Value.Width / 2, TextureAssets.Npc[Type].Value.Height / Main.npcFrameCount[Type] / 2);
            int afterimageAmt = NPC.ai[0] == 2.1f ? 7 : 0;

            if (CalamityClientConfig.Instance.Afterimages)
            {
                for (int i = 1; i < afterimageAmt; i += 2)
                {
                    Color afterimageColor = drawColor;
                    afterimageColor = Color.Lerp(afterimageColor, Color.Gold, 0.5f);
                    afterimageColor = NPC.GetAlpha(afterimageColor);
                    afterimageColor *= (afterimageAmt - i) / 15f;
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

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CopperCoin, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CopperCoin, hit.HitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
