using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Items.Armor.Bloodflare;
using CalamityMod.Items.Armor.GodSlayer;
using CalamityMod.Items.Armor.Silva;
using CalamityMod.Items.Armor.Tarragon;

namespace CalamityMod.Items.Armor.Auric
{
    [AutoloadEquip(EquipType.Body)]
    public class AuricTeslaBodyArmor : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PostMoonLord";
        public override void Load()
        {
            // All code below runs only if we're not loading on a server
            if (Main.netMode != NetmodeID.Server)
            {
                // Add equip textures
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/Auric/AuricTeslaBodyArmor_Back", EquipType.Back, this);
            }
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 34;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.defense = 48;
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override void UpdateEquip(Player player)
        {
            var modPlayer = player.Calamity();
            player.statLifeMax2 += 100;
            player.GetDamage<GenericDamageClass>() += 0.08f;
            player.GetCritChance<GenericDamageClass>() += 5;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GodSlayerChestplate>().
                AddIngredient<BloodflareBodyArmor>().
                AddIngredient<TarragonBreastplate>().
                AddIngredient<AuricBar>(18).
                AddTile<CosmicAnvil>().
                Register();

            CreateRecipe().
                AddIngredient<SilvaArmor>().
                AddIngredient<BloodflareBodyArmor>().
                AddIngredient<TarragonBreastplate>().
                AddIngredient<AuricBar>(18).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
