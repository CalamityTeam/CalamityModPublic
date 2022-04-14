using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Materials
{
    public class BloodOrb : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
            DisplayName.SetDefault("Blood Orb");
        }

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 10;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 1, copper: 20);
            Item.rare = ItemRarityID.Blue;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float num = Main.rand.Next(90, 111) * 0.01f;
            num *= Main.essScale;
            Lighting.AddLight((int)((Item.position.X + (Item.width / 2)) / 16f), (int)((Item.position.Y + (Item.height / 2)) / 16f), 0.75f * num, 0f, 0f);
        }
    }
}
