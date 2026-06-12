using CalamityMod.Items.Placeables.Furniture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Furniture
{
    public class GluttonyBlenderTile : ModTile
    {
        public const int Width = 2;
        public const int Height = 3;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.CoordinateHeights = [16, 16, 18];
            ModTileEntity entity = ModContent.GetInstance<GluttonyBlenderTE>();
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(entity.Hook_AfterPlacement, -1, 0, true);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(128, 128, 128), CalamityUtils.GetItemName<GluttonyBlender>());
        }

        public override bool CreateDust(int i, int j, ref int type) => false;

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            if (TileEntity.TryGet<GluttonyBlenderTE>(new(i, j), out var entity))
            {
                frameXOffset = 36 * entity.CurrentFrame;
            }
        }

        public override void MouseOver(int i, int j)
        {
            Player p = Main.LocalPlayer;
            // You shouldn't be able to put the slop or something better than it back into the blender
            if (BuffID.Sets.IsWellFed[p.HeldItem.buffType] && !(p.HeldItem.buffType == BuffID.WellFed3 && p.HeldItem.buffTime >= CalamityUtils.MinutesToFrames(30)))
            {
                p.noThrow = 2;
                p.cursorItemIconEnabled = true;
                p.cursorItemIconID = p.HeldItem.type;
            }
        }
    }

    public class GluttonyBlenderTE : ModTileEntity
    {
        public Vector2 BlenderTop => Position.ToWorldCoordinates(8 * GluttonyBlenderTile.Width, 0f);
        public int CurrentFrame = 0;
        private ushort FrameCounter = 0;
        private const ushort TotalAnimFrames = 6;

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<GluttonyBlenderTile>() && tile.TileFrameX == 0 && tile.TileFrameY == 0;
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, GluttonyBlenderTile.Width, GluttonyBlenderTile.Height);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type);
                return -1;
            }
            return Place(i, j);
        }

        public override void OnNetPlace() => NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);
        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(CurrentFrame);
            writer.Write(FrameCounter);
        }
        public override void NetReceive(BinaryReader reader)
        {
            CurrentFrame = reader.ReadInt32();
            FrameCounter = reader.ReadUInt16();
        }

        public override void Update()
        {
            bool blenderAnim = Main.projectile.Any(i => i.active && i.type == ModContent.ProjectileType<GluttonyBlenderAnimation>() &&
                i.ai[1] >= GluttonyBlenderAnimation.TimeToReachBlender && Vector2.DistanceSquared(BlenderTop, i.Center) < 256f);
            if (blenderAnim)
            {
                if (CurrentFrame < 1)
                    CurrentFrame = 1;
                FrameCounter++;
                if (FrameCounter >= 4)
                {
                    FrameCounter = 0;
                    CurrentFrame++;
                    if (CurrentFrame > TotalAnimFrames)
                        CurrentFrame = 1;
                    
                    if (Main.dedServ)
                        NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);
                }
            }
            else
            {
                if (CurrentFrame > 0)
                {
                    CurrentFrame = 0;
                    FrameCounter = 0;

                    if (Main.dedServ)
                        NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);
                }
            }
        }
    }

    public class GluttonyBlenderGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        internal bool FromGluttonyBlender = false;

        public override bool CanUseItem(Item item, Player player)
        {
            // Exit early if anyone of the following are true:
            // * Running on client that is not the local player
            // * The item is not a food, is not consumable, or is Quality Slop/better than Quality Slop (you cannot feed the slop back into the blender)
            // * The Gluttony Blender tile entity doesn't exist for some reason
            // * The player isn't within the tile's interaction range
            if (Main.LocalPlayer.whoAmI != player.whoAmI)
                return true;
            if (!BuffID.Sets.IsWellFed[item.buffType] || !item.consumable || (item.buffType == BuffID.WellFed3 && item.buffTime >= CalamityUtils.MinutesToFrames(30)))
                return true;

            Point mouseTile = Main.MouseWorld.ToTileCoordinates();
            GluttonyBlenderTE entity = CalamityUtils.FindTileEntity<GluttonyBlenderTE>(mouseTile.X, mouseTile.Y, GluttonyBlenderTile.Width, GluttonyBlenderTile.Height);
            if (entity == null)
                return true;
            if (!player.IsInTileInteractionRange(mouseTile.X, mouseTile.Y, TileReachCheckSettings.Simple))
                return true;

            // Spawns a projectile to handle the visual animation of the food moving into the blender and the conversion to slop
            // The projectile doesn't use velocity, so the top of the blender tile is passed in as the velocity
            Projectile.NewProjectile(player.GetSource_TileInteraction(mouseTile.X, mouseTile.Y), player.Center, entity.BlenderTop, ModContent.ProjectileType<GluttonyBlenderAnimation>(),
                0, 0f, player.whoAmI, item.type);
            if (ItemLoader.ConsumeItem(item, player))
            {
                item.stack--;
                if (player.selectedItem == 58)
                    Main.mouseItem.stack--;
                if (item.stack <= 0)
                    item.TurnToAir();
            }
            return false;
        }

        public override bool CanStackInWorld(Item destination, Item source)
        {
            return !destination.GetGlobalItem<GluttonyBlenderGlobalItem>().FromGluttonyBlender || !destination.GetGlobalItem<GluttonyBlenderGlobalItem>().FromGluttonyBlender;
        }
    }

    public class GluttonyBlenderAnimation : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private const int Lifetime = 120;
        public const int TimeToReachBlender = 60;
        private int ItemType => (int)Projectile.ai[0];
        private ref float Timer => ref Projectile.ai[1];
        private ref float ArcVelocity => ref Projectile.ai[2];
        private Vector2 Start;
        private Vector2 Destination => Projectile.velocity;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = Lifetime;
            Projectile.scale = 0f;
        }

        public override bool ShouldUpdatePosition() => false;
        public override void AI()
        {
            // Initialization
            if (Timer == 0)
            {
                SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
                Start = Projectile.Center;
                ArcVelocity = Main.rand.NextFloat(2.2f, 4.4f);
            }

            if (Timer < TimeToReachBlender)
            {
                Vector2 moveDistBeforeArc = Destination - Start;
                // The second half of this applies an arcing motion as the projectile moves
                Projectile.Center += (moveDistBeforeArc / (float)TimeToReachBlender) - Vector2.UnitY * (ArcVelocity - (ArcVelocity / 30f * Timer));
                Projectile.rotation = moveDistBeforeArc.X * 0.005f;

                if (Timer < 4)
                    Projectile.scale += 0.25f;
                if (Timer >= TimeToReachBlender - 4)
                    Projectile.scale -= 0.25f;
            }
            else
            {
                Projectile.Center = Destination;
                if (Timer == TimeToReachBlender)
                    SoundEngine.PlaySound(SoundID.Item22, Projectile.Center);

                Color[] dustArray = ItemID.Sets.FoodParticleColors[ItemType];
                if (dustArray == null || dustArray.Length == 0)
                    dustArray = ItemID.Sets.DrinkParticleColors[ItemType];
                if (dustArray != null && dustArray.Length != 0 && Main.rand.NextBool(4))
                {
                    Vector2 dustVel = -Vector2.UnitY.RotatedByRandom(MathHelper.Pi / 5f) * 2f;
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.FoodPiece, dustVel, 0, dustArray[Main.rand.Next(dustArray.Length)], Main.rand.NextFloat(1.3f, 1.75f));
                    dust.fadeIn = 0f;
                }
            }

            if (Projectile.timeLeft == 1 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Item dummy = new Item();
                dummy.SetDefaults(ItemType);
                // 0.5% per minute of duration, multiplied by 1.5x for T@ food and 2x for T3 food
                float goodPercent = dummy.buffTime * (dummy.buffType == BuffID.WellFed3 ? 2f : dummy.buffType == BuffID.WellFed2 ? 1.5f : 1f) / 3600f / 2f;

                int itemDrop = Main.rand.NextFloat(100f) < goodPercent ? ModContent.ItemType<QualitySlop>() : ModContent.ItemType<DisgustingSlop>();
                int i = Item.NewItem(Projectile.GetItemSource_DropAsItem(), Projectile.Center, itemDrop);
                Main.item[i].GetGlobalItem<GluttonyBlenderGlobalItem>().FromGluttonyBlender = true;
            }
            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Timer >= TimeToReachBlender)
                return false;

            Texture2D tex = TextureAssets.Item[ItemType].Value;
            Rectangle frame = tex.Frame(1, Main.itemAnimations[ItemType].FrameCount, 0, 0);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}
