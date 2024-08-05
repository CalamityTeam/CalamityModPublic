using System;
using CalamityMod.Projectiles.Environment;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Abyss.AbyssAmbient
{
    public class ThermalVent1 : ModTile
    {
        int steamTimer = 0;

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(132, 56, 42), CalamityUtils.GetText($"{LocalizationCategory}.ThermalVent.MapEntry"));
            DustType = 162;

            base.SetStaticDefaults();
        }

        public override bool IsTileDangerous(int i, int j, Player player) => true;

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 2;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.41f;
            g = 0.21f;
            b = 0f;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            Tile t = CalamityUtils.ParanoidTileRetrieval(i, j);
            Vector2 spawnPosition = new(i * 16f + 24f, j * 16f - 4f);

            if (!Main.gamePaused && t.TileFrameX % 36 == 0 && t.TileFrameY % 36 == 0 && Collision.CanHitLine(spawnPosition, 1, 1, spawnPosition - Vector2.UnitY * 100f, 1, 1))
            {
                steamTimer += Main.rand.Next(0, 2);
                if (steamTimer >= 600)
                    steamTimer = 0;
            }
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Tile t = CalamityUtils.ParanoidTileRetrieval(i, j);
            Vector2 spawnPosition = new(i * 16f + 24f, j * 16f - 4f);

            if (!Main.gamePaused && t.TileFrameX % 36 == 0 && t.TileFrameY % 36 == 0 && Collision.CanHitLine(spawnPosition, 1, 1, spawnPosition - Vector2.UnitY * 100f, 1, 1))
            {
                if (steamTimer <= 450)
                    return;

                if (!Main.rand.NextBool(3))
                    return;

                float positionInterpolant = (i + j) * 0.041f % 1f;
                Vector2 smokeVelocity = -Vector2.UnitY.RotatedByRandom(0.11f) * MathHelper.Lerp(4.8f, 8.1f, positionInterpolant);
                smokeVelocity.X += (float)Math.Cos(MathHelper.TwoPi * positionInterpolant) * 1.1f;
                smokeVelocity.Y -= Main.rand.Next(3, 6);

                Projectile.NewProjectile(new EntitySource_WorldEvent(), spawnPosition, smokeVelocity, ModContent.ProjectileType<ThermalSteam>(), Main.expertMode ? 20 : 30, 0f);
            }
        }
    }

    public class ThermalVent2 : ThermalVent1
    {
    }

    public class ThermalVent3 : ThermalVent1
    {
    }
}
