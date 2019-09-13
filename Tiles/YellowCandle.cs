using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles
{
    public class YellowCandle : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Spiteful Candle");
            AddMapEntry(new Color(238, 145, 105), name);
            animationFrameHeight = 34;
        }

		public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter >= 6)
            {
                frame = (frame + 1) % 6;
                frameCounter = 0;
            }
        }

		public override void NearbyEffects(int i, int j, bool closer)
		{
			Player player = Main.LocalPlayer;
			if (!player.dead && player.active)
			{
				player.AddBuff(mod.BuffType("YellowDamageCandle"), 20);
				if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int m = 0; m < 200; m++)
					{
						if (Main.npc[m].active && !Main.npc[m].friendly)
						{
							Main.npc[m].buffImmune[mod.BuffType("YellowDamageCandle")] = false;
							if (Main.npc[m].type == mod.NPCType("CeaselessVoid") || Main.npc[m].type == mod.NPCType("EidolonWyrmHeadHuge"))
							{
								Main.npc[m].buffImmune[mod.BuffType("YellowDamageCandle")] = true;
							}
							Main.npc[m].AddBuff(mod.BuffType("YellowDamageCandle"), 20, false);
						}
					}
				}
			}
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
			r = 0.75f;
            g = 0.75f;
			b = 0.35f;
        }

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 16, 32, mod.ItemType("YellowCandle"));
        }
    }
}
