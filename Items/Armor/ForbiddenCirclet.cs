using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class ForbiddenCirclet : ModItem
    {
        public const int manaCost = 60;
        public const int tornadoBaseDmg = 80;
        public const float tornadoBaseKB = 1f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Forbidden Circlet");
            Tooltip.SetDefault("10% increased summon damage and 15% increased rogue velocity");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.defense = 1;
            // This item has the same rarity and sell price as Forbidden Mask
            item.value = Item.buyPrice(gold: 25);
            item.rare = ItemRarityID.Pink;
            item.Calamity().donorItem = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemID.AncientBattleArmorShirt && legs.type == ItemID.AncientBattleArmorPants;
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowLokis = true;
            player.armorEffectDrawOutlinesForbidden = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            int stormMana = (int)(manaCost * player.manaCost);
            string hotkey = CalamityMod.TarraHotKey.TooltipHotkeyString();
            player.setBonus = "Press " + hotkey + " to call an ancient storm to the cursor location\n" +
                    "The ancient storm costs " + stormMana + " mana and benefits from both summon and rogue bonuses\n" +
                    "Rogue stealth strikes spawn homing eaters on enemy hits\n" +
                    "Rogue and summon attacks will scale off of the stat with a higher boost\n" +
                    "Rogue stealth builds while not attacking and slower while moving, up to a max of 40\n" +
                    "Once you have built max stealth, you will be able to perform a Stealth Strike\n" +
                    "Rogue stealth only reduces when you attack, it does not reduce while moving\n" +
                    "The higher your rogue stealth the higher your rogue damage, crit, and movement speed";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.forbiddenCirclet = true;
            modPlayer.rogueStealthMax += 0.4f;
            modPlayer.wearingRogueArmor = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.minionDamage += 0.1f;
            player.Calamity().throwingVelocity += 0.15f;
        }

        public override void AddRecipes()
        {
            //Same recipe as Forbidden Mask
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("AnyAdamantiteBar", 10);
            recipe.AddIngredient(ItemID.AncientBattleArmorMaterial);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
