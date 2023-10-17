using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class BloodBoiler : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        
        public static readonly SoundStyle Heartbeat = new("CalamityMod/Sounds/Item/Heartbeat") { PitchVariance = 0.2f, Volume = 0.55f };
        
        public bool shotReturn = false;
        public override void SetDefaults()
        {
            Item.damage = 185;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 60;
            Item.height = 30;
            Item.useTime = 5;
            Item.useAnimation = 20;
            Item.autoReuse = true;
            Item.UseSound = Heartbeat;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.shootSpeed = 9.5f;
            Item.shoot = ModContent.ProjectileType<BloodBoilerFire>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            shotReturn = !shotReturn;
            if (Main.rand.NextFloat() > 0.60f)
                player.statLife -= 1;
            if (player.statLife <= 0)
            {
                PlayerDeathReason pdr = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.BloodBoiler" + Main.rand.Next(1, 2 + 1)).Format(player.name));
                player.KillMe(pdr, 1000.0, 0, false);
                return false;
            }
            Vector2 newVel = velocity.RotatedBy(shotReturn ? 0.03f : -0.03f);
            Projectile.NewProjectile(source, position, newVel, type, damage, knockback, player.whoAmI, 0, 0, shotReturn ? 5 : 0);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BloodstoneCore>(6).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
