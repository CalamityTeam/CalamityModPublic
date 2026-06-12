using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalamityMod.DataStructures;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Particles;
using CalamityMod.Utilities.Daybreak;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles
{
    public class GreedPotTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileObsidianKill[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);

            ModTileEntity te = ModContent.GetInstance<GreedPotTE>();
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(te.Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            DustType = DustID.Iron;

            AddMapEntry(new Color(250, 142, 4));
        }

        public override void MouseOver(int i, int j)
        {
            List<int> availableOres = GreedTransmutation.GreedChain.Where(p => p.Value.Availability.Invoke()).Select(p => p.Key).ToList();

            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = availableOres[(int)((Main.GlobalTimeWrappedHourly * 2) % availableOres.Count)];
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (TileEntity.TryGet<GreedPotTE>(new(i, j), out var greedPotTE) && greedPotTE.Activated && Main.LocalPlayer.miscCounter % 5 == 0)
            {
                Tile tile = Main.tile[i, j];

                int left = i - tile.TileFrameX % (3 * 18) / 18;
                if (left != i)
                    return;
                int top = j - tile.TileFrameY % (3 * 18) / 18;
                if (top != j)
                    return;

                Point position = new(left, top);
                HeavySmokeParticle p = new(position.ToWorldCoordinates(24 + Main.rand.Next(-10, 11), 2), Vector2.UnitY * -2, Color.Crimson, 90, Main.rand.NextFloat(0.2f, 0.3f), 0.75f, Main.rand.NextFloat(-0.05f, 0.05f), true);
                GeneralParticleHandler.SpawnParticle(p);
            }
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            if (TileEntity.TryGet<GreedPotTE>(new(i, j), out var greedPotTE))
                frameXOffset = 18 * 3 * greedPotTE.MyFrame;
        }
    }

    public class GreedPotTE : ModTileEntity
    {
        internal bool Activated = false;
        internal ushort TimeSinceActivation = 0;
        private ushort FrameTime = 0;
        private byte CurrentFrame = 0;
        public int MyFrame => (int)CurrentFrame;

        private const byte FullActiveFrame = 4;
        private const byte FrameRate = 3;
        private const byte FrameTotal = 8;

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];

            if (!tile.HasTile || tile.TileType != ModContent.TileType<GreedPotTile>())
                return false;

            int style = 0, alt = 0;
            TileObjectData.GetTileInfo(tile, ref style, ref alt);
            TileObjectData data = TileObjectData.GetTileData(tile.TileType, style, alt);

            if (data == null)
                return false;

            int sheetSquare = 16 + data.CoordinatePadding;
            int FrameX = tile.TileFrameX / sheetSquare % data.Width;
            int FrameY = tile.TileFrameY / sheetSquare % data.Height;

            return tile.HasTile && tile.TileType == ModContent.TileType<GreedPotTile>() && FrameX == 0 && FrameY == 0;
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 3, 3);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type);
                return -1;
            }
            return Place(i, j);
        }

        public override void OnNetPlace() => NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);

        public void UpdateOnAllClients()
        {
            if (Activated)
            {
                if (CurrentFrame != 3)
                {
                    FrameTime++;
                    if (FrameTime % FrameRate == 0)
                    {
                        FrameTime = 0;
                        CurrentFrame = (byte)((CurrentFrame + 1) % FrameTotal);
                    }
                }

                if (TimeSinceActivation >= 150)
                {
                    Activated = false;
                    TimeSinceActivation = 0;
                    return;
                }

                TimeSinceActivation++;
            }
            else
            {
                if (CurrentFrame != 0)
                {
                    FrameTime++;
                    if (FrameTime % FrameRate == 0)
                    {
                        FrameTime = 0;
                        CurrentFrame = (byte)((CurrentFrame + 1) % FrameTotal);
                    }
                }
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(Activated);
            writer.Write(FrameTime);
            writer.Write(TimeSinceActivation);
            writer.Write(CurrentFrame);
        }
        public override void NetReceive(BinaryReader reader)
        {
            Activated = reader.ReadBoolean();
            FrameTime = reader.ReadUInt16();
            TimeSinceActivation = reader.ReadUInt16();
            CurrentFrame = reader.ReadByte();
        }
    }

    //Mod Tile Entity's Update functions don't get called on multiplayer clients
    public class GreedPotTEUpdateSystem : ModSystem
    {
        public override void PostUpdateProjectiles()
        {
            foreach (TileEntity te in TileEntity.ByID.Values)
            {
                if (te is not GreedPotTE greedTE)
                    continue;

                greedTE.UpdateOnAllClients();
            }
        }
    }
    
    public class GreedTransmutation : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            static bool twoMechsDowned() =>
                NPC.downedMechBoss1 && NPC.downedMechBoss2 && !NPC.downedMechBoss3 ||
                NPC.downedMechBoss2 && NPC.downedMechBoss3 && !NPC.downedMechBoss1 ||
                NPC.downedMechBoss3 && NPC.downedMechBoss1 && !NPC.downedMechBoss2 ||
                NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3;

            GreedChain = new()
            {
                { ItemID.CopperOre, (ItemID.IronOre, () => true) },
                { ItemID.IronOre, (ItemID.SilverOre, () => true) },
                { ItemID.SilverOre, (ItemID.GoldOre, () => true) },
                { ItemID.TinOre, (ItemID.LeadOre, () => true) },
                { ItemID.LeadOre, (ItemID.TungstenOre, () => true) },
                { ItemID.TungstenOre, (ItemID.PlatinumOre, () => true) },
                { ItemID.GoldOre, (ModContent.ItemType<AerialiteOre>(), () => DownedBossSystem.downedHiveMind || DownedBossSystem.downedPerforator) },
                { ItemID.PlatinumOre, (ModContent.ItemType<AerialiteOre>(), () => DownedBossSystem.downedHiveMind || DownedBossSystem.downedPerforator) },
                { ModContent.ItemType<AerialiteOre>(), (ItemID.PalladiumOre, () => Main.hardMode) },
                { ItemID.CobaltOre, (ItemID.MythrilOre, () => CalamityServerConfig.Instance.EarlyHardmodeProgressionRework ? NPC.downedMechBossAny : Main.hardMode) },
                { ItemID.MythrilOre, (ItemID.AdamantiteOre, () => CalamityServerConfig.Instance.EarlyHardmodeProgressionRework ? twoMechsDowned() : Main.hardMode) },
                { ItemID.PalladiumOre, (ItemID.OrichalcumOre, () => CalamityServerConfig.Instance.EarlyHardmodeProgressionRework ? NPC.downedMechBossAny : Main.hardMode) },
                { ItemID.OrichalcumOre, (ItemID.TitaniumOre, () => CalamityServerConfig.Instance.EarlyHardmodeProgressionRework ? twoMechsDowned() : Main.hardMode) },
                { ItemID.AdamantiteOre, (ModContent.ItemType<HallowedOre>(), () => CalamityServerConfig.Instance.EarlyHardmodeProgressionRework ? NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3 : NPC.downedMechBossAny) },
                { ItemID.TitaniumOre, (ModContent.ItemType<HallowedOre>(), () => CalamityServerConfig.Instance.EarlyHardmodeProgressionRework ? NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3 : NPC.downedMechBossAny) },
                { ModContent.ItemType<HallowedOre>(), (ItemID.ChlorophyteOre, () => NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3) },
                { ItemID.ChlorophyteOre, (ModContent.ItemType<PerennialOre>(), () => NPC.downedPlantBoss) },
                { ModContent.ItemType<PerennialOre>(), (ModContent.ItemType<ScoriaOre>(), () => NPC.downedAncientCultist) },
                { ModContent.ItemType<ScoriaOre>(), (ItemID.LunarOre, () => NPC.downedMoonlord) },
                { ItemID.LunarOre, (ModContent.ItemType<ExodiumCluster>(), () => NPC.downedMoonlord) },
                { ModContent.ItemType<ExodiumCluster>(), (ModContent.ItemType<UelibloomOre>(), () => DownedBossSystem.downedProvidence) },
                { ModContent.ItemType<UelibloomOre>(), (ModContent.ItemType<AuricOre>(), () => DownedBossSystem.downedCalamitas || DownedBossSystem.downedExoMechs) },
            };
        }

        internal static Dictionary<int, (int Result, Func<bool> Availability)> GreedChain;

        private int InputItemType => (int)Projectile.ai[0];
        private int SuccessOutputItemType => (int)Projectile.ai[1];

        private Vector2 PotPosition = Vector2.Zero;
        private Vector2 StartPosition = Vector2.Zero;

        BezierCurve path = null;
        private bool Success = false;

        private int random = -1;

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
        }

        public override void OnSpawn(IEntitySource source)
        {
            StartPosition = Projectile.Center;
            PotPosition = Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
            Success = Main.rand.NextBool(3);
            Projectile.netUpdate = true;
        }

        public override bool PreAI()
        {
            if (Projectile.timeLeft == 120)
            {
                GreedPotTE te = TileEntity.ByID[(int)Projectile.ai[2]] as GreedPotTE;
                te.Activated = true;
                te.TimeSinceActivation = 0;
            }
            return true;
        }

        public override void AI()
        {
            if (path == null)
            {
                Vector2 startPoint = StartPosition;
                Vector2 endPoint = PotPosition - Vector2.UnitY * 64;

                Vector2 direction = endPoint - startPoint;
                float curveIntensity = Main.rand.NextFloat(0.2f, 0.4f);
                Vector2 perpindicular = direction.RotatedBy(MathHelper.PiOver2);
                if (direction.X > 0)
                    curveIntensity *= -1;

                Vector2 controlPoint1 = startPoint + (direction * 0.25f) + (perpindicular * curveIntensity);
                Vector2 controlPoint2 = startPoint + (direction * 0.75f) + (perpindicular * curveIntensity);

                path = new BezierCurve(startPoint, controlPoint1, controlPoint2, endPoint);
            }

            float lerp = 1 - MathHelper.Clamp(((Projectile.timeLeft - 60) / 60f), 0f, 1f);

            Projectile.Center = path.Evaluate(CalamityUtils.CircOutEasing(lerp, 1));
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                int i = Item.NewItem(Projectile.GetItemSource_DropAsItem(), Projectile.Center, Success ? SuccessOutputItemType : ItemID.StoneBlock);
                if (i >= 0)
                {
                    Main.item[i].GetGlobalItem<GreedTransmutationGlobalItem>().FromGreedPot = true;
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int animTime = 120 - Projectile.timeLeft;
            bool beforeDarken = animTime < 50;
            bool darken = animTime < 90;
            bool transform = animTime < 110;

            if (random == -1)
                random = Main.rand.Next(120);

            if (beforeDarken)
            {
                Texture2D tex = TextureAssets.Item[InputItemType].Value;
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition + (Vector2.UnitY * MathF.Sin(Main.GlobalTimeWrappedHourly + random) * 8), null, lightColor, Projectile.rotation, tex.Size() * 0.5f, 1f, 0, 0);
            }
            else if (darken)
            {
                Main.spriteBatch.End(out var snap);

                var newSnap = snap with { BlendState = BlendState.Additive };
                Main.spriteBatch.Begin(newSnap);

                float glowLerp = MathHelper.Clamp((animTime - 45) / 10f, 0f, 1f);
                glowLerp = CalamityUtils.SineInEasing(glowLerp, 1);

                Texture2D glow = ModContent.Request<Texture2D>("CalamityMod/Particles/SmallBloom").Value;
                Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition + (Vector2.UnitY * MathF.Sin(Main.GlobalTimeWrappedHourly + random) * 8), null, Color.Crimson * glowLerp, Projectile.rotation, glow.Size() * 0.5f, MathHelper.Lerp(0.05f, 0.15f, glowLerp), 0, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(snap);

                float darkenLerp = MathHelper.Clamp((animTime - 50) / 10f, 0f, 1f);
                darkenLerp = CalamityUtils.CircInEasing(darkenLerp, 1);

                Texture2D tex = TextureAssets.Item[InputItemType].Value;
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition + (Vector2.UnitY * MathF.Sin(Main.GlobalTimeWrappedHourly + random) * 8), null, Color.Lerp(lightColor, Color.Black, darkenLerp), Projectile.rotation, tex.Size() * 0.5f, 1f, 0, 0);
            }
            else if(transform)
            {
                Main.spriteBatch.End(out var snap);

                var newSnap = snap with { BlendState = BlendState.Additive };
                Main.spriteBatch.Begin(newSnap);

                Texture2D glow = ModContent.Request<Texture2D>("CalamityMod/Particles/SmallBloom").Value;
                Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition + (Vector2.UnitY * MathF.Sin(Main.GlobalTimeWrappedHourly + random) * 8), null, Color.Crimson, Projectile.rotation, glow.Size() * 0.5f, 0.15f, 0, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(snap);

                Texture2D inputTex = TextureAssets.Item[InputItemType].Value;
                Texture2D outputTex = TextureAssets.Item[Success ? SuccessOutputItemType : ItemID.StoneBlock].Value;
                float transformLerp = MathHelper.Clamp((animTime - 90) / 10f, 0f, 1f);
                //transformLerp = CalamityUtils.SineInOutEasing(transformLerp, 1);

                Main.spriteBatch.Draw(inputTex, Projectile.Center - Main.screenPosition + (Vector2.UnitY * MathF.Sin(Main.GlobalTimeWrappedHourly + random) * 8), null, Color.Black, Projectile.rotation, inputTex.Size() * 0.5f, 1 - transformLerp, 0, 0);
                Main.spriteBatch.Draw(outputTex, Projectile.Center - Main.screenPosition + (Vector2.UnitY * MathF.Sin(Main.GlobalTimeWrappedHourly + random) * 8), null, Color.Black, Projectile.rotation, inputTex.Size() * 0.5f, transformLerp, 0, 0);
            }
            else
            {
                float undarkenLerp = MathHelper.Clamp((animTime - 110) / 10f, 0f, 1f);
                undarkenLerp = CalamityUtils.SineOutEasing(undarkenLerp, 1);

                Main.spriteBatch.End(out var snap);

                var newSnap = snap with { BlendState = BlendState.Additive };
                Main.spriteBatch.Begin(newSnap);

                Texture2D glow = ModContent.Request<Texture2D>("CalamityMod/Particles/SmallBloom").Value;
                Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition + (Vector2.UnitY * MathF.Sin(Main.GlobalTimeWrappedHourly + random) * 8), null, Color.Crimson * (1 - undarkenLerp), Projectile.rotation, glow.Size() * 0.5f, MathHelper.Lerp(0.05f, 0.15f, (1 - undarkenLerp)), 0, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(snap);

                Texture2D tex = TextureAssets.Item[Success ? SuccessOutputItemType : ItemID.StoneBlock].Value;
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition + (Vector2.UnitY * MathF.Sin(Main.GlobalTimeWrappedHourly + random) * 8), null, Color.Lerp(Color.Black, lightColor, undarkenLerp), Projectile.rotation, tex.Size() * 0.5f, 1f, 0, 0);
            }
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(StartPosition);
            writer.WriteVector2(PotPosition);
            writer.Write(Success);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            StartPosition = reader.ReadVector2();
            PotPosition = reader.ReadVector2();
            Success = reader.ReadBoolean();
        }
    }

    public class GreedTransmutationGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        internal bool FromGreedPot = false;

        public override bool? UseItem(Item item, Player player)
        {
            if (!GreedTransmutation.GreedChain.TryGetValue(item.type, out var transmutation))
                return null;

            if (!transmutation.Availability())
                return null;

            Point mouseTile = Main.MouseWorld.ToTileCoordinates();

            GreedPotTE greedPotTE = CalamityUtils.FindTileEntity<GreedPotTE>(mouseTile.X, mouseTile.Y, 3, 3, 18);
            if (greedPotTE == null)
                return null;

            if (!player.IsInTileInteractionRange(mouseTile.X, mouseTile.Y, TileReachCheckSettings.Simple))
                return null;

            if (Main.myPlayer == player.whoAmI)
                Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), player.Center, greedPotTE.Position.ToWorldCoordinates(24, 24) + Main.rand.NextVector2Circular(12, 32), ModContent.ProjectileType<GreedTransmutation>(), 0, 0, player.whoAmI, item.type, transmutation.Result, greedPotTE.ID);

            greedPotTE.Activated = true;
            greedPotTE.TimeSinceActivation = 0;
            player.ApplyItemTime(item, 0.5f, false);
            return false;
        }

        public override bool CanStackInWorld(Item destination, Item source)
        {
            return !destination.GetGlobalItem<GreedTransmutationGlobalItem>().FromGreedPot && !source.GetGlobalItem<GreedTransmutationGlobalItem>().FromGreedPot;
        }

        public override void NetSend(Item item, BinaryWriter writer)
        {
            writer.Write(FromGreedPot);
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            FromGreedPot = reader.ReadBoolean();
        }
    }
}
