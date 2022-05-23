using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer.Dashes;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class AsgardsValor : ModItem
    {
        public const int ShieldSlamIFrames = 12;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Asgard's Valor");
            Tooltip.SetDefault("Grants immunity to knockback\n" +
                "Immune to most debuffs\n" +
                "+16 defense while submerged in liquid\n" +
                "+20 max life\n" +
                "Grants a holy dash which can be used to ram enemies");
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 44;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.defense = 16;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.DashID = AsgardsValorDash.ID;
            player.dash = 0;
            player.noKnockback = true;
            player.fireWalk = true;
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
            CreateRecipe().
                AddIngredient(ItemID.AnkhShield).
                AddIngredient<OrnateShield>().
                AddIngredient<ShieldoftheOcean>().
                AddIngredient<CoreofCalamity>().
                AddIngredient(ItemID.LifeFruit, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
