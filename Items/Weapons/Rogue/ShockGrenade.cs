using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class ShockGrenade : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shock Grenade");
            Tooltip.SetDefault("Throws a grenade that explodes into a burst of lightning\n" +
                "Stealth strikes cause the grenade to leave an electrifying aura when it explodes");
        }

        public override void SafeSetDefaults()
        {
            item.width = 14;
            item.damage = 90;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.consumable = true;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 18;
            item.knockBack = 1f;
            item.autoReuse = true;
            item.height = 30;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.shoot = ModContent.ProjectileType<ShockGrenadeProjectile>();
            item.shootSpeed = 12.5f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
                if (p.WithinBounds(Main.maxProjectiles))
                    Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.GetTexture("CalamityMod/Items/Weapons/Rogue/ShockGrenadeGlow"));
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Grenade, 20);
            recipe.AddIngredient(ItemID.MartianConduitPlating, 5);
            recipe.AddIngredient(ItemID.Nanites, 5);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this, 100);
            recipe.AddRecipe();
        }
    }
}
