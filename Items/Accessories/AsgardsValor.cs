using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class AsgardsValor : ModItem
    {
        public const int ShieldSlamIFrames = 6;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Asgard's Valor");
            Tooltip.SetDefault("Grants immunity to knockback\n" +
                "Immune to most debuffs and reduces the damage caused by the Brimstone Flames debuff\n" +
                "+16 defense while submerged in liquid\n" +
                "+20 max life\n" +
                "Grants a holy dash which can be used to ram enemies");
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 44;
            item.value = CalamityGlobalItem.Rarity7BuyPrice;
            item.rare = ItemRarityID.Lime;
            item.defense = 16;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dashMod = 2;
            player.dash = 0;
            player.noKnockback = true;
            player.fireWalk = true;
            modPlayer.abaddon = true;
            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[BuffID.Chilled] = true;
            player.buffImmune[BuffID.Frostburn] = true;
            player.buffImmune[BuffID.Weak] = true;
            player.buffImmune[BuffID.BrokenArmor] = true;
            player.buffImmune[BuffID.Bleeding] = true;
            player.buffImmune[BuffID.Poisoned] = true;
            player.buffImmune[BuffID.Slow] = true;
            player.buffImmune[BuffID.Confused] = true;
            player.buffImmune[BuffID.Silenced] = true;
            player.buffImmune[BuffID.Cursed] = true;
            player.buffImmune[BuffID.Darkness] = true;
            player.buffImmune[BuffID.WindPushed] = true;
            player.statLifeMax2 += 20;
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            { player.statDefense += 16; }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.AnkhShield);
            recipe.AddIngredient(ModContent.ItemType<OrnateShield>());
            recipe.AddIngredient(ModContent.ItemType<ShieldoftheOcean>());
            recipe.AddIngredient(ModContent.ItemType<Abaddon>());
            recipe.AddIngredient(ModContent.ItemType<CoreofCalamity>());
            recipe.AddIngredient(ItemID.LifeFruit, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
