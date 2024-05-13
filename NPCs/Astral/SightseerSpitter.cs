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
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Astral
{
    public class SightseerSpitter : ModNPC
    {
        public static Asset<Texture2D> glowmask;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            if (!Main.dedServ)
                glowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/Astral/SightseerSpitterGlow", AssetRequestMode.AsyncLoad);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.7f,
                Velocity = 2f,
                PortraitPositionYOverride = 0
            };
            value.Position.X += 15;
            value.Position.Y -= 10;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.width = 64;
            NPC.height = 56;
            NPC.damage = 50;
            NPC.defense = 20;
            NPC.DR_NERD(0.15f);
            NPC.lifeMax = 430;
            NPC.DeathSound = CommonCalamitySounds.AstralNPCDeathSound;
            NPC.noGravity = true;
            NPC.knockBackResist = 0.8f;
            NPC.value = Item.buyPrice(0, 0, 20, 0);
            NPC.aiStyle = -1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<SightseerSpitterBanner>();
            if (DownedBossSystem.downedAstrumAureus)
            {
                NPC.damage = 85;
                NPC.defense = 30;
                NPC.knockBackResist = 0.7f;
                NPC.lifeMax = 640;
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
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.SightseerSpitter")
            });
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.05f + NPC.velocity.Length() * 0.667f;
            if (NPC.frameCounter >= 8)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > NPC.height * 3)
                {
                    NPC.frame.Y = 0;
                }
            }

            //DO DUST
            Dust d = CalamityGlobalNPC.SpawnDustOnNPC(NPC, 118, frameHeight, ModContent.DustType<AstralOrange>(), new Rectangle(70, 18, 48, 18), Vector2.Zero, 0.45f, true);
            if (d != null)
            {
                d.customData = 0.04f;
            }
        }

        public override void AI()
        {
            CalamityGlobalNPC.DoFlyingAI(NPC, 4f, 0.025f, 300f);

            NPC.ai[1]++;
            Player target = Main.player[NPC.target];

            if (NPC.justHit || target.dead)
            {
                //reset if hit
                NPC.ai[1] = 0;
            }

            //if can see target and waited long enough
            if (Collision.CanHit(target.position, target.width, target.height, NPC.position, NPC.width, NPC.height))
            {
                Vector2 vector = target.Center - NPC.Center;
                vector.Normalize();
                Vector2 spawnPoint = NPC.Center + vector * 42f;

                if (NPC.ai[1] >= (CalamityWorld.death ? 80f : CalamityWorld.revenge ? 120f : 160f))
                {
                    NPC.ai[1] = 0f;

                    int n = NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPoint.X, (int)spawnPoint.Y, ModContent.NPCType<AstralSeekerSpit>());
                    Main.npc[n].Center = spawnPoint;
                    Main.npc[n].velocity = vector * (CalamityWorld.death ? 12f : CalamityWorld.revenge ? 11f : 10f);
                }
                else if (NPC.ai[1] >= 140f) //oozin dust at the "mouth"
                {
                    int dustType = Main.rand.NextBool() ? ModContent.DustType<AstralOrange>() : ModContent.DustType<AstralBlue>();
                    int d = Dust.NewDust(spawnPoint - new Vector2(5), 10, 10, dustType);
                    Main.dust[d].velocity = NPC.velocity * 0.3f;
                    Main.dust[d].customData = true;
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 15;
                SoundEngine.PlaySound(CommonCalamitySounds.AstralNPCHitSound, NPC.Center);
            }

            CalamityGlobalNPC.DoHitDust(NPC, hit.HitDirection, (Main.rand.Next(0, Math.Max(0, NPC.life)) == 0) ? 5 : ModContent.DustType<AstralEnemy>(), 1f, 4, 22);

            //if dead do gores
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        float rand = Main.rand.NextFloat(-0.18f, 0.18f);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.NextFloat(0f, NPC.width), Main.rand.NextFloat(0f, NPC.height)), NPC.velocity * rand, Mod.Find<ModGore>("SightseerSpitterGore" + i).Type);
                    }
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!NPC.IsABestiaryIconDummy)
                spriteBatch.Draw(glowmask.Value, NPC.Center - screenPos + new Vector2(0, 4f), NPC.frame, Color.White * 0.75f, NPC.rotation, new Vector2(59f, 28f), NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.Player))
            {
                return 0f;
            }
            else if (spawnInfo.Player.InAstral(1))
            {
                return spawnInfo.Player.ZoneDesert ? 0.14f : 0.17f;
            }
            return 0f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 45, true);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<StarblightSoot>(), 1, 2, 3, 3, 4));
        }
    }

    public class AstralSeekerSpit : ModNPC
    {
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.ProjectileNPC[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 16;
            NPC.height = 16;
            NPC.damage = 45;
            NPC.defense = 0;
            NPC.lifeMax = 1;
            NPC.HitSound = null;
            NPC.DeathSound = SoundID.NPCDeath9;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.alpha = 80;
            NPC.aiStyle = -1;
            if (DownedBossSystem.downedAstrumAureus)
            {
                NPC.damage = 75;
            }
        }

        public override void AI()
        {
            //DUST
            NPC.ai[0] += 0.18f;
            float angle = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            float pulse = (float)Math.Sin(NPC.ai[0]);
            float radius = 5.8f;
            Vector2 offset = angle.ToRotationVector2() * pulse * radius;
            Dust.NewDustPerfect(NPC.Center + offset, ModContent.DustType<AstralOrange>(), Vector2.Zero);
            Dust.NewDustPerfect(NPC.Center - offset, ModContent.DustType<AstralBlue>(), Vector2.Zero);

            //kill on tile collide
            if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.StrikeInstantKill();
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (modifiers.GetDamage(NPC.damage, false) > 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.StrikeInstantKill();
            }
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (modifiers.GetDamage(NPC.damage, 0f, 0f) > 0)
            {
                if (target.HasNPCBannerBuff(ModContent.NPCType<SightseerSpitter>()))
                {
                    if (Main.expertMode)
                        modifiers.IncomingDamageMultiplier *= 0.5f;
                    else
                        modifiers.IncomingDamageMultiplier *= 0.75f;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.StrikeInstantKill();
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            DoKillDust();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 45, true);
        }

        private void DoKillDust()
        {
            int numDust = Main.rand.Next(17, 25);
            float rotPerIter = MathHelper.TwoPi / numDust;
            float angle = 0;
            for (int i = 0; i < numDust; i++)
            {
                Vector2 vel = (angle + Main.rand.NextFloat(-0.04f, 0.04f)).ToRotationVector2();
                int dustType = Main.rand.NextBool() ? ModContent.DustType<AstralOrange>() : ModContent.DustType<AstralBlue>();
                Dust d = Dust.NewDustPerfect(NPC.Center, dustType, vel * Main.rand.NextFloat(1.8f, 2.2f));
                d.customData = NPC;

                angle += rotPerIter;
            }
        }
    }
}
