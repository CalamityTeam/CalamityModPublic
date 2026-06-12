using CalamityMod.CalPlayer;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class V8000Engine : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static float DashDelayModifier = 3.5f;
        public const int ShieldSlamDamage = 900;
        public const float ShieldSlamKnockback = 0.4f;
        public const int ShieldSlamIFrames = 14; // While this has more i-frames than Elysian's dash, the dash speed sets this one back comparitively

        public static SoundStyle DashSound = new("CalamityMod/Sounds/Item/V8000Boost");

        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 50;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.accessory = true;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded) => !player.Calamity().hasEngineDash;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.hasEngineDash = true;
            modPlayer.v8000Engine = true;
            modPlayer.DashID = V8000EngineDash.ID;
            player.dashType = 0;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ModContent.ItemType<V8Engine>()).
                AddIngredient(ModContent.ItemType<RuinousSoul>(), 5).
                AddIngredient(ItemID.Nanites, 200).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
