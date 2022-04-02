using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class AsgardianAegis : ModItem
    {
        public const int ShieldSlamIFrames = 6;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Asgardian Aegis");
            Tooltip.SetDefault("Grants immunity to knockback\n" +
                "Immune to most debuffs\n" +
                "+40 max life and increased life regeneration\n" +
                "Grants a supreme holy flame dash\n" +
                "Can be used to ram enemies\n" +
                "TOOLTIP LINE HERE\n" +
                "Activating this buff will reduce your movement speed and increase enemy aggro\n" +
                "+20 defense while submerged in liquid");
        }

        public override void SetDefaults()
        {
            item.width = 60;
            item.height = 54;
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.defense = 28;
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityMod.AegisHotKey.TooltipHotkeyString();
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "Tooltip5")
                {
                    line2.text = "Press " + hotkey + " to activate buffs to all damage, crit chance, and defense";
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dashMod = 4;
            player.dash = 0;
            modPlayer.elysianAegis = true;
            modPlayer.abaddon = true;
            player.noKnockback = true;
            player.fireWalk = true;
            player.statLifeMax2 += 40;
            player.lifeRegen++;
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
            player.buffImmune[ModContent.BuffType<BrimstoneFlames>()] = true;
            player.buffImmune[ModContent.BuffType<HolyFlames>()] = true;
            player.buffImmune[ModContent.BuffType<GlacialState>()] = true;
            player.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = true;
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            { player.statDefense += 20; }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AsgardsValor>());
            recipe.AddIngredient(ModContent.ItemType<ElysianAegis>());
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 10);
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 4);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
