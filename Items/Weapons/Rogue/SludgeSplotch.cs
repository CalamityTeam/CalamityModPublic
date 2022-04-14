using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class SludgeSplotch : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sludge Splotch");
            Tooltip.SetDefault("Slows non-boss enemies slightly\n" +
                "Stealth strikes cause the main ball of sludge to split on hit, inflicting more damage");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 32;
            Item.damage = 30;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.consumable = true;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 18;
            Item.knockBack = 1f;
            Item.autoReuse = true;
            Item.height = 30;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 0, 40);
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<SludgeSplotchProj1>();
            Item.shootSpeed = 10f;
            Item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundEngine.PlaySound(SoundID.NPCKilled, (int)player.position.X, (int)player.position.Y, 9, 2, 0);

            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(source, position.X, position.Y, velocity.X * 1.2f, velocity.Y * 1.2f, type, damage, knockback * 3, player.whoAmI, 0f, 1f);
                if (p.WithinBounds(Main.maxProjectiles))
                    Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(100).AddIngredient(ModContent.ItemType<EbonianGel>(), 15).AddRecipeGroup("Boss2Material").Register();
        }
    }
}
