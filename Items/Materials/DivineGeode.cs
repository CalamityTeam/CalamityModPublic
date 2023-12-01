using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class DivineGeode : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
			ItemID.Sets.SortingPriorityMaterials[Type] = 104;
        }

        public override void SetDefaults()
        {
            Item.width = 15;
            Item.height = 12;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(0, 6, 50, 0);
            Item.rare = ModContent.RarityType<Turquoise>();
        }
        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = (float)Main.rand.Next(90, 111) * 0.01f;
            brightness *= Main.essScale;
            Lighting.AddLight((int)((Item.position.X + (float)(Item.width / 2)) / 16f), (int)((Item.position.Y + (float)(Item.height / 2)) / 16f), 0.45f * brightness, 0.3f * brightness, 0f * brightness);
        }
    }
}
