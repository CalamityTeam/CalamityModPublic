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
        public Player Target => Main.player[npc.target];
        public bool LatchingOntoTarget
        {
            get => npc.ai[0] == 1f;
            set => npc.ai[0] = value.ToInt();
        }
        public ref float LackOfWaterDeathTimer => ref npc.ai[2];
        public const float ChasePromptDistance = 55f;
        public const float ChaseMaxDistance = 140f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Water Leech");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.width = 26;
            npc.height = 14;

            npc.lifeMax = 30;
            npc.damage = 0;

            if (CalamityWorld.downedPolterghast)
            {
                npc.lifeMax = 1235;
                npc.defense = 10;
            }
            else if (CalamityWorld.downedAquaticScourge)
                npc.lifeMax = 90;

            npc.value = Item.buyPrice(0, 0, 2, 5);
            npc.lavaImmune = false;
            npc.noGravity = true;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit33;
            npc.DeathSound = SoundID.NPCDeath1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<WaterLeechBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(npc.dontTakeDamage);

        public override void ReceiveExtraAI(BinaryReader reader) => npc.dontTakeDamage = reader.ReadBoolean();

        public override void AI()
        {
            if (npc.localAI[0] == 0f)
            {
                npc.TargetClosest();
                npc.localAI[0] = 1f;
            }

            bool playerAlreadyTargeted = false;
            float antiStickyAcceleration = 0.25f;

            // Repel other leeches from this one. Also determine if one is already targeting a player.
            // Only one leech is allowed to target one player at a time.
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].type != npc.type || !Main.npc[i].active || Main.npc[i].whoAmI == npc.whoAmI)
                    continue;

                if (Main.npc[i].target == npc.target && !playerAlreadyTargeted)
                    playerAlreadyTargeted = true;
                if (Main.npc[i].Hitbox.Intersects(npc.Hitbox))
                    npc.velocity += npc.DirectionFrom(Main.npc[i].Center) * antiStickyAcceleration;
            }

            if (!playerAlreadyTargeted || Target.Calamity().waterLeechTarget == -1)
                Target.Calamity().waterLeechTarget = npc.whoAmI;

            if (LatchingOntoTarget)
            {
                LatchOntoTarget();
                return;
            }

            npc.direction = npc.spriteDirection = (npc.velocity.X > 0).ToDirectionInt();

            // If the leech is not in water, fall and quickly cease horizontal movement.
            // After some time, if still not in water, die due to a lack of water.
            if (!npc.wet)
            {
                npc.velocity.X *= 0.97f;
                npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y + 0.4f, -4f, 16f);
                LackOfWaterDeathTimer++;
                if (LackOfWaterDeathTimer >= 300f)
                {
                    npc.life = 0;
                    npc.HitEffect();
                    npc.active = false;
                    npc.netUpdate = true;
                }
            }

            // Swim towards the target if they are in water.
            else if (Target.wet && Target.Calamity().waterLeechTarget == npc.whoAmI)
            {
                float swimSpeed = Target.wet ? 23f : 17f;
                float swimIntertia = 24f;
                if (CalamityWorld.downedPolterghast)
                {
                    swimSpeed *= 1.6f;
                    swimIntertia = 17f;
                }
                npc.velocity = (npc.velocity * (swimIntertia - 1f) + npc.SafeDirectionTo(Target.Center, -Vector2.UnitY) * swimSpeed) / swimIntertia;
            }

            // Rapidly slow down if the target is not in water.
            else if (!Target.wet)
                npc.velocity *= 0.9f;

            if (npc.WithinRange(Target.Top, ChasePromptDistance) && Target.active && !Target.dead)
            {
                LatchingOntoTarget = true;
                npc.netUpdate = true;
            }

            // If the target is not in water, become invincible.
            bool shouldNotTakeDamage = !Target.wet;
            if (Main.netMode != NetmodeID.MultiplayerClient && npc.dontTakeDamage != shouldNotTakeDamage)
            {
                npc.dontTakeDamage = shouldNotTakeDamage;
                npc.netUpdate = true;
            }
        }

        public void LatchOntoTarget()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && npc.dontTakeDamage)
            {
                npc.dontTakeDamage = false;
                npc.netUpdate = true;
            }

            // If the target moved far enough away, stop attempting to attach to them.
            if (!npc.WithinRange(Target.Center, ChaseMaxDistance))
            {
                LatchingOntoTarget = false;
                npc.netUpdate = true;
                return;
            }

            Vector2 destination = (Target.gravDir == 1f ? Target.Top : Target.Bottom) + Target.direction * Vector2.UnitX * 4f;
            float speed = MathHelper.Lerp(7f, 23f, Utils.InverseLerp(10f, ChaseMaxDistance, npc.Distance(destination), true));
            npc.velocity = npc.SafeDirectionTo(destination) * speed;

            if (npc.WithinRange(destination, 45f))
            {
                Target.AddBuff(BuffID.Bleeding, 180, true);
                Target.AddBuff(ModContent.BuffType<HeavyBleeding>(), 30, true);
            }

            // Stop searching if the target is not in water anymore and they're not close.
            else if (!npc.wet && !npc.WithinRange(destination, 85f))
            {
                LatchingOntoTarget = false;
                npc.netUpdate = true;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter >= 4)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y >= Main.npcFrameCount[npc.type] * frameHeight)
                    npc.frame.Y = 0;
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !spawnInfo.player.Calamity().ZoneSulphur || !Main.raining)
                return 0f;

            return 0.115f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 8; k++)
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
        }

        public override void NPCLoot()
        {
            float dropChance = CalamityWorld.downedAquaticScourge ? 0.01f : 0.05f;
            DropHelper.DropItemChance(npc, ModContent.ItemType<ParasiticSceptor>(), dropChance);
        }
    }
}
