using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Particles;

namespace CalamityMod.NPCs.SupremeCalamitas
{
    [AutoloadBossHead]
    public class SupremeCatastrophe : ModNPC
    {
        public int VerticalOffset = -375;
        public int CurrentFrame;
        public bool SlashingFromRight;
        public const int HorizontalOffset = 750;
        public const int SlashCounterLimit = 45;
        public const int DartBurstCounterLimit = 300;
        public Player Target => Main.player[npc.target];
        public ref float SlashCounter => ref npc.ai[1];
        public ref float DartBurstCounter => ref npc.ai[2];
        public ref float ElapsedVerticalDistance => ref npc.ai[3];
        public ref float AttackDelayTimer => ref npc.localAI[0];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Catastrophe");
            Main.npcFrameCount[npc.type] = 8;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.damage = 0;
            npc.npcSlots = 5f;
            npc.width = 120;
            npc.height = 120;
            npc.defense = 80;
            npc.DR_NERD(0.25f);
            npc.LifeMaxNERB(230000, 276000, 100000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.DD2_OgreRoar;
            npc.DeathSound = SoundID.NPCDeath52;
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToCold = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(VerticalOffset);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            VerticalOffset = reader.ReadInt32();
        }

        public override void FindFrame(int frameHeight)
        {
            float slashCounter = SlashCounter + (SlashingFromRight ? 0f : SlashCounterLimit);
            float slashInterpolant = Utils.InverseLerp(0f, SlashCounterLimit * 2f, slashCounter, true);
            if (AttackDelayTimer < 120f)
            {
                npc.frameCounter += 0.15f;
                if (npc.frameCounter >= 1f)
                    CurrentFrame = (CurrentFrame + 1) % 6;
            }
            else
            {
                CurrentFrame = (int)Math.Round(MathHelper.Lerp(6f, 15f, slashInterpolant));
            }

            int xFrame = CurrentFrame / Main.npcFrameCount[npc.type];
            int yFrame = CurrentFrame % Main.npcFrameCount[npc.type];

            npc.frame.Width = 400;
            npc.frame.Height = 230;
            npc.frame.X = xFrame * npc.frame.Width;
            npc.frame.Y = yFrame * npc.frame.Height;
        }

        public override void AI()
        {
            // Set the whoAmI variable.
            CalamityGlobalNPC.SCalCatastrophe = npc.whoAmI;

            // Disappear if Supreme Calamitas is not present.
            if (CalamityGlobalNPC.SCal < 0 || !Main.npc[CalamityGlobalNPC.SCal].active)
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }

            float totalLifeRatio = npc.life / (float)npc.lifeMax;
            if (CalamityGlobalNPC.SCalCataclysm != -1)
            {
                if (Main.npc[CalamityGlobalNPC.SCalCataclysm].active)
                    totalLifeRatio += Main.npc[CalamityGlobalNPC.SCalCataclysm].life / (float)Main.npc[CalamityGlobalNPC.SCalCataclysm].lifeMax;
            }
            totalLifeRatio *= 0.5f;

