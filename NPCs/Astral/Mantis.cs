using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Astral
{
    public class Mantis : ModNPC
    {
        private static Texture2D glowmask;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mantis");
            Main.npcFrameCount[npc.type] = 14;
            if (!Main.dedServ)
                glowmask = ModContent.GetTexture("CalamityMod/NPCs/Astral/MantisGlow");
        }

        public override void SetDefaults()
        {
            npc.Calamity().canBreakPlayerDefense = true;
            npc.damage = 55;
            npc.width = 60;
            npc.height = 58;
            npc.aiStyle = -1;
            npc.defense = 6;
            npc.DR_NERD(0.15f);
            npc.lifeMax = 340;
            npc.knockBackResist = 0.2f;
            npc.value = Item.buyPrice(0, 0, 15, 0);
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/AstralEnemyDeath");
            banner = npc.type;
            bannerItem = ModContent.ItemType<MantisBanner>();
            if (CalamityWorld.downedAstrageldon)
            {
                npc.damage = 85;
                npc.defense = 16;
                npc.knockBackResist = 0.1f;
                npc.lifeMax = 510;
            }
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToSickness = false;
        }

        public override void AI()
        {
            npc.TargetClosest(false);

            Player target = Main.player[npc.target];

            if (npc.ai[0] == 0f)
            {
                float acceleration = CalamityWorld.death ? 0.07f : 0.045f;
                float maxSpeed = CalamityWorld.death ? 10.5f : 6.8f;
                if (npc.Center.X > target.Center.X)
                {
                    npc.velocity.X -= acceleration;
                    if (npc.velocity.X > 0)
                        npc.velocity.X -= acceleration;
                    if (npc.velocity.X < -maxSpeed)
                        npc.velocity.X = -maxSpeed;
                }
                else
                {
                    npc.velocity.X += acceleration;
                    if (npc.velocity.X < 0)
                        npc.velocity.X += acceleration;
                    if (npc.velocity.X > maxSpeed)
                        npc.velocity.X = maxSpeed;
                }

                //if need to jump
                if (npc.velocity.Y == 0f && (HoleBelow() || (npc.collideX && npc.position.X == npc.oldPosition.X)))
                {
                    npc.velocity.Y = CalamityWorld.death ? -7f : -5f;
                }

                //check if we can shoot at target.
                Vector2 vector = npc.Center - target.Center;
                if (vector.Length() < 480f && Collision.CanHit(npc.position, npc.width, npc.height, target.position, target.width, target.height))
                {
                    npc.ai[1] += 1f;
                    if (npc.ai[1] >= (CalamityWorld.death ? 60f : 120f))
                    {
                        //fire projectile
                        npc.ai[0] = 1f;
                        npc.ai[1] = npc.ai[2] = 0f;
                        npc.frame.Y = 400;
                        npc.frameCounter = 0;
                    }
                }
                else
                    npc.ai[1] -= 0.5f;

                if (npc.justHit)
                    npc.ai[1] -= 60f;

                if (npc.ai[1] < 0f)
                    npc.ai[1] = 0f;
            }
            else
            {
                npc.ai[2] += 1f;
                npc.velocity.X *= 0.95f;
                if (npc.ai[2] == 20f) //Don't do >= 20f or it'll cause a wave of scythes
                {
                    Main.PlaySound(SoundID.Item71, npc.position);
                    Vector2 vector = Main.player[npc.target].Center - npc.Center;
                    vector.Normalize();
                    int damage = CalamityWorld.downedAstrageldon ? 55 : 45;
                    Projectile.NewProjectile(npc.Center + (npc.Center.X < target.Center.X ? -14f : 14f) * Vector2.UnitX, vector * 7f, ModContent.ProjectileType<MantisRing>(), damage, 0f);
                }
            }

            npc.direction = npc.Center.X > target.Center.X ? 0 : 1;
            npc.spriteDirection = npc.direction;
        }

        private bool HoleBelow()
        {
            //width of npc in tiles
            int tileWidth = 4;
            int tileX = (int)(npc.Center.X / 16f) - tileWidth;
            if (npc.velocity.X > 0) //if moving right
            {
                tileX += tileWidth;
            }
            int tileY = (int)((npc.position.Y + npc.height) / 16f);
            for (int y = tileY; y < tileY + 2; y++)
            {
                for (int x = tileX; x < tileX + tileWidth; x++)
                {
                    if (Main.tile[x, y].active())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override void FindFrame(int frameHeight)
        {
            if (npc.ai[0] == 0f)
            {
                if (npc.velocity.Y != 0)
                {
                    npc.frame.Y = frameHeight * 13;
                    npc.frameCounter = 20;
                }
                else
                {
                    npc.frameCounter += 0.8f + Math.Abs(npc.velocity.X) * 0.5f;
                    if (npc.frameCounter > 10.0)
                    {
                        npc.frameCounter = 0;
                        npc.frame.Y += frameHeight;
                        if (npc.frame.Y > frameHeight * 5)
                        {
                            npc.frame.Y = 0;
                        }
                    }
                }
            }
            else
            {
                npc.frameCounter++;
                if (npc.frameCounter > 4)
                {
                    npc.frameCounter = 0;
                    npc.frame.Y += frameHeight;
                    if (npc.frame.Y >= frameHeight * 13)
                    {
                        npc.frame.Y = 0;
                        npc.frameCounter = 0;
                        npc.ai[0] = 0f;
                    }
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

            CalamityGlobalNPC.DoHitDust(npc, hitDirection, ModContent.DustType<AstralOrange>(), 1f, 4, 24);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            //draw glowmask
            spriteBatch.Draw(glowmask, npc.Center - Main.screenPosition - new Vector2(0, 8), npc.frame, Color.White * 0.6f, npc.rotation, new Vector2(70, 40), 1f, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.player))
            {
                return 0f;
            }
            else if (spawnInfo.player.InAstral(1))
            {
                return 0.16f;
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
            DropHelper.DropItemCondition(npc, ModContent.ItemType<AstralScythe>(), CalamityWorld.downedAstrageldon, 7, 1, 1);
        }
    }
}
