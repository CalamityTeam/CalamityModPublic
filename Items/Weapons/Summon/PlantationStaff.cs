using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class PlantationStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plantation Staff");
            Tooltip.SetDefault("Summons a miniature plantera to protect you\n" +
            "Fires seeds, spiky balls, and spore clouds from afar to poison targets\n" +
            "Enrages when you get under 75% health and begins ramming enemies\n" +
            "Occupies 3 minion slots and there can only be one");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 55;
            Item.mana = 10;
            Item.width = 66;
            Item.height = 70;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item76;
            Item.shoot = ModContent.ProjectileType<PlantSummon>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0 && player.maxMinions >= 3;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                CalamityUtils.KillShootProjectileMany(player, new int[] { type, ModContent.ProjectileType<PlantTentacle>() });

                float speed = Item.shootSpeed;
                player.itemTime = Item.useTime;
                Vector2 playerPos = player.RotatedRelativePoint(player.MountedCenter, true);
                float directionX = (float)Main.mouseX + Main.screenPosition.X - playerPos.X;
                float directionY = (float)Main.mouseY + Main.screenPosition.Y - playerPos.Y;
                if (player.gravDir == -1f)
                {
                    directionY = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - playerPos.Y;
                }
                Vector2 spinningpoint = new Vector2(directionX, directionY);
                float vectorDist = spinningpoint.Length();
                if ((float.IsNaN(spinningpoint.X) && float.IsNaN(spinningpoint.Y)) || (spinningpoint.X == 0f && spinningpoint.Y == 0f))
                {
                    spinningpoint.X = (float)player.direction;
                    spinningpoint.Y = 0f;
                    vectorDist = speed;
                }
                else
                {
                    vectorDist = speed / vectorDist;
                }
                spinningpoint.X *= vectorDist;
                spinningpoint.Y *= vectorDist;
                playerPos.X = (float)Main.mouseX + Main.screenPosition.X;
                playerPos.Y = (float)Main.mouseY + Main.screenPosition.Y;
                spinningpoint = spinningpoint.RotatedBy(MathHelper.PiOver2, default);
                int p = Projectile.NewProjectile(source, playerPos + spinningpoint, spinningpoint, type, damage, knockback, player.whoAmI, 0f, 0f);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<EyeOfNight>().
                AddIngredient<DeepseaStaff>().
                AddIngredient(ItemID.OpticStaff).
                AddIngredient<LivingShard>(12).
                AddTile(TileID.MythrilAnvil).
                Register();

            CreateRecipe().
                AddIngredient<FleshOfInfidelity>().
                AddIngredient<DeepseaStaff>().
                AddIngredient(ItemID.OpticStaff).
                AddIngredient<LivingShard>(12).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
