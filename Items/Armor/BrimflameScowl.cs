using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class BrimflameScowl : ModItem
    {
        private bool frenzy = false;
        public static int CooldownLength = 1800;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Brimflame Cowl");
            Tooltip.SetDefault("5% increased magic damage and critical strike chance\n" +
                "Increases maximum mana by 70 and reduces mana usage by 10%\n" +
                "Immunity to On Fire!, Brimstone Flames and Frostburn");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.defense = 11;
        }

        private void UpdateFrenzy(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (!frenzy)
            {
                if (modPlayer.brimflameFrenzy)
                    frenzy = true;
            }
            else
            {
                if (!modPlayer.brimflameFrenzy)
                {
                    frenzy = false;
                    player.AddCooldown(BrimflameFrenzy.ID, CooldownLength);
                }
            }
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic) += 0.05f;
            player.GetCritChance(DamageClass.Magic) += 5;
            player.statManaMax2 += 70;
            player.manaCost *= 0.9f;
            player.buffImmune[ModContent.BuffType<BrimstoneFlames>()] = true;
            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[BuffID.Frostburn] = true;
            UpdateFrenzy(player);
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<BrimflameRobes>() && legs.type == ModContent.ItemType<BrimflameBoots>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.brimflameSet = true;
            player.GetDamage(DamageClass.Magic) += 0.15f;
            player.GetCritChance(DamageClass.Magic) += 15;
            string hotkey = CalamityKeybinds.SetBonusHotKey.TooltipHotkeyString();
            player.setBonus = "Grants an additional 15% increased magic damage and crit\n" +
                "Press " + hotkey + " to trigger a brimflame frenzy effect\n" +
                "While under this effect, your damage is significantly boosted\n" +
                "However, this comes at the cost of rapid life loss and no mana regeneration\n" +
                "This can be toggled off, however, a brimstone frenzy has a 30 second cooldown";
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CalamityDust>(4).
                AddIngredient<UnholyCore>(2).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
