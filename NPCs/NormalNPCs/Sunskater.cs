using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class Sunskater : ModNPC
    {
        private bool hasBeenHit = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sunskater");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.noGravity = true;
            npc.lavaImmune = true;
            npc.damage = 20;
            npc.width = 58;
            npc.height = 22;
            npc.defense = 4;
            npc.lifeMax = 100;
            npc.aiStyle = -1;
            aiType = -1;
            npc.value = Item.buyPrice(0, 0, 5, 0);
            npc.HitSound = SoundID.NPCHit50;
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/Sunskater");
            npc.knockBackResist = 0.7f;
            banner = npc.type;
            bannerItem = ModContent.ItemType<SunskaterBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToCold = true;
            npc.Calamity().VulnerableToSickness = true;
            npc.Calamity().VulnerableToWater = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(hasBeenHit);
            writer.Write(npc.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            hasBeenHit = reader.ReadBoolean();
            npc.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
            npc.spriteDirection = (npc.direction > 0) ? 1 : -1;
            npc.noGravity = true;
            if (npc.direction == 0)
            {
                npc.TargetClosest(true);
            }
            if (npc.justHit)
            {
                hasBeenHit = true;
            }
            npc.chaseable = hasBeenHit;
            if (!npc.wet)
            {
                bool flag14 = hasBeenHit;
                npc.TargetClosest(false);
                if ((Main.player[npc.target].wet || Main.player[npc.target].dead) && flag14)
                {
                    flag14 = false;
                }
                if (!flag14)
                {
                    if (npc.collideX)
                    {
                        npc.velocity.X = npc.velocity.X * -1f;
                        npc.direction *= -1;
                        npc.netUpdate = true;
                    }
                    if (npc.collideY)
                    {
                        npc.netUpdate = true;
                        if (npc.velocity.Y > 0f)
                        {
                            npc.velocity.Y = Math.Abs(npc.velocity.Y) * -1f;
                            npc.directionY = -1;
                            npc.ai[0] = -1f;
                        }
                        else if (npc.velocity.Y < 0f)
                        {
                            npc.velocity.Y = Math.Abs(npc.velocity.Y);
                            npc.directionY = 1;
                            npc.ai[0] = 1f;
                        }
                    }
                }
                if (flag14)
                {
                    npc.TargetClosest(true);
                    npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.15f;
                    npc.velocity.Y = npc.velocity.Y + (float)npc.directionY * 0.15f;
                    npc.velocity.X = MathHelper.Clamp(npc.velocity.X, -10f, 10f);
                    npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y, -10f, 10f);
                }
                else
                {
                    npc.velocity.X += (float)npc.direction * 0.1f;
                    if (npc.velocity.X < -2f || npc.velocity.X > 2f)
                    {
                        npc.velocity.X *= 0.95f;
                    }
                    if (npc.ai[0] == -1f)
                    {
                        npc.velocity.Y -= 0.01f;
                        if (npc.velocity.Y < -0.3f)
                        {
                            npc.ai[0] = 1f;
                        }
                    }
                    else
                    {
                        npc.velocity.Y += 0.01f;
                        if (npc.velocity.Y > 0.3f)
                        {
                            npc.ai[0] = -1f;
                        }
                    }
                }
                int num258 = (int)(npc.position.X + (float)(npc.width / 2)) / 16;
                int num259 = (int)(npc.position.Y + (float)(npc.height / 2)) / 16;
                if (Main.tile[num258, num259 - 1] == null)
                {
                    Main.tile[num258, num259 - 1] = new Tile();
                }
                if (Main.tile[num258, num259 + 1] == null)
                {
                    Main.tile[num258, num259 + 1] = new Tile();
                }
                if (Main.tile[num258, num259 + 2] == null)
                {
                    Main.tile[num258, num259 + 2] = new Tile();
                }
                if (Main.tile[num258, num259 - 1].liquid < 128) //problem?
                {
                    if (Main.tile[num258, num259 + 1].active())
                    {
                        npc.ai[0] = -1f;
                    }
                    else if (Main.tile[num258, num259 + 2].active())
                    {
                        npc.ai[0] = -1f;
                    }
                }
                if (npc.velocity.Y > 0.4f || npc.velocity.Y < -0.4f)
                {
                    npc.velocity.Y *= 0.95f;
                }
            }
            else
            {
                if (npc.velocity.Y == 0f)
                {
                    npc.velocity.X *= 0.94f;
                    if (npc.velocity.X > -0.2f && npc.velocity.X < 0.2f)
                    {
                        npc.velocity.X = 0f;
                    }
                }
                npc.velocity.Y = npc.velocity.Y + 0.3f;
                if (npc.velocity.Y > 5f)
                {
                    npc.velocity.Y = 5f;
                }
                npc.ai[0] = 1f;
            }
            npc.rotation = npc.velocity.Y * (float)npc.direction * 0.1f;
            if ((double)npc.rotation < -0.1)
            {
                npc.rotation = -0.1f;
            }
            if ((double)npc.rotation > 0.1)
            {
                npc.rotation = 0.1f;
                return;
            }
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion && !projectile.Calamity().overridesMinionDamagePrevention)
            {
                return hasBeenHit;
            }
            return null;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += hasBeenHit ? 0.15f : 0.075f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe)
            {
                return 0f;
            }
            return SpawnCondition.Sky.Chance * 0.15f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<HolyFlames>(), 120, true);
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemCondition(npc, ModContent.ItemType<EssenceofCinder>(), Main.hardMode, 3, 1, 1);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 64, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 25; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 64, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
