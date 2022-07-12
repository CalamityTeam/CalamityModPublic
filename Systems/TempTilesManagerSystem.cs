using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using CalamityMod.Items.Tools;
using CalamityMod.Systems;
using Terraria;
using CalamityMod.Items.Materials;
using System.Linq;
using Terraria.ID;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.Graphics.Effects;
using CalamityMod.Tiles;

namespace CalamityMod.Systems
{
    /// <summary>
    /// Useful for temporary tiles that may want to use custom drawing effects with a constant high update rate idependent of the lightning mode
    /// </summary>
    public interface ISpecialTempTileDraw
    {
        public void CoolDraw(int i, int j, SpriteBatch spriteBatch);
    }

    public abstract class TemporaryTileManager
    {
        /// <summary>
        /// All the tile types managed by this manager.
        /// </summary>
        public abstract int[] ManagedTypes { get; }

        /// <summary>
        /// Called every frame. Use this to create special effects or update stuff
        /// </summary>
        /// <param name="tile"></param>
        public virtual void UpdateEffect(TemporaryTile tile) { }

        /// <summary>
        /// Creates a temporary tile
        /// </summary>
        /// <param name="tile"></param>
        public abstract TemporaryTile Setup(Point pos);

        /// <summary>
        /// Called when a tile runs out of time
        /// </summary>
        public virtual void EndEffect(TemporaryTile tile) { }
    }

    public struct TemporaryTile
    {
        public Point position;
        public int timeleft;
        public TemporaryTileManager manager;

        public TemporaryTile(Point pos, TemporaryTileManager manager_, int timeLeft = 60)
        {
            position = pos;
            manager = manager_;
            timeleft = timeLeft;
        }
    }

    public class TempTilesManagerSystem : ModSystem
    {
        public static int[] TemporaryTileIDs;

        public static List<TemporaryTile> ManagedTiles = new List<TemporaryTile>();
        public static List<TemporaryTile> DeletableTiles = new List<TemporaryTile>();

        public override void Load()
        {
            ManagedTiles = new List<TemporaryTile>();
            DeletableTiles = new List<TemporaryTile>();
        }

        public override void Unload()
        {
            ManagedTiles = null;
            DeletableTiles = null;
        }
        public override void PostAddRecipes()
        {
            TemporaryTileIDs = new int[] { WulfrumScaffoldKit.PlacedTileType };
        }

        /// <summary>
        /// Adds a temporary tile at the specified position with the specified tile manager
        /// The temporary tile manager is used to manage all the effects and placement of the temporary tile.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="manager"></param>
        public static void AddTemporaryTile(Point position, TemporaryTileManager manager)
        {
            TemporaryTile tile = manager.Setup(position);
            ManagedTiles.Add(tile);
        }

        /// <summary>
        /// Requests the time left for a temporary tile at a specified position.
        /// </summary>
        /// <param name="position">The position of the temporary tile</param>
        /// <returns></returns>
        public static int GetTemporaryTileTime(Point position)
        {
            return ManagedTiles.Find(t => t.position == position).timeleft;
        }

        public override void OnWorldUnload()
        {
            ManagedTiles = new List<TemporaryTile>();
            DeletableTiles = new List<TemporaryTile>();
        }

        public override void PostDrawTiles()
        {
            if (ManagedTiles.Count <= 0)
                return;

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            foreach (TemporaryTile tile in ManagedTiles)
            {
                if (TileLoader.GetTile(Main.tile[tile.position].TileType) is ISpecialTempTileDraw coolTile)
                    coolTile.CoolDraw(tile.position.X, tile.position.Y, Main.spriteBatch);
            }

            Main.spriteBatch.End();
        }

        public override void PostUpdateEverything()
        {
            for (int i = 0; i < ManagedTiles.Count; i++)
            {
                TemporaryTile tile = ManagedTiles[i];
                TemporaryTileManager manager = tile.manager;

                if (!manager.ManagedTypes.Contains(Main.tile[tile.position].TileType))
                {
                    DeletableTiles.Add(tile);
                    continue;
                }

                manager.UpdateEffect(tile);
                tile.timeleft--;

                if (tile.timeleft < 0)
                {
                    manager.EndEffect(tile);
                    WorldGen.KillTile(tile.position.X, tile.position.Y);
                    NetMessage.SendTileSquare(-1, tile.position.X, tile.position.Y, TileChangeType.None);
                    DeletableTiles.Add(tile);
                }

                ManagedTiles[i] = tile;
            }

            foreach (TemporaryTile deletableTile in DeletableTiles)
                ManagedTiles.Remove(deletableTile);

            DeletableTiles.Clear();
        }
    }
}
