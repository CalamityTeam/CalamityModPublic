using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Armor.PlagueReaper
{
    [AutoloadEquip(EquipType.Head)]
    public class PlagueReaperMask : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Hardmode";
        public static readonly SoundStyle ActivationSound = new("CalamityMod/Sounds/Custom/AbilitySounds/PlagueReaperAbility");

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 9; //35
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<PlagueReaperVest>() && legs.type == ModContent.ItemType<PlagueReaperStriders>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            var hotkey = CalamityKeybinds.ArmorSetBonusHotKey.TooltipHotkeyString();
            player.setBonus = this.GetLocalization("SetBonus").Format(hotkey);
            var modPlayer = player.Calamity();
            modPlayer.plagueReaper = true;
            player.ammoCost75 = true;

            var hasPlagueBlackoutCD = modPlayer.cooldowns.TryGetValue(PlagueBlackout.ID, out var cd);
            if (hasPlagueBlackoutCD && cd.timeLeft > 1500)
            {
                player.blind = true;
                player.headcovered = true;
                player.blackout = true;
                player.GetDamage<RangedDamageClass>() += 0.6f; //60% ranged dmg and 20% crit
                player.GetCritChance<RangedDamageClass>() += 20;
            }

            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_Accessory(Item);
                if (player.immune)
                {
                    if (player.miscCounter % 10 == 0)
                    {
                        var damage = (int)player.GetTotalDamage<RangedDamageClass>().ApplyTo(40);
                        var cinder = CalamityUtils.ProjectileRain(source, player.Center, 400f, 100f, 500f, 800f, 22f, ModContent.ProjectileType<TheSyringeCinder>(), damage, 4f, player.whoAmI);
                        if (cinder.whoAmI.WithinBounds(Main.maxProjectiles))
                            cinder.DamageType = DamageClass.Generic;
                    }
                }
            }
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<RangedDamageClass>() += 0.1f;
            player.GetCritChance<RangedDamageClass>() += 8;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.NecroHelmet).
                AddIngredient<PlagueCellCanister>(15).
                AddIngredient(ItemID.Nanites, 11).
                AddTile(TileID.MythrilAnvil).
                Register();

            CreateRecipe().
                AddIngredient(ItemID.AncientNecroHelmet).
                AddIngredient<PlagueCellCanister>(15).
                AddIngredient(ItemID.Nanites, 11).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
