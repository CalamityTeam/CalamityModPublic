using CalamityMod.Dusts;
using CalamityMod.Projectiles.Environment;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Abyss
{
    public class SteamGeyser : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.DrawYOffset = 4;
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Geyser");
            AddMapEntry(new Color(150, 100, 50), name);
            DustType = (int)CalamityDusts.SulfurousSeaAcid;

            base.SetDefaults();
        }

        public override bool IsTileDangerous(int i, int j, Player player) => true;

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 2;
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            bool noTilesBetweenPlayer = Collision.CanHit(new Vector2(i, j) * 16, 1, 1, new Vector2(i * 16f, Main.LocalPlayer.Center.Y), 1, 1);

            if (WorldGen.genRand.NextBool(60) && !Main.gamePaused && noTilesBetweenPlayer)
            {
                if (Math.Abs(Main.LocalPlayer.Center.X - i * 16) < 80 &&
                    Main.LocalPlayer.Bottom.Y < j * 16 &&
                    Main.LocalPlayer.Bottom.Y > j * 16 - 300)
                {
                    float yeetSpeed = MathHelper.Clamp(10f - (10f * Math.Abs(Main.LocalPlayer.Bottom.Y - j * 16) / 300f), 0f, 10f);
                    Main.LocalPlayer.velocity.Y -= yeetSpeed;
                    Projectile.NewProjectile(i * 16 + 8, j * 16 - 20, 0f, -10f, ModContent.ProjectileType<HotSteam>(), Main.expertMode ? 12 : 15, 0f);
                }
            }
        }
    }
}
