using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using Terraria.Audio;

namespace CalamityMod.Items.Accessories
{
    //Developer item, dedicatee: Nincity
    public class AngelicAlliance : ModItem
    {
        public static readonly SoundStyle ActivationSound = new("Sounds/Custom/AbilitySounds/AngelicAllianceActivation");
        
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Angelic Alliance");
            Tooltip.SetDefault("Call upon the force of heaven to empower your attacks and minions\n" +
            "Courage, Enlightenment, Bliss. United in Judgement\n" +
            "+2 max minions, 15% increased summon damage, and 8% increased damage to all other classes\n" +
            "Life regeneration is boosted while jumping\n" +
            "This line is modified in the code below. If you can read this, someone probably did something wrong (It was Ben)\n" +
            "While under the effects of Divine Bless, for every minion you have, an archangel shall be summoned to aid you in combat\n" +
            "Each spawned angel will instantly heal you for two health\n" +
            "All minion attacks inflict Banishing Fire and you are granted a flat health boost of four health per second\n" +
            "This effect has a cooldown of 1 minute");
        }

        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 92;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.accessory = true;
            Item.rare = 10;
            Item.Calamity().customRarity = CalamityRarity.HotPink;
            Item.Calamity().devItem = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityKeybinds.AngelicAllianceHotKey.TooltipHotkeyString();

            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip4");

            if (line != null)
                line.Text = "Press " + hotkey + " to grace yourself in divinity for 15 seconds";
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.angelicAlliance = true;
            player.GetDamage<GenericDamageClass>() += 0.08f;
            player.GetDamage<SummonDamageClass>() += 0.07f; //7% + 8% = 15%
            player.maxMinions += 2;
            if (player.controlJump)
                player.lifeRegen += 4;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyHallowedHelmet").
                AddIngredient(ItemID.HallowedPlateMail).
                AddIngredient(ItemID.HallowedGreaves).
                AddIngredient(ItemID.PaladinsShield).
                AddIngredient(ItemID.TrueExcalibur).
                AddIngredient(ItemID.CrossNecklace).
                AddIngredient<ShadowspecBar>(5).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
