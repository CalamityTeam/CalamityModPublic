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
            Main.npcFrameCount[NPC.type] = 16;
        }

        public override void SetDefaults()
        {
            NPC.noGravity = true;
            NPC.damage = 45;
            NPC.width = 56;
            NPC.height = 42;
            NPC.defense = 22;
            NPC.lifeMax = 920;
            NPC.aiStyle = AIType = -1;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit38;
            NPC.DeathSound = SoundID.NPCDeath46;
            NPC.knockBackResist = 0.04f;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<AnthozoanCrabBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
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
            if (NPC.ai[1]++ % 360f < 280f)
            {
                if (NPC.ai[2] > 1f)
                {
                    NPC.ai[2]--;
                }
                NPC.aiAction = 0;
                if (NPC.ai[2] == 0f)
                {
                    NPC.ai[0] = -90f;
                    NPC.ai[2] = 1f;
                    NPC.TargetClosest(true);
                }
                NPC.TargetClosest(false);
                Player player = Main.player[NPC.target];
                if (NPC.velocity.Y == 0f && !Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                {
                    if (NPC.collideY && NPC.oldVelocity.Y != 0f && Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                    {
                        NPC.position.X -= NPC.velocity.X + NPC.direction;
                    }
                    if (NPC.ai[3] == NPC.position.X)
                    {
                        NPC.direction *= -1;
                        NPC.ai[2] = 200f;
                    }
                    NPC.spriteDirection = NPC.direction;
                    NPC.ai[3] = 0f;
                    NPC.velocity.X *= 0.8f;
                    if (Math.Abs(NPC.velocity.X) < 0.1f)
                    {
                        NPC.velocity.X = 0f;
                    }

                    NPC.ai[0] += 5f;

                    int state = 0;
                    if (NPC.ai[0] >= 0f)
                    {
                        state = 1;
                    }
                    if (NPC.ai[0] >= -1000f && NPC.ai[0] <= -500f)
                    {
                        state = 2;
                    }
                    if (NPC.ai[0] >= -2000f && NPC.ai[0] <= -1500f)
                    {
                        state = 3;
                    }
                    if (state > 0)
                    {
                        NPC.netUpdate = true;
                        if (state == 3)
                        {
                            NPC.velocity.Y -= 9f;
                            NPC.velocity.X += 8f * NPC.direction;
                            NPC.ai[0] = -120f;
                            NPC.ai[3] = NPC.position.X;
                        }
                        else
                        {
                            NPC.velocity.Y -= 8f;
                            NPC.velocity.X += 11f * NPC.direction;
                            NPC.ai[0] = -80f;
                            if (state == 1)
                            {
                                NPC.ai[0] -= 1000f;
                            }
                            else
                            {
                                NPC.ai[0] -= 2000f;
                            }
                        }
                    }
                    else if (NPC.ai[0] >= -30f)
                    {
                        NPC.aiAction = 1;
                        return;
                    }
                }
                else if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                {
                    NPC.direction = NPC.spriteDirection = (NPC.SafeDirectionTo(player.Center).X < 0).ToDirectionInt();
                    if (Math.Abs(NPC.velocity.X) < 14f && Math.Abs(player.Center.X - NPC.Center.X) > 65f)
                        NPC.velocity.X += NPC.spriteDirection * -0.08f;
                }
            }
            else
            {
                NPC.velocity.X *= 0.9f;
                if (NPC.ai[1] % 360f == 300f)
                {
                    NPC.velocity.X = 0f;
                    Vector2 rockSpawnPosition = new Vector2(16f * -NPC.spriteDirection + NPC.Center.X, NPC.Bottom.Y - 6f);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        boulderIndex = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), rockSpawnPosition, Vector2.Zero, ModContent.ProjectileType<CrabBoulder>(), 29, 6f);
                        NPC.netUpdate = true;
                    }
                }
                if (NPC.ai[1] % 360f == 330f)
                {
                    Main.projectile[boulderIndex].velocity = new Vector2(0f, -11f).RotatedBy(-NPC.spriteDirection * 0.8f);
                    boulderIndex = -1;
                    NPC.netUpdate = true;
                }
            }
            NPC.velocity.Y += 0.25f;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.ai[1] % 360f < 280f)
            {
                if (NPC.frameCounter % 6 == 5)
                {
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y >= 4 * frameHeight)
                    {
                        NPC.frame.Y = frameHeight;
                    }
                }
            }
            else
            {
                NPC.frame.Y = 3 * frameHeight + frameHeight * (int)MathHelper.Clamp(NPC.ai[1] % 280f / 60f * 9, 0, 9);
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !spawnInfo.Player.Calamity().ZoneSulphur || !CalamityWorld.downedAquaticScourge)
            {
                return 0f;
            }
            return 0.135f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(NPC, ModContent.ItemType<CorrodedFossil>(), 15); // Rarer to encourage fighting Acid Rain
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/AcidRain/AnthozoanCrabGore"), NPC.scale);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/AcidRain/AnthozoanCrabGore2"), NPC.scale);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/AcidRain/AnthozoanCrabGore3"), NPC.scale);
            }
        }
    }
}
