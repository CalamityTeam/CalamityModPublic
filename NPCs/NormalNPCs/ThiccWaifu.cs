using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.Enemy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class ThiccWaifu : ModNPC
    {
        public enum AttackState
        {
            Hover,
            CloudTeleport,
            LightningSummon,
            TornadoSummon,
            LightningBladeSlice,
            NimbusSummon
        }

        public Player Target => Main.player[npc.target];
        public AttackState CurrentAttackState
        {
            get => (AttackState)(int)npc.ai[0];
            set
            {
                if (npc.ai[0] != (int)value)
                {
                    npc.ai[0] = (int)value;
                    npc.netUpdate = true;
                }
            }
        }
        public bool Phase2 => npc.life < npc.lifeMax * 0.5f;
        public ref float AttackTimer => ref npc.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cloud Elemental");
            Main.npcFrameCount[npc.type] = 8;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 3f;
            npc.damage = 38;
            npc.width = 80;
            npc.height = 140;
            npc.defense = 18;
            npc.DR_NERD(0.05f);
            npc.lifeMax = 6000;
            npc.knockBackResist = 0.05f;
            npc.value = Item.buyPrice(0, 1, 50, 0);
            npc.HitSound = SoundID.NPCHit23;
            npc.DeathSound = SoundID.NPCDeath39;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.rarity = 2;
            banner = npc.type;
            bannerItem = ModContent.ItemType<CloudElementalBanner>();
            npc.Calamity().VulnerableToCold = true;
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToElectricity = false;
            npc.Calamity().VulnerableToWater = false;
            npc.Calamity().VulnerableToHeat = false;
        }

        public override void AI()
        {
            // Emit light.
            Lighting.AddLight((int)(npc.Center.X / 16), (int)(npc.Center.Y / 16), 0.375f, 0.5f, 0.625f);

            // Get a new target if the current one is dead.
            if (Target.dead || !Target.active || !Main.player.IndexInRange(npc.target))
                npc.TargetClosest();

            switch (CurrentAttackState)
            {
                case AttackState.Hover:
                    DoBehavior_Hover();
                    break;
                case AttackState.CloudTeleport:
                    DoBehavior_CloudTeleport();
                    break;
                case AttackState.LightningSummon:
                    DoBehavior_LightningSummon();
                    break;
                case AttackState.TornadoSummon:
                    DoBehavior_TornadoSummon();
                    break;
                case AttackState.LightningBladeSlice:
                    break;
                case AttackState.NimbusSummon:
                    DoBehavior_NimbusSummon();
                    break;
            }

            AttackTimer++;
        }

        public void DoBehavior_Hover()
        {
            float lifeRatio = npc.life / (float)npc.lifeMax;
            int hoverTime = (int)MathHelper.Lerp(330f, 180f, 1f - lifeRatio);
            float hoverAcceleration = MathHelper.Lerp(0.2f, 0.425f, 1f - lifeRatio);
            Vector2 hoverSpeed = new Vector2(8.5f, 4.5f);

            if (Main.rand.NextBool(8) && !Main.dedServ)
            {
                for (int i = 0; i < 2; i++)
                {
                    Dust cloudDust = Dust.NewDustDirect(npc.position, npc.width, npc.height, 16);
                    cloudDust.velocity = Main.rand.NextVector2CircularEdge(4f, 4f);
                    cloudDust.velocity.Y /= 3f;
                    cloudDust.scale = Main.rand.NextFloat(1.15f, 1.35f);
                    cloudDust.noGravity = true;
                }
            }

            if (AttackTimer < hoverTime - 30)
            {
                Vector2 idealVelocity = npc.SafeDirectionTo(Target.Center) * hoverSpeed;

                if (Math.Abs(npc.Center.X - Target.Center.X) > 30f)
                {
                    npc.SimpleFlyMovement(idealVelocity, hoverAcceleration);
                    npc.spriteDirection = (npc.velocity.X > 0).ToDirectionInt();
                }
            }
            else
                npc.velocity *= 0.95f;

            if (AttackTimer >= hoverTime)
            {
                List<AttackState> potentialAttackStates = new List<AttackState>()
                {
                    AttackState.NimbusSummon,
                    AttackState.TornadoSummon,
                };

                // Gain new attacks in phase 2.
                if (Phase2)
                {
                    potentialAttackStates.Add(AttackState.LightningSummon);
                    potentialAttackStates.Add(AttackState.LightningBladeSlice);
                }

                // Don't summon more Nimbi if there's already a lot, to prevent NPC spam.
                if (NPC.CountNPCS(NPCID.AngryNimbus) >= 10)
                    potentialAttackStates.Remove(AttackState.NimbusSummon);

                if (Main.rand.NextBool(3))
                    CurrentAttackState = AttackState.CloudTeleport;
                else
                    CurrentAttackState = Main.rand.Next(potentialAttackStates);

                AttackTimer = 0f;
                npc.netUpdate = true;
            }
        }

        public void DoBehavior_CloudTeleport()
        {
            int teleportFadeoutTime = 75;
            int teleportFadeinTime = 60;

            // Fade out and release some gaseous particles.
            if (AttackTimer <= teleportFadeoutTime)
            {
                float fadeoutCompletion = Utils.InverseLerp(0f, teleportFadeoutTime, AttackTimer, true);
                float particleSpawnRate = MathHelper.Clamp(fadeoutCompletion + 0.6f, 0.5f, 1f);
                npc.Opacity = MathHelper.Lerp(1f, 0f, fadeoutCompletion);

                if (Main.rand.NextFloat() < particleSpawnRate && !Main.dedServ)
                {
                    Dust.NewDustDirect(npc.position, npc.width, npc.height, 16);

                    if (Main.rand.NextBool(15))
                    {
                        int smokeType = Utils.SelectRandom(Main.rand, GoreID.ChimneySmoke1, GoreID.ChimneySmoke2, GoreID.ChimneySmoke3);
                        Vector2 smokeVelocity = Main.rand.NextVector2CircularEdge(6f, 6f);
                        Gore.NewGorePerfect(npc.Center + Main.rand.NextVector2Circular(40f, 40f), smokeVelocity, smokeType);
                    }
                }
            }

            // Teleport when ready.
            if (AttackTimer == teleportFadeoutTime)
            {
                float teleportRadius = 420f;
                npc.Center = Target.Center + Main.rand.NextVector2CircularEdge(teleportRadius, teleportRadius);
                npc.netUpdate = true;
            }

            // Fade in and release some particles.
            if (AttackTimer > teleportFadeoutTime && AttackTimer <= teleportFadeoutTime + teleportFadeinTime)
            {
                float fadeinCompletion = Utils.InverseLerp(teleportFadeoutTime, teleportFadeoutTime + teleportFadeinTime, AttackTimer, true);
                float particleSpawnRate = MathHelper.Clamp(fadeinCompletion + 0.6f, 0.5f, 1f);
                npc.Opacity = fadeinCompletion;

                if (Main.rand.NextFloat() < particleSpawnRate && !Main.dedServ)
                    Dust.NewDustDirect(npc.position, npc.width, npc.height, 16);
            }

            if (AttackTimer >= teleportFadeoutTime + teleportFadeinTime)
            {
                CurrentAttackState = AttackState.Hover;
                AttackTimer = 0f;
                npc.netUpdate = true;
            }
        }

        public void DoBehavior_LightningSummon()
        {
            int cloudSummonDelay = 60;
            int cloudSummonRate = 30;
            int totalCloudWavesToSummon = 5;
            int lightningDamage = Main.expertMode ? 23 : 36;
            if (Phase2)
            {
                cloudSummonRate -= 5;
                totalCloudWavesToSummon += 2;
                lightningDamage += 2;
            }

            // Slow down.
            npc.velocity *= 0.96f;

            // And create lightning clouds.
            if (AttackTimer > cloudSummonDelay)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient && (AttackTimer - cloudSummonDelay) % cloudSummonRate == cloudSummonRate - 1)
                {
                    int projectileType = ModContent.ProjectileType<LightningCloud>();
                    float cloudSpawnOutwardness = (AttackTimer - cloudSummonDelay) / cloudSummonRate * 50f;

                    Vector2 spawnPosition = npc.Top + new Vector2(cloudSpawnOutwardness, -36);
                    Projectile.NewProjectileDirect(spawnPosition, Vector2.Zero, projectileType, lightningDamage, 0f, Main.myPlayer);
                    spawnPosition = npc.Top + new Vector2(-cloudSpawnOutwardness, -36);
                    Projectile.NewProjectileDirect(spawnPosition, Vector2.Zero, projectileType, lightningDamage, 0f, Main.myPlayer);
                }
            }

            if (AttackTimer > cloudSummonDelay + cloudSummonRate * totalCloudWavesToSummon)
            {
                CurrentAttackState = AttackState.Hover;
                AttackTimer = 0f;
                npc.netUpdate = true;
            }
        }

        public void DoBehavior_TornadoSummon()
        {
            int tornadoSpawnDelay = 60;
            int totalTornadosToSummon = Phase2 ? 8 : 5;

            // Slow down.
            npc.velocity *= 0.96f;

            // And create tornadoes.
            if (Main.netMode != NetmodeID.MultiplayerClient && AttackTimer == tornadoSpawnDelay)
            {
                int projectileType = ModContent.ProjectileType<StormMarkHostile>();
                for (int i = 0; i < totalTornadosToSummon; i++)
                {
                    float angle = MathHelper.TwoPi / totalTornadosToSummon * i;
                    Vector2 spawnPosition = Target.Center + angle.ToRotationVector2() * 620f;
                    Projectile.NewProjectile(spawnPosition, Vector2.Zero, projectileType, 0, 0f, Main.myPlayer);
                }
            }

            if (AttackTimer >= tornadoSpawnDelay + 180f)
            {
                CurrentAttackState = AttackState.Hover;
                AttackTimer = 0f;
                npc.netUpdate = true;
            }
        }

        public void DoBehavior_NimbusSummon()
        {
            int nimbusSummonDelay = 45;
            int totalNimbiToSummon = 5;
            int nimbusSummonRate = 50;
            if (Phase2)
            {
                totalNimbiToSummon++;
                nimbusSummonRate -= 10;
            }

            // Slow down in anticipation of the summoning.
            if (AttackTimer < nimbusSummonDelay)
                npc.velocity *= 0.92f;

            // Summon a circle of nimbi.
            else if ((AttackTimer - nimbusSummonDelay) % nimbusSummonRate == nimbusSummonRate - 1)
            {
                Point spawnPosition = (npc.Center + npc.ai[2].ToRotationVector2() * 300f).ToPoint();
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Tile tileAtPosition = CalamityUtils.ParanoidTileRetrieval(spawnPosition.X, spawnPosition.Y);
                    if (!(tileAtPosition.active() && Main.tileSolid[tileAtPosition.type]))
                        NPC.NewNPC(spawnPosition.X, spawnPosition.Y, NPCID.AngryNimbus);
                }

                // Create sound cloud dust at the position where the nimbus was spawned.
                if (!Main.dedServ)
                {
                    for (int i = 0; i < 20; i++)
                        Dust.NewDustDirect(spawnPosition.ToVector2(), -20, 20, 16);

                    Main.PlaySound(SoundID.Item122, spawnPosition.ToVector2());
                }
                npc.ai[2] += MathHelper.TwoPi / totalNimbiToSummon;
            }

            // Return to hovering after the nimbi have been summoned.
            if (AttackTimer >= nimbusSummonRate * totalNimbiToSummon)
            {
                npc.ai[2] = 0f;
                CurrentAttackState = AttackState.Hover;
                AttackTimer = 0f;
                npc.netUpdate = true;
            }
        }

        public void DoBehavior_LightningBladeSlice()
        {
            int totalSlices = 4;
            int sliceChargeTime = 45;
            int sliceChargeDelay = 15;
            float sliceChargeSpeed = 22f;

            // Slow down.
            if (AttackTimer % (sliceChargeTime + sliceChargeDelay) < sliceChargeDelay)
                npc.velocity *= 0.92f;

            // And charge.
            if (AttackTimer % (sliceChargeTime + sliceChargeDelay) == sliceChargeDelay)
            {
                npc.damage = npc.defDamage * 2;
                npc.velocity = npc.SafeDirectionTo(Target.Center) * sliceChargeSpeed;
                npc.spriteDirection = (npc.velocity.X > 0).ToDirectionInt();
                npc.netUpdate = true;
            }

            if (AttackTimer >= (sliceChargeTime + sliceChargeDelay) * totalSlices)
            {
                npc.damage = npc.defDamage;
                CurrentAttackState = AttackState.Hover;
                AttackTimer = 0f;
                npc.netUpdate = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = ModContent.GetTexture("CalamityMod/NPCs/NormalNPCs/ThiccWaifuAttack");
            if (CurrentAttackState != AttackState.Hover)
                CalamityMod.DrawTexture(spriteBatch, texture, 0, npc, drawColor);
            else
                CalamityMod.DrawTexture(spriteBatch, Main.npcTexture[npc.type], 0, npc, drawColor);
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter = npc.frameCounter + MathHelper.Max(npc.velocity.Length() * 0.1f, 0.6f) + 1.0;
            if (npc.frameCounter >= (CurrentAttackState != AttackState.Hover ? 16.0 : 8.0))
            {
                npc.frame.Y += frameHeight;
                npc.frameCounter = 0.0;
            }

            if (npc.frame.Y >= frameHeight * 8)
                npc.frame.Y = 0;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !Main.hardMode || !Main.raining || NPC.AnyNPCs(ModContent.NPCType<ThiccWaifu>()))
                return 0f;

            return SpawnCondition.Sky.Chance * 0.1f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Electrified, 180, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(npc.position, npc.width, npc.height, 16, hitDirection, -1f, 0, default, 1f);

            if (npc.life <= 0)
            {
                for (int k = 0; k < 50; k++)
                    Dust.NewDust(npc.position, npc.width, npc.height, 16, hitDirection, -1f, 0, default, 1f);
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ModContent.ItemType<EssenceofCinder>(), 2, 3);
            DropHelper.DropItemChance(npc, ModContent.ItemType<EyeoftheStorm>(), Main.expertMode ? 3 : 4);
            DropHelper.DropItemChance(npc, ModContent.ItemType<StormSaber>(), 5);
        }
    }
}
