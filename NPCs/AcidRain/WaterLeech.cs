using CalamityMod.Dusts;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.StatDebuffs;
using System.IO;

namespace CalamityMod.NPCs.AcidRain
{
    public class WaterLeech : ModNPC
    {
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

            npc.damage = 10;
            npc.lifeMax = 30;

            if (CalamityWorld.downedPolterghast)
            {
                npc.damage = 60;
                npc.lifeMax = 2250;
                npc.defense = 10;
            }
            else if (CalamityWorld.downedAquaticScourge)
            {
                npc.damage = 30;
                npc.lifeMax = 90;
            }

            npc.value = Item.buyPrice(0, 0, 2, 5);
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.lavaImmune = false;
            npc.noGravity = true;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit33;
            npc.DeathSound = SoundID.NPCDeath1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<WaterLeechBanner>();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.dontTakeDamage = reader.ReadBoolean();
        }

        public override void AI()
        {
            if (npc.localAI[0] == 0f)
            {
                npc.TargetClosest(false);
                npc.localAI[0] = 1f;
            }

            // Anti-sticky movement and player targeting detection (only one leech and attack the player at once)
            Player player = Main.player[npc.target];
            bool playerAlreadyTargeted = false;
            float antiStickyAcceleration = 0.25f;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].type == npc.type &&
                    Main.npc[i].whoAmI != npc.whoAmI &&
                    Main.npc[i].active)
                {
                    if (Main.npc[i].target == npc.target && !playerAlreadyTargeted)
                    {
                        playerAlreadyTargeted = true;
                    }
                    if (Main.npc[i].Hitbox.Intersects(npc.Hitbox))
                    {
                        npc.velocity += npc.DirectionFrom(Main.npc[i].Center) * antiStickyAcceleration;
                    }
                }
            }

            if (!playerAlreadyTargeted || player.Calamity().waterLeechTarget == -1)
            {
                player.Calamity().waterLeechTarget = npc.whoAmI;
            }

            // Latch onto player
            if (npc.ai[0] == 1f)
            {
                if (npc.dontTakeDamage)
                {
                    npc.dontTakeDamage = false;
                    npc.netUpdate = true;
                }
                if (npc.Distance(player.Top) >= 140f)
                {
                    npc.ai[0] = 0f;
                    npc.netUpdate = true;
                    return;
                }
                Vector2 destination = ((player.gravDir == 1f) ? player.Top : player.Bottom) + player.direction * 4 * Vector2.UnitX;
                float speed = Utils.SmoothStep(10f, ChaseMaxDistance, npc.Distance(destination)) * 16f;
                npc.velocity = npc.DirectionTo(destination) * (speed + 7f);
                if (npc.Distance(destination) < 45f)
                {
                    player.AddBuff(BuffID.Bleeding, 180, true);
                }
                else if (!npc.wet && npc.Distance(destination) > 85f)
                {
                    npc.ai[0] = 0f;
                    npc.netUpdate = true;
                }
                return;
            }
            npc.direction = npc.spriteDirection = (npc.velocity.X > 0).ToDirectionInt();
            if (!npc.wet)
            {
                npc.velocity.X *= 0.97f;
                npc.velocity.Y += 0.5f;
                npc.ai[2] += 1f;
                if (npc.ai[2] >= 300f)
                {
                    npc.life = 0;
                    npc.HitEffect();
                    npc.active = false;
                    npc.netUpdate = true;
                }
            }
            else if (player.wet && player.Calamity().waterLeechTarget == npc.whoAmI)
            {
                float speed = player.wet ? 23f : 17f;
                float swimIntertia = 24f;
                if (CalamityWorld.downedPolterghast)
                {
                    speed *= 1.6f;
                    swimIntertia = 17f;
                }
                npc.velocity = (npc.velocity * (swimIntertia - 1f) + npc.DirectionTo(player.Center) * speed) / swimIntertia;
            }
            else if (!player.wet)
            {
                npc.velocity *= 0.9f;
            }

            if (npc.Distance(player.Top) < ChasePromptDistance && player.active && !player.dead)
            {
                npc.ai[0] = 1f;
                npc.netUpdate = true;
            }
            npc.dontTakeDamage = !player.wet;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter >= 4)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y >= Main.npcFrameCount[npc.type] * frameHeight)
                {
                    npc.frame.Y = 0;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !spawnInfo.player.Calamity().ZoneSulphur || !Main.raining)
            {
                return 0f;
            }
            return 0.115f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 8; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
        }

        public override void NPCLoot()
        {
			float dropChance = CalamityWorld.downedAquaticScourge ? 0.01f : 0.05f;
            DropHelper.DropItemChance(npc, ModContent.ItemType<ParasiticSceptor>(), dropChance);
        }
    }
}
