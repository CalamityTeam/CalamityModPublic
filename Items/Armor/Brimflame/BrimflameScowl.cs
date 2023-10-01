using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Armor.Brimflame
{
    [AutoloadEquip(EquipType.Head)]
    public class BrimflameScowl : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Hardmode";
        public static readonly SoundStyle ActivationSound = new("CalamityMod/Sounds/Custom/AbilitySounds/AngelicAllianceActivation");

        // TODO -- what the fuck is this? this is not how you implement a set bonus
        private bool frenzy = false;
        public static int CooldownLength = 1800;

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.defense = 11;
        }

        private void UpdateFrenzy(Player player)
        {
            var modPlayer = player.Calamity();
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

            if (frenzy)
                player.GetDamage<MagicDamageClass>() += 0.4f;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MagicDamageClass>() += 0.05f;
            player.GetCritChance<MagicDamageClass>() += 5;
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
            var modPlayer = player.Calamity();
            modPlayer.brimflameSet = true;
            player.GetDamage<MagicDamageClass>() += 0.15f;
            player.GetCritChance<MagicDamageClass>() += 15;
            var hotkey = CalamityKeybinds.ArmorSetBonusHotKey.TooltipHotkeyString();
            player.setBonus = this.GetLocalization("SetBonus").Format(hotkey);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AshesofCalamity>(4).
                AddIngredient<UnholyCore>(2).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
