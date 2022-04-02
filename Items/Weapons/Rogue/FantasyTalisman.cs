using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class FantasyTalisman : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fantasy Talisman");
            Tooltip.SetDefault(@"Fires high velocity talismans that ignore gravity
Talismans attach to enemies, causing them to release lost souls
Stealth strikes release more souls and leave behind souls as they travel");
        }

        public override void SafeSetDefaults()
        {
            item.width = 34;
            item.damage = 93;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 16;
            item.useTime = 16;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 62;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 0, 60, 0);
            item.rare = ItemRarityID.Lime;
            item.shoot = ModContent.ProjectileType<FantasyTalismanProj>();
            item.shootSpeed = 18f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<FantasyTalismanStealth>(), (int)(damage * 0.8f), knockBack, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SolarVeil>(), 2);
            recipe.AddIngredient(ItemID.Silk);
            recipe.AddIngredient(ItemID.Ectoplasm);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this, 100);
            recipe.AddRecipe();
        }
    }
}
