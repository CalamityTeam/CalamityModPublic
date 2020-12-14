using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SulphurousSea
{
	public class Mauler : ModNPC
    {
        private bool hasBeenHit = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mauler");
            Main.npcFrameCount[npc.type] = 8;
        }

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.noGravity = true;
            npc.damage = 135;
            npc.width = 180;
            npc.height = 90;
            npc.defense = 50;
			npc.DR_NERD(0.05f);
            npc.lifeMax = 165000;
            npc.aiStyle = -1;
            aiType = -1;
            npc.value = Item.buyPrice(0, 25, 0, 0);
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath60;
            npc.knockBackResist = 0f;
            npc.rarity = 2;
            banner = npc.type;
            bannerItem = ModContent.ItemType<MaulerBanner>();
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
            if (npc.wet)
            {
                bool flag14 = hasBeenHit;
                npc.TargetClosest(false);
                if (Main.player[npc.target].wet && !Main.player[npc.target].dead &&
                    Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) &&
                    (Main.player[npc.target].Center - npc.Center).Length() < 300f)
                {
                    flag14 = true;
                }
                if (Main.player[npc.target].dead && flag14)
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
                    if (!Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    {
                        npc.localAI[1] += 1f;
                    }
                    else
                    {
                        if (npc.localAI[1] > 0f)
                        {
                            npc.localAI[1] -= 1f;
                        }
                    }
                    npc.localAI[2] += 1f;
                    npc.TargetClosest(true);
                    npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.5f;
                    npc.velocity.Y = npc.velocity.Y + (float)npc.directionY * 0.25f;
                    if (npc.velocity.X > 22f)
                    {
                        npc.velocity.X = 22f;
                    }
                    if (npc.velocity.X < -22f)
                    {
                        npc.velocity.X = -22f;
                    }
                    if (npc.velocity.Y > 13f)
                    {
                        npc.velocity.Y = 13f;
                    }
                    if (npc.velocity.Y < -13f)
                    {
                        npc.velocity.Y = -13f;
                    }
                }
                else
                {
                    npc.velocity.X += (float)npc.direction * 0.2f;
                    if (npc.velocity.X < -4f || npc.velocity.X > 4f)
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
                if (Main.tile[num258, num259 - 1].liquid > 128)
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
                npc.localAI[1] += 1f;
                npc.localAI[0] += 1f;
                if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] >= 30f)
                {
                    npc.localAI[0] = 0f;
                    if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    {
                        float speed = 12f;
                        Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)(npc.height / 2));
                        float num6 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector.X + (float)Main.rand.Next(-20, 21);
                        float num7 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector.Y + (float)Main.rand.Next(-20, 21);
                        float num8 = (float)Math.Sqrt((double)(num6 * num6 + num7 * num7));
                        num8 = speed / num8;
                        num6 *= num8;
                        num7 *= num8;
                        int damage = 50;
                        if (Main.expertMode)
                        {
                            damage = 45;
                        }
                        int beam = Projectile.NewProjectile(npc.Center.X + (npc.spriteDirection == 1 ? 60f : -60f), npc.Center.Y, num6, num7, ModContent.ProjectileType<SulphuricAcidMist>(), damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
                if (npc.velocity.Y == 0f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.velocity.Y = (float)Main.rand.Next(-200, -150) * 0.1f; //50 20
                        npc.velocity.X = (float)Main.rand.Next(-20, 20) * 0.1f; //20 20
                        npc.netUpdate = true;
                    }
                }
                npc.velocity.Y = npc.velocity.Y + 0.4f; //0.4
                if (npc.velocity.Y > 16f)
                {
                    npc.velocity.Y = 16f;
                }
                npc.ai[0] = 1f;
            }
            if (hasBeenHit)
            {
                if (Main.rand.NextBool(300))
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/MaulerRoar"), (int)npc.position.X, (int)npc.position.Y);
                }
            }
            if (npc.localAI[1] >= 255f || npc.localAI[2] >= 600f)
            {
                npc.localAI[2] = 0f;
                BlowUp();
                return;
            }
            else if (npc.localAI[1] > 0f)
            {
                for (int k = 0; k < (int)((double)npc.localAI[1] * 0.05); k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 5, 0f, 0f, 0, default, 1f);
                }
            }
            npc.rotation = npc.velocity.Y * (float)npc.direction * 0.1f;
            if (npc.rotation < -0.2f)
            {
                npc.rotation = -0.2f;
            }
            if (npc.rotation > 0.2f)
            {
                npc.rotation = 0.2f;
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

        public void BlowUp()
        {
            Main.PlaySound(SoundID.NPCDeath60, npc.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 valueBoom = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float spreadBoom = 15f * 0.0174f;
                double startAngleBoom = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spreadBoom / 2;
                double deltaAngleBoom = spreadBoom / 8f;
                double offsetAngleBoom;
                int iBoom;
                int damageBoom = npc.localAI[1] >= 255f ? 200 : 50;
                for (iBoom = 0; iBoom < 25; iBoom++)
                {
                    int projectileType = Main.rand.NextBool(2) ? ModContent.ProjectileType<SulphuricAcidMist>() : ModContent.ProjectileType<SulphuricAcidBubble>();
                    offsetAngleBoom = startAngleBoom + deltaAngleBoom * (iBoom + iBoom * iBoom) / 2f + 32f * iBoom;
                    int boom1 = Projectile.NewProjectile(valueBoom.X, valueBoom.Y, (float)(Math.Sin(offsetAngleBoom) * 6f), (float)(Math.Cos(offsetAngleBoom) * 6f), projectileType, damageBoom, 0f, Main.myPlayer, 0f, 0f);
                    int boom2 = Projectile.NewProjectile(valueBoom.X, valueBoom.Y, (float)(-Math.Sin(offsetAngleBoom) * 6f), (float)(-Math.Cos(offsetAngleBoom) * 6f), projectileType, damageBoom, 0f, Main.myPlayer, 0f, 0f);
                }
            }
            for (int num621 = 0; num621 < 25; num621++)
            {
                int num622 = Dust.NewDust(npc.position, npc.width, npc.height, 31, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
                Main.dust[num622].noGravity = true;
            }
            for (int num623 = 0; num623 < 50; num623++)
            {
                int num624 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
                Main.dust[num624].noGravity = true;
            }
            if (npc.localAI[1] >= 255f)
            {
                npc.active = false;
            }
            npc.netUpdate = true;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += hasBeenHit ? 0.15f : 0.075f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Bleeding, 300, true);
            player.AddBuff(BuffID.Venom, 300, true);
            player.AddBuff(BuffID.Rabies, 300, true);
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemCondition(npc, ModContent.ItemType<SulphuricAcidCannon>(), CalamityWorld.downedPolterghast, 3, 1, 1);
            int item = Item.NewItem(npc.Center, npc.Size, ItemID.SharkFin, Main.rand.Next(2, 5), false, 0, false, false);
            Main.item[item].color = new Color(151, 115, 57, 255);
            NetMessage.SendData(MessageID.ItemTweaker, -1, -1, null, item, 1f, 0f, 0f, 0, 0, 0);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe)
            {
                return 0f;
            }
            if (spawnInfo.player.Calamity().ZoneSulphur && spawnInfo.water && !NPC.AnyNPCs(ModContent.NPCType<Mauler>()) &&
                !NPC.AnyNPCs(ModContent.NPCType<AquaticScourgeHead>()))
            {
                if (!Main.hardMode)
                {
                    return 0.001f;
                }
                if (!NPC.downedMoonlord)
                {
                    return 0.01f;
                }
                return 0.1f;
            }
            return 0f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 30; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Mauler"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Mauler2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Mauler3"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Mauler4"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Mauler5"), 1f);
            }
        }
    }
}
