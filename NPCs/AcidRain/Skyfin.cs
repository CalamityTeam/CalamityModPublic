using CalamityMod.Dusts;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Materials;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.StatDebuffs;
namespace CalamityMod.NPCs.AcidRain
{
    public class Skyfin : ModNPC
    {
        public const float DiveDelay = 120f;
        public const float DiveTime = 90f;
        public const float TotalTime = DiveDelay + DiveTime;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skyfin");
            Main.npcFrameCount[npc.type] = 5;
        }

        public override void SetDefaults()
        {
            npc.width = 46;
            npc.height = 22;
            npc.aiStyle = aiType = -1;

            npc.damage = 40;
            npc.lifeMax = 360;
            npc.defense = 6;

            if (CalamityWorld.downedPolterghast)
            {
                npc.damage = 285;
                npc.lifeMax = 6300;
                npc.defense = 58;
            }
            else if (CalamityWorld.downedAquaticScourge)
            {
                npc.damage = 85;
                npc.lifeMax = 700;
            }

            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 0, 3, 65);
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.lavaImmune = false;
            npc.noGravity = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<SkyfinBanner>();
        }
        public override void AI()
        {
            npc.TargetClosest(false);
            Player player = Main.player[npc.target];
            npc.ai[0] += 1f;
            if (npc.ai[1] > 0f)
                npc.ai[1] -= 1f;
            if (npc.wet)
            {
                // Swim around, moving towards the player
                bool canSwimToPlayer = Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height);
                if (canSwimToPlayer)
                {
                    if (npc.ai[0] % 55f == 54f)
                    {
                        npc.velocity = Vector2.UnitX * (player.Center.X - npc.Center.X > 0).ToDirectionInt() * 27f;
                    }
                    if ((Math.Abs(player.Center.Y - npc.Center.Y) > 50f && player.wet) || (!player.wet && npc.ai[1] <= 0f))
                    {
                        float speedY = CalamityWorld.downedPolterghast ? 15f : 6f;
                        npc.velocity.Y = (player.Center.Y - npc.Center.Y > 0).ToDirectionInt() * speedY;
                    }
                    if (Math.Abs(npc.velocity.X) < 6f)
                        npc.velocity.X *= 1.04f;
                }
                else if (!canSwimToPlayer && Math.Abs(npc.velocity.Y) < 4f)
                {
                    npc.velocity.Y *= 0.97f;
                }
                // Turn around if we hit a tile on the X axis
                if (!canSwimToPlayer && npc.collideX)
                {
                    npc.velocity.X *= -1f;
                }

                // Check if we can dive
                if (player.Center.Y < npc.Top.Y - 10f &&
                    npc.ai[1] <= 0f)
                {
                    if (Main.rand.NextBool(10))
                    {
                        npc.ai[1] = TotalTime;
                    }
                    npc.ai[2] = (player.Center.X - npc.Center.X > 0).ToDirectionInt() * 10f;
                }
            }
            else
            {
                // Dive upward in an attempt to hit to the player
                if (npc.ai[1] > TotalTime - DiveTime)
                {
                    npc.velocity.X = npc.ai[2];
                    if (npc.ai[1] > TotalTime - DiveTime * 0.5f)
                    {
                        float flySpeed = CalamityWorld.downedAquaticScourge ? 0.135f : 0.085f;
                        if (CalamityWorld.downedPolterghast)
                        {
                            flySpeed = 0.185f;
                        }
                        npc.velocity.Y -= flySpeed;
                    }
                    else
                    {
                        npc.ai[1] = TotalTime - DiveTime;
                        npc.velocity.Y += 0.2f;
                    }
                }
                else
                {
                    // Don't fall too fast because of wings
                    npc.ai[1] = TotalTime - DiveTime;
                    npc.velocity.Y += 0.1f;
                }
            }
            int idealDirection = (npc.velocity.X > 0).ToDirectionInt();
            npc.direction = npc.spriteDirection = idealDirection;
            if (idealDirection != npc.direction)
            {
                npc.netUpdate = true;
            }
            // If sitting on land, rotate in a way that looks like we're stuck on the ground
            if (!npc.wet)
            {
                npc.velocity.X *= 0.92f;
            }
            npc.rotation = npc.velocity.ToRotation() +
                (npc.direction > 0).ToInt() * MathHelper.Pi;
        }
        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter >= 5)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y >= Main.npcFrameCount[npc.type] * frameHeight)
                {
                    npc.frame.Y = 0;
                }
            }
        }
        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<SulfuricScale>(), 2 * (CalamityWorld.downedAquaticScourge ? 6 : 1), 1, 3);
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.85f);
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 8; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/SkyfinGore"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/SkyfinGore2"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/SkyfinGore3"), npc.scale);
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }
    }
}
