using CalamityMod.BiomeManagers;
using CalamityMod.Dusts;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.AcidRain
{
    public class Skyfin : ModNPC
    {
        public ref float AttackState => ref NPC.ai[0];
        public ref float AttackTimer => ref NPC.ai[1];
        public Player Target => Main.player[NPC.target];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Rotation = MathHelper.Pi
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.width = 46;
            NPC.height = 22;
            NPC.aiStyle = AIType = -1;

            NPC.damage = 12;
            NPC.lifeMax = 50;
            NPC.defense = 6;
            NPC.knockBackResist = 1f;

            if (DownedBossSystem.downedPolterghast)
            {
                NPC.knockBackResist = 0.8f;
                NPC.damage = 88;
                NPC.lifeMax = 3025;
                NPC.defense = 18;
                NPC.DR_NERD(0.05f);
            }
            else if (DownedBossSystem.downedAquaticScourge)
            {
                NPC.damage = 38;
                NPC.lifeMax = 220;
                NPC.DR_NERD(0.05f);
            }

            NPC.value = Item.buyPrice(0, 0, 3, 65);
            NPC.lavaImmune = false;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<SkyfinBanner>();
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
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Skyfin")
            });
        }

        public override void AI()
        {
            NPC.TargetClosest(false);
            int idealDirection = (NPC.velocity.X > 0).ToDirectionInt();
            NPC.spriteDirection = idealDirection;

            switch ((int)AttackState)
            {
                // Rise upward.
                case 0:
                    Vector2 flyDestination = Target.Center + new Vector2((Target.Center.X < NPC.Center.X).ToDirectionInt() * 400f, -240f);
                    Vector2 idealVelocity = NPC.SafeDirectionTo(flyDestination) * 10f;
                    NPC.velocity = (NPC.velocity * 29f + idealVelocity) / 29f;
                    NPC.velocity = NPC.velocity.MoveTowards(idealVelocity, 1.5f);

                    // Decide rotation.
                    NPC.rotation = NPC.velocity.ToRotation() + (NPC.spriteDirection > 0).ToInt() * MathHelper.Pi;

                    if (NPC.WithinRange(flyDestination, 40f) || AttackTimer > 150f)
                    {
                        AttackState = 1f;
                        NPC.velocity *= 0.65f;
                        NPC.netUpdate = true;
                    }
                    break;

                // Slow down and look at the target.
                case 1:
                    NPC.spriteDirection = (Target.Center.X > NPC.Center.X).ToDirectionInt();
                    NPC.velocity *= 0.97f;
                    NPC.velocity = NPC.velocity.MoveTowards(Vector2.Zero, 0.25f);
                    NPC.rotation = NPC.rotation.AngleTowards(NPC.AngleTo(Target.Center) + (NPC.spriteDirection > 0).ToInt() * MathHelper.Pi, 0.2f);

                    // Charge once sufficiently slowed down.
                    float chargeSpeed = 11.5f;
                    if (DownedBossSystem.downedAquaticScourge)
                        chargeSpeed += 4f;
                    if (DownedBossSystem.downedPolterghast)
                        chargeSpeed += 3.5f;
                    if (NPC.velocity.Length() < 1.25f)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown, NPC.Center);
                        for (int i = 0; i < 36; i++)
                        {
                            Dust acid = Dust.NewDustPerfect(NPC.Center, (int)CalamityDusts.SulfurousSeaAcid);
                            acid.velocity = (MathHelper.TwoPi * i / 36f).ToRotationVector2() * 6f;
                            acid.scale = 1.1f;
                            acid.noGravity = true;
                        }

                        AttackState = 2f;
                        AttackTimer = 0f;
                        NPC.velocity = NPC.SafeDirectionTo(Target.Center) * chargeSpeed;
                        NPC.netUpdate = true;
                    }
                    break;

                // Charge and swoop.
                case 2:
                    float angularTurnSpeed = MathHelper.Pi / 300f;
                    idealVelocity = NPC.SafeDirectionTo(Target.Center);
                    Vector2 leftVelocity = NPC.velocity.RotatedBy(-angularTurnSpeed);
                    Vector2 rightVelocity = NPC.velocity.RotatedBy(angularTurnSpeed);
                    if (leftVelocity.AngleBetween(idealVelocity) < rightVelocity.AngleBetween(idealVelocity))
                        NPC.velocity = leftVelocity;
                    else
                        NPC.velocity = rightVelocity;

                    // Decide rotation.
                    NPC.rotation = NPC.velocity.ToRotation() + (NPC.spriteDirection > 0).ToInt() * MathHelper.Pi;

                    if (AttackTimer > 50f)
                    {
                        AttackState = 0f;
                        AttackTimer = 0f;
                        NPC.velocity = Vector2.Lerp(NPC.velocity, -Vector2.UnitY * 8f, 0.14f);
                        NPC.netUpdate = true;
                    }
                    break;
            }
            AttackTimer++;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= Main.npcFrameCount[NPC.type] * frameHeight)
                    NPC.frame.Y = 0;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<SulphuricScale>(), 2, 1, 3);
            LeadingConditionRule postAS = npcLoot.DefineConditionalDropSet(DropHelper.PostAS());
            postAS.Add(ModContent.ItemType<SkyfinBombers>(), 20);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 8; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hit.HitDirection, -1f, 0, default, 1f);
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SkyfinGore").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SkyfinGore2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SkyfinGore3").Type, NPC.scale);
                }
                for (int k = 0; k < 20; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hit.HitDirection, -1f, 0, default, 1f);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
        }
    }
}
