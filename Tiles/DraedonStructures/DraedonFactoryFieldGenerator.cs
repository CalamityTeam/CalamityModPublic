using CalamityMod.TileEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles
{
    public class DraedonFactoryFieldGenerator : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileWaterDeath[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TEDraedonFieldGenerator>().Hook_AfterPlacement, -1, 0, true);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Field Generator");
            AddMapEntry(new Color(53, 49, 52), name);
            minPick = 65;
        }

        public TEDraedonFieldGenerator RetrieveTileEntity(int i, int j)
        {
            int determinedID = ModContent.GetInstance<TEDraedonFieldGenerator>().Find(i, j);
            if (determinedID == -1 || !(TileEntity.ByID[determinedID] is TEDraedonFieldGenerator))
            {
                ModTileEntity modTileEntity = ModTileEntity.ConstructFromType(ModContent.GetInstance<TEDraedonFieldGenerator>().type);
                modTileEntity.Position = new Point16(i, j);
                modTileEntity.ID = TileEntity.AssignNewID();
                modTileEntity.type = ModContent.GetInstance<TEDraedonFieldGenerator>().type;
                TileEntity.ByID[modTileEntity.ID] = modTileEntity;
                TileEntity.ByPosition[modTileEntity.Position] = modTileEntity;
                determinedID = modTileEntity.ID;
            }
            return (TEDraedonFieldGenerator)TileEntity.ByID[determinedID];
        }

        public override bool CanExplode(int i, int j) => false;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 226);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TEDraedonFieldGenerator charger = RetrieveTileEntity(i, j);
            Color drawColor = Color.Lerp(new Color(73, 68, 72), Color.Cyan, Utils.InverseLerp(0f, TEDraedonFieldGenerator.ActiveTimerMax, charger.ActiveTimer, true));

            int xPos = Main.tile[i, j].frameX;
            int yPos = Main.tile[i, j].frameY;
            Texture2D glowmask = ModContent.GetTexture("CalamityMod/Tiles/DraedonStructures/DraedonFactoryFieldGeneratorGlow");
            Vector2 screenOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + screenOffset;
            Tile trackTile = Main.tile[i, j];
            if (!trackTile.halfBrick() && trackTile.slope() == 0)
            {
                spriteBatch.Draw(glowmask, drawOffset, new Rectangle?(new Rectangle(xPos, yPos, 18, 18)), drawColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
            else if (trackTile.halfBrick())
            {
                spriteBatch.Draw(glowmask, drawOffset + new Vector2(0f, 8f), new Rectangle?(new Rectangle(xPos, yPos, 18, 8)), drawColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }

            Vector2 positionWorldCoords = new Vector2(i, j) * 16f;
            Player player = Main.player[Player.FindClosest(positionWorldCoords, 1, 1)];
            if (player.Distance(positionWorldCoords) < 1600f)
            {
                Texture2D laserTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/RedLightningTexture");
                Vector2[] pointsToDraw = new Vector2[65];
                for (int k = 0; k < pointsToDraw.Length; k++)
                {
                    float angle = k / (float)pointsToDraw.Length * MathHelper.TwoPi;
                    pointsToDraw[k] = angle.ToRotationVector2() * TEDraedonFieldGenerator.Radius;
                    pointsToDraw[k] += drawOffset;
                    pointsToDraw[k] += Main.rand.NextVector2Circular(1f, 1f);

                    angle = (k + 1) % pointsToDraw.Length / (float)pointsToDraw.Length * MathHelper.TwoPi;
                    pointsToDraw[(k + 1) % pointsToDraw.Length] = angle.ToRotationVector2() * TEDraedonFieldGenerator.Radius;
                    pointsToDraw[(k + 1) % pointsToDraw.Length] += drawOffset;
                    pointsToDraw[(k + 1) % pointsToDraw.Length] += Main.rand.NextVector2Circular(1f, 1f);

                    if (trackTile.halfBrick())
                        pointsToDraw[k] += Vector2.UnitY * 8f;
                    if (trackTile.halfBrick())
                        pointsToDraw[(k + 1) % pointsToDraw.Length] += Vector2.UnitY * 8f;

                    DelegateMethods.c_1 = Color.Cyan;
                    DelegateMethods.f_1 = 1f;
                    DelegateMethods.c_1 *= Utils.InverseLerp(TEDraedonFieldGenerator.Radius, TEDraedonFieldGenerator.Radius - 300f, Math.Abs(positionWorldCoords.X - player.Center.X), true) *
                        Utils.InverseLerp(TEDraedonFieldGenerator.Radius, TEDraedonFieldGenerator.Radius - 300f, Math.Abs(positionWorldCoords.Y - player.Center.Y), true);
                    DelegateMethods.f_1 *= Utils.InverseLerp(TEDraedonFieldGenerator.Radius, TEDraedonFieldGenerator.Radius - 300f, Math.Abs(positionWorldCoords.X - player.Center.X), true) *
                        Utils.InverseLerp(TEDraedonFieldGenerator.Radius, TEDraedonFieldGenerator.Radius - 300f, Math.Abs(positionWorldCoords.Y - player.Center.Y), true);
                    Utils.DrawLaser(spriteBatch, laserTexture, pointsToDraw[k], pointsToDraw[(k + 1) % pointsToDraw.Length], new Vector2(0.3f), new Utils.LaserLineFraming(DelegateMethods.LightningLaserDraw));

                    DelegateMethods.c_1 = Color.White * 0.7f;
                    DelegateMethods.f_1 = 0.7f;
                    DelegateMethods.c_1 *= Utils.InverseLerp(TEDraedonFieldGenerator.Radius, TEDraedonFieldGenerator.Radius - 300f, Math.Abs(positionWorldCoords.X - player.Center.X), true) *
                        Utils.InverseLerp(TEDraedonFieldGenerator.Radius, TEDraedonFieldGenerator.Radius - 300f, Math.Abs(positionWorldCoords.Y - player.Center.Y), true);
                    DelegateMethods.f_1 *= Utils.InverseLerp(TEDraedonFieldGenerator.Radius, TEDraedonFieldGenerator.Radius - 300f, Math.Abs(positionWorldCoords.X - player.Center.X), true) *
                        Utils.InverseLerp(TEDraedonFieldGenerator.Radius, TEDraedonFieldGenerator.Radius - 300f, Math.Abs(positionWorldCoords.Y - player.Center.Y), true);
                    Utils.DrawLaser(spriteBatch, laserTexture, pointsToDraw[k], pointsToDraw[(k + 1) % pointsToDraw.Length], new Vector2(0.6f), new Utils.LaserLineFraming(DelegateMethods.LightningLaserDraw));
                }
            }
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            TEDraedonFieldGenerator generator = RetrieveTileEntity(i, j);
            generator.Kill(i, j);
        }
    }
}
