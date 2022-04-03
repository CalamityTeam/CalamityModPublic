using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class DormantBrimseeker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dormant Brimseeker");
            Tooltip.SetDefault("You could've sworn that they turned even scarier when you looked at their reflections in a mirror\n" +
                               "Summons a brimseeker to keep you company\n" +
                               "Firing another brimseeker when all minion slots are filled summons a brimstone aura\n" +
                               "The aura empowers your brimseeker summons and produces damaging fireballs\n" +
                               "Only one aura can persist at a time");
        }

        public override void SetDefaults()
        {
            Item.damage = 42;
            Item.mana = 10;
            Item.width = Item.height = 32;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DormantBrimseekerSummoner>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;

            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float totalSlots = 0f;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].minion && Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI)
                {
                    totalSlots += Main.projectile[i].minionSlots;
                }
            }
            if (totalSlots < player.maxMinions)
            {
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f);
            }
            else if (player.ownedProjectileCounts[ModContent.ProjectileType<DormantBrimseekerAura>()] <= 0f)
            {
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<DormantBrimseekerAura>(), damage * 2, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }
    }
}
