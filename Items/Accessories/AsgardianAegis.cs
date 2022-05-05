using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class AsgardianAegis : ModItem
    {
        public const int ShieldSlamIFrames = 6;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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
            Item.width = 60;
            Item.height = 54;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.defense = 28;
            Item.accessory = true;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityKeybinds.AegisHotKey.TooltipHotkeyString();
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip5");

            if (line != null)
                line.Text = "Press " + hotkey + " to activate buffs to all damage, crit chance, and defense";
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
            player.buffImmune[ModContent.BuffType<ArmorCrunch>()] = true;
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            { player.statDefense += 20; }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AsgardsValor>().
                AddIngredient<ElysianAegis>().
                AddIngredient<CosmiliteBar>(10).
                AddIngredient<AscendantSpiritEssence>(4).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
