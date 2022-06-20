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
using System;

namespace CalamityMod.Projectiles.Typeless
{
    public class WulfrumScaffoldKitHoldout : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];
        public WulfrumScaffoldKit Kit => Owner.HeldItem.ModItem as WulfrumScaffoldKit;
        public bool CanOwnerGoOn => Kit.storedScrap > 0 || Owner.HasItem(ModContent.ItemType<WulfrumShard>());

        public bool CanSelectTile(Point tilePos)
        {
            //You only need to check when starting a scaffold
            if (SelectedTiles.Count > 0)
                return CanSelectMoreTiles(tilePos);


            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (Main.tile[tilePos.X + i, tilePos.Y + j].IsTileFull())
                        return true;
                }
            }

            return false;
        }

        public bool CanSelectMoreTiles(Point tilePos)
        {
            for (int i = -2; i < 3; i++)
            {
                for (int j = -2; j < 3; j++)
                {
                    if (Math.Abs(i) == 2 && Math.Abs(j) == 2)
                        continue;

                    if (Main.tile[tilePos.X + i, tilePos.Y + j].TileType == WulfrumScaffoldKit.PlacedTileType || SelectedTiles.ContainsKey(new Point(tilePos.X + i, tilePos.Y + j)))
                        return true;
                }
            }

            return false;
        }



        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Scaffold Kit");
        }

        public static int tileGlowTime = 10;

        public Dictionary<Point, int> SelectedTiles = new Dictionary<Point, int>(); //Might need some cloning stuff for mp? idk
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        /*
        public Vector2 raytracePosition()
        {
            //Don't raycast when the scaffold has been started, since itd be able to reach around walls and such.
            //At this stage, the raycast relies on the fact it needs proximity with other scaffolding
            if (SelectedTiles.Count > 0)
                return Projectile.position;

            Vector2 properPosition = Owner.Center;
            Vector2 finalPosition = Projectile.position;

            float steps = (properPosition - finalPosition).Length() / 8f;
            float strikes = 0;

            //Fun fact this was a while loop before and obviously it ended up doing an infinite loop for whatever reason. Truly, while loops are peak.
            for (int i = 0; i < steps; i ++)
            {
                properPosition = properPosition.MoveTowards(finalPosition, 8);

                if (Collision.SolidCollision(properPosition - Vector2.One * 1, 2, 2))
                    strikes++;

                if (strikes > 1)
                    break;
            }

            return properPosition;
        }
        */

        public override void AI()
        {
            if (Owner.channel && CanOwnerGoOn)
            {
                //Initialize the position
                if (Projectile.timeLeft > 2)
                    Projectile.position = Owner.Calamity().mouseWorld;

                Projectile.position = Projectile.position.MoveTowards(Owner.Calamity().mouseWorld, 16);
                //Projectile.position = raytracePosition();

                if ((Projectile.position - Owner.Center).Length() > WulfrumScaffoldKit.TileReach * 16)
                    Projectile.position = Owner.Center + Vector2.Normalize(Projectile.position - Owner.Center) * WulfrumScaffoldKit.TileReach * 16;

                if (Owner.whoAmI == Main.myPlayer)
                {
                    Point tilePos = Projectile.position.ToTileCoordinates();
                    Tile hoveredTile = Main.tile[tilePos];

                    if (!hoveredTile.HasTile && !SelectedTiles.ContainsKey(tilePos) && CanSelectTile(tilePos))
                    {
                        SelectedTiles.Add(tilePos, tileGlowTime);

                        if (Kit.storedScrap > 0)
                            Kit.storedScrap--;

                        else
                        {
                            Owner.ConsumeItem(ModContent.ItemType<WulfrumShard>());
                            Kit.storedScrap = WulfrumScaffoldKit.TilesPerScrap - 1;
                        }
                    }

                    foreach (Point position in SelectedTiles.Keys)
                    {
                        if (SelectedTiles[position] > 0)
                            SelectedTiles[position]--;
                    }
                }

                Projectile.timeLeft = 2;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.myPlayer != Owner.whoAmI)
                return false;

            Texture2D sprite = ModContent.Request<Texture2D>(Texture).Value;

            Effect tileEffect = Filters.Scene["WulfrumScaffoldSelection"].GetShader().Shader;

            tileEffect.Parameters["mainOpacity"].SetValue(1f);
            tileEffect.Parameters["tileEdgeBlendStrenght"].SetValue(2f);
            tileEffect.Parameters["placementGlowColor"].SetValue(Color.GreenYellow.ToVector4());
            tileEffect.Parameters["baseTintColor"].SetValue(Color.DeepSkyBlue.ToVector4() * 0.5f);
            tileEffect.Parameters["scanlineColor"].SetValue(Color.YellowGreen.ToVector4() * 1f);
            tileEffect.Parameters["tileEdgeColor"].SetValue(Color.GreenYellow.ToVector3());
            tileEffect.Parameters["Resolution"].SetValue(8f);

            tileEffect.Parameters["time"].SetValue(Main.GameUpdateCount);
            Vector4[] scanLines = new Vector4[]
            {
                new Vector4(0f, 4f, 0.1f, 0.5f),
                new Vector4(1f, 4f, 0.1f, 0.5f),
                new Vector4(37f, 60f, 0.4f, 1f),
                new Vector4(2f, 6f, -0.2f, 0.3f),
                new Vector4(0f, 4f, 0.1f, 0.5f), //vertical start
                new Vector4(1f, 4f, 0.1f, 0.5f),
                new Vector4(2f, 6f, -0.2f, 0.3f)
            };

            tileEffect.Parameters["ScanLines"].SetValue(scanLines);
            tileEffect.Parameters["ScanLinesCount"].SetValue(scanLines.Length);
            tileEffect.Parameters["verticalScanLinesIndex"].SetValue(4);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, tileEffect, Main.GameViewMatrix.TransformationMatrix);

            foreach (Point pos in SelectedTiles.Keys)
            {
                tileEffect.Parameters["blinkTime"].SetValue(SelectedTiles[pos] / (float)tileGlowTime);
                tileEffect.Parameters["cardinalConnections"].SetValue(new bool[] { Connected(pos, 0, -1), Connected(pos, -1, 0), Connected(pos, 1, 0), Connected(pos, 0, 1) });
                tileEffect.Parameters["tilePosition"].SetValue(pos.ToVector2() * 16f);

                Main.spriteBatch.Draw(sprite, pos.ToWorldCoordinates() - Main.screenPosition, null, Color.White, 0, new Vector2(sprite.Width / 2f, sprite.Height / 2f), 16f, 0, 0);
            }

            CalamityUtils.ExitShaderRegion(Main.spriteBatch);

            return false;
        }

        public bool Connected(Point pos, int displaceX, int displaceY) => SelectedTiles.ContainsKey(new Point(pos.X + displaceX, pos.Y + displaceY));


        public override void Kill(int timeLeft)
        {
            
            if (Main.myPlayer == Owner.whoAmI)
            {
                foreach (Point pos in SelectedTiles.Keys)
                {
                    WorldGen.PlaceTile(pos.X, pos.Y, WulfrumScaffoldKit.PlacedTileType) ;
                    NetMessage.SendTileSquare(-1, pos.X, pos.Y, TileChangeType.None);
                }

                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.position, Vector2.Zero, ModContent.ProjectileType<WulfrumScaffoldKitCleanupManager>(), 0, 0, Owner.whoAmI);
                (proj.ModProjectile as WulfrumScaffoldKitCleanupManager).ManagedTiles = SelectedTiles.Keys.ToList<Point>();
            }
        }
    }
}
