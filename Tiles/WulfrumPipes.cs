using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using System;

namespace CalamityMod.Tiles
{
    public class WulfrumPipes : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            HitSound = SoundID.Item52;
            DustType = 32;
            MinPick = 30;
            AddMapEntry(new Color(128, 90, 77));
        }

        public override bool CanExplode(int i, int j) => false;

        public override void PlaceInWorld(int i, int j, Item item)
        {
            SoundEngine.PlaySound(SoundID.Item52 with { Volume = SoundID.Item52.Volume * 0.75f, Pitch = SoundID.Item52.Pitch - 0.5f }, new Vector2( i * 16, j * 16 ));
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            drawData.tileWidth += (int)Math.Sin(Main.GlobalTimeWrappedHourly) * 4;
        }
    }
}
