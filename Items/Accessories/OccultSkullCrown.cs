using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Face)]
    public class OccultSkullCrown : ModItem
    {
        public override void SetStaticDefaults()
        {
           
            if (Main.netMode != NetmodeID.Server)
            {
                int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Face);
				ArmorIDs.Face.Sets.PreventHairDraw[equipSlot] = true;
				ArmorIDs.Face.Sets.OverrideHelmet[equipSlot] = true;
            }
        }

        public override void SetDefaults()
        {
            Item.width = 82;
            Item.height = 62;
            Item.defense = 5;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.accessory = true;
        }

        public override void UpdateEquip(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.laudanum = true;
            modPlayer.heartOfDarkness = true;
            modPlayer.stressPills = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<HeartofDarkness>().
                AddIngredient<Laudanum>().
                AddIngredient<StressPills>().
                AddIngredient<NightmareFuel>(20).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
