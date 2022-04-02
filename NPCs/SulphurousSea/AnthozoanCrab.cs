using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SulphurousSea
{
    public class AnthozoanCrab : ModNPC
    {
        public int boulderIndex;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Anthozoan Crab");
            Main.npcFrameCount[npc.type] = 16;
        }

        public override void SetDefaults()
        {
            npc.noGravity = true;
            npc.damage = 45;
            npc.width = 56;
            npc.height = 42;
            npc.defense = 22;
            npc.lifeMax = 920;
            npc.aiStyle = aiType = -1;
            npc.value = Item.buyPrice(0, 0, 1, 0);
            npc.HitSound = SoundID.NPCHit38;
            npc.DeathSound = SoundID.NPCDeath46;
            npc.knockBackResist = 0.04f;
            banner = npc.type;
            bannerItem = ModContent.ItemType<AnthozoanCrabBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(boulderIndex);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            boulderIndex = reader.ReadInt32();
        }
        public override void AI()
        {
            if (npc.ai[1]++ % 360f < 280f)
            {
                if (npc.ai[2] > 1f)
                {
                    npc.ai[2]--;
                }
                npc.aiAction = 0;
                if (npc.ai[2] == 0f)
                {
                    npc.ai[0] = -90f;
                    npc.ai[2] = 1f;
                    npc.TargetClosest(true);
                }
                npc.TargetClosest(false);
                Player player = Main.player[npc.target];
                if (npc.velocity.Y == 0f && !Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
                {
                    if (npc.collideY && npc.oldVelocity.Y != 0f && Collision.SolidCollision(npc.position, npc.width, npc.height))
                    {
                        npc.position.X -= npc.velocity.X + npc.direction;
                    }
                    if (npc.ai[3] == npc.position.X)
                    {
                        npc.direction *= -1;
                        npc.ai[2] = 200f;
                    }
                    npc.spriteDirection = npc.direction;
                    npc.ai[3] = 0f;
                    npc.velocity.X *= 0.8f;
                    if (Math.Abs(npc.velocity.X) < 0.1f)
                    {
                        npc.velocity.X = 0f;
                    }

                    npc.ai[0] += 5f;

                    int state = 0;
                    if (npc.ai[0] >= 0f)
                    {
                        state = 1;
                    }
                    if (npc.ai[0] >= -1000f && npc.ai[0] <= -500f)
                    {
                        state = 2;
                    }
                    if (npc.ai[0] >= -2000f && npc.ai[0] <= -1500f)
                    {
                        state = 3;
                    }
                    if (state > 0)
                    {
                        npc.netUpdate = true;
                        if (state == 3)
                        {
                            npc.velocity.Y -= 9f;
                            npc.velocity.X += 8f * npc.direction;
                            npc.ai[0] = -120f;
                            npc.ai[3] = npc.position.X;
                        }
                        else
                        {
                            npc.velocity.Y -= 8f;
                            npc.velocity.X += 11f * npc.direction;
                            npc.ai[0] = -80f;
                            if (state == 1)
                            {
                                npc.ai[0] -= 1000f;
                            }
                            else
                            {
                                npc.ai[0] -= 2000f;
                            }
                        }
                    }
                    else if (npc.ai[0] >= -30f)
                    {
                        npc.aiAction = 1;
                        return;
                    }
                }
                else if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
                {
                    npc.direction = npc.spriteDirection = (npc.SafeDirectionTo(player.Center).X < 0).ToDirectionInt();
                    if (Math.Abs(npc.velocity.X) < 14f && Math.Abs(player.Center.X - npc.Center.X) > 65f)
                        npc.velocity.X += npc.spriteDirection * -0.08f;
                }
            }
            else
            {
                npc.velocity.X *= 0.9f;
                if (npc.ai[1] % 360f == 300f)
                {
                    npc.velocity.X = 0f;
                    Vector2 rockSpawnPosition = new Vector2(16f * -npc.spriteDirection + npc.Center.X, npc.Bottom.Y - 6f);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        boulderIndex = Projectile.NewProjectile(rockSpawnPosition, Vector2.Zero, ModContent.ProjectileType<CrabBoulder>(), 29, 6f);
                        npc.netUpdate = true;
                    }
                }
                if (npc.ai[1] % 360f == 330f)
                {
                    Main.projectile[boulderIndex].velocity = new Vector2(0f, -11f).RotatedBy(-npc.spriteDirection * 0.8f);
                    boulderIndex = -1;
                    npc.netUpdate = true;
                }
            }
            npc.velocity.Y += 0.25f;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.ai[1] % 360f < 280f)
            {
                if (npc.frameCounter % 6 == 5)
                {
                    npc.frame.Y += frameHeight;
                    if (npc.frame.Y >= 4 * frameHeight)
                    {
                        npc.frame.Y = frameHeight;
                    }
                }
            }
            else
            {
                npc.frame.Y = 3 * frameHeight + frameHeight * (int)MathHelper.Clamp(npc.ai[1] % 280f / 60f * 9, 0, 9);
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !spawnInfo.player.Calamity().ZoneSulphur || !CalamityWorld.downedAquaticScourge)
            {
                return 0f;
            }
            return 0.135f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<CorrodedFossil>(), 15); // Rarer to encourage fighting Acid Rain
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/AnthozoanCrabGore"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/AnthozoanCrabGore2"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/AnthozoanCrabGore3"), npc.scale);
            }
        }
    }
}
