using CalamityMod.Projectiles.Environment;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using System;

namespace CalamityMod.Tiles.Abyss.AbyssAmbient
{
    public class AbyssVent1 : ModTile
    {
        int steamTimer = 0;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(106, 80, 102), CalamityUtils.GetText($"{LocalizationCategory}.AbyssVent.MapEntry"));
            DustType = 33;

            base.SetStaticDefaults();
        }

        public override bool IsTileDangerous(int i, int j, Player player) => true;

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 2;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            Tile t = CalamityUtils.ParanoidTileRetrieval(i, j);
            Vector2 spawnPosition = new(i * 16f + 24f, j * 16f - 4f);

            if (!Main.gamePaused && t.TileFrameX % 36 == 0 && t.TileFrameY % 36 == 0 && Collision.CanHitLine(spawnPosition, 1, 1, spawnPosition - Vector2.UnitY * 100f, 1, 1))
            {
                steamTimer += Main.rand.Next(0, 2);
                if (steamTimer >= 360)
                    steamTimer = 0;
            }
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Tile t = CalamityUtils.ParanoidTileRetrieval(i, j);
            Vector2 spawnPosition = new(i * 16f + 24f, j * 16f - 4f);
            
            if (!Main.gamePaused && t.TileFrameX % 36 == 0 && t.TileFrameY % 36 == 0 && Collision.CanHitLine(spawnPosition, 1, 1, spawnPosition - Vector2.UnitY * 100f, 1, 1))
            {
                float positionInterpolant = (i + j) * 0.041f % 1f;
                Vector2 smokeVelocity = -Vector2.UnitY.RotatedByRandom(0.11f) * MathHelper.Lerp(4.8f, 8.1f, positionInterpolant);
                smokeVelocity.X += (float)Math.Cos(MathHelper.TwoPi * positionInterpolant) * 1.1f;
                smokeVelocity.Y -= Main.rand.Next(3, 6);

                if (steamTimer >= 300 && Main.rand.NextBool(3))
                    Projectile.NewProjectile(new EntitySource_WorldEvent(), spawnPosition, smokeVelocity, ModContent.ProjectileType<MurkySteam>(), Main.expertMode ? 17 : 25, 0f);
            }
        }
    }

    public class AbyssVent2 : AbyssVent1
    {
    }

    public class AbyssVent3 : AbyssVent1
    {
    }
}
