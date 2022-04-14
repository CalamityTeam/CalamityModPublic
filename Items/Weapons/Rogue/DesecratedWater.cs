using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class DesecratedWater : RogueWeapon
    {
        public const int BaseDamage = 55;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Desecrated Water");
            Tooltip.SetDefault(@"Throws an unholy flask of water that explodes into an explosion of bubbles on death
Stealth strikes spawn additional bubbles that inflict Ichor and Cursed Inferno");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.damage = BaseDamage;
            Item.width = 22;
            Item.height = 24;
            Item.useAnimation = 29;
            Item.useTime = 29;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4.5f;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item106;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DesecratedWaterProj>();
            Item.shootSpeed = 12f;
            Item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddRecipeGroup("AnyEvilWater", 100).AddRecipeGroup("AnyAdamantiteBar", 5).AddRecipeGroup("CursedFlameIchor", 5).AddIngredient(ItemID.SoulofNight, 7).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
