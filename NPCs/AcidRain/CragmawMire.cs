using CalamityMod.Dusts;
using CalamityMod.Items.Accessories;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.Projectiles.Environment;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Rogue;

namespace CalamityMod.NPCs.AcidRain
{
    public class CragmawMire : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cragmaw Mire");
            Main.npcFrameCount[npc.type] = 2;
        }

        public override void SetDefaults()
        {
            npc.width = 68;
            npc.height = 54;
            npc.aiStyle = aiType = -1;

            npc.damage = 66;
            npc.lifeMax = 4000;
            npc.defense = 25;

            if (CalamityWorld.downedPolterghast)
            {
                npc.damage = 160;
                npc.lifeMax = 146600;
                npc.defense = 80;
            }

            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 3, 60, 0);
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.lavaImmune = false;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
        }
        public bool Phase2
        {
            get
            {
                float ratio = CalamityWorld.revenge ? 0.85f : 0.7f;
                return npc.life / (float)npc.lifeMax < ratio && CalamityWorld.downedPolterghast;
            }
        }
        public override void AI()
        {
            npc.TargetClosest(false);
            Player player = Main.player[npc.target];

            npc.ai[0]++;

            // Summons a spinning spiral of dust and a dust telegraph
            if (npc.ai[0] % 420f > 240f && npc.ai[0] % 420f < 300f)
            {
                float vectorRotationAngle = (npc.ai[0] % 420f - 240f) / 60f * MathHelper.Pi * 4f;
                for (int i = 0; i < 25; i++)
                {
                    float angle = MathHelper.TwoPi * i / 25f;
                    float y = (float)Math.Sin(angle) * (float)Math.Log(Math.Abs(Math.Cos(angle)));
                    if (!float.IsNaN(y))
                    {
                        Vector2 velocity = new Vector2((float)Math.Cos(angle), y) * MathHelper.Lerp(2.4f, 4.2f, (npc.ai[0] - 240f) / 60f);
                        Dust dust = Dust.NewDustPerfect(npc.Center, (int)CalamityDusts.SulfurousSeaAcid);
                        dust.velocity = velocity.RotatedBy(vectorRotationAngle);
                        dust.noGravity = true;
                        dust.scale = MathHelper.Lerp(1.2f, 2.5f, (npc.ai[0] - 240f) / 60f);
                    }
                }

                float length = MathHelper.Lerp(20f, 1200f, (npc.ai[0] % 420f - 240f) / 60f);
                float outwardness = 1f - (npc.ai[0] % 420f - 240f) / 60f;
                for (float i = npc.Top.Y + 4f; i >= npc.Top.Y + 4f - length; i -= 6f)
                {
                    float angle = i / 32f;
                    vectorRotationAngle = -(i / 20f) % 0.4f - 0.2f;
                    Vector2 spawnPosition = new Vector2(npc.Center.X, i);
                    Dust dust = Dust.NewDustPerfect(spawnPosition, (int)CalamityDusts.SulfurousSeaAcid);
                    dust.scale = 1.5f;
                    dust.velocity = (Vector2.UnitX * (float)Math.Cos(angle) * 4f * outwardness).RotatedBy(vectorRotationAngle);
                    dust.noGravity = true;
                }
            }
            // Release a laser beam and create an explosion
            if (npc.ai[0] % 420f == 300f)
            {
                npc.ai[1] = Main.rand.NextFloat(3f);
                npc.ai[2] = Main.rand.NextFloat(4f);
                npc.netUpdate = true;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Main.PlaySound(SoundID.Zombie, (int)npc.position.X, (int)npc.position.Y, 104); // Moon lord beam sound
                    int damage = CalamityWorld.downedPolterghast ? 120 : 40;
                    Projectile.NewProjectile(npc.Center, -Vector2.UnitY, ModContent.ProjectileType<CragmawBeam>(), damage, 4f, Main.myPlayer, 0f, npc.whoAmI);
                }
                Vector2 circlePointVector = -Vector2.UnitY;
                float lerpStart = Main.rand.Next(12, 16);
                float lerpEnd = Main.rand.Next(3, 6);
                for (int h = 0; h < 4; h++)
                {
                    for (float i = 0; i < 9f; ++i)
                    {
                        for (int j = 0; j < 2; ++j)
                        {
                            Vector2 randomCirclePointRotated = circlePointVector.RotatedBy((j == 0 ? 1 : -1) * MathHelper.TwoPi / 18f).RotatedBy(h / 4f * MathHelper.Pi);
                            for (float k = 0f; k < 20f; ++k)
                            {
                                Vector2 randomCirclePointLerped = Vector2.Lerp(circlePointVector, randomCirclePointRotated, k / 20f);
                                float lerpMultiplier = MathHelper.Lerp(lerpStart, lerpEnd, k / 20f) * 0.9f;
                                int dustIndex = Dust.NewDust(npc.Top + 6f * Vector2.UnitY, 0, 0,
                                    (int)CalamityDusts.SulfurousSeaAcid,
                                    0f, 0f, 100, default, 1.3f);
                                Main.dust[dustIndex].noGravity = true;
                                Main.dust[dustIndex].scale = 2f;
                                Main.dust[dustIndex].velocity = randomCirclePointLerped * lerpMultiplier;
                            }
                        }

                        circlePointVector = circlePointVector.RotatedBy(MathHelper.TwoPi / 9f);
                    }
                }
            }
            else if (npc.ai[0] % 420f > 300f && npc.ai[0] % 7f == 6f && CalamityWorld.downedPolterghast)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(npc.Center, -Vector2.UnitY.RotatedByRandom(0.6f) * Main.rand.NextFloat(10f, 15f), ModContent.ProjectileType<AcidDrop>(), 69, 4f, Main.myPlayer, 0f, npc.whoAmI);
                }
            }

            if (!Phase2)
            {
                npc.Calamity().DR = 0.3f;
                if (!player.wet)
                    npc.Calamity().DR = 0.5f;
                npc.HitSound = SoundID.NPCHit42;
                if (npc.ai[0] % 420f < 240f)
                {
                    float bubbleShootTimer = CalamityWorld.downedPolterghast ? 24f : 50f;
                    if (npc.ai[0] % bubbleShootTimer == bubbleShootTimer - 1f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int damage = CalamityWorld.downedPolterghast ? 47 : 30;
                        int idx = Projectile.NewProjectile(npc.Top + new Vector2(0f, 5f), -Vector2.UnitY.RotatedByRandom(0.25f) * 4f, ModContent.ProjectileType<CragmawBubble>(), damage, 1f);
                        Main.projectile[idx].timeLeft = Main.rand.Next(120, 180);
                        Main.projectile[idx].netUpdate = true;
                    }
                    if (npc.ai[0] % 50f == 49f && Main.rand.NextBool(4))
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            List<int> possibleEnemies = new List<int>();
                            if (CalamityWorld.downedPolterghast)
                            {
                                possibleEnemies.Add(ModContent.NPCType<GammaSlime>());
                                possibleEnemies.Add(ModContent.NPCType<NuclearToad>());
                                possibleEnemies.Add(ModContent.NPCType<Orthocera>());
                            }
                            else
                            {
                                possibleEnemies.Add(ModContent.NPCType<WaterLeech>());
                                possibleEnemies.Add(ModContent.NPCType<Trilobite>());
                            }

                            Vector2 spawnPosition = npc.Center + Utils.RandomVector2(Main.rand, -360f, 360f);
                            int attempts = 0;
                            while (!WorldGen.InWorld((int)spawnPosition.X / 16, (int)spawnPosition.Y / 16))
                            {
                                spawnPosition = npc.Center + Utils.RandomVector2(Main.rand, -460f, 460f);
                                attempts++;
                                if (attempts > 200)
                                    return;
                            }
                            attempts = 0;

                            while (CalamityUtils.TileSelectionSolidSquare((int)spawnPosition.X / 16, (int)spawnPosition.Y / 16, 8, 8) ||
                                CalamityUtils.ParanoidTileRetrieval((int)spawnPosition.X / 16, (int)spawnPosition.Y / 16).liquid != 255)
                            {
                                spawnPosition = npc.Center + Utils.RandomVector2(Main.rand, -620f, 620f);
                                attempts++;
                                if (attempts > 300)
                                    return;
                            }

                            NPC.NewNPC((int)spawnPosition.X, (int)spawnPosition.Y, Utils.SelectRandom(Main.rand, possibleEnemies.ToArray()));
                        }
                    }
                }
            }
            else
            {
                if (npc.localAI[0] == 0f)
                {
                    Gore.NewGore(npc.position, -Vector2.UnitY.RotatedByRandom(0.4f) * 4f, mod.GetGoreSlot("Gores/AcidRain/CragmawMireP1Gore"), npc.scale);
                    Gore.NewGore(npc.position, -Vector2.UnitY.RotatedByRandom(0.4f) * 4f, mod.GetGoreSlot("Gores/AcidRain/CragmawMireP1Gore2"), npc.scale);
                    Gore.NewGore(npc.position, -Vector2.UnitY.RotatedByRandom(0.4f) * 4f, mod.GetGoreSlot("Gores/AcidRain/CragmawMireP1Gore3"), npc.scale);
                    npc.localAI[0] = 1f;
                }
                npc.Calamity().DR = CalamityWorld.revenge ? 0.125f : 0f;
                npc.HitSound = SoundID.NPCHit1;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (npc.ai[0] % 600f == 20f)
                    {
                        Projectile.NewProjectile(npc.Center, Vector2.UnitY, ModContent.ProjectileType<CragmawVibeCheckChain>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.target);
                    }
                    if (npc.ai[0] % 600f == 240f)
                    {
                        int damage = CalamityWorld.downedPolterghast ? 52 : 33;
                        for (int i = 0; i < 16; i++)
                        {
                            float angle = MathHelper.TwoPi / 16f * i;
                            float angleDelta = Main.rand.NextFloat(MathHelper.TwoPi / 16f) * 0.6f;
                            angle += angleDelta - angleDelta / 2f;
                            Projectile.NewProjectile(npc.Center, angle.ToRotationVector2() * 6f, ModContent.ProjectileType<CragmawSpike>(), damage, 4f);
                        }
                    }
                }
            }
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.85f);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = Phase2 ? ModContent.GetTexture("CalamityMod/NPCs/AcidRain/CragmawMire2") : ModContent.GetTexture("CalamityMod/NPCs/AcidRain/CragmawMire");
            CalamityMod.DrawTexture(spriteBatch, texture, 0, npc, drawColor, true);
            return false;
        }
        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter % 6 == 5)
            {
                npc.frame.Y += frameHeight;
            }
            if (npc.frame.Y >= frameHeight * 2)
            {
                npc.frame.Y = 0;
            }
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, -Vector2.UnitY.RotatedByRandom(0.4f) * 4f, mod.GetGoreSlot("Gores/AcidRain/CragmawMireP2Gore"), npc.scale);
                Gore.NewGore(npc.position, -Vector2.UnitY.RotatedByRandom(0.4f) * 4f, mod.GetGoreSlot("Gores/AcidRain/CragmawMireP2Gore2"), npc.scale);
                Gore.NewGore(npc.position, -Vector2.UnitY.RotatedByRandom(0.4f) * 4f, mod.GetGoreSlot("Gores/AcidRain/CragmawMireP2Gore3"), npc.scale);
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 300);
        }
        public override void NPCLoot()
        {
			DropHelper.DropItemChance(npc, ModContent.ItemType<NuclearRod>(), CalamityWorld.downedPolterghast ? 0.1f : 1f);
            DropHelper.DropItemChance(npc, ModContent.ItemType<SpentFuelContainer>(), CalamityWorld.downedPolterghast ? 0.1f : 1f);
        }
    }
}
