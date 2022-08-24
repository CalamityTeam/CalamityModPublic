using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class BlastBarrel : ModItem
    {
        public const int BaseDamage = 32;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blast Barrel");
            Tooltip.SetDefault("Throws a rolling barrel that explodes on wall collision\n" +
                               "Stealth strikes makes the barrel bounce twice before disappearing with varied effects after each bounce\n" +
                               "'Some people used to jump over these'");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 48;
            Item.damage = BaseDamage;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 22;
            Item.useTime = 22;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(0, 12, 0, 0); //2 gold 40 silver sellprice
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<BlastBarrelProjectile>();
            Item.shootSpeed = 12f;
            Item.DamageType = RogueDamageClass.Instance;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = (ContentSamples.CreativeHelper.ItemGroup)CalamityResearchSorting.RogueWeapon;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            velocity.Y *= 0.85f;
            Vector2 initialVelocity = velocity;

            // A vertical offset is added to ensure that the barrel does not immediately collide with tiles and explode.
            int p = Projectile.NewProjectile(source, position - Vector2.UnitY * 12f, initialVelocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && p.WithinBounds(Main.maxProjectiles))
                Main.projectile[p].Calamity().stealthStrike = true;
            return false;
        }
    }
}
