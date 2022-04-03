using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class TitanHeartMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Titan Heart Mask");
            Tooltip.SetDefault("5% increased rogue damage and knockback\n" +
            "Rogue weapons inflict the Astral Infection debuff");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 12, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 12; //43
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<TitanHeartMantle>() && legs.type == ModContent.ItemType<TitanHeartBoots>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "15% increased rogue damage and knockback\n" +
                    "Stealth strikes deal double knockback and cause an astral explosion\n" +
                    "Grants immunity to knockback\n" +
                    "Rogue stealth builds while not attacking and slower while moving, up to a max of 100\n" +
                    "Once you have built max stealth, you will be able to perform a Stealth Strike\n" +
                    "Rogue stealth only reduces when you attack, it does not reduce while moving\n" +
                    "The higher your rogue stealth the higher your rogue damage, crit, and movement speed";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.titanHeartSet = true;
            modPlayer.throwingDamage += 0.15f;
            modPlayer.rogueStealthMax += 1f;
            modPlayer.wearingRogueArmor = true;
            player.noKnockback = true;
        }

        public override void UpdateEquip(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.titanHeartMask = true;
            modPlayer.throwingDamage += 0.05f;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<AstralMonolith>(), 10).AddIngredient(ModContent.ItemType<TitanHeart>()).AddTile(TileID.Anvils).Register();
        }
    }
}
