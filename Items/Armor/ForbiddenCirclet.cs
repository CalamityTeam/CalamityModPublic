using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class ForbiddenCirclet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Hardmode";
        public const int manaCost = 60;
        public const int tornadoBaseDmg = 80;
        public const float tornadoBaseKB = 1f;

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 1;
            // This item has the same rarity and sell price as Forbidden Mask
            Item.value = Item.buyPrice(gold: 25);
            Item.rare = ItemRarityID.Pink;
            Item.Calamity().donorItem = true;
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
            string hotkey = CalamityKeybinds.ArmorSetBonusHotKey.TooltipHotkeyString();
            player.setBonus = this.GetLocalization("SetBonus").Format(hotkey, stormMana);
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.forbiddenCirclet = true;
            modPlayer.rogueStealthMax += 0.4f;
            modPlayer.wearingRogueArmor = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<SummonDamageClass>() += 0.1f;
            player.Calamity().rogueVelocity += 0.15f;
        }

        public override void AddRecipes()
        {
            //Same recipe as Forbidden Mask
            CreateRecipe()
                .AddRecipeGroup("AnyAdamantiteBar", 10)
                .AddIngredient(ItemID.AncientBattleArmorMaterial)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
