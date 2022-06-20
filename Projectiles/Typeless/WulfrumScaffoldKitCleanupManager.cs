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

namespace CalamityMod.Projectiles.Typeless
{
    public class WulfrumScaffoldKitCleanupManager : ModProjectile
    {
        //Probs need mp syncing

        public Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Scaffold Kit");
        }

        public List<Point> ManagedTiles = new List<Point>(); //Might need some cloning stuff for mp? idk
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = WulfrumScaffoldKit.TileTime;
        }

        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            if (Projectile.timeLeft < WulfrumScaffoldKit.TileTime * 0.1f)
            {
                int tileIndex = Main.rand.Next(ManagedTiles.Count);

                if (Main.tile[ManagedTiles[tileIndex]].TileType == WulfrumScaffoldKit.PlacedTileType)
                {
                    Vector2 dustpos = ManagedTiles[tileIndex].ToWorldCoordinates();
                    Dust.NewDustPerfect(dustpos, 226, Main.rand.NextVector2Circular(4f, 4f), Scale: Main.rand.NextFloat(0.4f, 1f));
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            foreach (Point pos in ManagedTiles)
            {
                if (Main.tile[pos].TileType == WulfrumScaffoldKit.PlacedTileType)
                {
                    WorldGen.KillTile(pos.X, pos.Y);
                    NetMessage.SendTileSquare(-1, pos.X, pos.Y, TileChangeType.None);
                }
            }
        }
    }
}
