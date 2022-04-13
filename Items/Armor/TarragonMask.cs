using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class TarragonMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tarragon Mask");
            Tooltip.SetDefault("Temporary immunity to lava\n" +
                "Can move freely through liquids\n" +
                "20% increased magic damage and 10% increased magic critical strike chance\n" +
                "5% increased damage reduction, +100 max mana, and 15% reduced mana usage");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 50, 0, 0);
            Item.defense = 10; //98
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<TarragonBreastplate>() && legs.type == ModContent.ItemType<TarragonLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.tarraSet = true;
            modPlayer.tarraMage = true;
            player.setBonus = "Reduces enemy spawn rates\n" +
                "Increased heart pickup range\n" +
                "Enemies have a chance to drop extra hearts on death\n" +
                "On every 5th critical strike you will fire a leaf storm\n" +
                "Magic projectiles heal you on enemy hits\n" +
                "Amount healed is based on projectile damage";
        }

        public override void UpdateEquip(Player player)
        {
            player.manaCost *= 0.85f;
            player.GetDamage(DamageClass.Magic) += 0.2f;
            player.GetCritChance(DamageClass.Magic) += 10;
            player.endurance += 0.05f;
            player.lavaMax += 240;
            player.statManaMax2 += 100;
            player.ignoreWater = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<UeliaceBar>(7).
                AddIngredient<DivineGeode>(6).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
