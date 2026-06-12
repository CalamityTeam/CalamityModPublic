using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Buffs.Summon;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class CausticStaff : ModItem, ILocalizedModType, IHoldShiftTooltipItem
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.OnFire3, BuffID.Venom, BuffID.Ichor, BuffID.CursedInferno, ModContent.BuffType<MarkedforDeath>()];
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 28;
            Item.mana = 10;
            Item.damage = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.buffType = ModContent.BuffType<CausticStaffBuff>();
            Item.shoot = ModContent.ProjectileType<CausticStaffSummon>();
            Item.UseSound = SoundID.Item77;
            Item.useAnimation = Item.useTime = 36;

            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.Calamity().donorItem = true;

            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.DamageType = DamageClass.Summon;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            var minion = Projectile.NewProjectileDirect(source, player.ClampedMouseWorld(), Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, 1f);
            minion.originalDamage = Item.damage;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyEvilBar", 10).
                AddRecipeGroup("CursedFlameIchor", 10).
                AddIngredient(ItemID.SoulofNight, 10).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
