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
    public class TacticalPlagueEngine : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tactical Plague Engine");
            Tooltip.SetDefault("Summons a plague jet to pummel your enemies into submission\n" +
                               "Jets will fire ammo from your inventory, 66% chance to not consume ammo\n" +
                               "Sometimes shoots a missile instead of a bullet");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 52;
            Item.mana = 10;
            Item.width = 28;
            Item.height = 20;
            Item.useTime = Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 0.5f;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item14;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Summon;
            Item.shoot = ModContent.ProjectileType<TacticalPlagueJet>();
            Item.shootSpeed = 16f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                int p = Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI, 0f, 1f);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<BlackHawkRemote>()).AddIngredient(ModContent.ItemType<InfectedRemote>()).AddIngredient(ModContent.ItemType<FuelCellBundle>()).AddIngredient(ModContent.ItemType<PlagueCellCluster>(), 15).AddIngredient(ModContent.ItemType<InfectedArmorPlating>(), 8).AddIngredient(ItemID.LunarBar, 5).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
