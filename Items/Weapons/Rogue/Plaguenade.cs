using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Plaguenade : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plaguenade");
            Tooltip.SetDefault("Releases a swarm of angry plague bees\n" +
                "Stealth strikes spawn more bees and generate a larger explosion");
        }

        public override void SafeSetDefaults()
        {
            item.width = 20;
            item.damage = 63;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.consumable = true;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 15;
            item.knockBack = 1.5f;
            item.maxStack = 999;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 28;
            item.value = Item.buyPrice(gold: 1);
            item.rare = ItemRarityID.Yellow;
            item.Calamity().donorItem = true;
            item.shoot = ModContent.ProjectileType<PlaguenadeProj>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f);
                if (proj.WithinBounds(Main.maxProjectiles))
                    Main.projectile[proj].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Beenade, 20);
            recipe.AddIngredient(ModContent.ItemType<PlagueCellCluster>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this, 100);
            recipe.AddRecipe();
        }
    }
}
