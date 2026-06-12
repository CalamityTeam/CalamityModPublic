using CalamityMod.Buffs.Summon;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class ResurrectionButterfly : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.StaffMinionSlotsRequired[Type] = 2f;
        }
        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 46;
            Item.damage = 66; // clueless
            Item.mana = 10;
            Item.useAnimation = Item.useTime = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 1f;

            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.Calamity().donorItem = true;

            Item.UseSound = SoundID.Item44;
            Item.autoReuse = true;
            Item.buffType = ModContent.BuffType<ResurrectionButterflyBuff>();
            Item.shoot = ModContent.ProjectileType<PinkButterfly>();
            Item.DamageType = DamageClass.Summon;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            Vector2 clampedMouse = player.ClampedMouseWorld();
            Vector2 mouseDirection = Vector2.Normalize(clampedMouse - player.Center) * 10f;

            var pink = Projectile.NewProjectileDirect(source, clampedMouse, mouseDirection.RotatedBy(MathHelper.PiOver2), ModContent.ProjectileType<PinkButterfly>(), damage, knockback, Main.myPlayer, 0f, 0f);
            pink.originalDamage = Item.damage;
            var purple = Projectile.NewProjectileDirect(source, clampedMouse, mouseDirection.RotatedBy(-MathHelper.PiOver2), ModContent.ProjectileType<PurpleButterfly>(), damage, knockback, Main.myPlayer, 0f, 0f);
            purple.originalDamage = Item.damage;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<LifeAlloy>(5).
                AddIngredient(ItemID.ButterflyDust, 2).
                AddIngredient(ItemID.Silk, 40).
                AddIngredient(ItemID.Ectoplasm, 20).
                AddTile(TileID.Loom).
                Register();
        }
    }
}
