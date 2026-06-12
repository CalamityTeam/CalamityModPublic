using CalamityMod.Items.Fishing;
using CalamityMod.Tiles.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class GluttonyBlender : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GluttonyBlenderTile>());
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 10); // Sold by Shady Salesman
        }
    }

    public class QualitySlop : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Potions";
        public override string Texture => "CalamityMod/Items/Potions/Food/QualitySlop";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.FoodParticleColors[Type] = [
                new Color(183, 65, 68),
                new Color(167, 116, 36),
                new Color(175, 87, 190)
            ];
            ItemID.Sets.IsFood[Type] = true;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.Ambrosia;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(32, 30, BuffID.WellFed3, CalamityUtils.MinutesToFrames(30));
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Orange;
        }
    }

    public class DisgustingSlop : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Misc";
        public override string Texture => "CalamityMod/Items/Materials/DisgustingSlop";

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ExtractinatorMode[Type] = Type;
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 36;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Gray;
            Item.MakeUsableWithChlorophyteExtractinator();
        }

        public override void ExtractinatorUse(int extractinatorBlockType, ref int resultType, ref int resultStack)
        {
            float dropRand = Main.rand.NextFloat();
            resultStack = 1;

            // 50% chance for Poo
            // 25% chance for Clay Block
            // 15% chance for Apprentice Bait
            // 7.5% chance for Grasshopper
            // 2.5% chance for Rage Bait
            if (dropRand < 0.025f)
                resultType = ModContent.ItemType<RageBait>();
            else if (dropRand < 0.1f)
                resultType = ItemID.Grasshopper;
            else if (dropRand < 0.25f)
                resultType = ItemID.ApprenticeBait;
            else if (dropRand < 0.5f)
                resultType = ItemID.ClayBlock;
            else
                resultType = ItemID.PoopBlock;
        }
    }
}
