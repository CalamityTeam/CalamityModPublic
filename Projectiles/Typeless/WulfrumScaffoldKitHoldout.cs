using System;
using System.Collections.Generic;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Tools;
using CalamityMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class WulfrumScaffoldKitHoldout : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<WulfrumScaffoldKit>();
        public override void Load()
        {
            PipeCleanupManager = new WulfrumPipeManager();
        }

        public Player Owner => Main.player[Projectile.owner];
        public WulfrumScaffoldKit Kit => Owner.HeldItem.ModItem as WulfrumScaffoldKit;
        public bool CanOwnerGoOn => Kit.storedScrap > 0 || Owner.HasItem(ModContent.ItemType<WulfrumMetalScrap>());

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

        public static TemporaryTileManager PipeCleanupManager;

        public static int tileGlowTime = 10;

        public Dictionary<Point, int> SelectedTiles = new Dictionary<Point, int>(); //Might need some cloning stuff for mp? idk, probably not
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

        public override void AI()
        {
            if (Owner.channel && CanOwnerGoOn)
            {
                //Initialize the position
                if (Projectile.timeLeft > 2)
                    Projectile.position = Owner.Calamity().mouseWorld;

                Owner.itemTime = 2;
                Owner.itemAnimation = 2;

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
                            Owner.ConsumeItem(ModContent.ItemType<WulfrumMetalScrap>());
                            Kit.storedScrap = WulfrumScaffoldKit.TilesPerScrap - 1;
                            SoundEngine.PlaySound(SoundID.Item65);
                            if (Main.netMode != NetmodeID.Server)
                            {
                                Gore shard = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), Owner.Center, Main.rand.NextVector2Circular(4f, 4f), Mod.Find<ModGore>("WulfrumPinger2").Type, Main.rand.NextFloat(0.5f, 1f));
                                shard.timeLeft = 10;
                                shard.alpha = 100 - Main.rand.Next(0, 60);
                            }
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

            Effect tileEffect = Filters.Scene["CalamityMod:WulfrumScaffoldSelection"].GetShader().Shader;

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


        public override void OnKill(int timeLeft)
        {
            if (SelectedTiles.Keys.Count > 0)
                SoundEngine.PlaySound(SoundID.Item101 with { Volume = SoundID.Item101.Volume * 0.6f}, Owner.Center);

            if (Main.myPlayer == Owner.whoAmI)
            {
                foreach (Point pos in SelectedTiles.Keys)
                {
                    if (PipeCleanupManager == null)
                        PipeCleanupManager = new WulfrumPipeManager();

                    TempTilesManagerSystem.AddTemporaryTile(pos, PipeCleanupManager);
                    WorldGen.PlaceTile(pos.X, pos.Y, WulfrumScaffoldKit.PlacedTileType) ;
                    NetMessage.SendTileSquare(-1, pos.X, pos.Y, TileChangeType.None);
                }
            }
        }
    }


    public class WulfrumPipeManager : TemporaryTileManager
    {
        public override int[] ManagedTypes => new int[] { WulfrumScaffoldKit.PlacedTileType };

        public override TemporaryTile Setup(Point pos)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 dustpos = pos.ToWorldCoordinates();
                Dust.NewDustPerfect(dustpos, 83, Main.rand.NextVector2Circular(3f, 3f), Scale: Main.rand.NextFloat(0.4f, 0.7f));
            }

            TemporaryTile tile = new TemporaryTile(pos, this, WulfrumScaffoldKit.TileTime);
            return tile;
        }

        public override void UpdateEffect(TemporaryTile tile)
        {
            if (tile.timeleft < WulfrumScaffoldKit.TileTime * 0.1f && Main.rand.NextBool(10))
            {
                Vector2 dustpos = tile.position.ToWorldCoordinates();
                Dust.NewDustPerfect(dustpos, 226, Main.rand.NextVector2Circular(4f, 4f), Scale: Main.rand.NextFloat(0.4f, 1f));
            }
        }
    }
}
