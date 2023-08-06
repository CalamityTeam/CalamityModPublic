using CalamityMod.Dusts;
using CalamityMod.Gores.Trees;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.NPCs.AcidRain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Enums;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Abyss
{
    public class AcidWoodTree : ModPalmTree
    {
        public override void SetStaticDefaults()
        {
            // Grows on sulphurous sand
            GrowsOnTileId = new int[] { ModContent.TileType<SulphurousSand>() };
        }

        //Copypasted from vanilla, just as ExampleMod did, due to the lack of proper explanation
        public override TreePaintingSettings TreeShaderSettings => new TreePaintingSettings
        {
            UseSpecialGroups = true,
            SpecialGroupMinimalHueValue = 0.153f,
            SpecialGroupMaximumHueValue = 0.25f,
            SpecialGroupMinimumSaturationValue = 0.8802f,
            SpecialGroupMaximumSaturationValue = 1f
        };

        public override Asset<Texture2D> GetTopTextures() => ModContent.Request<Texture2D>("CalamityMod/Tiles/Abyss/AcidWoodTreeTops");
        public override Asset<Texture2D> GetTexture() => ModContent.Request<Texture2D>("CalamityMod/Tiles/Abyss/AcidWoodTree");

        //I don't know what this means. Why do palm trees have branches?? Since when. Also acidwood trees arent meant to have oasis alts
        public override Asset<Texture2D> GetOasisTopTextures() => ModContent.Request<Texture2D>("CalamityMod/Tiles/Abyss/AcidWoodTreeOasisTops");

        public override int DropWood() => ModContent.ItemType<Acidwood>();
        public override int CreateDust() => (int)CalamityDusts.SulfurousSeaAcid;

        public override int SaplingGrowthType(ref int style)
        {
            style = 0;
            return ModContent.TileType<AcidWoodTreeSapling>();
        }

		public override int TreeLeaf() => ModContent.GoreType<SulphurLeaf>();

		// Returning false at the end prevents vanilla behavior as the default is palm tree behavior which can include undesirable stuff like seagulls
		public override bool Shake(int x, int y, ref bool createLeaves)
		{
			int randAmt = Main.rand.Next(1, 3);
			
			if (Main.getGoodWorld && Main.rand.NextBool(15))
			{
				Projectile.NewProjectile(new EntitySource_ShakeTree(x, y), x * 16, y * 16, Main.rand.NextFloat(-100f, 100f) * 0.002f, 0f, ProjectileID.Bomb, 0, 0f, Player.FindClosest(new Vector2(x * 16, y * 16), 16, 16));
			}
			else if (Main.rand.NextBool(35) && Main.halloween)
			{
				createLeaves = true;
				Item.NewItem(WorldGen.GetItemSource_FromTreeShake(x, y), x * 16, y * 16, 16, 16, ItemID.RottenEgg, randAmt);
			}
			else if (Main.rand.NextBool(12))
			{
				createLeaves = true;
				Item.NewItem(WorldGen.GetItemSource_FromTreeShake(x, y), x * 16, y * 16, 16, 16, DropWood(), Main.rand.Next(1, 4));
			}
			else if (Main.rand.NextBool(20))
			{
				createLeaves = true;
				int coin = ItemID.CopperCoin;
				int amount = Main.rand.Next(50, 100);
				if (Main.rand.NextBool(30))
				{
					coin = ItemID.GoldCoin;
					amount = 1;
					if (Main.rand.NextBool(5))
						amount++;

					if (Main.rand.NextBool(10))
						amount++;
				}
				else if (Main.rand.NextBool(10))
				{
					coin = ItemID.SilverCoin;
					amount = Main.rand.Next(1, 21);
					if (Main.rand.NextBool(3))
						amount += Main.rand.Next(1, 21);

					if (Main.rand.NextBool(4))
						amount += Main.rand.Next(1, 21);
				}

				Item.NewItem(WorldGen.GetItemSource_FromTreeShake(x, y), x * 16, y * 16, 16, 16, coin, amount);
			}
			else if (Main.rand.NextBool(20))
			{
				createLeaves = true;
				int type = ModContent.NPCType<BabyFlakCrab>();
				NPC.NewNPC(new EntitySource_ShakeTree(x, y), x * 16, y * 16, type);
			}
			else if (Main.rand.NextBool(15))
			{
				createLeaves = true;
				int type = -1;
				if (DownedBossSystem.downedEoCAcidRain)
					type = ModContent.ItemType<SulphuricScale>();
				if (DownedBossSystem.downedAquaticScourgeAcidRain && Main.rand.NextBool())
					type = ModContent.ItemType<CorrodedFossil>();
				if (type != -1)
					Item.NewItem(WorldGen.GetItemSource_FromTreeShake(x, y), new Vector2(x, y) * 16, type, randAmt);
			}
			return false;
		}
    }
}
