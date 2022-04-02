using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Astral
{
    public class Nova : ModNPC
    {
        private static Texture2D glowmask;

        private float travelAcceleration = 0.2f;
        private float targetTime = 120f;
        private const float waitBeforeTravel = 20f;
        private const float maxTravelTime = 300f;
        private const float slowdown = 0.84f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nova");
            Main.npcFrameCount[npc.type] = 8;

            if (!Main.dedServ)
                glowmask = ModContent.GetTexture("CalamityMod/NPCs/Astral/NovaGlow");
        }

        public override void SetDefaults()
        {
            npc.width = 78;
            npc.height = 50;
            npc.damage = 45;
            npc.defense = 15;
            npc.DR_NERD(0.15f);
            npc.lifeMax = 230;
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/AstralEnemyDeath");
            npc.noGravity = true;
            npc.knockBackResist = 0.5f;
            npc.value = Item.buyPrice(0, 0, 20, 0);
            npc.aiStyle = -1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<NovaBanner>();
            if (CalamityWorld.downedAstrageldon)
            {
                npc.damage = 75;
                npc.defense = 25;
                npc.knockBackResist = 0.4f;
                npc.lifeMax = 350;
            }
            if (CalamityWorld.death)
            {
                travelAcceleration = 0.3f;
                targetTime = 60f;
            }
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToSickness = false;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.ai[3] >= 0f)
            {
                if (npc.frameCounter >= 8)
                {
                    npc.frameCounter = 0;
                    npc.frame.Y += frameHeight;
                    if (npc.frame.Y >= frameHeight * 4)
                    {
                        npc.frame.Y = 0;
                    }
                }
            }
            else
            {
                if (npc.frameCounter >= 7)
                {
                    npc.frameCounter = 0;
                    npc.frame.Y += frameHeight;
                    if (npc.frame.Y >= frameHeight * 8)
                    {
                        npc.frame.Y = frameHeight * 4;
                    }
                }
            }

            //DO DUST
            Dust d = CalamityGlobalNPC.SpawnDustOnNPC(npc, 114, frameHeight, ModContent.DustType<AstralOrange>(), new Rectangle(78, 34, 36, 18), Vector2.Zero, 0.45f, true);
            if (d != null)
            {
                d.customData = 0.04f;
            }
        }

        public override void AI()
        {
            Player target = Main.player[npc.target];
            if (npc.ai[3] >= 0)
            {
                CalamityGlobalNPC.DoFlyingAI(npc, (CalamityWorld.death ? 8.25f : 5.5f), (CalamityWorld.death ? 0.0525f : 0.035f), 400f, 150, false);

                if (Collision.CanHit(npc.position, npc.width, npc.height, target.position, target.width, target.height))
                {
                    npc.ai[3]++;
                }
                else
                {
                    npc.ai[3] = 0f;
                }

                Vector2 between = target.Center - npc.Center;

                //after locking target for x amount of time and being far enough away
                int random = CalamityWorld.death ? 90 : 180;
                if (between.Length() > 150 && npc.ai[3] >= targetTime && Main.rand.NextBool(random))
                {
                    //set ai mode to target and travel
                    npc.ai[3] = -1f;
                }
                return;
            }
            else
            {
                npc.ai[3]--;
                Vector2 between = target.Center - npc.Center;

                if (npc.ai[3] < -waitBeforeTravel)
                {
                    if (npc.collideX || npc.collideY || npc.ai[3] < -maxTravelTime)
                    {
                        Explode();
                    }

                    npc.velocity += new Vector2(npc.ai[1], npc.ai[2]) * travelAcceleration; //acceleration per frame

                    //rotation
                    npc.rotation = npc.velocity.ToRotation();
                }
                else if (npc.ai[3] == -waitBeforeTravel)
                {
                    between.Normalize();
                    npc.ai[1] = between.X;
                    npc.ai[2] = between.Y;

                    //rotation
                    npc.rotation = between.ToRotation();
                    npc.velocity = Vector2.Zero;
                }
                else
                {
                    //slowdown
                    npc.velocity *= slowdown;

                    //rotation
                    npc.rotation = between.ToRotation();
                }
                npc.rotation += MathHelper.Pi;
            }
        }

        private void Explode()
        {
            //kill NPC
            Main.PlaySound(SoundID.Item14, npc.Center);

            //change stuffs
            Vector2 center = npc.Center;
            npc.width = 200;
            npc.height = 200;
            npc.Center = center;

            Rectangle myRect = npc.getRect();

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 200; i++)
                {
                    if (Main.player[i].getRect().Intersects(myRect))
                    {
                        int direction = npc.Center.X - Main.player[i].Center.X < 0 ? -1 : 1;
                        Main.player[i].Hurt(PlayerDeathReason.ByNPC(npc.whoAmI), npc.damage, direction);
                    }
                }
            }

            //other things
            npc.ai[3] = -20000f;
            npc.value = 0f;
            npc.extraValue = 0f;
            npc.StrikeNPCNoInteraction(9999, 1f, 1);

            int size = 30;
            Vector2 off = new Vector2(size / -2f);

            for (int i = 0; i < 45; i++)
            {
                int dust = Dust.NewDust(npc.Center - off, size, size, ModContent.DustType<AstralEnemy>(), Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f), 0, default, Main.rand.NextFloat(1f, 2f));
                Main.dust[dust].velocity *= 1.4f;
            }
            for (int i = 0; i < 15; i++)
            {
                int dust = Dust.NewDust(npc.Center - off, size, size, 31, 0f, 0f, 100, default, 1.7f);
                Main.dust[dust].velocity *= 1.4f;
            }
            for (int i = 0; i < 27; i++)
            {
                int dust = Dust.NewDust(npc.Center - off, size, size, 6, 0f, 0f, 100, default, 2.4f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 5f;
                dust = Dust.NewDust(npc.Center - off, size, size, 6, 0f, 0f, 100, default, 1.6f);
                Main.dust[dust].velocity *= 3f;
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

            CalamityGlobalNPC.DoHitDust(npc, hitDirection, (Main.rand.Next(0, Math.Max(0, npc.life)) == 0) ? 5 : ModContent.DustType<AstralEnemy>(), 1f, 3, 40);

            //if dead do gores
            if (npc.life <= 0)
            {
                for (int i = 0; i < 7; i++)
                {
                    Gore.NewGore(npc.Center, npc.velocity * 0.3f, mod.GetGoreSlot("Gores/Nova/NovaGore" + i));
                }
            }
        }

        public override bool PreNPCLoot()
        {
            return npc.ai[3] > -10000;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            spriteBatch.Draw(glowmask, npc.Center - Main.screenPosition - new Vector2(0, 4f), npc.frame, Color.White * 0.75f, npc.rotation, new Vector2(57f, 37f), npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.player))
            {
                return 0f;
            }
            else if (spawnInfo.player.InAstral(1))
            {
                return 0.19f;
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
            DropHelper.DropItemCondition(npc, ModContent.ItemType<StellarCannon>(), CalamityWorld.downedAstrageldon, 7, 1, 1);
            DropHelper.DropItemChance(npc, ModContent.ItemType<GloriousEnd>(), 7, 1, 1);
        }
    }
}
