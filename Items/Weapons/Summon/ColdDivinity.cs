using Terraria.DataStructures;
using CalamityMod.Buffs.Summon;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Summon
{
    public class ColdDivinity : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cold Divinity");
            Tooltip.SetDefault("Summons the power of the ancient ice castle\n" +
                "For each minion slot used, you will gain an additional orbiting shield spike\n" +
                "These spikes accelerate rapidly towards a nearby enemy to inflict heavy damage\n" +
                "They take some time to regenerate after launching themselves at the target, however\n" +
                "On right click, summons a duplicate ring around the targeted enemy, which slowly converges before exploding");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 48;
            Item.mana = 10;
            Item.width = 52;
            Item.height = 50;
            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 4.5f;
            Item.UseSound = SoundID.Item30;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ColdDivinityPointyThing>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;

            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float totalMinionSlots = 0f;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].minion && Main.projectile[i].owner == player.whoAmI)
                {
                    totalMinionSlots += Main.projectile[i].minionSlots;
                }
            }
            if (player.altFunctionUse != 2 && totalMinionSlots < player.maxMinions)
            {
                player.AddBuff(ModContent.BuffType<ColdDivinityBuff>(), 120, true);
                position = Main.MouseWorld;
                int p = Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
                int pointyThingCount = 0;
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == type && Main.projectile[i].owner == player.whoAmI)
                    {
                        if (!(Main.projectile[i].ModProjectile as ColdDivinityPointyThing).circlingPlayer)
                            continue;
                        pointyThingCount++;
                    }
                }
                float angleVariance = MathHelper.TwoPi / pointyThingCount;
                float angle = 0f;
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == type && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].ai[1] == 0f)
                    {
                        if (!(Main.projectile[i].ModProjectile as ColdDivinityPointyThing).circlingPlayer)
                            continue;
                        Main.projectile[i].ai[0] = angle;
                        Main.projectile[i].netUpdate = true;
                        angle += angleVariance;
                        for (int j = 0; j < 22; j++)
                        {
                            Dust dust = Dust.NewDustDirect(Main.projectile[i].position, Main.projectile[i].width, Main.projectile[i].height, DustID.Ice);
                            dust.velocity = Vector2.UnitY * Main.rand.NextFloat(3f, 5.5f) * Main.rand.NextBool(2).ToDirectionInt();
                            dust.noGravity = true;
                        }
                    }
                }
            }
            return false;
        }

        public override bool AltFunctionUse(Player player)
        {
            return base.AltFunctionUse(player);
        }
    }

}
