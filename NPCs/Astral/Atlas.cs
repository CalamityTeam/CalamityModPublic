using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Astral
{
    public class Atlas : ModNPC
    {
        //TODO -- draw 4 pixels lower than actual bottom position (arm overlaps tiles)

        private static Texture2D glowmask;

        //CONSTANTS
        private const int sheetHeight = 852;
        private const int startWalkFrameY = 710;
        private const float idle_walkMaxSpeed = 0.8f;
        private const float idle_walkAcceleration = 0.1f;
        private float target_walkMaxSpeed = 1.6f;
        private float target_walkAcceleration = 0.12f;
        private const float swing_minXDistance = 96f;
        private const float swing_minYDistance = 48f;
        private const int swing_minCounterHit = 16;
        private const int swing_maxCounterHit = 25; //the minimum and maximum values for the counter in which the swing's main hitbox is active.
        private const int swing_playSoundOnFrame = 15;

        private bool idling;
        private bool idle_impulseWalk;
        private bool idle_walkLeft;
        private bool idle_blocked;

        private bool swinging;
        private bool swingYeet;
        private byte swing_counter;
        private short swing_untilNext;

        private float idle_counter
        {
            get
            {
                return npc.ai[0];
            }
            set
            {
                npc.ai[0] = value;
            }
        }
        private float target_counter
        {
            get
            {
                return npc.ai[1];
            }
            set
            {
                npc.ai[1] = value;
            }
        }
        private float grounded_counter
        {
            get
            {
                return npc.ai[2];
            }
            set
            {
                npc.ai[2] = value;
            }
        }
        private Player Target
        {
            get
            {
                if (npc.HasValidTarget)
                {
                    return Main.player[npc.target];
                }
                return null;
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Atlas");
            Main.npcFrameCount[npc.type] = 6;
            //not really important seeing as custom drawing, but for heights sake, 6
            //also it's visuals are messed up on npc spawners etc. because the sheet is 3 wide.
            //not much we can do. looks fine in-game so /shrug
            if (!Main.dedServ)
                glowmask = ModContent.GetTexture("CalamityMod/NPCs/Astral/AtlasGlow");
        }

        public override void SetDefaults()
        {
            npc.Calamity().canBreakPlayerDefense = true;
            npc.lavaImmune = true;
            npc.width = 78;
            npc.height = 88;
            npc.damage = 70;
            npc.defense = 40;
            npc.DR_NERD(0.15f);
            npc.lifeMax = 1200;
            npc.knockBackResist = 0.08f;
            npc.value = Item.buyPrice(0, 1, 0, 0);
            npc.aiStyle = -1;
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/AtlasDeath");
            banner = npc.type;
            bannerItem = ModContent.ItemType<AtlasBanner>();
            if (CalamityWorld.downedAstrageldon)
            {
                npc.damage = 100;
                npc.defense = 50;
                npc.knockBackResist = 0.04f;
                npc.lifeMax = 1600;
            }
            if (NPC.downedAncientCultist)
            {
                npc.damage = 150;
                npc.defense = 75;
                npc.knockBackResist = 0f;
                npc.lifeMax = 2400;
            }
            if (CalamityWorld.death)
            {
                target_walkAcceleration = 0.18f;
                target_walkMaxSpeed = 3.2f;
            }
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToSickness = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            BitsByte bb = new BitsByte(idling, idle_impulseWalk, idle_walkLeft, idle_blocked, swinging, swingYeet);
            writer.Write(bb);
            writer.Write(swing_counter);
            writer.Write(swing_untilNext);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            BitsByte bb = reader.ReadByte();
            idling = bb[0];
            idle_impulseWalk = bb[1];
            idle_walkLeft = bb[2];
            idle_blocked = bb[3];
            swinging = bb[4];
            swingYeet = bb[5];

            swing_counter = reader.ReadByte();
            swing_untilNext = reader.ReadInt16();
        }

        public override void AI()
        {
            //PICK A TARGET
            if (!npc.HasValidTarget || idling)
            {
                //target and reset
                npc.TargetClosest(false);
                idling = false;

                if (!npc.HasValidTarget) //if we don't have valid target
                {
                    idling = true;
                }
                else if (!Collision.CanHit(npc.position + Vector2.UnitY * 8, 15, 15, Target.position, Target.width, Target.height)) //else if we do but can't see them
                {
                    idling = true;
                }
                else
                {
                    //PLAY SOUND (SAD) *but this time it has a target. more like an indicator. he sad because he has to kill you now :c
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/AtlasSad0"), npc.Center);
                }
            }

            if (npc.velocity.Y == 0)
            {
                grounded_counter++;
            }
            else
            {
                grounded_counter = 0;
            }

            swing_untilNext--;

            if (idling)
            {
                DoIdleAI();
            }
            else if (swinging)
            {
                DoSwingUpdate();
            }
            else
            {
                DoTargetAI();
            }

            npc.stepSpeed = 0.75f;

            if (npc.velocity.Y >= 0f)
            {
                Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY, 1, false, 1);
            }
        }

        private void DoSwingUpdate()
        {
            npc.velocity.X *= 0.84f;
            swing_counter++;

            if (swing_counter == swing_playSoundOnFrame)
            {
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/AtlasSwing"), npc.Center);
            }

            if (swing_counter >= swing_minCounterHit && swing_counter <= swing_maxCounterHit)
            {
                int centerX = (int)npc.Center.X;
                int centerY = (int)npc.Center.Y;
                int width = (int)swing_minXDistance;
                int height = 142;
                Rectangle hitbox = new Rectangle(
                    centerX + (npc.direction == -1 ? -width : 0),
                    centerY - height / 2,
                    width, height);

                for (int i = 0; i < Main.player.Length; i++)
                {
                    if (Main.player[i].active && !Main.player[i].dead && Main.player[i].getRect().Intersects(hitbox))
                    {
                        Vector2 before = Main.player[i].velocity;
                        Main.player[i].Hurt(PlayerDeathReason.ByNPC(npc.whoAmI), npc.damage, npc.direction);
                        Vector2 after = Main.player[i].velocity;
                        Vector2 difference = after - before;

                        float horMult = 3.5f;
                        float verMult = 1.2f;

                        swingYeet = false;

                        if (Main.rand.NextBool(1000)) //Launch the player very fast very rarely
                        {
                            horMult = 12f;
                            verMult = 2.3f;
                            swingYeet = true;
                        }

                        if (Main.player[i].noKnockback)
                        {
                            horMult *= 0.5f;
                            verMult *= 0.5f;
                        }

                        difference *= new Vector2(horMult, verMult);

                        Main.player[i].velocity = before + difference;
                    }
                }
            }
        }

        private void DoIdleAI()
        {
            //idling, so I should mostly just walk around and do nothing useful with my life
            //oh hey, it's just me haha
            if (HoleBelow() || (npc.collideX && npc.oldPosition.X == npc.position.X))
            {
                idle_impulseWalk = false;
                idle_blocked = true;
            }

            idle_counter++;
            if (idle_impulseWalk)
            {
                if (idle_counter == 4)
                {
                    npc.frame.Y = startWalkFrameY; //reset frame to start walk animation.
                }

                npc.velocity.X += idle_walkAcceleration * (idle_walkLeft ? -1 : 1);
                if (Math.Abs(npc.velocity.X) > idle_walkMaxSpeed)
                {
                    npc.velocity.X = idle_walkLeft ? -idle_walkMaxSpeed : idle_walkMaxSpeed;
                }
                idle_impulseWalk = idle_impulseWalk && idle_counter > 20 && Main.rand.Next(150) != 0;
                if (!idle_impulseWalk)
                {
                    idle_counter = 0;
                }

                npc.direction = idle_walkLeft ? -1 : 1;
            }
            else
            {
                idle_impulseWalk = idle_counter > 100 && Main.rand.NextBool(400); //try to impulse
                if (idle_impulseWalk) //if we've just decided to stroll
                {
                    idle_counter = 0;
                    if (idle_blocked) //if we were blocked before, pick the other direction.
                    {
                        idle_blocked = false;
                        idle_walkLeft = !idle_walkLeft;
                    }
                    else
                    {
                        idle_walkLeft = Main.rand.NextBool(); //pick direction
                    }
                    npc.frameCounter = 0; // reset framecounter to start walk.

                    //PLAY SOUND (IDLE)
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/AtlasIdle" + Main.rand.Next(2)), npc.Center);
                }
                else
                {
                    npc.velocity.X *= 0.9f;
                    if (Math.Abs(npc.velocity.X) < 0.1f)
                    {
                        npc.velocity.X = 0f;
                    }
                }
            }
        }

        private void DoTargetAI()
        {
            //got a target, so I'm gonna chase them down and smack em.
            bool targetToLeft = Target.Center.X < npc.Center.X;
            int mult = targetToLeft ? -1 : 1;

            npc.velocity.X += target_walkAcceleration * mult;
            if (Math.Abs(npc.velocity.X) > target_walkMaxSpeed)
            {
                npc.velocity.X = target_walkMaxSpeed * mult;
            }

            //based on velocity, as he a big hunk and he can't walk backwards whilst facing target
            npc.direction = npc.velocity.X < 0f ? -1 : 1;

            //if have been on ground for at least 1.5 seonds, and are hitting wall or there is a hole
            if (grounded_counter > 90 && (HoleBelow() || (npc.collideX && npc.position.X == npc.oldPosition.X)))
            {
                //jump
                npc.velocity.Y = -6f;
                target_counter++;
            }

            bool canHitTarget = Collision.CanHit(npc.position + Vector2.UnitY * 8, 15, 15, Target.position, Target.width, Target.height);

            if (target_counter > 0 && !canHitTarget)
            {
                target_counter++;
            }
            else
            {
                target_counter = 0;
            }

            if (target_counter > 180)
            {
                idling = true;
                target_counter = 0;

                //PLAY SOUND (SAD)        he sad cos he lost his target :C
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/AtlasSad1"), npc.Center);
            }
            else if (target_counter == 0 && npc.velocity.Y == 0)
            {
                //check if we can swing at the target
                Vector2 distance = npc.Center - Target.Center;
                if (swing_untilNext < 0 && canHitTarget && Math.Abs(distance.X) < swing_minXDistance && Math.Abs(distance.Y) < swing_minYDistance)
                {
                    StartSwing();
                }
            }
        }

        private bool HoleBelow()
        {
            //width of npc in tiles
            int tileWidth = 5;
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

        private void StartSwing()
        {
            swinging = true;
            swing_counter = 0;
            swing_untilNext = 300;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            //play sound
            if (npc.soundDelay == 0)
            {
                npc.soundDelay = 15;
                switch (Main.rand.Next(3))
                {
                    case 0:
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AtlasHurt0"), npc.Center);
                        break;
                    case 1:
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AtlasHurt1"), npc.Center);
                        break;
                    case 2:
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AtlasHurt2"), npc.Center);
                        break;
                }
            }

            CalamityGlobalNPC.DoHitDust(npc, hitDirection, (Main.rand.Next(0, Math.Max(0, npc.life)) == 0) ? 5 : ModContent.DustType<AstralEnemy>(), 1f, 3, 30);

            //if dead do gores
            if (npc.life <= 0)
            {
                //head
                Gore.NewGore(npc.Top, npc.velocity * 0.5f, mod.GetGoreSlot("Gores/Atlas/AtlasGore4"));
                //hand
                Gore.NewGore(npc.direction == 1 ? npc.Right : npc.Left, npc.velocity * 0.5f, mod.GetGoreSlot("Gores/Atlas/AtlasGore2"));
                //rest
                Gore.NewGore(npc.Center, npc.velocity * 0.5f, mod.GetGoreSlot("Gores/Atlas/AtlasGore0"));
                Gore.NewGore(npc.Center, npc.velocity * 0.5f, mod.GetGoreSlot("Gores/Atlas/AtlasGore1"));
                Gore.NewGore(npc.Center, npc.velocity * 0.5f, mod.GetGoreSlot("Gores/Atlas/AtlasGore3"));
                Gore.NewGore(npc.Center, npc.velocity * 0.5f, mod.GetGoreSlot("Gores/Atlas/AtlasGore5"));
                Gore.NewGore(npc.Center, npc.velocity * 0.5f, mod.GetGoreSlot("Gores/Atlas/AtlasGore6"));
            }
        }

        public override void FindFrame(int frameHeight)
        {
            int width = 142;
            int height = 142;

            //ensure width and height are set.
            npc.frame.Width = width;
            npc.frame.Height = height;

            if (idling)
            {
                npc.frameCounter++;
                if (!idle_impulseWalk)
                {
                    npc.frame.X = 0;
                    if (npc.frameCounter > 9)
                    {
                        npc.frameCounter = 0;
                        npc.frame.Y += height;
                        if (npc.frame.Y >= sheetHeight)
                        {
                            npc.frame.Y = 0;
                        }
                    }
                }
                else
                {
                    npc.frame.X = width;
                    if (npc.frameCounter > 8)
                    {
                        npc.frameCounter = 0;
                        npc.frame.Y += height;
                        if (npc.frame.Y >= sheetHeight)
                        {
                            npc.frame.Y = 0;
                        }
                    }
                }
            }
            else if (swinging)
            {
                npc.frameCounter = 0;
                npc.frame.X = width * 2;
                if (swing_counter < 8)
                {
                    npc.frame.Y = 0;
                }
                else if (swing_counter < 16)
                {
                    npc.frame.Y = height;
                }
                else if (swing_counter < 23)
                {
                    npc.frame.Y = height * 2;
                }
                else if (swing_counter < 30)
                {
                    npc.frame.Y = height * 3;
                }
                else if (swing_counter < 38)
                {
                    npc.frame.Y = height * 4;
                }
                else if (swing_counter < 46)
                {
                    npc.frame.Y = height * 5;
                }
                else
                {
                    swinging = false;
                    swing_counter = 0;
                    idling = false;
                }
            }
            else
            {
                //if walking
                npc.frameCounter++;
                npc.frame.X = width;
                if (npc.frameCounter > 7)
                {
                    npc.frameCounter = 0;
                    npc.frame.Y += height;
                    if (npc.frame.Y >= sheetHeight)
                    {
                        npc.frame.Y = 0;
                    }
                }
            }

            //if we're in the air
            if (npc.velocity.Y != 0)
            {
                npc.frame.X = width;
                npc.frame.Y = height * 4; //5th frame down
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Vector2 position = npc.position - new Vector2(30, 48) - Main.screenPosition;
            SpriteEffects effect = npc.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            //draw actual sprite
            spriteBatch.Draw(Main.npcTexture[npc.type], position, npc.frame,
                drawColor, 0f, default, 1f, //color, rotation, origin, scale
                effect, 0f); //effect, drawlayer

            //draw glowmask
            spriteBatch.Draw(
                glowmask, position, npc.frame,
                Color.White * 0.65f, 0f, default, 1f, //color, rotation, origin, scale
                effect, 0f); //effect, drawlayer

            return false;
        }

        private Vector2 GetEyePos()
        {
            int x = 0;
            int y = 0;
            switch (npc.frame.Y)
            {
                case 0:
                    x = 45;
                    y = 79;
                    break;
                case 142:
                    x = 41;
                    y = 79;
                    break;
                case 284:
                    x = 35;
                    y = 75;
                    break;
                case 426:
                    x = 25;
                    y = 73;
                    break;
                case 568:
                    x = 31;
                    y = 77;
                    break;
                case 710:
                    x = 43;
                    y = 81;
                    break;
            }
            if (npc.direction == 1)
            {
                x = 142 - x;
            }
            return new Vector2(x, y);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.player))
            {
                return 0f;
            }
            else if (spawnInfo.player.InAstral(1) && NPC.downedAncientCultist && !CalamityWorld.downedStarGod)
            {
                return 0.27f;
            }
            else if (spawnInfo.player.InAstral(1))
            {
                return 0.09f;
            }
            return 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 300, true);
        }

        public override void NPCLoot()
        {
            int minStardust = Main.expertMode ? 7 : 6;
            int maxStardust = Main.expertMode ? 9 : 8;
            DropHelper.DropItem(npc, ModContent.ItemType<Stardust>(), minStardust, maxStardust);

            DropHelper.DropItemCondition(npc, ModContent.ItemType<TitanArm>(), CalamityWorld.downedAstrageldon, 7, 1, 1);
            DropHelper.DropItem(npc, ModContent.ItemType<TitanHeart>());
        }
    }
}