            // Get a target if no valid one has been found.
            if (npc.target < 0 || npc.target == Main.maxPlayers || Target.dead || !Target.active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away.
            if (!npc.WithinRange(Target.Center, CalamityGlobalNPC.CatchUpDistance200Tiles))
                npc.TargetClosest();

            float acceleration = 1.5f;

            // Reduce acceleration if target is holding a true melee weapon
            Item targetSelectedItem = Main.player[npc.target].inventory[Main.player[npc.target].selectedItem];
            if (targetSelectedItem.melee && (targetSelectedItem.shoot == ProjectileID.None || targetSelectedItem.Calamity().trueMelee))
                acceleration *= 0.5f;

            int verticalSpeed = (int)Math.Round(MathHelper.Lerp(2f, 6.5f, 1f - totalLifeRatio));

            // Move down.
            if (ElapsedVerticalDistance < HorizontalOffset)
            {
                ElapsedVerticalDistance += verticalSpeed;
                VerticalOffset += verticalSpeed;
            }

            // Move up.
            else if (ElapsedVerticalDistance < HorizontalOffset * 2)
            {
                ElapsedVerticalDistance += verticalSpeed;
                VerticalOffset -= verticalSpeed;
            }

            // Reset the vertical distance once a single period has concluded.
            else
                ElapsedVerticalDistance = 0f;

            // Reset rotation to zero.
            npc.rotation = 0f;

            // Set direction.
            npc.spriteDirection = (Target.Center.X > npc.Center.X).ToDirectionInt();

            // Hover to the side of the target.
            Vector2 idealVelocity = npc.SafeDirectionTo(Target.Center + new Vector2(-HorizontalOffset, VerticalOffset)) * 60f;
            npc.SimpleFlyMovement(idealVelocity, acceleration);

            // Have a small delay prior to shooting projectiles.
            if (AttackDelayTimer < 120f)
                AttackDelayTimer++;

            // Handle projectile shots.
            else
            {
                // Shoot sword slashes.
                float fireRate = (CalamityWorld.malice || BossRushEvent.BossRushActive) ? 2f : MathHelper.Lerp(1f, 2.5f, 1f - totalLifeRatio);
                SlashCounter += fireRate;
                if (SlashCounter >= SlashCounterLimit)
                {
                    SlashCounter = 0f;
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SCalSounds/BrimstoneShoot"), npc.Center);

                    int type = ModContent.ProjectileType<SupremeCatastropheSlash>();
                    int damage = npc.GetProjectileDamage(type);
                    Vector2 slashSpawnPosition = npc.Center + Vector2.UnitX * 125f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(slashSpawnPosition, Vector2.UnitX * 4f, type, damage, 0f, Main.myPlayer, 0f, SlashingFromRight.ToInt());
                    SlashingFromRight = !SlashingFromRight;
                    CurrentFrame = 0;
                }

                // Shoot dart spreads.
                fireRate = (CalamityWorld.malice || BossRushEvent.BossRushActive) ? 3f : MathHelper.Lerp(1f, 4f, 1f - totalLifeRatio);
                DartBurstCounter += fireRate;
                if (DartBurstCounter >= DartBurstCounterLimit)
                {
                    DartBurstCounter = 0f;
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SCalSounds/BrimstoneShoot"), npc.Center);

                    // TODO: Consider changing this to use RotatedBy or ToRotationVector2.
                    float speed = 7f;
                    int type = ModContent.ProjectileType<BrimstoneBarrage>();
                    int damage = npc.GetProjectileDamage(type);
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                            Projectile.NewProjectile(npc.Center.X, npc.Center.Y, (float)(Math.Sin(offsetAngle) * speed), (float)(Math.Cos(offsetAngle) * speed), type, damage, 0f, Main.myPlayer, 0f, 1f);
                            Projectile.NewProjectile(npc.Center.X, npc.Center.Y, (float)(-Math.Sin(offsetAngle) * speed), (float)(-Math.Cos(offsetAngle) * speed), type, damage, 0f, Main.myPlayer, 0f, 1f);
                        }
                    }

                    for (int i = 0; i < 6; i++)
                        Dust.NewDust(npc.position + npc.velocity, npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture = Main.npcTexture[npc.type];
            Vector2 origin = npc.frame.Size() * 0.5f;
            int afterimageCount = 4;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int i = 1; i < afterimageCount; i += 2)
                {
                    Color afterimageColor = npc.GetAlpha(Color.Lerp(lightColor, Color.White, 0.5f)) * ((afterimageCount - i) / 15f);
                    Vector2 drawPosition = npc.oldPos[i] + npc.Size * 0.5f - Main.screenPosition;
                    spriteBatch.Draw(texture, drawPosition, npc.frame, afterimageColor, npc.rotation, origin, npc.scale, spriteEffects, 0f);
                }
            }

            Vector2 mainDrawPosition = npc.Center - Main.screenPosition;
            spriteBatch.Draw(texture, mainDrawPosition, npc.frame, npc.GetAlpha(lightColor), npc.rotation, origin, npc.scale, spriteEffects, 0f);

            texture = ModContent.GetTexture("CalamityMod/NPCs/SupremeCalamitas/SupremeCatastropheGlow");
            Color baseGlowmaskColor = Color.Lerp(Color.White, Color.Cyan, 0.35f);

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int i = 1; i < afterimageCount; i++)
                {
                    Color afterimageColor = Color.Lerp(baseGlowmaskColor, Color.White, 0.5f) * ((afterimageCount - i) / 15f);
                    Vector2 drawPosition = npc.oldPos[i] + npc.Size * 0.5f - Main.screenPosition;
                    spriteBatch.Draw(texture, drawPosition, npc.frame, afterimageColor, npc.rotation, origin, npc.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture, mainDrawPosition, npc.frame, baseGlowmaskColor, npc.rotation, origin, npc.scale, spriteEffects, 0f);

            return false;
        }

        public override void NPCLoot()
        {
            if (!CalamityWorld.revenge)
            {
                int heartAmt = Main.rand.Next(3) + 3;
                for (int i = 0; i < heartAmt; i++)
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Heart);
            }
            DropHelper.DropItemChance(npc, ModContent.ItemType<SupremeCatastropheTrophy>(), 10);
        }

        public override bool CheckActive() => false;

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 100;
                npc.height = 100;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }

                // Turn into dust on death.
                if (npc.life <= 0)
                    DeathAshParticle.CreateAshesFromNPC(npc);
            }
        }
    }
}
