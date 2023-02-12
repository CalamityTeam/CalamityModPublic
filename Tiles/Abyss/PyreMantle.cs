using CalamityMod.Tiles.Abyss.AbyssAmbient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Abyss
{
    public class PyreMantle : ModTile
    {
        public static readonly SoundStyle MineSound = new("CalamityMod/Sounds/Custom/VoidstoneMine", 3) { Volume = 0.4f };
        
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithAbyss(Type);

            TileID.Sets.ChecksForMerge[Type] = true;
            HitSound = MineSound;
            MineResist = 10f;
            MinPick = 180;
            ItemDrop = ModContent.ItemType<Items.Placeables.PyreMantle>();
            AddMapEntry(new Color(19, 20, 32));
        }

        int animationFrameWidth = 288;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 162, 0f, 0f, 1, new Color(128, 128, 128), 1f);
            return false;
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, ModContent.TileType<AbyssGravel>(), false, false, false, false, resetFrame);
            return false;
        }
    }
}
