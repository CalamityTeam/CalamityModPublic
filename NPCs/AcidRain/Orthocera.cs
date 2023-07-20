using System;
using System.IO;
using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Particles.Metaballs;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.Projectiles.Magic;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AcidRain
{
    public class Orthocera : ModNPC
    {
        public ref float FallDelay => ref NPC.ai[0];
        public ref float Time => ref NPC.ai[1];
        public ref float HorizontalSpeed => ref NPC.ai[2];
        public bool PerformingJump
        {
            get => NPC.ai[3] == 1f;
            set
            {
                if (Main.netMode == NetmodeID.Server && value != PerformingJump)
                    NPC.netUpdate = true;
                NPC.ai[3] = value.ToInt();
            }
        }
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.width = 62;
            NPC.height = 34;
            NPC.aiStyle = AIType = -1;

            NPC.damage = 45;
            NPC.lifeMax = 280;
            NPC.defense = 15;
            NPC.DR_NERD(0.075f);

            if (DownedBossSystem.downedPolterghast)
            {
                NPC.damage = 120;
                NPC.lifeMax = 3850;
                NPC.defense = 35;
                NPC.DR_NERD(0.15f);
            }

            NPC.knockBackResist = 0.6f;
            NPC.value = Item.buyPrice(0, 0, 4, 20);
            NPC.lavaImmune = false;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath13;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<OrthoceraBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<AcidRainBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Orthocera")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.Calamity().newAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.Calamity().newAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            Time++;
            NPC.TargetClosest(false);
            float maxSpeed = DownedBossSystem.downedPolterghast ? 12.8f : 10.5f;
            if (!Main.player.IndexInRange(NPC.target))
                return;

            Player player = Main.player[NPC.target];

            // Swim.
            if (Time % 250f < 180f)
            {
                if (NPC.wet)
                {
                    // Reset to allow a jumps.
                    if (PerformingJump)
                        PerformingJump = false;

                    // Swim towards the player if we're not too close.
                    // If close, simply resume current movement.
                    if (!NPC.WithinRange(player.Center, 150f))
                    {
                        NPC.velocity = (NPC.velocity * 17f + NPC.SafeDirectionTo(player.Center, -Vector2.UnitY) * maxSpeed) / 18f;
                        if (FallDelay != 12f)
                        {
                            FallDelay = 12f;
                            NPC.netUpdate = true;
                        }
                    }

                    // Store for horizontal movement for later.
                    // It seems to slow down for some dumb reason in the jump phase without this value.
                    HorizontalSpeed = NPC.velocity.X;

                    // Make sure the value isn't 0 since we're relying on multiplication
                    if (HorizontalSpeed == 0f)
                        HorizontalSpeed = 0.1f;

                    if (Math.Abs(HorizontalSpeed) < 7f)
                        HorizontalSpeed = Math.Abs(HorizontalSpeed) * 7f;
                    if (Math.Abs(HorizontalSpeed) > 16f)
                        HorizontalSpeed = Math.Abs(HorizontalSpeed) * 16f;
                    HorizontalSpeed = Math.Abs(HorizontalSpeed) * (player.Center.X - NPC.Center.X > 0).ToDirectionInt();
                }
                else
                {
                    if (FallDelay <= 0f)
                        NPC.velocity.Y += 0.2f;
                    else
                        FallDelay--;
                }
                NPC.direction = NPC.spriteDirection = (NPC.velocity.X > 0).ToDirectionInt();
            }
            // And jump/shoot.
            else if (Time % 220f > 180f)
            {
                float verticalAcceleration = DownedBossSystem.downedPolterghast ? 0.07f : 0.05f;
                if (Time % 220f < 200f)
                    NPC.velocity.Y -= verticalAcceleration;
                else
                    NPC.velocity.Y += verticalAcceleration;

                if (Time % 220f == 219f)
                    PerformingJump = true;
                if (!NPC.wet)
                    NPC.velocity.X = HorizontalSpeed;
            }

            // Don't jump mid-air.
            if (Time % 220f > 180f && PerformingJump)
            {
                Time = 0f;
                NPC.netUpdate = true;
            }

            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.PiOver2 + MathHelper.Pi;

            if (NPC.spriteDirection == -1)
                NPC.rotation -= MathHelper.PiOver2;

            // If sitting on land, slow down and, if in the middle of a jump, release a stream of acid.
            if (!NPC.wet)
            {
                NPC.velocity.X *= 0.92f;

                // Spit out a stream of acid based on the rotation of this NPC.
                if (Main.netMode != NetmodeID.MultiplayerClient && Time % 220f == 195f)
                {
                    float spitDirection = NPC.rotation - MathHelper.TwoPi * 0.875f;
                    if (NPC.spriteDirection == -1)
                        spitDirection += MathHelper.PiOver2;

                    int damage = DownedBossSystem.downedPolterghast ? 40 : DownedBossSystem.downedAquaticScourge ? 26 : 18;
                    if (Main.expertMode)
                        damage = (int)Math.Round(damage * 0.8);

                    // Spit two extra streams of acid at the target post-Polterghast.
                    if (DownedBossSystem.downedPolterghast)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            float offsetAngle = MathHelper.Lerp(-0.3f, 0.3f, i / 2f);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (spitDirection + offsetAngle).ToRotationVector2() * 10f, ModContent.ProjectileType<OrthoceraStream>(), damage, 2f);
                        }
                    }
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, spitDirection.ToRotationVector2() * 12f, ModContent.ProjectileType<OrthoceraStream>(), damage, 2f);
                }
            }

            // Prevent yeeting into the sky at the speed of light.
            NPC.velocity = Vector2.Clamp(NPC.velocity, new Vector2(-maxSpeed), new Vector2(maxSpeed));

            if (Main.zenithWorld && !(!NPC.wet && NPC.collideY))
            {
                // Spread the wrath of the damned
                NPC.Calamity().newAI[0]++;
                if (NPC.Calamity().newAI[0] % 5 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, Main.rand.NextVector2Circular(4f, 8f), ModContent.ProjectileType<RancorFog>(), 0, 0f, Main.myPlayer);
                    FusableParticleManager.GetParticleSetByType<RancorGroundLavaParticleSet>().SpawnParticle(NPC.Bottom + Main.rand.NextVector2Circular(10f, 10f), 135f);
                }
                if (NPC.Calamity().newAI[0] % 30 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, Vector2.Zero, ModContent.ProjectileType<RancorArm>(), 20, 0f, Main.myPlayer, 0, -1);
                    if (p.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[p].DamageType = DamageClass.Default;
                        Main.projectile[p].friendly = false;
                    }
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<OrthoceraShell>(), 20);
            LeadingConditionRule postPolter = npcLoot.DefineConditionalDropSet(() => DownedBossSystem.downedPolterghast);
            postPolter.Add(ModContent.ItemType<CorrodedFossil>(), 15, 1, 3, !DownedBossSystem.downedPolterghast);
            postPolter.AddFail(ModContent.ItemType<CorrodedFossil>(), 3, 1, 3, DownedBossSystem.downedPolterghast);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 6)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= Main.npcFrameCount[NPC.type] * frameHeight)
                    NPC.frame.Y = 0;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hit.HitDirection, -1f, 0, default, 1f);
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("OrthoceraGore").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("OrthoceraGore2").Type, NPC.scale);
                }
                for (int k = 0; k < 10; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }
    }
}
