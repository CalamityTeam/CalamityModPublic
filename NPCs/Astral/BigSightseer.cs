using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Astral
{
    public class BigSightseer : ModNPC
    {
        private static Texture2D glowmask;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Big Sightseer");
            Main.npcFrameCount[npc.type] = 4;
            if (!Main.dedServ)
                glowmask = ModContent.GetTexture("CalamityMod/NPCs/Astral/BigSightseerGlow");
        }

        public override void SetDefaults()
        {
            npc.width = 64;
            npc.height = 56;
            npc.damage = 50;
            npc.defense = 20;
            npc.DR_NERD(0.15f);
            npc.lifeMax = 430;
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/AstralEnemyDeath");
            npc.noGravity = true;
            npc.knockBackResist = 0.8f;
            npc.value = Item.buyPrice(0, 0, 20, 0);
            npc.aiStyle = -1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<BigSightseerBanner>();
            if (CalamityWorld.downedAstrageldon)
            {
                npc.damage = 85;
                npc.defense = 30;
                npc.knockBackResist = 0.7f;
                npc.lifeMax = 640;
            }
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToSickness = false;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.05f + npc.velocity.Length() * 0.667f;
            if (npc.frameCounter >= 8)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y > npc.height * 3)
                {
                    npc.frame.Y = 0;
                }
            }

            //DO DUST
            Dust d = CalamityGlobalNPC.SpawnDustOnNPC(npc, 118, frameHeight, ModContent.DustType<AstralOrange>(), new Rectangle(70, 18, 48, 18), Vector2.Zero, 0.45f, true);
            if (d != null)
            {
                d.customData = 0.04f;
            }
        }

        public override void AI()
        {
            CalamityGlobalNPC.DoFlyingAI(npc, 4f, 0.025f, 300f);

            npc.ai[1]++;
            Player target = Main.player[npc.target];

            if (npc.justHit || target.dead)
            {
                //reset if hit
                npc.ai[1] = 0;
            }

            //if can see target and waited long enough
            if (Collision.CanHit(target.position, target.width, target.height, npc.position, npc.width, npc.height))
            {
                Vector2 vector = target.Center - npc.Center;
                vector.Normalize();
                Vector2 spawnPoint = npc.Center + vector * 42f;

                if (npc.ai[1] >= (CalamityWorld.death ? 120f : 160f))
                {
                    npc.ai[1] = 0f;

                    int n = NPC.NewNPC((int)spawnPoint.X, (int)spawnPoint.Y, ModContent.NPCType<AstralSeekerSpit>());
                    Main.npc[n].Center = spawnPoint;
                    Main.npc[n].velocity = vector * (CalamityWorld.death ? 12f : 10f);
                }
                else if (npc.ai[1] >= 140f) //oozin dust at the "mouth"
                {
                    int dustType = Main.rand.NextBool(2) ? ModContent.DustType<AstralOrange>() : ModContent.DustType<AstralBlue>();
                    int d = Dust.NewDust(spawnPoint - new Vector2(5), 10, 10, dustType);
                    Main.dust[d].velocity = npc.velocity * 0.3f;
                    Main.dust[d].customData = true;
                }
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.soundDelay == 0)
            {
                npc.soundDelay = 15;
                switch (Main.rand.Next(3))
                {
                    case 0:
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyHit"), npc.Center);
                        break;
                    case 1:
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyHit2"), npc.Center);
                        break;
                    case 2:
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyHit3"), npc.Center);
                        break;
                }
            }

            CalamityGlobalNPC.DoHitDust(npc, hitDirection, (Main.rand.Next(0, Math.Max(0, npc.life)) == 0) ? 5 : ModContent.DustType<AstralEnemy>(), 1f, 4, 22);

            //if dead do gores
            if (npc.life <= 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    float rand = Main.rand.NextFloat(-0.18f, 0.18f);
                    Gore.NewGore(npc.position + new Vector2(Main.rand.NextFloat(0f, npc.width), Main.rand.NextFloat(0f, npc.height)), npc.velocity * rand, mod.GetGoreSlot("Gores/BigSightseer/BigSightseerGore" + i));
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            spriteBatch.Draw(glowmask, npc.Center - Main.screenPosition + new Vector2(0, 4f), npc.frame, Color.White * 0.75f, npc.rotation, new Vector2(59f, 28f), npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.player))
            {
                return 0f;
            }
            else if (spawnInfo.player.InAstral(1))
            {
                return spawnInfo.player.ZoneDesert ? 0.14f : 0.17f;
            }
            return 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120, true);
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ModContent.ItemType<Stardust>(), 2, 3);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<Stardust>(), Main.expertMode);
        }
    }

    public class AstralSeekerSpit : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seeker Spit");
            Main.npcFrameCount[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.width = 16;
            npc.height = 16;
            npc.damage = 45;
            npc.defense = 0;
            npc.lifeMax = 1;
            npc.HitSound = null;
            npc.DeathSound = SoundID.NPCDeath9;
            npc.noGravity = true;
            npc.knockBackResist = 0f;
            npc.noTileCollide = true;
            npc.alpha = 80;
            npc.aiStyle = -1;
            if (CalamityWorld.downedAstrageldon)
            {
                npc.damage = 75;
            }
        }

        public override void AI()
        {
            //DUST
            npc.ai[0] += 0.18f;
            float angle = npc.velocity.ToRotation() + MathHelper.PiOver2;
            float pulse = (float)Math.Sin(npc.ai[0]);
            float radius = 5.8f;
            Vector2 offset = angle.ToRotationVector2() * pulse * radius;
            Dust pink = Dust.NewDustPerfect(npc.Center + offset, ModContent.DustType<AstralOrange>(), Vector2.Zero);
            Dust blue = Dust.NewDustPerfect(npc.Center - offset, ModContent.DustType<AstralBlue>(), Vector2.Zero);

            //kill on tile collide
            if (Collision.SolidCollision(npc.position, npc.width, npc.height))
            {
                npc.StrikeNPCNoInteraction(9999, 0, 0);
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (damage > 0)
            {
                npc.StrikeNPCNoInteraction(9999, 0, 0);
            }
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (damage > 0)
            {
                npc.StrikeNPCNoInteraction(9999, 0, 0);
            }
        }

        //On death
        public override bool PreNPCLoot()
        {
            DoKillDust();
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            return false;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120, true);
        }

        private void DoKillDust()
        {
            int numDust = Main.rand.Next(17, 25);
            float rotPerIter = MathHelper.TwoPi / numDust;
            float angle = 0;
            for (int i = 0; i < numDust; i++)
            {
                Vector2 vel = (angle + Main.rand.NextFloat(-0.04f, 0.04f)).ToRotationVector2();
                int dustType = Main.rand.NextBool(2) ? ModContent.DustType<AstralOrange>() : ModContent.DustType<AstralBlue>();
                Dust d = Dust.NewDustPerfect(npc.Center, dustType, vel * Main.rand.NextFloat(1.8f, 2.2f));
                d.customData = npc;

                angle += rotPerIter;
            }
        }
    }
}
