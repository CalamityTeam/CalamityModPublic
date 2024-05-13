using System;
using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Sounds;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Astral
{
    public class AstralProbe : ModNPC
    {
        public static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetDefaults()
        {
            NPC.damage = 20;
            NPC.width = 30; //324
            NPC.height = 30; //216
            NPC.defense = 10;
            NPC.DR_NERD(0.15f);
            NPC.lifeMax = 50;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0.95f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.DeathSound = SoundID.NPCDeath14;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<AstralProbeBanner>();
            if (DownedBossSystem.downedAstrumAureus)
            {
                NPC.damage = 30;
                NPC.defense = 20;
                NPC.knockBackResist = 0.85f;
                NPC.lifeMax = 70;
            }
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<BiomeManagers.AstralInfectionBiome>().Type };

            // Scale stats in Expert and Master
            CalamityGlobalNPC.AdjustExpertModeStatScaling(NPC);
            CalamityGlobalNPC.AdjustMasterModeStatScaling(NPC);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.AstralProbe")
            });
        }

        public override void AI()
        {
            // Setting this in SetDefaults will disable expert mode scaling, so put it here instead
            NPC.damage = 0;

            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead)
            {
                NPC.TargetClosest(true);
            }
            float probeSpeed = CalamityWorld.death ? 8f : CalamityWorld.revenge ? 6.5f : 5f;
            float probeAcceleration = CalamityWorld.death ? 0.08f : CalamityWorld.revenge ? 0.065f : 0.05f;
            Vector2 probePosition = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
            float targetXDirection = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2);
            float targetYDirection = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2);
            targetXDirection = (float)((int)(targetXDirection / 8f) * 8);
            targetYDirection = (float)((int)(targetYDirection / 8f) * 8);
            probePosition.X = (float)((int)(probePosition.X / 8f) * 8);
            probePosition.Y = (float)((int)(probePosition.Y / 8f) * 8);
            targetXDirection -= probePosition.X;
            targetYDirection -= probePosition.Y;
            float targetDistance = (float)Math.Sqrt((double)(targetXDirection * targetXDirection + targetYDirection * targetYDirection));
            float accelerateDistance = targetDistance;
            bool tooFar = false;
            if (targetDistance > 600f)
            {
                tooFar = true;
            }
            if (targetDistance == 0f)
            {
                targetXDirection = NPC.velocity.X;
                targetYDirection = NPC.velocity.Y;
            }
            else
            {
                targetDistance = probeSpeed / targetDistance;
                targetXDirection *= targetDistance;
                targetYDirection *= targetDistance;
            }
            if (accelerateDistance > 100f)
            {
                NPC.ai[0] += 1f;
                if (NPC.ai[0] > 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y + 0.023f;
                }
                else
                {
                    NPC.velocity.Y = NPC.velocity.Y - 0.023f;
                }
                if (NPC.ai[0] < -100f || NPC.ai[0] > 100f)
                {
                    NPC.velocity.X = NPC.velocity.X + 0.023f;
                }
                else
                {
                    NPC.velocity.X = NPC.velocity.X - 0.023f;
                }
                if (NPC.ai[0] > 200f)
                {
                    NPC.ai[0] = -200f;
                }
            }
            if (Main.player[NPC.target].dead)
            {
                targetXDirection = (float)NPC.direction * probeSpeed / 2f;
                targetYDirection = -probeSpeed / 2f;
            }
            if (NPC.velocity.X < targetXDirection)
            {
                NPC.velocity.X = NPC.velocity.X + probeAcceleration;
            }
            else if (NPC.velocity.X > targetXDirection)
            {
                NPC.velocity.X = NPC.velocity.X - probeAcceleration;
            }
            if (NPC.velocity.Y < targetYDirection)
            {
                NPC.velocity.Y = NPC.velocity.Y + probeAcceleration;
            }
            else if (NPC.velocity.Y > targetYDirection)
            {
                NPC.velocity.Y = NPC.velocity.Y - probeAcceleration;
            }
            NPC.localAI[0] += 1f;
            if (NPC.justHit)
            {
                NPC.localAI[0] = 0f;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.localAI[0] >= 200f)
            {
                NPC.localAI[0] = 0f;
                if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                {
                    // These are already nerfed in Master Mode via the global scaling code
                    int projDamage = Main.expertMode ? 14 : 18;
                    if (DownedBossSystem.downedAstrumAureus)
                        projDamage += 6;

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), probePosition.X, probePosition.Y, targetXDirection, targetYDirection, ProjectileID.PinkLaser, projDamage, 0f, Main.myPlayer, 0f, 0f);
                }
            }
            int npcTileX = (int)NPC.position.X + NPC.width / 2;
            int npcTileY = (int)NPC.position.Y + NPC.height / 2;
            npcTileX /= 16;
            npcTileY /= 16;
            if (!WorldGen.SolidTile(npcTileX, npcTileY))
            {
                Lighting.AddLight((int)((NPC.position.X + (float)(NPC.width / 2)) / 16f), (int)((NPC.position.Y + (float)(NPC.height / 2)) / 16f), 0.3f, 0f, 0.25f);
            }
            if (targetXDirection > 0f)
            {
                NPC.spriteDirection = 1;
                NPC.rotation = (float)Math.Atan2((double)targetYDirection, (double)targetXDirection);
            }
            if (targetXDirection < 0f)
            {
                NPC.spriteDirection = -1;
                NPC.rotation = (float)Math.Atan2((double)targetYDirection, (double)targetXDirection) + 3.14f;
            }
            float recoilVelocity = 0.7f;
            if (NPC.collideX)
            {
                NPC.netUpdate = true;
                NPC.velocity.X = NPC.oldVelocity.X * -recoilVelocity;
                if (NPC.direction == -1 && NPC.velocity.X > 0f && NPC.velocity.X < 2f)
                {
                    NPC.velocity.X = 2f;
                }
                if (NPC.direction == 1 && NPC.velocity.X < 0f && NPC.velocity.X > -2f)
                {
                    NPC.velocity.X = -2f;
                }
            }
            if (NPC.collideY)
            {
                NPC.netUpdate = true;
                NPC.velocity.Y = NPC.oldVelocity.Y * -recoilVelocity;
                if (NPC.velocity.Y > 0f && (double)NPC.velocity.Y < 1.5)
                {
                    NPC.velocity.Y = 2f;
                }
                if (NPC.velocity.Y < 0f && (double)NPC.velocity.Y > -1.5)
                {
                    NPC.velocity.Y = -2f;
                }
            }
            if (tooFar)
            {
                if ((NPC.velocity.X > 0f && targetXDirection > 0f) || (NPC.velocity.X < 0f && targetXDirection < 0f))
                {
                    if (Math.Abs(NPC.velocity.X) < 12f)
                    {
                        NPC.velocity.X = NPC.velocity.X * 1.05f;
                    }
                }
                else
                {
                    NPC.velocity.X = NPC.velocity.X * 0.9f;
                }
            }
            if (((NPC.velocity.X > 0f && NPC.oldVelocity.X < 0f) || (NPC.velocity.X < 0f && NPC.oldVelocity.X > 0f) || (NPC.velocity.Y > 0f && NPC.oldVelocity.Y < 0f) || (NPC.velocity.Y < 0f && NPC.oldVelocity.Y > 0f)) && !NPC.justHit)
            {
                NPC.netUpdate = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 halfSizeTexture = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / 2));
            Vector2 drawPosition = NPC.Center - screenPos;
            drawPosition -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height)) * NPC.scale / 2f;
            drawPosition += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);

            spriteBatch.Draw(texture2D15, drawPosition, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            texture2D15 = GlowTexture.Value;

            spriteBatch.Draw(texture2D15, drawPosition, NPC.frame, Color.White * 0.6f, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 15;
                SoundEngine.PlaySound(CommonCalamitySounds.AstralNPCHitSound, NPC.Center);
            }

            if (NPC.life <= 0)
            {
                NPC.position = NPC.Center;
                NPC.width = NPC.height = 30;
                NPC.Center = NPC.position;
                for (int d = 0; d < 5; d++)
                {
                    int purple = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[purple].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[purple].scale = 0.5f;
                        Main.dust[purple].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int d = 0; d < 10; d++)
                {
                    int cosmos = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
                    Main.dust[cosmos].noGravity = true;
                    Main.dust[cosmos].velocity *= 5f;
                    cosmos = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[cosmos].velocity *= 2f;
                }

                if (Main.netMode != NetmodeID.Server)
                {
                    Vector2 goreSource = NPC.Center;
                    int goreAmt = 3;
                    Vector2 source = new Vector2(goreSource.X - 24f, goreSource.Y - 24f);
                    for (int goreIndex = 0; goreIndex < goreAmt; goreIndex++)
                    {
                        float velocityMult = 0.33f;
                        if (goreIndex < (goreAmt / 3))
                        {
                            velocityMult = 0.66f;
                        }
                        if (goreIndex >= (2 * goreAmt / 3))
                        {
                            velocityMult = 1f;
                        }
                        Mod mod = ModContent.GetInstance<CalamityMod>();
                        int type = Main.rand.Next(61, 64);
                        int smoke = Gore.NewGore(NPC.GetSource_Death(), source, default, type, 1f);
                        Gore gore = Main.gore[smoke];
                        gore.velocity *= velocityMult;
                        gore.velocity.X += 1f;
                        gore.velocity.Y += 1f;
                        type = Main.rand.Next(61, 64);
                        smoke = Gore.NewGore(NPC.GetSource_Death(), source, default, type, 1f);
                        gore = Main.gore[smoke];
                        gore.velocity *= velocityMult;
                        gore.velocity.X -= 1f;
                        gore.velocity.Y += 1f;
                        type = Main.rand.Next(61, 64);
                        smoke = Gore.NewGore(NPC.GetSource_Death(), source, default, type, 1f);
                        gore = Main.gore[smoke];
                        gore.velocity *= velocityMult;
                        gore.velocity.X += 1f;
                        gore.velocity.Y -= 1f;
                        type = Main.rand.Next(61, 64);
                        smoke = Gore.NewGore(NPC.GetSource_Death(), source, default, type, 1f);
                        gore = Main.gore[smoke];
                        gore.velocity *= velocityMult;
                        gore.velocity.X -= 1f;
                        gore.velocity.Y -= 1f;
                    }
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<StarblightSoot>(), 2, 1, 2, 1, 3));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.Player))
            {
                return 0f;
            }
            else if (spawnInfo.Player.InAstral())
            {
                return 0.1f;
            }
            return 0f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 75, true);
        }
    }
}
