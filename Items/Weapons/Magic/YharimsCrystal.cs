using CalamityMod.Projectiles.Magic;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class YharimsCrystal : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yharim's Crystal");
            Tooltip.SetDefault("Fires a beam of complete destruction\n" +
                "Only those that are worthy can use this item before Yharon is defeated");
        }

        public override void SetDefaults()
        {
            item.damage = 220;
            item.magic = true;
            item.mana = 12;
            item.width = 16;
            item.height = 16;
            item.useTime = 10;
            item.useAnimation = 10;
            item.reuseDelay = 5;
            item.useStyle = 5;
            item.UseSound = SoundID.Item13;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.knockBack = 0f;
            item.value = Item.buyPrice(platinum: 1, gold: 80);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<YharimsCrystalPrism>();
            item.shootSpeed = 30f;
            item.Calamity().customRarity = CalamityRarity.ItemSpecific;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<YharimsCrystalPrism>()] == 0;
            /*
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.type == ModContent.ProjectileType<YharimsCrystalPrism>() && p.owner == player.whoAmI)
                {
                    return false;
                }
            }
            return true;
            */
        }

        // developer or yharon restriction has been removed
        /*
        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (CalamityWorld.downedYharon || CalamityMod.developerList.Contains(player.name))
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 0f);

            // otherwise, zero damage dynamite
            else
                Projectile.NewProjectile(position.X, position.Y, 0f, 0f, ProjectileID.Dynamite, 0, 0f, player.whoAmI, 0f, 0f);
            return false;
        }
        */
    }
}
