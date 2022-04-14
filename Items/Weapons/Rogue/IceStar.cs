using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class IceStar : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Star");
            Tooltip.SetDefault("Throws homing ice stars\n" +
                "Stealth strikes pierce infinitely and spawn ice shards on hit\n" +
                "Ice Stars are too brittle to be recovered after being thrown");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 45;
            Item.noMelee = true;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 12;
            Item.knockBack = 2.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.width = Item.height = 66;
            Item.maxStack = 999;
            Item.value = Item.buyPrice(0, 0, 5, 0);
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<IceStarProjectile>();
            Item.shootSpeed = 14f;
            Item.Calamity().rogue = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 7;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (proj.WithinBounds(Main.maxProjectiles))
                    Main.projectile[proj].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}
