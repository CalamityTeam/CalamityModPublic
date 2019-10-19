using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Items.Materials;
using Terraria.ID;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class DaedalusVisor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daedalus Facemask");
            Tooltip.SetDefault("13% increased rogue damage and 7% increased rogue critical strike chance, increases rogue velocity by 15%\n" +
                "17% increased movement speed");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 25, 0, 0);
            item.rare = 5;
            item.defense = 7; //37
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<DaedalusBreastplate>() && legs.type == ModContent.ItemType<DaedalusLeggings>();
        }

        public override void DrawHair(ref bool drawHair, ref bool drawAltHair)
        {
            drawHair = true;
            drawAltHair = true;
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "5% increased rogue damage\n" +
                "Rogue projectiles throw out crystal shards as they travel\n" +
                "Rogue stealth builds while not attacking and not moving, up to a max of 110\n" +
                "Rogue stealth only reduces when you attack, it does not reduce while moving\n" +
                "The higher your rogue stealth the higher your rogue damage, crit, and movement speed";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.daedalusSplit = true;
            modPlayer.rogueStealthMax = 1.1f;
            player.Calamity().throwingDamage += 0.05f;
            player.Calamity().wearingRogueArmor = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().throwingVelocity += 0.15f;
            player.Calamity().throwingDamage += 0.13f;
            player.Calamity().throwingCrit += 7;
            player.moveSpeed += 0.17f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<VerstaltiteBar>(), 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
