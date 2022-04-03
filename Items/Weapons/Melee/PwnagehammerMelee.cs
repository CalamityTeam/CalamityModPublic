using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class PwnagehammerMelee : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pwnagehammer");
            Tooltip.SetDefault("Throws a heavy, gravity-affected hammer that creates a loud blast of hallowed energy when it hits something\n" +
            "There is a 20 percent chance for the hammer to home in on a target\n" +
            "Homing hammers summon an additional spectral hammer on hit and are guaranteed to land a critical hit");
        }

        public override void SetDefaults()
        {
            Item.width = 66;
            Item.damage = 210;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useAnimation = Item.useTime = 48;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 10f;
            Item.UseSound = SoundID.Item1;
            Item.DamageType = DamageClass.Melee;
            Item.height = 66;
            Item.value = Item.buyPrice(gold: 48);
            Item.rare = ItemRarityID.LightPurple;
            Item.shoot = ModContent.ProjectileType<PwnagehammerProj>();
            Item.shootSpeed = 24.4f;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 10;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 speed = new Vector2(speedX, speedY);
            Vector2 yeetOffset = Vector2.Normalize(speed) * 40f;
            if (Collision.CanHit(position, 0, 0, position + yeetOffset, 0, 0))
            {
                position += yeetOffset;
            }
            Projectile.NewProjectile(position, speed, type, damage, knockBack, player.whoAmI, Main.rand.NextBool(5) ? 1f : -1f);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.Pwnhammer).AddIngredient(ItemID.HallowedBar, 7).AddIngredient(ItemID.SoulofMight, 3).AddIngredient(ItemID.SoulofSight, 3).AddIngredient(ItemID.SoulofFright, 3).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
