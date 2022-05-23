using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class TerraDisk : ModItem
    {
        public static int BaseDamage = 100;
        public static float Speed = 12f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terra Disk");
            Tooltip.SetDefault(@"Throws a disk that has a chance to generate several disks if enemies are near it
A max of three disks can be active at a time
Stealth strikes travel slower and are rapidly orbited by the smaller disks");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 46;
            Item.damage = BaseDamage;
            Item.knockBack = 4f;
            Item.useAnimation = 16;
            Item.useTime = 16;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;

            Item.value = Item.buyPrice(gold: 80);
            Item.rare = ItemRarityID.Yellow;

            Item.DamageType = RogueDamageClass.Instance;
            Item.shoot = ModContent.ProjectileType<TerraDiskProjectile>();
            Item.shootSpeed = Speed;
        }

        public override bool CanUseItem(Player player)
        {
            //Stealth strikes ignore the proj cap
            int terraDiskCount = 0;
            for (int p = 0; p < Main.maxProjectiles; p++)
            {
                Projectile proj = Main.projectile[p];
                if (!proj.active || proj.owner != player.whoAmI)
                    continue;
                if (proj.type == Item.shoot && !proj.Calamity().stealthStrike)
                {
                    terraDiskCount++;
                }
                if (terraDiskCount >= 3)
                    break;
            }
            return terraDiskCount < 3;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                velocity.X *= 0.75f;
                velocity.Y *= 0.75f;
                damage = (int)(damage * 0.9f);
            }
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && proj.WithinBounds(Main.maxProjectiles))
            {
                Main.projectile[proj].Calamity().stealthStrike = true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SeashellBoomerang>().
                AddIngredient<Equanimity>().
                AddIngredient(ItemID.ThornChakram).
                AddIngredient<LivingShard>(8).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
