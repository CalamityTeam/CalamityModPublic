using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class AbyssalDivingSuit : ModItem
    {
        public override void Load()
        {
            // All code below runs only if we're not loading on a server
            if (Main.netMode != NetmodeID.Server)
            {
                // Add equip textures
                EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Head, this);
                EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Body}", EquipType.Body, this);
                EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
            }
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Abyssal Diving Suit");
            Tooltip.SetDefault("Transforms the holder into an armored diver\n" +
                "Increases max movement speed and acceleration while underwater but you move slowly outside of water\n" +
                "The suits' armored plates reduce damage taken by 15%\n" +
                "The plates will only take damage if the damage taken is over 50\n" +
                "After the suit has taken too much damage its armored plates will take 3 minutes to regenerate\n" +
                "Reduces the damage caused by the pressure of the abyss while out of breath\n" +
                "Removes the bleed effect caused by the abyss in all layers except the deepest one\n" +
                "Grants the ability to swim and greatly extends underwater breathing\n" +
                "Provides light underwater and extra mobility on ice\n" +
                "Provides a moderate amount of light in the abyss\n" +
                "Greatly reduces breath loss in the abyss\n" +
                "Reduces creature's ability to detect you in the abyss\n" +
                "Reduces the defense reduction that the abyss causes\n" +
                "Grants immunity to the sulphurous waters\n" +
                "Allows you to fall faster while in liquids");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;

            if (Main.netMode != NetmodeID.Server)
            {
                int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
                int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
                int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
                ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
                ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
                ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
                ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.abyssalDivingSuit = true;
            if (hideVisual)
                modPlayer.abyssalDivingSuitHide = true;
        }

        public override void UpdateVanity(Player player)
        {
            player.Calamity().abyssalDivingSuitHide = false;
            player.Calamity().abyssalDivingSuitForce = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AbyssalDivingGear>().
                AddIngredient<AnechoicPlating>().
                AddIngredient<IronBoots>().
                AddIngredient(ItemID.LunarBar, 5).
                AddIngredient<MolluskHusk>(15).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
