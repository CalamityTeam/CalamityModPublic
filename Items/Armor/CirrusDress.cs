using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class CirrusDress : ModItem
    {
        /* How to obtain
         * 1 - Have alcohol poisoning
         * 2 - Visit the Stylist while Cirrus is alive in the world
         * 3 - Open her shop to find the dress
         */

        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/CirrusDress_Legs", EquipType.Legs, this);
            }
        }
        
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Cirrus' Dress");
            Tooltip.SetDefault("Here, this should help you drink a lot more than usual!\n" +
                "5% increased magic damage and critical strike chance\n" +
                "You feel thick...");

            if (Main.netMode == NetmodeID.Server)
                return;
            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            ArmorIDs.Body.Sets.HidesHands[equipSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity16BuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;
            Item.defense = 8;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().cirrusDress = true;
            player.GetDamage<MagicDamageClass>() += 0.05f;
            player.GetCritChance<MagicDamageClass>() += 5;
        }

        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            robes = true;
            equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
        }
    }
}
