using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class SealedSingularity : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sealed Singularity");
            Tooltip.SetDefault("Shatters on impact, summoning a black hole that sucks in nearby enemies\n" +
            "Stealth strikes summon a black hole that lasts longer and sucks enemies with stronger force");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 260;
            Item.knockBack = 5f;
            Item.useAnimation = Item.useTime = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.Calamity().rogue = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SealedSingularityProj>();
            Item.shootSpeed = 14f;

            Item.noMelee = Item.noUseGraphic = true;
            Item.height = Item.width = 34;
            Item.UseSound = SoundID.Item106;

            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.Calamity().donorItem = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                // Directly nerf stealth strikes by 32%, but only stealth strikes.
                int stealthDamage = (int)(damage * 0.72f);
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, stealthDamage, knockBack, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<DuststormInABottle>()).AddIngredient(ModContent.ItemType<DarkPlasma>(), 3).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
