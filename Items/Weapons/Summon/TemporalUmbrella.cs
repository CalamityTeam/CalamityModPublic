using CalamityMod.Buffs.Summon;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon.Umbrella;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    [LegacyName("BensUmbrella")]
    public class TemporalUmbrella : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";

        public override void SetStaticDefaults()
        {
            ItemID.Sets.StaffMinionSlotsRequired[Type] = 5f;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.BetsysCurse, BuffID.Ichor];
        }

        public override void SetDefaults()
        {
            Item.width = 74;
            Item.height = 72;
            Item.damage = 193;
            Item.knockBack = 1f;
            Item.mana = 99;
            Item.useAnimation = Item.useTime = 24;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<MagicHatBuff>();
            Item.shoot = ModContent.ProjectileType<MagicHat>();

            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item68;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;
        }

        public override bool CanUseItem(Player player) => player.maxMinions >= 5;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            CalamityUtils.KillShootProjectileMany(player, type, ModContent.ProjectileType<MagicArrow>(), ModContent.ProjectileType<MagicHammer>(), ModContent.ProjectileType<MagicAxe>(), ModContent.ProjectileType<MagicUmbrella>(), ModContent.ProjectileType<MagicRifle>());
            var minion = Projectile.NewProjectileDirect(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
            minion.originalDamage = Item.damage;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ViridVanguard>().
                AddIngredient<SarosPossession>().
                AddIngredient(ItemID.Umbrella).
                AddIngredient(ItemID.TopHat).
                AddIngredient<ShadowspecBar>(5).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
