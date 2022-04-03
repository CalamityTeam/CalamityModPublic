using CalamityMod.Dusts;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.NPCs.AcidRain
{
    public class WaterLeech : ModNPC
    {
        public Player Target => Main.player[NPC.target];
        public bool LatchingOntoTarget
        {
            get => NPC.ai[0] == 1f;
            set => NPC.ai[0] = value.ToInt();
        }
        public ref float LackOfWaterDeathTimer => ref NPC.ai[2];
        public const float ChasePromptDistance = 55f;
        public const float ChaseMaxDistance = 140f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Water Leech");
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.width = 26;
            NPC.height = 14;

            NPC.lifeMax = 30;
            NPC.damage = 0;

            if (CalamityWorld.downedPolterghast)
            {
                NPC.lifeMax = 1235;
                NPC.defense = 10;
            }
            else if (CalamityWorld.downedAquaticScourge)
                NPC.lifeMax = 90;

            NPC.value = Item.buyPrice(0, 0, 2, 5);
            NPC.lavaImmune = false;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit33;
            NPC.DeathSound = SoundID.NPCDeath1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<WaterLeechBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(NPC.dontTakeDamage);

        public override void ReceiveExtraAI(BinaryReader reader) => NPC.dontTakeDamage = reader.ReadBoolean();

        public override void AI()
        {
            if (NPC.localAI[0] == 0f)
            {
                NPC.TargetClosest();
                NPC.localAI[0] = 1f;
            }

            bool playerAlreadyTargeted = false;
            float antiStickyAcceleration = 0.25f;

            // Repel other leeches from this one. Also determine if one is already targeting a player.
            // Only one leech is allowed to target one player at a time.
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].type != NPC.type || !Main.npc[i].active || Main.npc[i].whoAmI == NPC.whoAmI)
                    continue;

                if (Main.npc[i].target == NPC.target && !playerAlreadyTargeted)
                    playerAlreadyTargeted = true;
                if (Main.npc[i].Hitbox.Intersects(NPC.Hitbox))
                    NPC.velocity += NPC.DirectionFrom(Main.npc[i].Center) * antiStickyAcceleration;
            }

            if (!playerAlreadyTargeted || Target.Calamity().waterLeechTarget == -1)
                Target.Calamity().waterLeechTarget = NPC.whoAmI;

            if (LatchingOntoTarget)
            {
                LatchOntoTarget();
                return;
            }

            NPC.direction = NPC.spriteDirection = (NPC.velocity.X > 0).ToDirectionInt();

            // If the leech is not in water, fall and quickly cease horizontal movement.
            // After some time, if still not in water, die due to a lack of water.
            if (!NPC.wet)
            {
                NPC.velocity.X *= 0.97f;
                NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y + 0.4f, -4f, 16f);
                LackOfWaterDeathTimer++;
                if (LackOfWaterDeathTimer >= 300f)
                {
                    NPC.life = 0;
                    NPC.HitEffect();
                    NPC.active = false;
                    NPC.netUpdate = true;
                }
            }

            // Swim towards the target if they are in water.
            else if (Target.wet && Target.Calamity().waterLeechTarget == NPC.whoAmI)
            {
                float swimSpeed = Target.wet ? 23f : 17f;
                float swimIntertia = 24f;
                if (CalamityWorld.downedPolterghast)
                {
                    swimSpeed *= 1.6f;
                    swimIntertia = 17f;
                }
                NPC.velocity = (NPC.velocity * (swimIntertia - 1f) + NPC.SafeDirectionTo(Target.Center, -Vector2.UnitY) * swimSpeed) / swimIntertia;
            }

            // Rapidly slow down if the target is not in water.
            else if (!Target.wet)
                NPC.velocity *= 0.9f;

            if (NPC.WithinRange(Target.Top, ChasePromptDistance) && Target.active && !Target.dead)
            {
                LatchingOntoTarget = true;
                NPC.netUpdate = true;
            }

            // If the target is not in water, become invincible.
            bool shouldNotTakeDamage = !Target.wet;
            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.dontTakeDamage != shouldNotTakeDamage)
            {
                NPC.dontTakeDamage = shouldNotTakeDamage;
                NPC.netUpdate = true;
            }
        }

        public void LatchOntoTarget()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.dontTakeDamage)
            {
                NPC.dontTakeDamage = false;
                NPC.netUpdate = true;
            }

            // If the target moved far enough away, stop attempting to attach to them.
            if (!NPC.WithinRange(Target.Center, ChaseMaxDistance))
            {
                LatchingOntoTarget = false;
                NPC.netUpdate = true;
                return;
            }

            Vector2 destination = (Target.gravDir == 1f ? Target.Top : Target.Bottom) + Target.direction * Vector2.UnitX * 4f;
            float speed = MathHelper.Lerp(7f, 23f, Utils.InverseLerp(10f, ChaseMaxDistance, NPC.Distance(destination), true));
            NPC.velocity = NPC.SafeDirectionTo(destination) * speed;

            if (NPC.WithinRange(destination, 45f))
            {
                Target.AddBuff(BuffID.Bleeding, 180, true);
                Target.AddBuff(ModContent.BuffType<HeavyBleeding>(), 30, true);
            }

            // Stop searching if the target is not in water anymore and they're not close.
            else if (!NPC.wet && !NPC.WithinRange(destination, 85f))
            {
                LatchingOntoTarget = false;
                NPC.netUpdate = true;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 4)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= Main.npcFrameCount[NPC.type] * frameHeight)
                    NPC.frame.Y = 0;
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !spawnInfo.Player.Calamity().ZoneSulphur || !Main.raining)
                return 0f;

            return 0.115f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 8; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
        }

        public override void NPCLoot()
        {
            float dropChance = CalamityWorld.downedAquaticScourge ? 0.01f : 0.05f;
            DropHelper.DropItemChance(NPC, ModContent.ItemType<ParasiticSceptor>(), dropChance);
        }
    }
}
