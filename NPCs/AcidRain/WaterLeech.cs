using CalamityMod.Dusts;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.StatDebuffs;
using System.IO;

namespace CalamityMod.NPCs.AcidRain
{
    public class WaterLeech : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Water Leech");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.width = 26;
            npc.height = 14;

            npc.damage = 44;
            npc.lifeMax = 90;
            npc.defense = 0;

            if (CalamityWorld.downedPolterghast)
            {
                npc.damage = 172;
                npc.lifeMax = 4000;
                npc.defense = 30;
            }
            else if (CalamityWorld.downedAquaticScourge)
            {
                npc.damage = 100;
                npc.lifeMax = 320;
            }

            npc.knockBackResist = 0f;
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
            npc.TargetClosest(false);
            Player player = Main.player[npc.target];
            npc.direction = npc.spriteDirection = (npc.velocity.X > 0).ToDirectionInt();
            // Latch onto player
            if (npc.ai[0] == 1f)
            {
                if (!player.active || player.dead)
                {
                    // Revert back to swimming around after successfully devouring the player
                    npc.ai[0] = 0f;
                    npc.netUpdate = true;
                    return;
                }
                // Stick to player and make them bleed
                if (npc.ai[1] > 0f)
                {
                    npc.position = player.position;
                    npc.ai[1] -= 1f;
                    player.AddBuff(BuffID.Bleeding, 180);
                }
                // Die
                else
                {
                    for (int i = 0; i < 25; i++)
                    {
                        Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, Main.rand.NextFloat(-2f, 2f), -1f, 0, default, 1f);
                    }
                    npc.life = 0;
                    npc.HitEffect();
                    npc.active = false;
                    npc.netUpdate = true;
                }
                return;
            }
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
            else if (player.wet)
            {
                float speed = player.wet ? 23f : 15f;
                float swimIntertia = 24f;
                if (CalamityWorld.downedPolterghast)
                {
                    speed *= 1.6f;
                    swimIntertia = 13f;
                }

                if (npc.Distance(player.Center) < 26f)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 300f;
                    npc.netUpdate = true;
                }
                npc.velocity = (npc.velocity * (swimIntertia - 1f) + npc.DirectionTo(player.Center) * speed) / swimIntertia;
            }
            else if (!player.wet)
            {
                npc.velocity *= 0.9f;
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
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.85f);
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
    }
}
