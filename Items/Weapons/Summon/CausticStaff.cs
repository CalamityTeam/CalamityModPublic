using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Summon
{
    public class CausticStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Caustic Staff");
            Tooltip.SetDefault("Summons a mini dragon to fight for you\n" +
                "The dragon can inflict several debilitating debuffs if you hold a summon weapon or tool");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.mana = 10;
            Item.damage = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 10f;
            Item.shoot = ModContent.ProjectileType<CausticStaffSummon>();
            Item.width = 26;
            Item.height = 28;
            Item.UseSound = SoundID.Item77;
            Item.useAnimation = Item.useTime = 25;

            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.Calamity().donorItem = true;

            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.DamageType = DamageClass.Summon;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, 1f);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyEvilFlask", 5).
                AddIngredient(ItemID.Deathweed, 2).
                AddIngredient(ItemID.SoulofNight, 10).
                AddRecipeGroup("AnyEvilBar", 10).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
