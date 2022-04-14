using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Summon
{
    public class FlowersOfMortality : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flowers of Mortality");
            Tooltip.SetDefault("Summons five rainbow flowers over your head\n" +
                               "The combined flowers consume two and a half minion slots");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 140;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.width = 36;
            Item.height = 36;
            Item.useTime = Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item44;
            Item.shoot = ModContent.ProjectileType<FlowersOfMortalityPetal>();
            Item.shootSpeed = 10f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 5; //If you already have all 5, no need to resummon

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            CalamityUtils.KillShootProjectiles(false, type, player);
            for (int i = 0; i < 5; i++)
            {
                Projectile blossom = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, 0f);
                blossom.ai[0] = MathHelper.TwoPi * i / 5f;
                blossom.rotation = blossom.ai[0];
                blossom.originalDamage = Item.damage;
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<WitherBlossomsStaff>()).AddIngredient(ModContent.ItemType<ViralSprout>()).AddIngredient(ItemID.LunarBar, 5).AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5).AddIngredient(ModContent.ItemType<BarofLife>(), 5).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
