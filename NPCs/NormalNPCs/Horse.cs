using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Tools.ClimateChange;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class Horse : ModNPC
    {
        private int chargetimer = 0;
        private int basespeed = 1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Earth Elemental");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.npcSlots = 3f;
            npc.damage = 50;
            npc.width = 230;
            npc.height = 230;
            npc.defense = 20;
			npc.DR_NERD(0.1f);
            npc.lifeMax = 3800;
            npc.aiStyle = -1;
            aiType = -1;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[ModContent.BuffType<MarkedforDeath>()] = false;
			npc.buffImmune[BuffID.Frostburn] = false;
			npc.buffImmune[BuffID.CursedInferno] = false;
            npc.buffImmune[BuffID.Daybreak] = false;
			npc.buffImmune[BuffID.StardustMinionBleed] = false;
			npc.buffImmune[BuffID.DryadsWardDebuff] = false;
			npc.buffImmune[BuffID.Oiled] = false;
			npc.buffImmune[BuffID.BetsysCurse] = false;
			npc.buffImmune[ModContent.BuffType<AstralInfectionDebuff>()] = false;
			npc.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = false;
            npc.buffImmune[ModContent.BuffType<AbyssalFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<ArmorCrunch>()] = false;
            npc.buffImmune[ModContent.BuffType<DemonFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<HolyFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<Nightwither>()] = false;
            npc.buffImmune[ModContent.BuffType<Plague>()] = false;
            npc.buffImmune[ModContent.BuffType<Shred>()] = false;
            npc.buffImmune[ModContent.BuffType<WarCleave>()] = false;
            npc.buffImmune[ModContent.BuffType<WhisperingDeath>()] = false;
            npc.buffImmune[ModContent.BuffType<SilvaStun>()] = false;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 1, 50, 0);
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.rarity = 2;
            banner = npc.type;
            bannerItem = ModContent.ItemType<EarthElementalBanner>();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(chargetimer);
            writer.Write(basespeed);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            chargetimer = reader.ReadInt32();
            basespeed = reader.ReadInt32();
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !Main.hardMode || spawnInfo.player.Calamity().ZoneAbyss ||
                spawnInfo.player.Calamity().ZoneSunkenSea || NPC.AnyNPCs(ModContent.NPCType<Horse>()))
            {
                return 0f;
            }
            return SpawnCondition.Cavern.Chance * 0.005f;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter >= 8)
            {
                npc.frame.Y = (npc.frame.Y + frameHeight) % (Main.npcFrameCount[npc.type] * frameHeight);
                npc.frameCounter = 0;
            }
        }

        public override void NPCLoot()
        {
			DropHelper.DropItemChance(npc, ModContent.ItemType<AridArtifact>(), 3);
			DropHelper.DropItemChance(npc, ModContent.ItemType<SlagMagnum>(), 4);
			DropHelper.DropItemChance(npc, ModContent.ItemType<Aftershock>(), 4);
			DropHelper.DropItemChance(npc, ModContent.ItemType<EarthenPike>(), 4);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 31, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Main.PlaySound(SoundID.Item14, npc.position);
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 160;
                npc.height = 160;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 31, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Fire, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Fire, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
				CalamityUtils.ExplosionGores(npc.Center, 3);
            }
        }

        public override bool PreAI()
        {
			npc.TargetClosest(true);

			if (Main.player[npc.target].dead || !Main.player[npc.target].active)
			{
				if (npc.velocity.Y < -2f)
					npc.velocity.Y = -2f;
				npc.velocity.Y += 0.1f;
				if (npc.velocity.Y > 12f)
					npc.velocity.Y = 12f;

				if (npc.timeLeft > 60)
					npc.timeLeft = 60;
			}

			if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.localAI[0] += 1f;
                if (npc.localAI[0] >= 300f)
                {
                    npc.localAI[0] = 0f;
                    Main.PlaySound(SoundID.NPCHit43, npc.Center);
                    npc.TargetClosest(true);
                    if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    {
                        float num179 = 4f;
                        Vector2 value9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                        float num180 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - value9.X;
                        float num181 = Math.Abs(num180) * 0.1f;
                        float num182 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - value9.Y - num181;
                        float num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
                        npc.netUpdate = true;
                        num183 = num179 / num183;
                        num180 *= num183;
                        num182 *= num183;
                        int num184 = 30;
                        int num185 = ModContent.ProjectileType<EarthRockSmall>();
                        value9.X += num180;
                        value9.Y += num182;
                        for (int num186 = 0; num186 < 4; num186++)
                        {
                            num185 = Main.rand.NextBool(4) ? ModContent.ProjectileType<EarthRockBig>() : ModContent.ProjectileType<EarthRockSmall>();
                            num180 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - value9.X;
                            num182 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - value9.Y;
                            num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
                            num183 = num179 / num183;
                            num180 += (float)Main.rand.Next(-40, 41);
                            num182 += (float)Main.rand.Next(-40, 41);
                            num180 *= num183;
                            num182 *= num183;
                            Projectile.NewProjectile(value9.X, value9.Y, num180, num182, num185, num184, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                }
            }

            if (Math.Abs(npc.velocity.X) > 0.2)
                npc.spriteDirection = npc.direction;

            Vector2 direction = Main.player[npc.target].Center - npc.Center;
            direction.Normalize();
            chargetimer += Main.expertMode ? 2 : 1;
			if (chargetimer >= 600)
			{
				direction *= 6f;
				npc.velocity = direction;
				chargetimer = 0;
			}

            if (Math.Sqrt((npc.velocity.X * npc.velocity.X) + (npc.velocity.Y * npc.velocity.Y)) > basespeed)
                npc.velocity *= 0.985f;

            if (Math.Sqrt((npc.velocity.X * npc.velocity.X) + (npc.velocity.Y * npc.velocity.Y)) <= basespeed * 1.15)
                npc.velocity = direction * basespeed;

            return false;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<ArmorCrunch>(), 180);
        }
    }
}
