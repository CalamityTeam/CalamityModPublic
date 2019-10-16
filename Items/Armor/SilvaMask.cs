using CalamityMod.CalPlayer;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    [AutoloadEquip(EquipType.Head)]
    public class SilvaMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silva Mask");
            Tooltip.SetDefault("13% increased rogue damage and critical strike chance, 20% increased movement speed");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 90, 0, 0);
            item.defense = 30; //110
            item.Calamity().postMoonLordRarity = 15;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SilvaArmor>() && legs.type == ModContent.ItemType<SilvaLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.silvaSet = true;
            modPlayer.silvaThrowing = true;
            modPlayer.rogueStealthMax = 1.5f;
            modPlayer.wearingRogueArmor = true;
            player.setBonus = "You are immune to almost all debuffs\n" +
                "All projectiles spawn healing leaf orbs on enemy hits\n" +
                "Max run speed and acceleration boosted by 5%\n" +
                "If you are reduced to 1 HP you will not die from any further damage for 10 seconds\n" +
                "If you get reduced to 1 HP again while this effect is active you will lose 100 max life\n" +
                "This effect only triggers once per life and if you are reduced to 400 max life the invincibility effect will stop\n" +
                "Your max life will return to normal if you die\n" +
                "Rogue weapons have a faster throwing rate while you are above 50% life\n" +
                "After the silva invulnerability time your rogue weapons will do 10% more damage\n" +
                "Rogue stealth builds while not attacking and not moving, up to a max of 150\n" +
                "Rogue stealth only reduces when you attack, it does not reduce while moving\n" +
                "The higher your rogue stealth the higher your rogue damage, crit, and movement speed";
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().throwingDamage += 0.13f;
            player.Calamity().throwingCrit += 13;
            player.moveSpeed += 0.2f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "DarksunFragment", 5);
            recipe.AddIngredient(null, "EffulgentFeather", 5);
            recipe.AddIngredient(null, "CosmiliteBar", 5);
            recipe.AddIngredient(null, "Tenebris", 6);
            recipe.AddIngredient(null, "NightmareFuel", 14);
            recipe.AddIngredient(null, "EndothermicEnergy", 14);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
