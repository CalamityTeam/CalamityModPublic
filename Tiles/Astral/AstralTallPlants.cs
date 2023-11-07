using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Astral
{
    public class AstralTallPlants : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileCut[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileFrameImportant[Type] = true;
			TileID.Sets.ReplaceTileBreakUp[Type] = true;
			TileID.Sets.SwaysInWindBasic[Type] = true;
			TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.addTile(Type);

            DustType = ModContent.DustType<AstralBasic>();

            HitSound = SoundID.Grass;

            AddMapEntry(new Color(127, 111, 144));

            base.SetStaticDefaults();
        }

		public override void DropCritterChance(int i, int j, ref int wormChance, ref int grassHopperChance, ref int jungleGrubChance)
		{
			if (NPC.CountNPCS(NPCID.EnchantedNightcrawler) < 5 && Main.rand.NextBool(200))
			{
				int worm = NPC.NewNPC(new EntitySource_TileBreak(i, j), i * 16 + 10, j * 16, NPCID.EnchantedNightcrawler);
				Main.npc[worm].TargetClosest();
				Main.npc[worm].velocity.Y = Main.rand.NextFloat(-5f, -2.1f);
				Main.npc[worm].velocity.X = Main.rand.NextFloat(0f, 2.6f) * (float)(-Main.npc[worm].direction);
				Main.npc[worm].direction *= -1;
				Main.npc[worm].netUpdate = true;
			}
		}

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = -12;
            height = 32;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            Vector2 worldPosition = new Vector2(i, j).ToWorldCoordinates();
            Player nearestPlayer = Main.player[Player.FindClosest(worldPosition, 16, 16)];
            if (nearestPlayer.active)
            {                
                if (nearestPlayer.ActiveItem().type == ItemID.Sickle)
                    yield return new Item(ItemID.Hay, Main.rand.Next(2, 4 + 1));
                
                if (Main.rand.NextBool(20))
                    yield return new Item(ModContent.ItemType<Items.Placeables.AstralGrassSeeds>());
            }
        }

        public override bool IsTileBiomeSightable(int i, int j, ref Color sightColor)
        {
            sightColor = Color.Cyan;
            return true;
        }
    }
}
