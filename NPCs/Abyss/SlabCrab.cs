using System;
using System.IO;
using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Banners;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.Abyss
{
    public class SlabCrab : ModNPC
    {
        enum AIState
        {
            Hiding = 0,
            IdleAnim = 1,
            Enraged = 2,
            Active = 3,
            Walking = 4
        }

        public Player Target => Main.player[NPC.target];
        public ref float CurrentPhase => ref NPC.ai[0];
        public ref float AITimer => ref NPC.ai[1];
        public ref float HopTimer => ref NPC.ai[2];
        public ref float CalmDownTimer => ref NPC.ai[3];

        public bool playerCrossed = false;
        public const int BaseDefense = 10; // the crab's defense while hostile
        public const int BaseAttack = 20; // the crab's damage while hostile
        public const float BaseKB = 1f; // the crab's knockback taken while hostile
        public static Asset<Texture2D> GlowTexture;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 23;
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetDefaults()
        {
            NPC.width = 44;
            NPC.height = 30;

            NPC.damage = BaseAttack;
            NPC.lifeMax = 300;

            NPC.aiStyle = AIType = -1;

            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 2, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.chaseable = false;
            NPC.knockBackResist = 0f;
            NPC.defense = 999998;
            NPC.HitSound = SoundID.NPCHit33;
            NPC.DeathSound = SoundID.NPCDeath36;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<SlabCrabBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            NPC.GravityIgnoresLiquid = true;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<AbyssLayer1Biome>().Type };

            // Scale stats in Expert and Master
            CalamityGlobalNPC.AdjustExpertModeStatScaling(NPC);
            CalamityGlobalNPC.AdjustMasterModeStatScaling(NPC);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.SlabCrab")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(playerCrossed);
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            playerCrossed = reader.ReadBoolean();
            NPC.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            NPC.direction = NPC.velocity.X != 0 ? Math.Sign(NPC.velocity.X) : NPC.direction;
            NPC.spriteDirection = -NPC.direction;
            // the crab cannot be awoken for 1.5 seconds after spawning
            NPC.localAI[0]++;
            if (NPC.localAI[0] < 90)
                return;
            // Enables expert scaling, if damage is 0 in set defaults expert scaling will not happen.
            NPC.damage = 0;
            switch (CurrentPhase)
            {
                case (int)AIState.Hiding:
                    NPC.ShowNameOnHover = false;
                    NPC.chaseable = false;
                    NPC.defense = 999998;
                    AITimer++;
                    NPC.TargetClosest(false);
                    // if the block below it is mined, instantly start running
                    if (NPC.velocity.Y > 0)
                    {
                        ChangeAIHook((int)AIState.Active);
                    }
                    // randomly start looking around after a bit
                    if (AITimer > 300 && Main.rand.NextBool(420))
                    {
                        ChangeAIHook((int)AIState.IdleAnim);
                    }
                    HandlePlayerDetection();
                    HandlePickaxeInteraction();
                    break;
                case (int)AIState.IdleAnim:
                    NPC.ShowNameOnHover = true;
                    NPC.chaseable = false;
                    NPC.defense = 999998;
                    AITimer++;
                    NPC.TargetClosest(false);
                    // if the block below it is mined, instantly start running
                    if (NPC.velocity.Y > 0)
                    {
                        ChangeAIHook((int)AIState.Active);
                    }
                    // if the animation finishes, go back to stone mode
                    if (AITimer > 90)
                    {
                        ChangeAIHook((int)AIState.Hiding);
                    }
                    HandlePlayerDetection();
                    HandlePickaxeInteraction();
                    break;
                case (int)AIState.Enraged:
                    NPC.ShowNameOnHover = true;
                    AITimer++;
                    NPC.defense = BaseDefense;
                    NPC.knockBackResist = 0f;
                    NPC.chaseable = false;
                    NPC.TargetClosest(false);
                    // give the animation time to play out then start attacking
                    if (AITimer > 24)
                    {
                        ChangeAIHook((int)AIState.Active);
                    }
                    break;
                case (int)AIState.Active:
                    {
                        NPC.ShowNameOnHover = true;
                        NPC.defense = BaseDefense;
                        NPC.knockBackResist = BaseKB;
                        NPC.damage = BaseAttack;
                        NPC.chaseable = true;
                        // if the player is out of range (affected by if the crab can see the player), start ticking down the calmdown timer
                        bool outofRange = ((Target.Center.Distance(NPC.Center) > 600) || (Target.Center.Distance(NPC.Center) > 320 && !Collision.CanHitLine(NPC.Center, 1, 1, Target.Center, 1, 1)));
                        if (outofRange)
                        {
                            CalmDownTimer++;
                        }
                        else if (CalmDownTimer > 0)
                        {
                            CalmDownTimer--;
                        }
                        if (NPC.velocity.Y == 0f)
                        {
                            AITimer++;
                            NPC.knockBackResist = 0.6f;
                            NPC.TargetClosest(true);
                            NPC.velocity.X *= 0.85f;

                            float hopRate = MathHelper.Lerp(25f, 10f, 1f - NPC.life / (float)NPC.lifeMax);
                            float lungeForwardSpeed = 6f;
                            float jumpSpeed = 7f;
                            if (Collision.CanHit(NPC.Center, 1, 1, Target.Center, 1, 1))
                                lungeForwardSpeed *= 1.2f;

                            // ocne the calmdown timer hits 3 seconds and the crab is on the ground, go back to hiding
                            if (outofRange && CalmDownTimer > 180)
                            {
                                playerCrossed = false;
                                ChangeAIHook((int)AIState.Hiding);
                                NPC.velocity.X = 0;
                            }
                            // after 3 hops, start walking once the crab is on the ground
                            if (HopTimer >= 3)
                            {
                                ChangeAIHook((int)AIState.Walking);
                            }
                            if (Main.netMode != NetmodeID.MultiplayerClient && AITimer > hopRate)
                            {
                                HopTimer++;

                                // Make a bigger leap every 3 hops.
                                if (HopTimer % 3f == 2f)
                                    lungeForwardSpeed *= 1.5f;

                                AITimer = 0f;
                                NPC.velocity.Y -= jumpSpeed;
                                NPC.velocity.X = lungeForwardSpeed * NPC.direction;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            NPC.knockBackResist = 0.2f;
                            NPC.velocity.X *= 0.995f;
                        }
                    }
                    break;
                case 4:
                    {
                        AITimer++;
                        NPC.knockBackResist = BaseKB;
                        NPC.damage = BaseAttack;
                        NPC.defense = BaseDefense;
                        // deaggro code is the same as above
                        bool outofRange = ((Target.Center.Distance(NPC.Center) > 300) || (Target.Center.Distance(NPC.Center) > 120 && !Collision.CanHitLine(NPC.Center, 1, 1, Target.Center, 1, 1)));
                        if (outofRange)
                        {
                            CalmDownTimer++;
                        }
                        else if (CalmDownTimer > 0)
                        {
                            CalmDownTimer--;
                        }
                        // turn around
                        if (NPC.oldPosition == NPC.position)
                        {
                            NPC.direction *= -1;
                            NPC.netUpdate = true;
                        }
                        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, 5 * NPC.direction, 0.0125f);
                        if (outofRange && Main.rand.NextBool(3) && CalmDownTimer > 180)
                        {
                            playerCrossed = false;
                            ChangeAIHook((int)AIState.Hiding);
                            NPC.velocity.X = 0;
                        }
                        // after 4 seconds and the player is still in the crab's line of sight, start leaping at them again
                        if (AITimer > 240 && Collision.CanHitLine(NPC.Center, 1, 1, Target.Center, 1, 1))
                        {
                            ChangeAIHook((int)AIState.Active);
                        }
                    }
                    break;
            }
            // heavy
            if (NPC.velocity.Y > 0)
            {
                NPC.velocity.Y *= 1.1f;
            }
        }

        public void ChangePhase(float ai0, float ai1 = -1, float ai2 = -1, float ai3 = -1)
        {
            CurrentPhase = ai0;
            AITimer = ai1 == -1 ? 0 : ai1;
            HopTimer = ai2 == -1 ? 0 : ai2;
            CalmDownTimer = ai3 == -1 ? 0 : ai3;
            NPC.netUpdate = true;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);
        }

        public void ChangeAIHook(float phase)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                ChangePhase((int)phase);
            }
            else
            {
                var netMessage = Mod.GetPacket();
                netMessage.Write((byte)CalamityModMessageType.SyncSlabCrabAI);
                netMessage.Write(NPC.whoAmI);
                netMessage.Write((int)phase);
                netMessage.Send();
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            // Don't draw the bar if in stealth mode.
            if (CurrentPhase < (int)AIState.Enraged)
                return false;
            return null;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.Calamity().ZoneAbyssLayer1 && spawnInfo.Water)
            {
                return SpawnCondition.CaveJellyfish.Chance;
            }
            return 0f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<SulphurousShale>(), 5, 10, 30);
        }

        public override void FindFrame(int frameHeight)
        {
            // walkie
            if (NPC.IsABestiaryIconDummy || CurrentPhase == (int)AIState.Active)
            {
                if (NPC.velocity.Y == 0)
                {
                    if (NPC.frameCounter++ % 6 == 0)
                    {
                        NPC.frame.Y += frameHeight;
                    }
                }
                if (NPC.velocity.X != 0 || NPC.IsABestiaryIconDummy)
                {
                    if (NPC.frame.Y > frameHeight * 22 || NPC.frame.Y < frameHeight * 19)
                    {
                        NPC.frame.Y = frameHeight * 19;
                    }
                }
                else
                {
                    NPC.frame.Y = frameHeight * 19;
                }
                return;
            }
            switch (CurrentPhase)
            {
                case (int)AIState.Hiding:
                    NPC.frame.Y = 0;
                    break;
                case (int)AIState.IdleAnim:
                    if (NPC.frameCounter++ % 6 == 0)
                    {
                        NPC.frame.Y += frameHeight;
                    }
                    if (NPC.frame.Y > frameHeight * 14)
                    {
                        NPC.frame.Y = frameHeight * 0;
                    }
                    break;
                case (int)AIState.Enraged:
                    if (NPC.frame.Y > frameHeight * 18 || NPC.frame.Y < frameHeight * 15)
                    {
                        NPC.frame.Y = frameHeight * 15;
                    }
                    if (NPC.frame.Y < frameHeight * 18)
                    {
                        NPC.frameCounter++;
                    }
                    if (NPC.frameCounter >= 6)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0;
                    }
                    break;
                default:
                    if (NPC.frameCounter++ % 6 == 0)
                    {
                        NPC.frame.Y += frameHeight;
                    }
                    if (NPC.frame.Y > frameHeight * 22 || NPC.frame.Y < frameHeight * 19)
                    {
                        NPC.frame.Y = frameHeight * 19;
                    }
                    break;
            }
        }
        // the crab is invincible while hiding
        public override bool? CanBeHitByItem(Player player, Item item) => CurrentPhase > (int)AIState.IdleAnim;

        public override bool? CanBeHitByProjectile(Projectile projectile) => CurrentPhase > (int)AIState.IdleAnim;

        public void HandlePickaxeInteraction()
        {
            Player player = Main.LocalPlayer;
            Rectangle tileMaus = new Rectangle(Player.tileTargetX * 16, Player.tileTargetY * 16, 16, 16);
            // check if the player is holding a pickaxe
            if (player.HeldItem.pick > 0)
            {
                // check if the tile sized cursor intersects with the crab
                if (tileMaus.Intersects(NPC.getRect()))
                {
                    // check if the crab is within range of the player's mining distance
                    if (player.Distance(NPC.Center) < (new Vector2(Player.tileRangeX, Player.tileRangeY).Length() + player.HeldItem.tileBoost) * 16)
                    {
                        // finally check if the player is actually swinging their pickaxe
                        if (player.ItemAnimationActive)
                        {
                            // ouch!
                            player.ApplyDamageToNPC(NPC, (int)player.GetDamage(DamageClass.MeleeNoSpeed).ApplyTo(player.HeldItem.damage), 0, player.direction);
                            SoundEngine.PlaySound(SoundID.Dig, NPC.Center); // this is the dig sound that shale piles use
                            if (CurrentPhase < (int)AIState.Enraged)
                            {
                                ChangeAIHook((int)AIState.Enraged);
                            }
                        }
                    }
                }
            }
        }

        public void HandlePlayerDetection()
        {
            if (!playerCrossed)
            {
                // if the player is within a 4 pixel distance of the NPC's horizontal position and is within its line of sight, mark the player as noticed
                if (Math.Abs(Target.position.X - NPC.position.X) < 4 && Collision.CanHitLine(NPC.Center, 1, 1, Target.Center, 1, 1))
                {
                    playerCrossed = true;
                }
            }
            else
            {
                // once the player moves 8 blocks away, jump them
                if (Target.Distance(NPC.Center) > 128)
                {
                    ChangeAIHook((int)AIState.Enraged);
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Water, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Water, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 1; i < 5; i++)
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SlabCrab" + i).Type, 1f);
                    }
                }                
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<RiptideDebuff>(), 90);
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!NPC.IsABestiaryIconDummy)
            {
                var effects = NPC.spriteDirection != 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4),
                NPC.frame, Color.White * 0.5f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            }
        }
    }
}
