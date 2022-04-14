using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
namespace CalamityMod.Items.Weapons.Rogue
{
    public class AdamantiteThrowingAxe : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Adamantite Throwing Axe");
            Tooltip.SetDefault("Stealth strikes summon lightning bolts on enemy hits");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 26;
            Item.damage = 44;
            Item.noMelee = true;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 12;
            Item.knockBack = 3.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 30;
            Item.maxStack = 999;
            Item.value = 1600;
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<AdamantiteThrowingAxeProjectile>();
            Item.shootSpeed = 12f;
            Item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int stealth = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                    Main.projectile[stealth].usesLocalNPCImmunity = true;
                }
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(100).AddIngredient(ItemID.AdamantiteBar).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
