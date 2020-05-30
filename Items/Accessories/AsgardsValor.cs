using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
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
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Asgard's Valor");
            Tooltip.SetDefault("Grants immunity to fire blocks and knockback\n" +
                "Immune to most debuffs including Brimstone Flames, and Glacial State\n" +
                "10% damage reduction while submerged in liquid\n" +
                "+20 max life\n" +
                "Grants a holy dash which can be used to ram enemies\n" +
                "Toggle visibility of this accessory to enable/disable the dash");
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 44;
            item.value = Item.buyPrice(0, 45, 0, 0);
            item.rare = 9;
            item.defense = 8;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (!hideVisual)
            { modPlayer.dashMod = 2; }
            player.noKnockback = true;
            player.fireWalk = true;
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
            player.buffImmune[ModContent.BuffType<BrimstoneFlames>()] = true;
            player.buffImmune[ModContent.BuffType<GlacialState>()] = true;
            player.statLifeMax2 += 20;
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            { player.endurance += 0.1f; }
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
