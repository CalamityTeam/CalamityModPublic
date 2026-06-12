using CalamityMod.CalPlayer;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class V8Engine : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static float DashDelayModifier = 4f;

        public static SoundStyle DashSound = new("CalamityMod/Sounds/Item/V8Boost");

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 42;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded) => !player.Calamity().hasEngineDash;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.hasEngineDash = true;
            modPlayer.v8Engine = true;
            modPlayer.DashID = V8EngineDash.ID;
            player.dashType = 0;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyAdamantiteBar", 5).
                AddIngredient(ModContent.ItemType<UnholyCore>(), 4).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
