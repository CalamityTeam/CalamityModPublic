using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Summon;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using CalamityMod.Buffs.StatDebuffs;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class ForgottenApexWand : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<HeavyBleeding>(), ModContent.BuffType<ArmorCrunch>()];
        }
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 48;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.DamageType = DamageClass.Summon;
            Item.UseSound = SoundID.Item89;
            Item.noMelee = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.autoReuse = true;

            Item.knockBack = 4f;
            Item.mana = 10;
            Item.damage = 46;
            Item.useAnimation = Item.useTime = 36;
            Item.buffType = ModContent.BuffType<AncientMineralSharkBuff>();
            Item.shoot = ModContent.ProjectileType<ApexShark>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            var minion = Projectile.NewProjectileDirect(source, player.ClampedMouseWorld(), Vector2.Zero, ModContent.ProjectileType<ApexShark>(), damage, knockback, player.whoAmI);
            minion.originalDamage = Item.damage;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyAdamantiteBar", 5).
                AddIngredient(ItemID.AncientBattleArmorMaterial, 2).
                AddIngredient(ItemID.Amethyst, 4).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
