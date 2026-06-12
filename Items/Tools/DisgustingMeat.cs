using System;
using CalamityMod.Effects;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Potions.Food;
using CalamityMod.Packets;
using CalamityMod.Packets.Worlds;
using CalamityMod.Particles;
using CalamityMod.Systems.Graphic.PixelationSystem;
using CalamityMod.Utilities.Daybreak;
using CalamityMod.Utilities.Daybreak.Buffers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class DisgustingMeat : ModItem, ILocalizedModType
    {
        private static Asset<Texture2D> SmallGreyscaleCircle;

        public new string LocalizationCategory => "Items.Tools";

        public override void Load()
        {
            if (!Main.dedServ)
            {
                SmallGreyscaleCircle = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/SmallGreyscaleCircle");
            }
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.FoodParticleColors[Type] = new Color[6] {
                new Color(61, 69, 41),
                new Color(91, 100, 48),
                new Color(122, 123, 67),
                new Color(128, 69, 46),
                new Color(183, 116, 80),
                new Color(95, 59, 46),
            };
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(26, 36, 0, 0);
            Item.value = 0;
            Item.UseSound = SoundID.NPCHit20;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            DisgustingMeatPlayer modPlayer = player.GetModPlayer<DisgustingMeatPlayer>();
            return !modPlayer.DoingVomitAnimation;
        }

        public override bool? UseItem(Player player)
        {
            if (player.ItemTimeIsZero)
            {
                DisgustingMeatPlayer modPlayer = player.GetModPlayer<DisgustingMeatPlayer>();
                if (player.altFunctionUse == 2)
                    modPlayer.EjectMiscUpgrades = true;
                modPlayer.DoingVomitAnimation = true;

                return true;
            }

            return null;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            // Stink particles and flies coming off of the item while it's in the world (yuck!!) ((:sick:))
            if (Main.rand.NextBool(25))
            {
                int stinkFumesAmt = Main.rand.Next(1, 3);
                for (int i = 0; i < stinkFumesAmt; i++)
                {
                    Vector2 fumesSpawnPosition = Item.Center + Main.rand.NextVector2Circular(Item.width - 8, Item.height - 8);
                    Color fumesInitialColor = Color.Lerp(Color.OliveDrab, Color.DarkGreen, Main.rand.NextFloat());
                    Color fumesFadeColor = Color.Lerp(fumesInitialColor, Color.GhostWhite, Main.rand.NextFloat(0.3f, 0.5f));
                    TimedSmokeParticle fumes = new(fumesSpawnPosition, Vector2.Zero, fumesInitialColor, fumesFadeColor, Main.rand.NextFloat(0.15f, 0.2f), Main.rand.NextFloat(0.4f, 0.6f), Main.rand.Next(45, 60), 0.002f * Main.rand.NextBool().ToDirectionInt(), true);
                    GeneralParticleHandler.QueueParticleForNextFrame(fumes);
                }
            }

            if (Main.rand.NextBool(150))
            {
                Vector2 flyGroupSpawnPosition = Item.Top + new Vector2(Main.rand.NextFloat(-16f, 16f), Main.rand.NextFloat(-8f, 0f));
                int fliesAmt = Main.rand.Next(1, 4);
                for (int i = 0; i < fliesAmt; i++)
                {
                    Vector2 flySpawnPosition = flyGroupSpawnPosition + Main.rand.NextVector2Circular(8f, 8f);
                    float flyScale = Main.rand.NextFloat(0.8f, 1f);
                    int flyLifetime = Main.rand.Next(360, 480);
                    FlyParticle fly = new(flySpawnPosition, flyScale, flyLifetime, Item);
                    GeneralParticleHandler.QueueParticleForNextFrame(fly);
                }
            }

            // Stink lines.
            spriteBatch.End(out var snapshot);

            using var pixelationLease = RenderTargetPool.Shared.Rent(Main.graphics.GraphicsDevice, Main.screenWidth / 2, Main.screenHeight / 2, RenderTargetDescriptor.Default);
            using (pixelationLease.Scope(clearColor: Color.Transparent))
            {
                Effect sineWaveDistortion = CalamityShaders.SineWaveDistortionShader.Value;
                sineWaveDistortion.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 6f + whoAmI);
                sineWaveDistortion.Parameters["waveAmplitude"].SetValue(0.25f);
                sineWaveDistortion.Parameters["waveFrequency"].SetValue(20f);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, sineWaveDistortion, PixelationManager.PixelationMatrix);

                Main.GetItemDrawFrame(Item.type, out _, out Rectangle itemFrame);
                Vector2 baseItemDrawOrigin = itemFrame.Size() * 0.5f;
                Vector2 stinkLineScale = new(0.125f, 0.6f);

                int stinkLineAmt = 2;
                for (int i = 0; i < stinkLineAmt; i++)
                {
                    float riseAndFall = MathHelper.Lerp(4f, -4f, MathF.Sin((float)Main.timeForVisualEffects / 120f + whoAmI + (i * 14)) * 0.5f + 0.5f);
                    Vector2 stinkLineDrawPosition = Item.Bottom + new Vector2(Utils.Remap(i, 0, stinkLineAmt - 1, -8f, 8f, true) - 4f, -24f + riseAndFall);
                    spriteBatch.Draw(SmallGreyscaleCircle.Value, stinkLineDrawPosition - Main.screenPosition - new Vector2(0f, baseItemDrawOrigin.Y), null, Color.DarkOliveGreen * 0.82f, 0f, SmallGreyscaleCircle.Size() * 0.5f, stinkLineScale * 0.7f * Item.scale, 0, 0f);
                }

                spriteBatch.End();
            }

            spriteBatch.Begin(snapshot);
            spriteBatch.Draw(pixelationLease.Target, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 2f, 0, 0f);
        }
    }

    public class DisgustingMeatPlayer : ModPlayer
    {
        public static int VomitEjectTime => 75;

        public static int VomitMaxTime => 130;

        public bool DoingVomitAnimation = false;

        public bool EjectMiscUpgrades = false;

        public int VomitTime = 0;

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) => DisgustingMeatPlayerSyncPacket.Send(this, toWho, fromWho);

        public override void CopyClientState(ModPlayer targetCopy)
        {
            DisgustingMeatPlayer clientClone = (DisgustingMeatPlayer)targetCopy;
            clientClone.DoingVomitAnimation = DoingVomitAnimation;
            clientClone.EjectMiscUpgrades = EjectMiscUpgrades;
            clientClone.VomitTime = VomitTime;
            clientClone.Player.eyeHelper.CurrentEyeFrame = Player.eyeHelper.CurrentEyeFrame;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            DisgustingMeatPlayer clientClone = (DisgustingMeatPlayer)clientPlayer;
            if (clientClone.DoingVomitAnimation != DoingVomitAnimation)
                DisgustingMeatPlayerSyncPacket.Send(this);
        }

        public override void UpdateDead()
        {
            DoingVomitAnimation = false;
            EjectMiscUpgrades = false;
            VomitTime = 0;
        }

        public override void PostUpdateMiscEffects()
        {
            if (Player.whoAmI == Main.myPlayer && DoingVomitAnimation)
            {
                // Eject all necessary items at once.
                if (VomitTime == VomitEjectTime)
                {
                    EjectPermanentUpgrades();
                    SoundEngine.PlaySound(SoundID.NPCDeath13, Player.Center);
                }

                if (VomitTime >= VomitMaxTime)
                {
                    VomitTime = 0;
                    DoingVomitAnimation = false;
                    EjectMiscUpgrades = false;
                }

                VomitTime++;
            }

            // Vomit particles bleeehhhhggghghghgh
            if (VomitTime >= VomitEjectTime && VomitTime <= VomitMaxTime)
            {
                if (Main.rand.NextBool(2))
                {
                    int dustAmt = Main.rand.Next(2, 5);
                    for (int i = 0; i < dustAmt; i++)
                    {
                        int dustType = Utils.SelectRandom(Main.rand, DustID.ToxicBubble, DustID.GreenBlood, DustID.Blood);
                        Vector2 spawnPosition = Player.Center + new Vector2(9f * Player.direction, -8f);
                        Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.ToRadians(20f) + Player.headRotation) * Main.rand.NextFloat(6f, 8f) * Player.direction;
                        Dust.NewDust(spawnPosition, 1, 1, dustType, velocity.X, velocity.Y, Scale: Main.rand.NextFloat(0.8f, 1.2f));
                    }
                }

                if (Main.rand.NextBool(3))
                {
                    Vector2 spawnPosition = Player.Center - new Vector2(9f * Player.direction, 8f);
                    Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.ToRadians(20f) + Player.headRotation) * Main.rand.NextFloat(6f, 8f) * Player.direction;
                    Color color = Color.Lerp(Color.DarkOliveGreen, Color.Green, Main.rand.NextFloat());
                    float rotationSpeed = Main.rand.NextFloat(0.01f, 0.03f) * Main.rand.NextBool().ToDirectionInt();

                    TimedSmokeParticle vomit = new(spawnPosition, velocity, color, color, Main.rand.NextFloat(0.3f, 0.5f), Main.rand.NextFloat(0.8f, 1f), Main.rand.Next(30, 45), rotationSpeed);
                    GeneralParticleHandler.SpawnParticle(vomit, true);
                }
            }
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            if (DoingVomitAnimation)
            {
                // Rapidly shift between random angles while vomiting.
                float interpolant = Utils.GetLerpValue(0f, VomitEjectTime, VomitTime, true) * Utils.GetLerpValue(VomitMaxTime, VomitMaxTime - 15, VomitTime, true);
                float idealRotationDegrees = MathHelper.Lerp(0f, 15f, interpolant);
                Player.headRotation = MathHelper.ToRadians(Main.rand.NextFloat(-idealRotationDegrees, idealRotationDegrees));

                // Close the eyes as well.
                Player.eyeHelper.CurrentEyeFrame = Terraria.GameContent.PlayerEyeHelper.EyeFrame.EyeHalfClosed;
                if (VomitTime >= VomitEjectTime)
                    Player.eyeHelper.CurrentEyeFrame = Terraria.GameContent.PlayerEyeHelper.EyeFrame.EyeClosed;
            }
        }

        private void EjectPermanentUpgrades()
        {
            var calPlayer = Player.Calamity();
            if (EjectMiscUpgrades)
            {
                // Rage and Adrenaline upgrades.
                TryDropBoosterItem(ref calPlayer.rageBoostOne, ModContent.ItemType<MushroomPlasmaRoot>());
                TryDropBoosterItem(ref calPlayer.rageBoostTwo, ModContent.ItemType<InfernalBlood>());
                TryDropBoosterItem(ref calPlayer.rageBoostThree, ModContent.ItemType<RedLightningContainer>());
                TryDropBoosterItem(ref calPlayer.adrenalineBoostOne, ModContent.ItemType<ElectrolyteGelPack>());
                TryDropBoosterItem(ref calPlayer.adrenalineBoostTwo, ModContent.ItemType<StarlightFuelCell>());
                TryDropBoosterItem(ref calPlayer.adrenalineBoostThree, ModContent.ItemType<Ectoheart>());

                // Celestal Onion.
                TryDropBoosterItem(ref calPlayer.extraAccessoryML, ModContent.ItemType<CelestialOnion>());

                // Demon Heart.
                TryDropBoosterItem(ref Player.extraAccessory, ItemID.DemonHeart);

                // Shimmer upgrades.
                if (Player.usedAegisCrystal || Player.usedAegisFruit || Player.usedArcaneCrystal || Player.usedAmbrosia || Player.usedGalaxyPearl || Player.usedGummyWorm || NPC.peddlersSatchelWasUsed || NPC.combatBookWasUsed || NPC.combatBookVolumeTwoWasUsed)
                {
                    TryDropBoosterItem(ref Player.usedAegisCrystal, ItemID.AegisCrystal);
                    TryDropBoosterItem(ref Player.usedAegisFruit, ItemID.AegisFruit);
                    TryDropBoosterItem(ref Player.usedArcaneCrystal, ItemID.ArcaneCrystal);
                    TryDropBoosterItem(ref Player.usedAmbrosia, ItemID.Ambrosia);
                    TryDropBoosterItem(ref Player.usedGalaxyPearl, ItemID.GalaxyPearl);
                    TryDropBoosterItem(ref Player.usedGummyWorm, ItemID.GummyWorm);

                    // Sync the world data immediately for these three specifically since they aren't fully player side.
                    bool hadSatchel = NPC.peddlersSatchelWasUsed;
                    TryDropBoosterItem(ref NPC.peddlersSatchelWasUsed, ItemID.PeddlersSatchel);
                    if (hadSatchel && Main.netMode != NetmodeID.SinglePlayer)
                        DisableNPCUpgradesSyncPacket.Send(0);

                    bool hadCombatBook = NPC.combatBookWasUsed;
                    TryDropBoosterItem(ref NPC.combatBookWasUsed, ItemID.CombatBook);
                    if (hadCombatBook && Main.netMode != NetmodeID.SinglePlayer)
                        DisableNPCUpgradesSyncPacket.Send(1);

                    bool hadCombatBookTwo = NPC.combatBookVolumeTwoWasUsed;
                    TryDropBoosterItem(ref NPC.combatBookVolumeTwoWasUsed, ItemID.CombatBookVolumeTwo);
                    if (hadCombatBookTwo && Main.netMode != NetmodeID.SinglePlayer)
                        DisableNPCUpgradesSyncPacket.Send(2);
                }

                // Artisan Loaf.
                TryDropBoosterItem(ref Player.ateArtisanBread, ItemID.ArtisanLoaf);
            }
            else
            {
                // Calamity health booster fruits.
                if (calPlayer.sTangerine || calPlayer.mFruit || calPlayer.tCloudberry || calPlayer.sStrawberry)
                {
                    TryDropBoosterItem(ref calPlayer.sTangerine, ModContent.ItemType<SanguineTangerine>());
                    TryDropBoosterItem(ref calPlayer.mFruit, ModContent.ItemType<MiracleFruit>());
                    TryDropBoosterItem(ref calPlayer.tCloudberry, ModContent.ItemType<TaintedCloudberry>());
                    TryDropBoosterItem(ref calPlayer.sStrawberry, ModContent.ItemType<SacredStrawberry>());
                }

                // Life fruit.
                if (Player.ConsumedLifeFruit > 0)
                {
                    for (int i = 0; i < Player.ConsumedLifeFruit; i++)
                    {
                        int drop = Player.QuickSpawnItem(Player.GetSource_DropAsItem(), ItemID.LifeFruit);
                        Main.item[drop].noGrabDelay = 100;
                        Main.item[drop].velocity = new Vector2(Main.rand.NextFloat(3f, 9f) * Player.direction, Main.rand.NextFloat(-6f, -4f));
                    }
                    Player.ConsumedLifeFruit = 0;
                }

                // Heart crystals.
                if (Player.ConsumedLifeCrystals > 0)
                {
                    for (int j = 0; j < Player.ConsumedLifeCrystals; j++)
                    {
                        int drop = Player.QuickSpawnItem(Player.GetSource_DropAsItem(), ItemID.LifeCrystal);
                        Main.item[drop].noGrabDelay = 100;
                        Main.item[drop].velocity = new Vector2(Main.rand.NextFloat(3f, 9f) * Player.direction, Main.rand.NextFloat(-6f, -4f));
                    }
                    Player.ConsumedLifeCrystals = 0;
                }

                // Calamity mana upgrades.
                if (calPlayer.cShard || calPlayer.eCore || calPlayer.pHeart)
                {
                    TryDropBoosterItem(ref calPlayer.cShard, ModContent.ItemType<CometShard>());
                    TryDropBoosterItem(ref calPlayer.eCore, ModContent.ItemType<EtherealCore>());
                    TryDropBoosterItem(ref calPlayer.pHeart, ModContent.ItemType<PhantomHeart>());
                }

                // Mana Crystals.
                if (Player.ConsumedManaCrystals > 0)
                {
                    for (int k = 0; k < Player.ConsumedManaCrystals; k++)
                    {
                        int drop = Player.QuickSpawnItem(Player.GetSource_DropAsItem(), ItemID.ManaCrystal);
                        Main.item[drop].noGrabDelay = 100;
                        Main.item[drop].velocity = new Vector2(Main.rand.NextFloat(3f, 9f) * Player.direction, Main.rand.NextFloat(-6f, -4f));
                    }
                    Player.ConsumedManaCrystals = 0;
                }
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.SyncPlayer, -1, -1, null, Player.whoAmI);
                DisgustingMeatPlayerSyncPacket.Send(this);
            }
        }

        private void TryDropBoosterItem(ref bool condition, int itemType)
        {
            if (condition)
            {
                condition = false;
                int drop = Player.QuickSpawnItem(Player.GetSource_DropAsItem(), itemType);
                Main.item[drop].noGrabDelay = 100;
                Main.item[drop].velocity = new Vector2(Main.rand.NextFloat(3f, 9f) * Player.direction, Main.rand.NextFloat(-6f, -4f));
            }
        }
    }
}
