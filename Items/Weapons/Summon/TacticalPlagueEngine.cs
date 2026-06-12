using CalamityMod.Buffs.DamageOverTime;
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
    public class TacticalPlagueEngine : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public const int BulletShootRate = 125;
   
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 20;
            Item.damage = 140;
            Item.mana = 10;
            Item.useAnimation = Item.useTime = 24;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 0.5f;
            Item.value = CalamityGlobalItem.RarityPurpleBuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item14;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<TacticalPlagueEngineBuff>();
            Item.shoot = ModContent.ProjectileType<TacticalPlagueJet>();
            Item.shootSpeed = 7f; // Affects bullet speed
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            var minion = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 1f);
            minion.originalDamage = Item.damage;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BlackHawkRemote>().
                AddIngredient<FuelCellBundle>().
                AddIngredient(ItemID.LunarBar, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
