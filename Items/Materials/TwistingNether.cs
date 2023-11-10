using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class TwistingNether : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            ItemID.Sets.ItemNoGravity[Item.type] = true;

			ItemID.Sets.SortingPriorityMaterials[Type] = 109;
        }

        public override void SetDefaults()
        {
            Item.width = 15;
            Item.height = 12;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(0, 7, 0, 0);
            Item.rare = ModContent.RarityType<Turquoise>();
        }
        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = (float)Main.rand.Next(90, 111) * 0.01f;
            brightness *= Main.essScale;
            Lighting.AddLight((int)((Item.position.X + (float)(Item.width / 2)) / 16f), (int)((Item.position.Y + (float)(Item.height / 2)) / 16f), 0.5f * brightness, 0.1f * brightness, 0.7f * brightness);
        }
    }
}
