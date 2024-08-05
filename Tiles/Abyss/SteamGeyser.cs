using System;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Environment;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Abyss
{
    [LegacyName("SteamGeyser")]
    public class SteamGeyser1 : ModTile
    {
        int steamTimer = 0;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(103, 65, 64), CalamityUtils.GetText($"{LocalizationCategory}.SteamGeyser.MapEntry"));
            DustType = (int)CalamityDusts.SulphurousSeaAcid;

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
                if (steamTimer >= 240)
                    steamTimer = 0;
            }
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Tile t = CalamityUtils.ParanoidTileRetrieval(i, j);
            Vector2 spawnPosition = new(i * 16f + 24f, j * 16f - 4f);

            if (!Main.gamePaused && t.TileFrameX % 36 == 0 && t.TileFrameY % 36 == 0 && Collision.CanHitLine(spawnPosition, 1, 1, spawnPosition - Vector2.UnitY * 100f, 1, 1))
            {
                if (steamTimer <= 180)
                    return;

                if (!Main.rand.NextBool(3))
                    return;

                float positionInterpolant = (i + j) * 0.041f % 1f;
                Vector2 smokeVelocity = -Vector2.UnitY.RotatedByRandom(0.11f) * MathHelper.Lerp(4.8f, 8.1f, positionInterpolant);
                smokeVelocity.X += (float)Math.Cos(MathHelper.TwoPi * positionInterpolant) * 1.1f;
                smokeVelocity.Y -= Main.rand.Next(3, 6);

                Projectile.NewProjectile(new EntitySource_WorldEvent(), spawnPosition, smokeVelocity, ModContent.ProjectileType<HotSteam>(), Main.expertMode ? 15 : 23, 0f);
            }
        }
    }

    public class SteamGeyser2 : SteamGeyser1
    {
    }

    public class SteamGeyser3 : SteamGeyser1
    {
    }
}
