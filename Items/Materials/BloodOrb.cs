using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    public class BloodOrb : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(6, 4));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 28;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(silver: 1, copper: 20);
            Item.rare = ItemRarityID.Blue;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.rand.Next(90, 111) * 0.01f;
            brightness *= Main.essScale;
            Lighting.AddLight((int)((Item.position.X + (Item.width / 2)) / 16f), (int)((Item.position.Y + (Item.height / 2)) / 16f), 0.75f * brightness, 0f, 0f);
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.BloodMoonStarter).
                Register()
                .DisableDecraft();
        }
    }
}
