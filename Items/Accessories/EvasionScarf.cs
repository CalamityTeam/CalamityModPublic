using CalamityMod.CalPlayer;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Neck)]
    public class EvasionScarf : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Evasion Scarf");
            Tooltip.SetDefault("True melee strikes deal 15% more damage\n" +
                "Grants the ability to dash; dashing into an attack will cause you to dodge it\n" +
                "After a successful dodge you must wait 13 seconds before you can dodge again\n" +
                "This cooldown will be 50 percent longer if you have Chaos State\n" +
                "While on cooldown, Chaos State will be 50 percent longer");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.accessory = true;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = ItemRarityID.Pink;
            item.Calamity().donorItem = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            if (CalamityWorld.death)
            {
                foreach (TooltipLine line2 in list)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip4")
                    {
                        line2.text = "While on cooldown, Chaos State will be 50 percent longer\n" +
                        "Provides cold protection in Death Mode";
                    }
                }
            }
        }

        public override bool CanEquipAccessory(Player player, int slot) => !player.Calamity().dodgeScarf;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dodgeScarf = true;
            modPlayer.evasionScarf = true;
            modPlayer.dashMod = 1;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CounterScarf>());
            recipe.AddIngredient(ItemID.SoulofNight, 5);
            recipe.AddIngredient(ItemID.SoulofLight, 5);
            recipe.AddIngredient(ItemID.Silk, 15);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
